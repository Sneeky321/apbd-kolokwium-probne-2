using apbd_kolokwium_probne_2.Data;
using apbd_kolokwium_probne_2.DTOs;
using apbd_kolokwium_probne_2.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace apbd_kolokwium_probne_2.Services;

public class DbService : IDbService
{
    private readonly OrdersContext _context;
    
    public DbService(OrdersContext context)
    {
        _context = context;
    }

    public async Task<OrderDto> GetOrderById(int orderId)
    {
        var order = await _context.Orders
            .Select(e => new OrderDto
            {
                Id = e.Id,
                CreatedAt = e.CreatedAt,
                FulfilledAt = e.FulfilledAt,
                Status = e.Status.Nam,
                Client = new ClientInfoDto()
                {
                    FirstName = e.Client.FirstName,
                    LastName = e.Client.LastName,
                },
                Products = e.ProductOrders.Select(e => new OrderLineItemDto()
                {
                    Name = e.Product.Name,
                    Price = e.Product.Price,
                    Amount = e.Amount
                }).ToList()
            })
            .FirstOrDefaultAsync(e => e.Id == orderId);

        if (order is null)
            throw new NotFoundException();

        return order;
    }

    public async Task FulfillOrder(int orderId, FulfillOrderDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order is null)
            {
                throw new NotFoundException("Order not found");
            }

            var status = await _context.Statuses
                .FirstOrDefaultAsync(s => s.Nam.Equals(dto.StatusName));

            if (status is null)
            {
                throw new NotFoundException("Status not found");
            }

            if (order.FulfilledAt != null)
            {
                throw new Exception("Conflict");
            }

            order.StatusId = status.Id;
            order.FulfilledAt = DateTime.Now;

            var relatedProducts = _context.ProductOrders
                .Where(po => po.OrderId == orderId);
            _context.ProductOrders.RemoveRange(relatedProducts);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}