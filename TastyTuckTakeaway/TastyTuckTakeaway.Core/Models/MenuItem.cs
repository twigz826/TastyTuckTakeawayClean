namespace TastyTuckTakeaway.Core.Models
{
    public class MenuItem
    {
        public MenuItem(int id, string name, double price, string category)
        {
            Id = id;
            Name = name;
            Price = price;
            Category = category;
        }

        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public double Price { get; set; }

        public string Category { get; set; } = string.Empty;
    }
}