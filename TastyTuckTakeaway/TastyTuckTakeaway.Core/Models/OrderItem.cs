namespace TastyTuckTakeaway.Core.Models
{
    public class OrderItem
    {
        private OrderItem(MenuItem menuItem, int quantity)
        {
            MenuItem = menuItem;
            Quantity = quantity;
        }

        public MenuItem MenuItem { get; set; }
        public int Quantity { get; set; }

        public static OrderItem Create(MenuItem menuItem, int quantity)
        {
            return new OrderItem(menuItem, quantity);
        }
    }
}