using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Mirror;
using System.Linq;

public class ManageInventory : NetworkBehaviour
{
    public GameObject EMP;                      // Holds the EMP to be spawned
    private Player_UI playerUi;                 // Imports the Player UI's members and functions
    private Inventory inventory;                // Imports the inventory's members and functions
    private ItemWorld itemWorld;                // Imports the Item World Script's members and functions
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

        // Set up the inventory for the runner
        if(CustomNetworkManager.isRunner)
        {
            inventory = new Inventory(UseItem);
            playerUi = GameObject.Find("Player_UI").GetComponent<Player_UI>();
            playerUi.SetInventory(inventory);
        }
    }

    // Runs when colliding with an item
    private void OnTriggerEnter2D(Collider2D collider){
        if(isLocalPlayer)
        {
            if(collider == null)
            {
                Debug.LogError("Collider is null");
            }
            if(collider.gameObject == null)
            {
                Debug.LogError("itemWorld.gameObject == null");
            }
            ItemWorld itemWorld = collider.gameObject.GetComponent<ItemWorld>();

            // If touching an item
            if(itemWorld != null){
                // Checks to see if the item is a key
                if(itemWorld.GetItem() == null)
                {
                    Debug.LogError("itemWorld.GetItem() == null");
                }
                if(itemWorld.GetItem().isKey()){
                    itemWorld.PickUpKey();
                    playerUi.SetKey(itemWorld.GetItem());
                }
                else{
                    // Assign the chest a random item
                    itemWorld.SetItem(Item.getChestItem(false));

                    // Checks for the ability to hold more items
                    if(inventory.GetItemList().Count < 8 ||
                        inventory.anItemCanStack(itemWorld.GetItem())){
                        inventory.AddItem(itemWorld.GetItem());
                        // Controls if the item you pick up is selected (ready to be used) or not
                        if(inventory.GetItemList().Count == 1){
                            slotNumber = 0;
                            selectedItem = itemWorld.GetItem();
                            selectedItem.selected = true;
                        }
                        playerUi.RefreshInventoryItems();
                        itemWorld.cmd_OpenChest();
                    }
                    else{
                        Debug.Log("Your inventory is full");
                    }
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
        playerUi.RefreshInventoryItems();
        playerUi.selectedItem = Item.ItemType.None;
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
        playerUi.RefreshInventoryItems();
        playerUi.selectedItem = Item.ItemType.None;
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
        playerUi.RefreshInventoryItems();
        playerUi.selectedItem = Item.ItemType.None;
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
        playerUi.RefreshInventoryItems();
        playerUi.selectedItem = Item.ItemType.None;
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
        playerUi.RefreshInventoryItems();
        playerUi.selectedItem = Item.ItemType.None;
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
        playerUi.RefreshInventoryItems();
        playerUi.selectedItem = Item.ItemType.None;
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
        playerUi.RefreshInventoryItems();
        playerUi.selectedItem = Item.ItemType.None;
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
        playerUi.RefreshInventoryItems();
        playerUi.selectedItem = Item.ItemType.None;
    }

    // Does an action based of of which item is passed
    public void UseItem(Item item){ 
    switch (item.itemType){
        default:
            Debug.LogWarning("No item equipped");
            break;
        // Sledge Hammer Action
        case Item.ItemType.Sledge:
            ManageCrackedWalls.Instance.findClosestWall();
            ManageCrackedWalls.Instance.breakWall();
            break;
        // Smoke Bomb Action
        case Item.ItemType.SmokeBomb:
            RenderSmokeScreen.Instance.useSmoke();
            inventory.RemoveItem(item);
            break;
        // Common Grounds Coffee Action
        case Item.ItemType.Coffee:
            if(CoffeeController.Instance.coffeeIsOver){
                Debug.Log("Coffee used");
                inventory.RemoveItem(item);
                MoveCharacter runnerMovementScript = gameObject.GetComponent<MoveCharacter>();
                CoffeeController coffeeController = gameObject.GetComponent<CoffeeController>();
                runnerMovementScript.moveSpeed = 10.0f;
                coffeeController.setCooldown(10);
            }
            else{
                Debug.Log("Still in Use");
            }
            break;
        case Item.ItemType.GreenScreenSuit:
            if(GreenScreenController.Instance.greenScreenIsOver){
                Debug.Log("Green screen suit applied");
                MoveCharacter runnerScript = gameObject.GetComponent<MoveCharacter>();
                GreenScreenController greenScreenController = gameObject.GetComponent<GreenScreenController>();
                runnerScript.greenScreen();
                ItemAssets.Instance.GetComponent<CommandManager>().cmd_MakeRunnerInvisible();
                greenScreenController.setCooldown(5);
                inventory.RemoveItem(item);
            }
            else{
                Debug.Log("Still in Use");
            }
            break;
        case Item.ItemType.EMP:
            Debug.Log("EMP Used");
            cmd_useEMP();
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
                // If there are no items in a stackable item after use, remove the item
                if(selectedItem.amount <= 0){
                    inventory.RemoveItem(selectedItem);
                    selectedItem = null;
                    itemList = inventory.GetItemList();
                    if(itemList.Count > 0){
                        if(slotNumber + 1 <= itemList.Count){
                            selectedItem = itemList[slotNumber];
                            selectedItem.selected = true;
                        }
                        playerUi.RefreshInventoryItems();
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
        bool droppingIsEnabled = false; // Allows us to disable dropping items with the implemenation of chests
                                        // This is a temporary fix since I don't know if we want dropping in game now.

        if (selectedItem != null && droppingIsEnabled){
            Item duplicateItem = new Item {itemType = selectedItem.itemType, amount = 1};
            // if(itemWorld == null)
            // {
            //     Debug.LogError("itemWorld is null");
            // }
            if(MoveCharacter.Instance == null)
            {
                Debug.LogError("MoveCharacter.Instance == null");
            }
            else if(MoveCharacter.Instance.rigidBody == null)
            {
                Debug.LogError("MoveCharacter.Instance.rigidBody.position == null");
            }
            else if(MoveCharacter.Instance.rigidBody.position == null)
            {
                Debug.LogError("MoveCharacter.Instance.rigidBody.position == null");
            }
            if(duplicateItem == null)
            {
                Debug.LogError("duplicateItem is null");
            }
            ItemWorld.DropItem(MoveCharacter.Instance.rigidBody.position, duplicateItem);
            inventory.RemoveItem(selectedItem);
            selectedItem = null;
            itemList = inventory.GetItemList();
            if (itemList.Count > 0){
                if(slotNumber + 1 <= itemList.Count){
                    selectedItem = itemList[slotNumber];
                    selectedItem.selected = true;
                }
                playerUi.RefreshInventoryItems();
            }
        }
        else{
            Debug.Log("No item lol");
            if(!droppingIsEnabled)
            {
                Debug.LogWarning("Dropping items is disabled");
            }
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

    [Command(requiresAuthority = false)]
    void cmd_useEMP(){
        GameObject tempEMP = Instantiate(EMP, gameObject.transform.position, Quaternion.identity);
        NetworkServer.Spawn(tempEMP);
        //rpc_useEMP();
    }
}