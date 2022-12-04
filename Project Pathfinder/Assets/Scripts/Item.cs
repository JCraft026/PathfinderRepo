using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    public enum ItemType{
        Pickaxe, // Pickaxe
        Keys_0,  // Red Key
        Keys_1,  // Blue Key
        Keys_2,  // Green Key
        Keys_3   // Yellow Key
    }
    public ItemType itemType; // Holds the enum type of the item
    public int amount;        // The amount of each item
    public bool selected;     // Whether the item is selected or not

    // Returns the correct sprite that relates with the item
    public Sprite GetSprite(){
        switch(itemType){
            default:
            case ItemType.Pickaxe: return ItemAssets.Instance.pickaxeSprite;
            case ItemType.Keys_0:  return ItemAssets.Instance.Keys0Sprite;
            case ItemType.Keys_1:  return ItemAssets.Instance.Keys1Sprite;
            case ItemType.Keys_2:  return ItemAssets.Instance.Keys2Sprite;
            case ItemType.Keys_3:  return ItemAssets.Instance.Keys3Sprite;
        }
    }

    // Returns whether an item can stack or not
    public bool isStackable(){
        switch(itemType){
        default:
            return true;
        case ItemType.Pickaxe:
            return false;
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
}
