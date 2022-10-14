using System;
using System.Collections.Generic;

namespace DishesDelivery.Models
{
    public partial class Dish
    {
        public Dish()
        {
            Ingradients = new HashSet<Ingradient>();
            OrderItems = new HashSet<OrderItem>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public byte[]? Image { get; set; }

        

        public virtual ICollection<Ingradient> Ingradients { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
