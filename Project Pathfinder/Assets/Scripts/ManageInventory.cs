using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Mirror;

public class ManageInventory : NetworkBehaviour
{
    private UI_Inventory uiInventory;
    private Inventory inventory;
    private Item selectedItem;
    private List<Item> selectedItemList;
    private int slotNumber = 0;
    private MoveCharacter rigidBody;
    public InventoryControls inventoryControls;

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

    // Select the first item slot (Keypad1)
    void OnInventory1()
    {
        selectedItemList = inventory.GetItemList();
        if(selectedItemList.Count > 0){
            selectedItem = selectedItemList[0];
            selectedItem.selected = true;
            slotNumber = 0;
            unselectSlots(slotNumber);
        }
        uiInventory.RefreshInventoryItems();
    }

    // Select the first item slot (Keypad1)
    void OnInventory2()
    {
        selectedItemList = inventory.GetItemList();
        if(selectedItemList.Count > 1){
            selectedItem = selectedItemList[1];
            selectedItem.selected = true;
            slotNumber = 1;
            unselectSlots(slotNumber);
        }
        uiInventory.RefreshInventoryItems();
    }

    // Select the first item slot (Keypad1)
    void OnInventory3()
    {
        selectedItemList = inventory.GetItemList();
        if(selectedItemList.Count > 2){
            selectedItem = selectedItemList[2];
            selectedItem.selected = true;
            slotNumber = 2;
            unselectSlots(slotNumber);
        }
        uiInventory.RefreshInventoryItems();
    }

    // Select the first item slot (Keypad1)
    void OnInventory4()
    {
        selectedItemList = inventory.GetItemList();
        if(selectedItemList.Count > 3){
            selectedItem = selectedItemList[3];
            selectedItem.selected = true;
            slotNumber = 3;
            unselectSlots(slotNumber);
        }
        uiInventory.RefreshInventoryItems();
    }

    // Select the first item slot (Keypad1)
    void OnInventory5()
    {
        selectedItemList = inventory.GetItemList();
        if(selectedItemList.Count > 4){
            selectedItem = selectedItemList[4];
            selectedItem.selected = true;
            slotNumber = 4;
            unselectSlots(slotNumber);
        }
        uiInventory.RefreshInventoryItems();
    }

    // Select the first item slot (Keypad1)
    void OnInventory6()
    {
        selectedItemList = inventory.GetItemList();
        if(selectedItemList.Count > 5){
            selectedItem = selectedItemList[5];
            selectedItem.selected = true;
            slotNumber = 5;
            unselectSlots(slotNumber);
        }
        uiInventory.RefreshInventoryItems();
    }

    // Select the first item slot (Keypad1)
    void OnInventory7()
    {
        selectedItemList = inventory.GetItemList();
        if(selectedItemList.Count > 6){
            selectedItem = selectedItemList[6];
            selectedItem.selected = true;
            slotNumber = 6;
            unselectSlots(slotNumber);
        }
        uiInventory.RefreshInventoryItems();
    }

    // Select the first item slot (Keypad1)
    void OnInventory8()
    {
        selectedItemList = inventory.GetItemList();
        if(selectedItemList.Count > 7){
            selectedItem = selectedItemList[7];
            selectedItem.selected = true;
            slotNumber = 7;
            unselectSlots(slotNumber);
        }
        uiInventory.RefreshInventoryItems();
    }

    // Controls which item does which action
    public void UseItem(Item item){ 
    switch (item.itemType){
        case Item.ItemType.Pickaxe:
            Debug.Log("Pickaxe used");
            break;
        }
    }

    // Select the first item slot (Keypad1)
    void OnUseItem()
    {
        // Check if the item exists
        if(selectedItem != null){
            // Check if there are enough items to be used
            if(selectedItem.amount > 0){
                inventory.UseItem(selectedItem);
                // 
                if(selectedItem.amount <= 0){
                    inventory.RemoveItem(selectedItem);
                    selectedItem = null;

                    selectedItemList = inventory.GetItemList();
                    if(selectedItemList.Count > 0){
                        if(slotNumber + 1 <= selectedItemList.Count){
                            selectedItem = selectedItemList[slotNumber];
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

    void OnDropItem()
    {
        if (selectedItem != null){
            Item duplicateItem = new Item {itemType = selectedItem.itemType, amount = selectedItem.amount};
            ItemWorld.DropItem(MoveCharacter.Instance.rigidBody.position, duplicateItem);
            inventory.RemoveItem(selectedItem);
            selectedItem = null;
            selectedItemList = inventory.GetItemList();
            if (selectedItemList.Count > 0){
                if(slotNumber + 1 <= selectedItemList.Count){
                    selectedItem = selectedItemList[slotNumber];
                    selectedItem.selected = true;
                }
                uiInventory.RefreshInventoryItems();
            }
        }
        else{
            Debug.Log("No item lol");
        }
    }

    public void unselectSlots(int slotNumber)
    {
        int counter = 1;
        foreach(Item item in selectedItemList){
            if(counter != slotNumber + 1){
                item.selected = false;
            }
            counter += 1;
        }
    }
}
