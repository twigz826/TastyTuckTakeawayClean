using TastyTuckTakeaway.Core.Models;

namespace TastyTuckTakeaway.Core.Tests.Models
{
    public class MenuTests
    {
        private readonly Menu _menu;
        public MenuTests()
        {
            _menu = new Menu();
        }

        [Fact]
        public void Menu_GetMenuItems_ReturnsAListOfMenuItems()
        {
            var items = _menu.GetMenuItems();
            items.Should().BeOfType<List<MenuItem>>();
        }

        [Fact]
        public void Menu_GetMenuItems_ReturnsAMenuContainingSpecificMenuItems()
        {
            var items = _menu.GetMenuItems();
            items.FirstOrDefault(i => i.Name == "Spring Rolls").Should().NotBeNull();
            items.FirstOrDefault(i => i.Name == "Chicken Balls").Should().NotBeNull();
        }

        [Fact]
        public void Menu_GetMenuItems_ReturnsAllMenuItemsFromTheInMemoryDatastore()
        {
            var items = _menu.GetMenuItems();
            items.Count.Should().Be(28);
        }

        [Fact]
        public void Menu_GetMenuItemById_ReturnsTheMenuItemIfItExists()
        {
            var expectedName = "1/2 Aromatic Duck";
            var item = _menu.GetMenuItemById(7);
            item?.Name.Should().Be(expectedName);
        }

        [Fact]
        public void Menu_GetMenuItemById_ReturnsNullIfTheMenuItemDoesNotExist()
        {
            var item = _menu.GetMenuItemById(35);
            item?.Should().BeNull();
        }
    }
}