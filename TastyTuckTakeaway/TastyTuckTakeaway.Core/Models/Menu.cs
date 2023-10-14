﻿using Microsoft.Extensions.Logging;
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
        private readonly ILogger<Menu> _logger;

        public Menu(ILogger<Menu> logger)
        {
            if (_cachedMenuItems == null)
            {
                _cachedMenuItems = LoadItemsFromJson(MENU_FILE_PATH);
            }

            _logger = logger;
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

        private List<MenuItem> LoadItemsFromJson(string filePath)
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
                _logger.LogError("Error: Menu file not found - {ex.Message}", ex);
                return new List<MenuItem>();
            }
            catch (JsonException ex)
            {
                _logger.LogError("Error: Invalid JSON format - {ex.Message}", ex);
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