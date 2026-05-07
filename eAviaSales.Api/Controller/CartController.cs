using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controller;

[Route("api/cart")]
public sealed class CartController : ApiControllerBase
{
    [HttpPost("items")]
    public IActionResult AddItem()
    {
        return NotImplementedResponse("Add cart item");
    }

    [HttpGet]
    public IActionResult GetCart()
    {
        return NotImplementedResponse("Get cart");
    }

    [HttpDelete("items/{itemId}")]
    public IActionResult RemoveItem(string itemId)
    {
        return NotImplementedResponse($"Remove cart item {itemId}");
    }
}
