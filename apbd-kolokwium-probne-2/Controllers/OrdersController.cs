using apbd_kolokwium_probne_2.DTOs;
using apbd_kolokwium_probne_2.Services;
using apbd_kolokwium_probne_2.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace apbd_kolokwium_probne_2.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IDbService _dbService;
    
    public OrdersController(IDbService dbService)
    {
        _dbService = dbService;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrder([FromRoute] int id)
    {
        try
        {
            var order = await _dbService.GetOrderById(id);
            return Ok(order);
        }
        catch (NotFoundException e)
        {
            return NotFound();
        }
    }

    [HttpPut("{orderId:int}/fulfill")]
    public async Task<IActionResult> FulfillOrder([FromRoute] int orderId, FulfillOrderDto dto)
    {
        try
        {
            await _dbService.FulfillOrder(orderId, dto);
            return Ok();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return Conflict(e.Message);
        }
    }
}