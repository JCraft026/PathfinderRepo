using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Mirror;

public class ManageInventory : NetworkBehaviour
{
    private UI_Inventory uiInventory;           // Imports the UI_Inventory's members and functions
    private Inventory inventory;                // Imports the inventory's members and functions
    private Item selectedItem;                  // The item currently selected
    private List<Item> itemList;                // The local list of inventory items
    private int slotNumber = 0;                 // The slot number the player is choosing
    public InventoryControls inventoryControls; // Imports the inventory controller

    // Called on awake
    private void Awake(){
        inventoryControls = new InventoryControls();
    }

    // Ensures the input controller works correctly
    private void OnEnable(){
        inventoryControls.Enable();
    }
    
    // Ensures the input controller works correctly
    private void OnDisable(){
        inventoryControls.Disable();
    }

    // Runs on Start
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        inventory = new Inventory(UseItem);
        uiInventory = GameObject.Find("UI_Inventory").GetComponent<UI_Inventory>();
        uiInventory.SetInventory(inventory);
    }

    // Runs when colliding with an item
    private void OnTriggerEnter2D(Collider2D collider){
        ItemWorld itemWorld = collider.GetComponent<ItemWorld>();
        // If touching an item
        if(itemWorld != null){
            // Checks to see if the item is a key
            if(itemWorld.GetItem().isKey()){
                itemWorld.DestroySelf();
                uiInventory.SetKey(itemWorld.GetItem());
            }
            else{
                // Checks for the ability to hold more items
                if(inventory.GetItemList().Count < 8 ||
                    itemWorld.GetItem().isStackable()){
                    inventory.AddItem(itemWorld.GetItem());
                    itemWorld.DestroySelf();
                    // Controls if the item you pick up is selected (ready to be used) or not
                    if(inventory.GetItemList().Count == 1){
                        selectedItem = itemWorld.GetItem();
                        selectedItem.selected = true;
                        slotNumber = 0;
                    }
                    // If the item you just dropped is picked up, it will be selected again
                    else if(slotNumber + 1 == inventory.GetItemList().Count){
                        slotNumber = inventory.GetItemList().Count - 1;
                        selectedItem = itemWorld.GetItem();
                        selectedItem.selected = true;
                    }
                    uiInventory.RefreshInventoryItems();
                }
                else{
                    Debug.Log("Your inventory is full");
                }
            }
        }
    }

    // Select the first item slot (Keypad1)
    void OnInventory1()
    {
        itemList = inventory.GetItemList();
        if(itemList.Count > 0){
            selectedItem = itemList[0];
            selectedItem.selected = true;
            slotNumber = 0;
            unselectSlots(slotNumber);
        }
        uiInventory.RefreshInventoryItems();
    }

    // Select the second item slot (Keypad2)
    void OnInventory2()
    {
        itemList = inventory.GetItemList();
        if(itemList.Count > 1){
            selectedItem = itemList[1];
            selectedItem.selected = true;
            slotNumber = 1;
            unselectSlots(slotNumber);
        }
        uiInventory.RefreshInventoryItems();
    }

    // Select the third item slot (Keypad3)
    void OnInventory3()
    {
        itemList = inventory.GetItemList();
        if(itemList.Count > 2){
            selectedItem = itemList[2];
            selectedItem.selected = true;
            slotNumber = 2;
            unselectSlots(slotNumber);
        }
        uiInventory.RefreshInventoryItems();
    }

    // Select the fourth item slot (Keypad4)
    void OnInventory4()
    {
        itemList = inventory.GetItemList();
        if(itemList.Count > 3){
            selectedItem = itemList[3];
            selectedItem.selected = true;
            slotNumber = 3;
            unselectSlots(slotNumber);
        }
        uiInventory.RefreshInventoryItems();
    }

    // Select the fifth item slot (Keypad5)
    void OnInventory5()
    {
        itemList = inventory.GetItemList();
        if(itemList.Count > 4){
            selectedItem = itemList[4];
            selectedItem.selected = true;
            slotNumber = 4;
            unselectSlots(slotNumber);
        }
        uiInventory.RefreshInventoryItems();
    }

    // Select the sixth item slot (Keypad6)
    void OnInventory6()
    {
        itemList = inventory.GetItemList();
        if(itemList.Count > 5){
            selectedItem = itemList[5];
            selectedItem.selected = true;
            slotNumber = 5;
            unselectSlots(slotNumber);
        }
        uiInventory.RefreshInventoryItems();
    }

    // Select the seventh item slot (Keypad7)
    void OnInventory7()
    {
        itemList = inventory.GetItemList();
        if(itemList.Count > 6){
            selectedItem = itemList[6];
            selectedItem.selected = true;
            slotNumber = 6;
            unselectSlots(slotNumber);
        }
        uiInventory.RefreshInventoryItems();
    }

    // Select the eighth item slot (Keypad8)
    void OnInventory8()
    {
        itemList = inventory.GetItemList();
        if(itemList.Count > 7){
            selectedItem = itemList[7];
            selectedItem.selected = true;
            slotNumber = 7;
            unselectSlots(slotNumber);
        }
        uiInventory.RefreshInventoryItems();
    }

    // Does an action based of of which item is passed
    public void UseItem(Item item){ 
    switch (item.itemType){
        default:
            Debug.Log("This item don't do much, eh?");
            break;
        // Pickaxe Action
        case Item.ItemType.Pickaxe:
            ManageCrackedWalls.Instance.findClosestWall();
            ManageCrackedWalls.Instance.breakWall();
            break;
        // Smoke Bomb Action
        case Item.ItemType.SmokeBomb:
            RenderSmokeScreen.Instance.useSmoke();
            inventory.RemoveItem(item);
            break;
        }

    }

    // Select the first item slot (Keypad1)
    void OnUseItem(){
        // Check if the item exists
        if(selectedItem != null){
            // Check if there are enough items to be used
            if(selectedItem.amount > 0){
                inventory.UseItem(selectedItem);
                // 
                if(selectedItem.amount <= 0){
                    inventory.RemoveItem(selectedItem);
                    selectedItem = null;

                    itemList = inventory.GetItemList();
                    if(itemList.Count > 0){
                        if(slotNumber + 1 <= itemList.Count){
                            selectedItem = itemList[slotNumber];
                            selectedItem.selected = true;
                        }
                        uiInventory.RefreshInventoryItems();
                    }
                }
            }
        }
        else{
            Debug.Log("No item lol");
        }
    }

    // Drop's the selected item behind the runner
    void OnDropItem(){
        if (selectedItem != null){
            Item duplicateItem = new Item {itemType = selectedItem.itemType, amount = selectedItem.amount};
            ItemWorld.DropItem(MoveCharacter.Instance.rigidBody.position, duplicateItem);
            inventory.RemoveItem(selectedItem);
            selectedItem = null;
            itemList = inventory.GetItemList();
            if (itemList.Count > 0){
                if(slotNumber + 1 <= itemList.Count){
                    selectedItem = itemList[slotNumber];
                    selectedItem.selected = true;
                }
                uiInventory.RefreshInventoryItems();
            }
        }
        else{
            Debug.Log("No item lol");
        }
    }

    // Ensures only one slot is selected at a time
    public void unselectSlots(int slotNumber){
        int counter = 1;
        foreach(Item item in itemList){
            if(counter != slotNumber + 1){
                item.selected = false;
            }
            counter += 1;
        }
    }
}