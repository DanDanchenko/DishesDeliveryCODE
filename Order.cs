using System;
using System.Collections.Generic;

namespace DishesDelivery.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        public int Id { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? CloseTime { get; set; }
        public int? State { get; set; }
        public int ClientId { get; set; }
        public string? Address { get; set; }
        public int? ChefId { get; set; }
        public int? CurierId { get; set; }

        public virtual User Client { get; set; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
