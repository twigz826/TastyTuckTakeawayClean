using FakeItEasy;
using System.Collections.ObjectModel;
using TastyTuckTakeaway.Core.Models;

namespace TastyTuckTakeaway.Core.Tests.Models
{
    public class OrderTests
    {
        private readonly IMenu _menu;
        public OrderTests()
        {
            _menu = A.Fake<IMenu>();
            A.CallTo(() => _menu.GetMenuItemById(1)).Returns(new MenuItem(1, "Spring Rolls", 5, "starters"));
        }

        [Fact]
        public void Order_AddItemToBasketWithMenuItemId_AddsTheItemToTheOrderBasket()
        {
            var order = new Order(_menu);

            order.AddItemToBasket(1);
            order.Basket.Should().BeOfType<List<OrderItem>>();

            var basketItem = order.Basket.First();
            basketItem.MenuItem.Id.Should().Be(1);
        }

        [Fact]
        public void Order_AddItemToBasketWithMenuItemId_AddsMultipleSameItemsToTheOrderBasket()
        {
            var order = new Order(_menu);

            order.AddItemToBasket(1);
            order.AddItemToBasket(1);
            order.AddItemToBasket(1, 3);
            order.Basket.Should().HaveCount(1);

            var basketItem = order.Basket.First(i => i.MenuItem.Id == 1);
            basketItem.Quantity.Should().Be(5);
        }

        [Fact]
        public void Order_AddItemToBasketWithMenuItemId_AddsDifferentItemsToTheOrderBasket()
        {
            A.CallTo(() => _menu.GetMenuItemById(5)).Returns(new MenuItem(5, "Crispy Seaweed", 3.50, "starters"));
            var order = new Order(_menu);

            order.AddItemToBasket(1);
            order.AddItemToBasket(5);
            order.Basket.Should().HaveCount(2);
        }

        [Fact]
        public void Order_SeeAllItemsInBasket_ReturnsTheOrderBasket()
        {
            var order = new Order(_menu);

            order.AddItemToBasket(1, 3);
            order.ViewItemsInBasket().Should().BeOfType<ReadOnlyCollection<OrderItem>>();
        }

        [Fact]
        public void Order_CalculateTotal_ReturnsTheTotalCostOfItemsInBasket()
        {
            var order = new Order(_menu);

            order.CalculateTotal().Should().Be(0);

            var expectedValue = 30;
            order.AddItemToBasket(1, 6);
            order.CalculateTotal().Should().Be(expectedValue);
        }

        [Fact]
        public void Order_RemoveItemFromBasket_RemovesTheItemFromTheOrderBasket()
        {
            var order = new Order(_menu);

            order.AddItemToBasket(1, 6);
            order.RemoveItemFromBasket(1);
            order.Basket.Should().BeEmpty();
        }

        [Fact]
        public void Order_RemoveItemFromBasket_ThrowsExceptionIfNoItemsWithMatchingIdInBasket()
        {
            var order = new Order(_menu);

            order.AddItemToBasket(1, 6);
            order.RemoveItemFromBasket(2).Should().BeFalse();
        }

        [Fact]
        public void Order_EditQuantity_EditsTheQuantityOfAnItemInTheBasket()
        {
            var order = new Order(_menu);
            var expectedBasketValue = 15;

            order.AddItemToBasket(1, 6);
            order.EditQuantity(1, 3);

            var basketItem = order.Basket.First(i => i.MenuItem.Id == 1);
            basketItem.Quantity.Should().Be(3);
            order.CalculateTotal().Should().Be(expectedBasketValue);
        }

        [Fact]
        public void Order_PlaceOrder_ConfirmsThatPaymentIsNowRequired()
        {
            A.CallTo(() => _menu.GetMenuItemById(5)).Returns(new MenuItem(5, "Crispy Seaweed", 3.50, "starters"));
            var order = new Order(_menu);

            order.AddItemToBasket(1, 2);
            order.AddItemToBasket(5, 2);
            order.FinaliseOrder().Should().BeTrue();
        }

        [Fact]
        public void Order_PlaceOrder_ReturnsFalseIfBasketIsEmpty()
        {
            var order = new Order(_menu);
            order.FinaliseOrder().Should().BeFalse();
        }
    }
}