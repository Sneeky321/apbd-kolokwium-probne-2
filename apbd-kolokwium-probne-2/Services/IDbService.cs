using apbd_kolokwium_probne_2.DTOs;
using apbd_kolokwium_probne_2.Models;

namespace apbd_kolokwium_probne_2.Services;

public interface IDbService
{
    Task<OrderDto> GetOrderById(int orderId);
    Task FulfillOrder(int orderId, FulfillOrderDto dto);
}