using System;
using System.Collections.Generic;

namespace DishesDelivery.Models
{
    public partial class Product
    {
        public Product()
        {
            Ingradients = new HashSet<Ingradient>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Ingradient> Ingradients { get; set; }
    }
}
