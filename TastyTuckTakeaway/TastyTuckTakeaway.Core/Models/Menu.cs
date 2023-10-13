using System.Text.Json;

namespace TastyTuckTakeaway.Core.Models
{
    public interface IMenu
    {
        List<MenuItem> GetMenuItems();
        MenuItem? GetMenuItemById(int id);
    }

    public class Menu : IMenu
    {
        private static List<MenuItem>? _cachedMenuItems;
        private const string MENU_FILE_PATH = "menu.json";

        public Menu()
        {
            if (_cachedMenuItems == null)
            {
                _cachedMenuItems = LoadItemsFromJson(MENU_FILE_PATH);
            }
        }

        public List<MenuItem> GetMenuItems()
        {
            return new List<MenuItem>(_cachedMenuItems!);
        }

        public MenuItem? GetMenuItemById(int id)
        {
            var menuItem = _cachedMenuItems!.FirstOrDefault(item => item.Id == id);

            return menuItem;
        }

        private static List<MenuItem> LoadItemsFromJson(string filePath)
        {
            try
            {
                var jsonOptions = GetPropNameCaseInsensitiveOptions();
                var menuItems = JsonSerializer.Deserialize<List<MenuItem>>
                (
                    File.ReadAllText(path: filePath), jsonOptions
                );

                return menuItems ?? new List<MenuItem>();
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error: Menu file not found - {ex.Message}");
                return new List<MenuItem>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error: Invalid JSON format - {ex.Message}");
                return new List<MenuItem>();
            }

        }

        private static JsonSerializerOptions GetPropNameCaseInsensitiveOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
        }
    }
}