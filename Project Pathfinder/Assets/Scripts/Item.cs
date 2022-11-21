using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    public enum ItemType{
        Sword,
        Pickaxe,
        HealthPack
    }
    public ItemType itemType;
    public int amount;
    public bool selected;

    public Sprite GetSprite(){
        switch(itemType){
            default:
            //case ItemType.Sword: return ItemAssets.Instance.swordSprite;
            case ItemType.Pickaxe: return ItemAssets.Instance.pickaxeSprite;
            //case ItemType.HealthPack: return ItemAssets.Instance.healthPackSprite;
        }
    }

    public bool isStackable(){
        switch(itemType){
        default:
            return true;
        //case ItemType.HealthPack:
        //    return true;
        //case ItemType.Sword:
        case ItemType.Pickaxe:
            return false;
        }
    }

    public bool isSelected(){
        if (selected == true)
            return true;
        else{
            return false;
        }
    }
}
