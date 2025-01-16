namespace MercylandAdmin.Models
{
    public class PropertyDTO
    {
        public string Category { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string State { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Bedroom { get; set; }
        public string Bathroom { get; set; }
        public string Surface { get; set; }
        public string Year { get; set; }
        public string Price { get; set; }
        public IFormFile Image { get; set; }
    }
}
