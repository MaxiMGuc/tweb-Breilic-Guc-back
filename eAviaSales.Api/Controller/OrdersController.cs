using eAviaSales.Api.Contracts.Orders;
using eAviaSales.Api.Http;
using eAviaSales.Api.Services.Orders;
using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controller;

[ApiController]
[Route("api")]
public sealed class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [ProducesResponseType(typeof(CheckoutResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("checkout")]
    public ActionResult Checkout()
    {
        var customerKey = CartCustomerContext.Resolve(HttpContext);
        var result = _orderService.CreateCheckout(customerKey);
        return result.Success
            ? Ok(ToCheckoutResponse(result.Value!))
            : OrderProblem(result.ErrorCode, result.Message);
    }

    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("orders")]
    public ActionResult CreateOrder([FromBody] CreateOrderRequest request)
    {
        var customerKey = CartCustomerContext.Resolve(HttpContext);
        var result = _orderService.PlaceOrder(customerKey, request.CheckoutId);
        if (!result.Success)
        {
            return OrderProblem(result.ErrorCode, result.Message);
        }

        return StatusCode(StatusCodes.Status201Created, ToOrderResponse(result.Value!));
    }

    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("orders/{orderId}")]
    public ActionResult<OrderResponse> GetOrder(string orderId)
    {
        var customerKey = CartCustomerContext.Resolve(HttpContext);
        var order = _orderService.GetOrder(orderId);
        if (order is null || order.CustomerKey != customerKey)
        {
            return NotFound(new { message = "Order was not found." });
        }

        return Ok(ToOrderResponse(order));
    }

    [ProducesResponseType(typeof(IReadOnlyList<OrderResponse>), StatusCodes.Status200OK)]
    [HttpGet("orders")]
    public ActionResult<IReadOnlyList<OrderResponse>> GetOrders()
    {
        var customerKey = CartCustomerContext.Resolve(HttpContext);
        var list = _orderService.ListOrdersForCustomer(customerKey)
            .Select(ToOrderResponse)
            .ToList();
        return Ok(list);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost("orders/{orderId}/cancel")]
    public IActionResult CancelOrder(string orderId)
    {
        var customerKey = CartCustomerContext.Resolve(HttpContext);
        var result = _orderService.CancelOrder(customerKey, orderId);
        if (!result.Success)
        {
            return OrderErrorResult(result.ErrorCode, result.Message);
        }

        return NoContent();
    }

    [ProducesResponseType(typeof(IReadOnlyList<OrderTicketDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost("orders/{orderId}/issue")]
    public ActionResult IssueTickets(string orderId)
    {
        var result = _orderService.IssueTickets(orderId);
        if (!result.Success)
        {
            return OrderProblem(result.ErrorCode, result.Message);
        }

        var dtos = result.Value!.Select(t => ToTicketDto(orderId, t)).ToList();
        return Ok(dtos);
    }

    [ProducesResponseType(typeof(IReadOnlyList<OrderTicketDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("orders/{orderId}/tickets")]
    public ActionResult<IReadOnlyList<OrderTicketDto>> GetOrderTickets(string orderId)
    {
        var customerKey = CartCustomerContext.Resolve(HttpContext);
        var order = _orderService.GetOrder(orderId);
        if (order is null || order.CustomerKey != customerKey)
        {
            return NotFound(new { message = "Order was not found." });
        }

        var tickets = _orderService.GetTickets(orderId)
            .Select(t => ToTicketDto(orderId, t))
            .ToList();

        return Ok(tickets);
    }

    private IActionResult OrderErrorResult(string? errorCode, string? message) =>
        OrderProblem(errorCode, message);

    private ActionResult OrderProblem(string? errorCode, string? message)
    {
        var status = StatusForCode(errorCode);
        return StatusCode(status, new { message = message ?? "Request failed." });
    }

    private static int StatusForCode(string? errorCode) =>
        errorCode switch
        {
            "cart_empty" or "checkout_expired" => StatusCodes.Status400BadRequest,
            "checkout_not_found" or "order_not_found" => StatusCodes.Status404NotFound,
            "checkout_forbidden" or "order_forbidden" => StatusCodes.Status403Forbidden,
            "invalid_order_state" => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest
        };

    private static CheckoutResponse ToCheckoutResponse(CheckoutRecord c) =>
        new CheckoutResponse
        {
            CheckoutId = c.CheckoutId,
            Lines = ToLineDtos(c.Lines),
            TotalAmount = c.TotalAmount,
            CurrencyCode = c.CurrencyCode,
            ExpiresAtUtc = c.ExpiresAtUtc
        };

    private static OrderResponse ToOrderResponse(OrderRecord o) =>
        new OrderResponse
        {
            OrderId = o.OrderId,
            Status = o.Status,
            Lines = ToLineDtos(o.Lines),
            TotalAmount = o.TotalAmount,
            CurrencyCode = o.CurrencyCode,
            CreatedAtUtc = o.CreatedAtUtc
        };

    private static IReadOnlyList<OrderLineDto> ToLineDtos(IEnumerable<OrderLineRecord> lines) =>
        lines.Select(static l => new OrderLineDto
        {
            EventId = l.EventId,
            EventCode = l.EventCode,
            Quantity = l.Quantity,
            UnitPrice = l.UnitPrice,
            LineTotal = l.Quantity * l.UnitPrice
        }).ToList();

    private static OrderTicketDto ToTicketDto(string orderId, OrderTicketRecord t) =>
        new OrderTicketDto
        {
            TicketId = t.TicketId,
            OrderId = orderId,
            EventId = t.EventId,
            EventCode = t.EventCode,
            QrPayload = t.QrPayload
        };
}
