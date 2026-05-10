using eAviaSales.Api.Models.Cart;
using eAviaSales.Api.Helpers;
using eAviaSales.Api.Services.Cart;
using eAviaSales.BusinessLogic.Interface;
using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controllers;

[ApiController]
[Route("api/cart")]
public sealed class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly IFlightActions _flightActions;

    public CartController(ICartService cartService, IFlightActions flightActions)
    {
        _cartService = cartService;
        _flightActions = flightActions;
    }

    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost("items")]
    public async Task<ActionResult<CartResponse>> AddItem([FromBody] AddCartItemRequest request)
    {
        var flight = await _flightActions.GetFlightByIdActionAsync(request.EventId);
        if (flight is null)
        {
            return NotFound(new { message = $"Event with ID {request.EventId} was not found." });
        }

        var cartId = CartCustomerContext.Resolve(HttpContext);
        var snapshot = _cartService.AddOrUpdateItem(
            cartId,
            request.EventId,
            flight.FlightNumber,
            request.Quantity,
            flight.Price,
            flight.CurrencyCode);

        return Ok(ToResponse(snapshot));
    }

    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [HttpGet]
    public ActionResult<CartResponse> GetCart()
    {
        var snapshot = _cartService.Get(CartCustomerContext.Resolve(HttpContext));
        return Ok(ToResponse(snapshot));
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("items/{itemId}")]
    public IActionResult RemoveItem(string itemId)
    {
        var cartId = CartCustomerContext.Resolve(HttpContext);
        var deleted = _cartService.RemoveItem(cartId, itemId);
        if (!deleted)
        {
            return NotFound(new { message = $"Cart item {itemId} was not found." });
        }

        return NoContent();
    }

    private static CartResponse ToResponse(CartSnapshot snapshot)
    {
        var items = snapshot.Items
            .Select(item => new CartItemDto
            {
                ItemId = item.ItemId,
                EventId = item.EventId,
                EventCode = item.EventCode,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.Quantity * item.UnitPrice,
                CurrencyCode = item.CurrencyCode
            })
            .ToList();

        return new CartResponse
        {
            CartId = snapshot.CartId,
            Items = items,
            GrandTotal = items.Sum(static item => item.TotalPrice),
            CurrencyCode = items.FirstOrDefault()?.CurrencyCode ?? "USD"
        };
    }
}

