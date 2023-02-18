using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public event EventHandler OnItemListChanged; // Event recording every time the item list changes
    private List<Item> itemList;                 // List of items
    private Action<Item> useItemAction;          // Action using an item

    // Initiates the item list and the use item action 
    public Inventory(Action<Item> useItemAction)
    {
        this.useItemAction = useItemAction;
        itemList = new List<Item>();
    }

    // Adds the passed item into the inventory
    public void AddItem(Item item){
        // Checks to see if the item can stack or not
        if(item.isStackable()) {
            bool itemAlreadyInInventory = false;
            foreach (Item inventoryItem in itemList){
                if(inventoryItem.itemType == item.itemType){
                    inventoryItem.amount += item.amount;
                    itemAlreadyInInventory = true;
                }
            }
            // Checks to see if the item is already in the inventory
            if(!itemAlreadyInInventory){
                itemList.Add(item);
            }
        }
        else{
            itemList.Add(item);
        }
        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    // Removes the selected item from the inventory
    public void RemoveItem(Item item){
        if(item.isStackable()){
            Item itemInInventory = null;
            foreach (Item inventoryItem in itemList){
                if(inventoryItem.itemType == item.itemType){
                    inventoryItem.amount -= 1;
                    itemInInventory = inventoryItem;
                }
            }
            if (itemInInventory != null && itemInInventory.amount <= 0){
                itemList.Remove(itemInInventory);
            }
        }
        else{
            itemList.Remove(item);
        }
        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    // Use the selected item
    public void UseItem(Item item){
        Debug.Log("using item: " + item.itemType.ToString());
        useItemAction(item);
    }

    // Returns the list contained in the inventory
    public List<Item> GetItemList(){
        return itemList;
    }

    public bool anItemCanStack(Item item){
        bool listIsStackable = false;
        foreach(Item inventoryItem in GetItemList()){
            if (inventoryItem.isStackable()){
                if(inventoryItem.itemType == item.itemType)
                    listIsStackable = true;
            }
        }
        return listIsStackable;
    }
}
