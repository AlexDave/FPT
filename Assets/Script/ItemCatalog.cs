
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class ItemCatalog
{
    // Справочник предметов
    public Dictionary<string, Item> itemDictionary = new Dictionary<string, Item>();

    public ItemCatalog()
    {
        LoadDefaultItems(); // Загружаем стандартные предметы
    }

    // Метод для получения предмета из справочника по имени
    public Item GetItem(string itemName)
    {
        if (itemDictionary.ContainsKey(itemName))
        {
            return itemDictionary[itemName];
        }
        else
        {
            Debug.LogWarning("Предмет не найден в справочнике: " + itemName);
            return null;
        }
    }

    // Метод для добавления предмета в справочник
    public void AddItem(Item newItem)
    {
        if (!itemDictionary.ContainsKey(newItem.itemName))
        {
            itemDictionary.Add(newItem.itemName, newItem);
            Debug.Log($"Предмет {newItem.itemName} добавлен в справочник.");
        }
        else
        {
            Debug.LogWarning($"Предмет {newItem.itemName} уже существует в справочнике.");
        }
    }

    // Метод для удаления предмета из справочника
    public void RemoveItem(string itemName)
    {
        if (itemDictionary.ContainsKey(itemName))
        {
            itemDictionary.Remove(itemName);
            Debug.Log($"Предмет {itemName} удален из справочника.");
        }
        else
        {
            Debug.LogWarning($"Предмет {itemName} не найден для удаления.");
        }
    }

    // Метод для загрузки стандартных предметов (без JSON)
    private void LoadDefaultItems()
    {
        itemDictionary.Add("Посох отца", new Item
        {
            itemName = "Посох отца",
            level = 1,
            damage = 10f,
            cooldown = 1f,
            multiplier = 1f,
            imagePath = "Assets/Images/Weapons/staff.png"
        });

        itemDictionary.Add("Огненный шар", new Item
        {
            itemName = "Огненный шар",
            level = 1,
            damage = 8f,
            cooldown = 1.5f,
            multiplier = 1.2f,
            imagePath = "Assets/Images/Weapons/fireball.png"
        });

        itemDictionary.Add("Волна", new Item
        {
            itemName = "Волна",
            level = 1,
            damage = 12f,
            cooldown = 2f,
            multiplier = 1.1f,
            imagePath = "Assets/Images/Weapons/wave.png"
        });

        itemDictionary.Add("Тройной клинок", new Item
        {
            itemName = "Тройной клинок",
            level = 1,
            damage = 12f,
            cooldown = 2f,
            multiplier = 1.1f,
            imagePath = "Assets/Images/Weapons/triple_blade.png"
        });

        itemDictionary.Add("Каменный молот", new Item
        {
            itemName = "Каменный молот",
            level = 1,
            damage = 12f,
            cooldown = 2f,
            multiplier = 1.1f,
            imagePath = "Assets/Images/Weapons/stone_hammer.png"
        });

        itemDictionary.Add("Сосулька", new Item
        {
            itemName = "Сосулька",
            level = 1,
            damage = 12f,
            cooldown = 2f,
            multiplier = 1.1f,
            imagePath = "Assets/Images/Weapons/ice_shard.png"
        });
    }
}