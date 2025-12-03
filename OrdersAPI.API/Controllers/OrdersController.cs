using Microsoft.AspNetCore.Mvc;
using OrdersAPI.Core.Interfaces;
using OrdersAPI.Core.Models;
using StackExchange.Redis;
using Order = OrdersAPI.Core.Models.Order;
namespace OrdersAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController(
            IOrderRepository _orderRepository,
            ICacheService _cacheService,
            ILogger<OrdersController> _logger) : ControllerBase
    {
        private const string CacheKeyPrefix = "order_";
        private static readonly TimeSpan CacheTTL = TimeSpan.FromMinutes(5);

        // POST /api/orders
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] Order order)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdOrder = await _orderRepository.CreateAsync(order);
                _logger.LogInformation("Order {OrderId} created successfully", createdOrder.OrderId);

                return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.OrderId }, createdOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return StatusCode(500, "An error occurred while creating the order");
            }
        }

        // GET /api/orders/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderById(Guid id)
        {
            try
            {
                var cacheKey = $"{CacheKeyPrefix}{id}";

                // Try to get from cache first
                var cachedOrder = await _cacheService.GetAsync<Order>(cacheKey);
                if (cachedOrder != null)
                {
                    _logger.LogInformation("Order {OrderId} retrieved from cache", id);
                    return Ok(cachedOrder);
                }

                // If not in cache, get from database
                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null)
                {
                    _logger.LogWarning("Order {OrderId} not found", id);
                    return NotFound($"Order with ID {id} not found");
                }

                // Store in cache
                await _cacheService.SetAsync(cacheKey, order, CacheTTL);
                _logger.LogInformation("Order {OrderId} retrieved from database and cached", id);

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order {OrderId}", id);
                return StatusCode(500, "An error occurred while retrieving the order");
            }
        }

        // GET /api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            try
            {
                var orders = await _orderRepository.GetAllAsync();
                _logger.LogInformation("Retrieved {Count} orders from database and cached", orders.Count());
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders");
                return StatusCode(500, "An error occurred while retrieving orders");
            }
        }

        // DELETE /api/orders/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            try
            {
                //Remove from DB
                var deleted = await _orderRepository.DeleteAsync(id);
                if (!deleted)
                {
                    _logger.LogWarning("Order {OrderId} not found for deletion", id);
                    return NotFound($"Order with ID {id} not found");
                }

                // Remove from cache
                var cacheKey = $"{CacheKeyPrefix}{id}";
                await _cacheService.RemoveAsync(cacheKey);
                _logger.LogInformation("Order {OrderId} deleted successfully", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order {OrderId}", id);
                return StatusCode(500, "An error occurred while deleting the order");
            }
        }
    }
}