using System;
using System.Collections.Generic;

namespace DishesDelivery.Models
{
    public partial class Ingradient
    {
        public int Id { get; set; }
        public int DishId { get; set; }
        public int ProductId { get; set; }
        public int Amount { get; set; }

        public virtual Dish Dish { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
