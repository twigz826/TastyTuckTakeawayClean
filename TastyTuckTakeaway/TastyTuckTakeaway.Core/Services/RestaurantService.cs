using TastyTuckTakeaway.Core.Models;

namespace TastyTuckTakeaway.Core.Services
{
    public interface IRestaurantService
    {
        void ShowMenu();

        MenuItem? GetMenuItemById(int itemId);

        bool AddItemsToOrder(int itemId, int quantity);

        bool RemoveItemFromOrder(int itemId);

        bool EditItemInOrder(int itemId, int quantity);

        void PrintBasket();

        bool IsBasketEmpty();

        double CalculateTotalOrderCost();

        bool FinaliseOrder();

        void ClearBasket();

        int GetOrderNumber();

        void AddAddressToOrder(string houseNumber, string streetName, string postcode);

        IEnumerable<OrderItem> GetBasketItems();
    }

    public class RestaurantService : IRestaurantService
    {
        private readonly IMenu _menu;
        private readonly Order _order;

        public RestaurantService(IMenu menu, Order order)
        {
            _menu = menu;
            _order = order;
        }

        public void ShowMenu()
        {
            var items = _menu.GetMenuItems();
            ShowCategory(items.Where(i => i.Category == "starters"));
            ShowCategory(items.Where(i => i.Category == "mains"));
            ShowCategory(items.Where(i => i.Category == "desserts"));
            ShowCategory(items.Where(i => i.Category == "drinks"));
        }

        public MenuItem? GetMenuItemById(int itemId)
        {
            return _menu.GetMenuItemById(itemId);
        }

        public bool AddItemsToOrder(int itemId, int quantity)
        {
            return _order.AddItemToBasket(itemId, quantity);
        }

        public bool RemoveItemFromOrder(int itemId)
        {
            return _order.RemoveItemFromBasket(itemId);
        }

        public bool EditItemInOrder(int itemId, int quantity)
        {
            return _order.EditQuantity(itemId, quantity);
        }

        public void PrintBasket()
        {
            var basket = GetBasketItems();
            foreach (var item in basket)
            {
                Console.WriteLine($"Number: {item.MenuItem.Id}, Name: {item.MenuItem.Name}, Quantity: {item.Quantity}, Price: £{item.MenuItem.Price * item.Quantity}");
            }
        }

        public double CalculateTotalOrderCost()
        {
            return _order.CalculateTotal();
        }

        public bool IsBasketEmpty()
        {
            return _order.IsBasketEmpty();
        }

        public bool FinaliseOrder()
        {
            return _order.FinaliseOrder();
        }

        public void AddAddressToOrder(string houseNumber, string streetName, string postcode)
        {
            _order.UpdateDeliveryAddress(houseNumber, streetName, postcode);
        }

        public void ClearBasket()
        {
            if (IsBasketEmpty())
            {
                Console.WriteLine("Basked is already empty");
                return;
            }
            _order.ClearBasket();
            Console.WriteLine("Basket is now empty");
        }

        public IEnumerable<OrderItem> GetBasketItems()
        {
            return _order.ViewItemsInBasket();
        }

        private void ShowCategory(IEnumerable<MenuItem> categoryItems)
        {
            Console.WriteLine($"{categoryItems.First().Category.ToUpper()}:");
            foreach (var item in categoryItems)
            {
                Console.WriteLine($"Number: {item.Id}; Name: {item.Name}, Price: £{item.Price}");
            }
        }

        public int GetOrderNumber()
        {
            return _order.OrderNumber;
        }
    }
}