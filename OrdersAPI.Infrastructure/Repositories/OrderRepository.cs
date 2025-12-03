using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrdersAPI.Core.Interfaces;
using OrdersAPI.Core.Models;
using OrdersAPI.Infrastructure.Data;

namespace OrdersAPI.Infrastructure.Repositories
{
    public class OrderRepository(ApplicationDbContext _context) : IOrderRepository
    {
        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders.OrderByDescending(o => o.CreatedAt).ToListAsync();
        }
        public async Task<Order?> GetByIdAsync(Guid id)
        {
            return await _context.Orders.FindAsync(id);
        }
        public async Task<Order> CreateAsync(Order order)
        {
            order.OrderId = Guid.NewGuid();
            order.CreatedAt = DateTime.UtcNow;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return false;
            _context.Orders.Remove(order!);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
