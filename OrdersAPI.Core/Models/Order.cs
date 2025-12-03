using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdersAPI.Core.Models
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Product { get; set; } = string.Empty;

        [Range(1, double.MaxValue, ErrorMessage = "Amount must be at least 1")]
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
