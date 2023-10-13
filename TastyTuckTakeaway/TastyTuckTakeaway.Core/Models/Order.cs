namespace TastyTuckTakeaway.Core.Models
{
    public class Order
    {
        private readonly IMenu _menu;

        public Order(IMenu menu)
        {
            Basket = new List<OrderItem>();
            _menu = menu;
        }

        public int OrderNumber { get; private set; }

        public List<OrderItem> Basket { get; private set; }

        public Address? DeliveryAddress { get; private set; }

        public bool AddItemToBasket(int itemId, int quantity = 1)
        {
            var menuItem = _menu.GetMenuItemById(itemId);

            if (menuItem == null)
            {
                return false;
            }

            if (!Basket.Any(oi => oi.MenuItem == menuItem))
            {
                var orderItem = OrderItem.Create(menuItem, quantity);
                Basket.Add(orderItem);
            }
            else
            {
                Basket.First(oi => oi.MenuItem == menuItem).Quantity += quantity;
            }
            return true;
        }

        public IEnumerable<OrderItem> ViewItemsInBasket()
        {
            return Basket.AsReadOnly();
        }

        public bool RemoveItemFromBasket(int itemId)
        {
            if (!IsItemInBasket(itemId))
            {
                return false;
            }

            Basket.RemoveAll(oi => oi.MenuItem.Id == itemId);
            return true;
        }

        public bool EditQuantity(int itemId, int newQuantity)
        {
            if (!IsItemInBasket(itemId))
            {
                return false;
            }

            Basket.First(oi => oi.MenuItem.Id == itemId).Quantity = newQuantity;
            return true;
        }

        public void UpdateDeliveryAddress(string houseNumber, string streetName, string postcode)
        {
            var address = new Address(houseNumber, streetName, postcode);
            DeliveryAddress = address;
        }

        public double CalculateTotal()
        {
            double total = 0;
            if (Basket.Count == 0)
            {
                return 0;
            }

            foreach (var item in Basket)
            {
                total += item.MenuItem.Price * item.Quantity;
            }

            return total;
        }

        public bool FinaliseOrder()
        {
            if (IsBasketEmpty())
            {
                return false;
            }

            OrderNumber = GenerateRandomOrderNumber();

            //Console.WriteLine($"Your order total is £{CalculateTotal()}. Please provide additional information below to confirm your order");
            return true;
        }

        private static int GenerateRandomOrderNumber()
        {
            return new Random().Next(100000, 1000000);
        }

        public bool IsBasketEmpty()
        {
            return !Basket.Any();
        }

        public void ClearBasket()
        {
            Basket.Clear();
        }

        private bool IsItemInBasket(int itemId)
        {
            return Basket.Any(oi => oi.MenuItem.Id == itemId);
        }
    }
}