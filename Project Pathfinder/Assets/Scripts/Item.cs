using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    static class ItemConstants{
        public const int COMMONGROUNDSCOFFEE = 1; // Common grounds coffee item ID
        public const int GREENSCREENSUIT     = 2; // Green screen suit item ID
        public const int SLEDGEHAMMER        = 3; // Sledgehammer item ID
        public const int SMOKEBOMB           = 4; // Smoke bomb item ID
    }

    public enum ItemType{
        Sledge,    // Sledge Hammer
        SmokeBomb, // Smoke Bomb
        Coffee,    // Common Grounds Coffee 
        GreenScreenSuit, 
                   // Green Screen Suit
        Keys_0,    // Red Key
        Keys_1,    // Blue Key
        Keys_2,    // Green Key
        Keys_3,    // Yellow Key
        None       // Empty item type
    }
    public ItemType itemType; // Holds the enum type of the item
    public int amount;        // The amount of each item
    public bool selected;     // Whether the item is selected or not

    public static int greenScreenSpawnLimit   = 5;  // Spawn in chest limit for the green screen suit
    public static int smokeBombSpawnLimit     = 11; // Spawn in chest limit for the smoke bomb
    public static int coffeeSpawnLimit        = 8;  // Spawn in chest limit for the common grounds coffee
    public static int sledgehammerSpawnLimit  = 1;  // Spawn in chest limit for the sledgehammer
    public static int sledgehammerSpawnChance = 12;  // Chance of the runner picking up the sledgehammer
    public static int initialGSSpawnLimit     = 5;  // Initial value for green screen suit spawn limit
    public static int initialSBSpawnLimit     = 11; // Initial value for smoke bomb spawn limit
    public static int initialCFSpawnLimit     = 8;  // Initial value for common grounds coffee spawn limit
    public static int initialSHSpawnLimit     = 1;  // Initial value for sledgehammer spawn limit

    // Returns the correct sprite that relates with the item
    public Sprite GetSprite(){
        switch(itemType){
            default:
            case ItemType.Sledge:    return ItemAssets.Instance.sledgeSprite;
            case ItemType.SmokeBomb: return ItemAssets.Instance.smokeBombSprite;
            case ItemType.Coffee:    return ItemAssets.Instance.coffeeSprite;
            case ItemType.GreenScreenSuit:
                                     return ItemAssets.Instance.greenScreenSuitSprite;
            case ItemType.Keys_0:    return ItemAssets.Instance.Keys0Sprite;
            case ItemType.Keys_1:    return ItemAssets.Instance.Keys1Sprite;
            case ItemType.Keys_2:    return ItemAssets.Instance.Keys2Sprite;
            case ItemType.Keys_3:    return ItemAssets.Instance.Keys3Sprite;
        }
    }

    // Returns whether an item can stack or not
    public bool isStackable(){
        switch(itemType){
        default:
            return true;
        case ItemType.Sledge:
            return false;
        case ItemType.SmokeBomb:
            return true;
        case ItemType.Coffee:
            return true;
        case ItemType.Keys_0:
            return false;
        case ItemType.Keys_1:
            return false;
        case ItemType.Keys_2:
            return false;
        case ItemType.Keys_3:
            return false;
        }
    }

    // Returns whether an item is selected in the inventory or not 
    public bool isSelected(){
        if (selected == true)
            return true;
        else{
            return false;
        }
    }

    // Returns whether an item is a key or a regular item
    public bool isKey(){
        switch(itemType){
            default: return false;
            case ItemType.Keys_0: return true;
            case ItemType.Keys_1: return true;
            case ItemType.Keys_2: return true;
            case ItemType.Keys_3: return true;
        }
    }

    // Give an item to the user opening a chest
    public static Item getChestItem(bool isTemporaryItem){
        Item item                 = new Item();
        List<int> validSpawnItems = new List<int>();

        // Get placeholder item for initial chest spawn
        if(isTemporaryItem){
            item.itemType = ItemType.Sledge;
        }

        // Get permanent item for opening chest
        else{
            // Fill list of valid item to spawn
            if(coffeeSpawnLimit > 0){
                for (int itemCount = 0; itemCount < coffeeSpawnLimit; itemCount++)
                {
                    validSpawnItems.Add(ItemConstants.COMMONGROUNDSCOFFEE);
                }
            }
            if(greenScreenSpawnLimit > 0){
                for (int itemCount = 0; itemCount < greenScreenSpawnLimit; itemCount++)
                {
                    validSpawnItems.Add(ItemConstants.GREENSCREENSUIT);
                }
            }
            if(sledgehammerSpawnLimit > 0){
                for (int itemCount = 0; itemCount < sledgehammerSpawnChance; itemCount++)
                {
                    validSpawnItems.Add(ItemConstants.SLEDGEHAMMER);
                }
            }
            if(smokeBombSpawnLimit > 0){
                for (int itemCount = 0; itemCount < smokeBombSpawnLimit; itemCount++)
                {
                    validSpawnItems.Add(ItemConstants.SMOKEBOMB);
                }
            }

            // Get random valid spawn item index
            var randomValidSpawnIndex = UnityEngine.Random.Range(0, validSpawnItems.Count);

            // Select random item
            switch(validSpawnItems[randomValidSpawnIndex])
            {
                case ItemConstants.COMMONGROUNDSCOFFEE:
                    item.itemType = ItemType.Coffee;
                    coffeeSpawnLimit--;
                    break;
                case ItemConstants.GREENSCREENSUIT:
                    item.itemType = ItemType.GreenScreenSuit;
                    greenScreenSpawnLimit--;
                    break;
                case ItemConstants.SLEDGEHAMMER:
                    item.itemType = ItemType.Sledge;
                    sledgehammerSpawnLimit--;
                    break;
                case ItemConstants.SMOKEBOMB:
                    item.itemType = ItemType.SmokeBomb;
                    smokeBombSpawnLimit--;
                    break;
            }
        }

        // Limit items per chest to 1
        item.amount = 1;
        
        return item;
    }

    public static Item GetKey(int keyNum)
    {
        Item key = new Item();
        switch(keyNum)
        {
            case 0:
                key.itemType = Item.ItemType.Keys_0;
            break;
            case 1:
                key.itemType = Item.ItemType.Keys_1;
            break;
            case 2:
                key.itemType = Item.ItemType.Keys_2;
            break;
            case 3:
                key.itemType = Item.ItemType.Keys_3;
            break;
        }
        key.amount = 1;
        return key;
    }
}
