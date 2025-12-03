using System.ComponentModel.DataAnnotations;

namespace OrdersAPI.API.Dtos.OrderDtos
{
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "Customer name is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Customer name must be between 2 and 200 characters")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Product must be between 2 and 200 characters")]
        public string Product { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
    }
}
