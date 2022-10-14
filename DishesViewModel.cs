using Microsoft.AspNetCore.Http;
namespace DishesDelivery.Models
{
    public class DishesViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IFormFile Avatar { get; set; }

        public byte[]? Image { get; set; }
    }
}
