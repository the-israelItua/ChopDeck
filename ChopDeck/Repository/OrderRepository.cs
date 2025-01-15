using ChopDeck.Data;
using ChopDeck.Dtos.Orders;
using ChopDeck.Dtos.Restaurants;
using ChopDeck.Enums;
using ChopDeck.helpers;
using ChopDeck.Interfaces;
using ChopDeck.Mappers;
using ChopDeck.Models;
using Microsoft.EntityFrameworkCore;

namespace ChopDeck.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDBContext _applicationDBContext;
        public OrderRepository(ApplicationDBContext applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
        }
        public async Task<List<RestaurantOrderListDto>> GetRestaurantOrdersAsync(string userId, RestaurantOrdersQueryObject queryObject)
        {
            var query = _applicationDBContext.Orders
                .Where(o => o.Restaurant.ApplicationUserId == userId && o.Status == queryObject.Status)
                .Select(o => new RestaurantOrderListDto
                {
                    OrderId = o.Id,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    Amount = o.OrderItems.Sum(oi => oi.Quantity * oi.Product.Price),
                    OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        Product = oi.Product.ToProductDto(),
                        Quantity = oi.Quantity,
                    }).ToList()
                });


            var skipNumber = (queryObject.PageNumber - 1) * queryObject.PageSize;

            return await query.Skip(skipNumber).Take(queryObject.PageSize).ToListAsync();
        }
        public async Task<List<RestaurantOrderListDto>> GetPreparedOrdersAsync(PaginationQueryObject queryObject)
        {
            var query = _applicationDBContext.Orders
                .Where(o => o.Status == OrderStatus.OrderPrepared.ToString())
                .Select(o => new RestaurantOrderListDto
                {
                    OrderId = o.Id,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    Amount = o.OrderItems.Sum(oi => oi.Quantity * oi.Product.Price),
                    OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        Product = oi.Product.ToProductDto(),
                        Quantity = oi.Quantity,
                    }).ToList()
                });


            var skipNumber = (queryObject.PageNumber - 1) * queryObject.PageSize;

            return await query.Skip(skipNumber).Take(queryObject.PageSize).ToListAsync();
        }
        public async Task<RestaurantOrderDto?> GetRestaurantOrderByIdAsync(int id, string userId)
        {
            return await _applicationDBContext.Orders
        .Where(o => o.Id == id && o.Restaurant.ApplicationUserId == userId)

        .Select(o => new RestaurantOrderDto
        {
            OrderId = o.Id,
            Status = o.Status,
            CreatedAt = o.CreatedAt,
            CustomerId = o.Customer.Id,
            Amount = o.OrderItems.Sum(oi => oi.Quantity * oi.Product.Price),
            OrderItems = o.OrderItems.Select(oi => new OrderItemDto
            {
                Id = oi.Id,
                Product = oi.Product.ToProductDto(),
                Quantity = oi.Quantity,
            }).ToList()
        }).FirstOrDefaultAsync();
        }
        public async Task<List<Order>> GetCustomerOrdersAsync(string userId, CustomerOrdersQueryObject queryObject)
        {
            var orders = _applicationDBContext.Orders.Include(c => c.OrderItems!).ThenInclude(c => c.Product).Include(c => c.Restaurant).ThenInclude(c => c.ApplicationUser).Include(c => c.Customer!).ThenInclude(c => c.ApplicationUser).Where(o => o.Customer.ApplicationUserId == userId).AsQueryable();

            if (queryObject.Status == CustomerOrderStatus.Pending.ToString())
            {
                orders = orders.Where(p => p.Status == OrderStatus.PendingPayment.ToString() || p.Status == OrderStatus.PendingRestaurantConfirmation.ToString());
            }

            if (queryObject.Status == CustomerOrderStatus.Ongoing.ToString())
            {
                orders = orders.Where(p => p.Status != OrderStatus.PendingPayment.ToString() || p.Status != OrderStatus.PendingRestaurantConfirmation.ToString() || p.Status != OrderStatus.OrderDelivered.ToString());
            }

            if (queryObject.Status == CustomerOrderStatus.Completed.ToString())
            {
                orders = orders.Where(p => p.Status == OrderStatus.OrderDelivered.ToString());
            }

            var skipNumber = (queryObject.PageNumber - 1) * queryObject.PageSize;
            return await orders.Skip(skipNumber).Take(queryObject.PageSize).ToListAsync();
        }

        public async Task<Order?> GetCustomerOrderByIdAsync(int id, string userId)
        {
            return await _applicationDBContext.Orders.Include(c => c.OrderItems!).ThenInclude(c => c.Product).Include(c => c.Restaurant).ThenInclude(c => c.ApplicationUser).Include(c => c.Customer!).ThenInclude(c => c.ApplicationUser).FirstOrDefaultAsync(s => s.Id == id && s.Customer.ApplicationUserId == userId);
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _applicationDBContext.Orders.Include(c => c.OrderItems!).ThenInclude(c => c.Product).Include(c => c.Restaurant).ThenInclude(c => c.ApplicationUser).Include(c => c.Customer!).ThenInclude(c => c.ApplicationUser).Include(c => c.Driver).ThenInclude(c => c.ApplicationUser).FirstOrDefaultAsync(s => s.Id == id);
        }
        public async Task<Order> UpdateOrderAsync(Order order)
        {
            _applicationDBContext.Orders.Update(order);
            await _applicationDBContext.SaveChangesAsync();
            return order;
        }
    }
}