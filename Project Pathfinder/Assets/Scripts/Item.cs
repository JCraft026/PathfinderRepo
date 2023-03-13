using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
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

    public static Item getRandomItem(bool canBeSledge=true){
        Item item = new Item();
        switch(UnityEngine.Random.Range(0,4))
        {
            default:
                Debug.LogWarning("RNG generated unsupported item type");
                break;
            case 0:
                if(canBeSledge)
                {
                    item.itemType = ItemType.Sledge;
                    Debug.Log(item.itemType);
                }
                else
                {
                    item = getRandomItem(false); // recursively generate a different random item that is not a sledge
                    Debug.Log("Found a sledge, generated a " + item.itemType + " instead");
                }
                break;
            case 1:
                item.itemType = ItemType.Coffee;
                Debug.Log(item.itemType);
                break;
            case 2:
                item.itemType = ItemType.GreenScreenSuit;
                Debug.Log(item.itemType);
                break;
            case 3:
                item.itemType = ItemType.SmokeBomb;
                Debug.Log(item.itemType);
                break;

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