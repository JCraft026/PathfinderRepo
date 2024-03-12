using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class Player_UI : NetworkBehaviour
{
    public GameObject     player;
    private Inventory     inventory;           // Reference to the inventory's members and functions
    private Transform     itemSlotContainer,   // Area where the UI inventory lies
                          itemSlotTemplate,    // Template for each item slot that instantiates
                          keySlotContainer,    // Area where the Key UI lies
                          keySlot0,            // Slot holding the first key
                          keySlot1,            // Slot holding the second key
                          keySlot2,            // Slot holding the third key
                          keySlot3;            // Slot holding the fourth key
    private MoveCharacter rigidBody;           // The player's rigidbody coordinates
    public bool           hasKey0 = false,     // Boolean variable describing whether key 0 is present
                          hasKey1 = false,     // Boolean variable describing whether key 1 is present
                          hasKey2 = false,     // Boolean variable describing whether key 2 is present
                          hasKey3 = false;     // Boolean variable describing whether key 3 is present
    public Item.ItemType selectedItem = Item.ItemType.None; 
                                               // Item just selected
    public Item.ItemType activeSelectedItem = Item.ItemType.None; 
                                               // Current active item

    // Run when the UI activates
    private void Awake(){
        // Find all important transforms for later instantiation or reference
        itemSlotContainer = transform.Find("ItemSlotContainer");
        itemSlotTemplate  = itemSlotContainer.Find("ItemSlotTemplate");
        keySlotContainer  = transform.Find("KeyContainer");
        keySlot0          = keySlotContainer.Find("keySlot0");
        keySlot1          = keySlotContainer.Find("keySlot1");
        keySlot2          = keySlotContainer.Find("keySlot2");
        keySlot3          = keySlotContainer.Find("keySlot3");
    }

    public override void OnStartClient(){
        if(CustomNetworkManager.IsRunner == false){
            itemSlotContainer.gameObject.SetActive(false);
            keySlotContainer.gameObject.SetActive(false);
        }
    }

    // Set the UI player's rigidbody to the MoveCharacter rigidbody
    public void SetPlayer(MoveCharacter rigidBody){
        this.rigidBody = rigidBody;
    }

    // Assigns the inventory and updates the inventory to the current state
    public void SetInventory(Inventory inventory){
        this.inventory = inventory;
        inventory.OnItemListChanged += Inventory_OnItemListChanged;
        RefreshInventoryItems();
    }

    // Event to update the inventory's item list
    private void Inventory_OnItemListChanged(object sender, System.EventArgs e){
        RefreshInventoryItems();
    }

    // Places the key in the key slots
    public void SetKey(Item item){
        // Control using the type of key
        switch(item.itemType){
        default:
            Debug.Log("Hmmm maybe this line should not be running lol");
            break;
        case Item.ItemType.Keys_0:
            keySlot0.gameObject.SetActive(true); 
            hasKey0 = true;
            break;
        case Item.ItemType.Keys_1: keySlot1.gameObject.SetActive(true);
            keySlot1.gameObject.SetActive(true); 
            hasKey1 = true;
            break;
        case Item.ItemType.Keys_2: keySlot2.gameObject.SetActive(true);
            keySlot2.gameObject.SetActive(true); 
            hasKey2 = true;
            break;
        case Item.ItemType.Keys_3: keySlot3.gameObject.SetActive(true);
            keySlot3.gameObject.SetActive(true); 
            hasKey3 = true;
            break;
        }
    }

    // Returns whether a key has been picked up or not
    public bool GetKey(int keyNumber){
        switch(keyNumber){
            default: 
                Debug.Log("Invalid Key Number, bruh");
                return false;
            case 0:
                if(hasKey0)
                    return true;
                break;
            case 1:
                if(hasKey1)
                    return true;
                break;
            case 2:
                if(hasKey2)
                    return true;
                break;
            case 3:
                if(hasKey3)
                    return true;
                break;
        }
        return false;
    }

    // Update the inventory's item list and which item is selected
    public void RefreshInventoryItems(){
        foreach (Transform child in itemSlotContainer){
            if (child == itemSlotTemplate) continue;
            Destroy(child.gameObject);
        }

        int   slotX            = 0;     // UI inventory item slot x position
        int   slotY            = 0;     // UI inventory item slot y position 
        float itemSlotCellSize = 55.0f; // Size of each item slot cell
        int currentSlotNumber = 0;      // Number of the current slot in the list
        
        //  
        foreach (Item item in inventory.GetItemList()){
            RectTransform itemSlotRectTransform =
                Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            itemSlotRectTransform.gameObject.SetActive(true);

            switch (item.itemType)
            {
                case Item.ItemType.Sledge:
                    itemSlotRectTransform.gameObject.name = "Sledge Item Slot";
                    itemSlotRectTransform.Find("Popup").gameObject.name = "Sledge Popup";
                    itemSlotRectTransform.Find("Sledge Popup").Find("itemDescription").gameObject.name = "Sledge Description";
                    itemSlotRectTransform.Find("Sledge Popup").Find("Sledge Description").gameObject.GetComponent<TextMeshProUGUI>().text = "<align=center><color=#b58500><size=12>Sledgehammer</size></color>\nhas the power to destroy cracked walls and steam generators\n\nunlimited uses\n\n[j] use</align>";
                    break;
                case Item.ItemType.Coffee:
                    itemSlotRectTransform.gameObject.name = "Coffee Item Slot";
                    itemSlotRectTransform.Find("Popup").gameObject.name = "Coffee Popup";
                    itemSlotRectTransform.Find("Coffee Popup").Find("itemDescription").gameObject.name = "Coffee Description";
                    itemSlotRectTransform.Find("Coffee Popup").Find("Coffee Description").gameObject.GetComponent<TextMeshProUGUI>().text = "<align=center><color=#b58500><size=11>common grounds coffee</size></color>\ntemproarily doubles running speed\n\n<color=red>Duration: </color>5 sec\n\n[j] use</align>";
                    break;
                case Item.ItemType.SmokeBomb:
                    itemSlotRectTransform.gameObject.name = "Smoke Item Slot";
                    itemSlotRectTransform.Find("Popup").gameObject.name = "Smoke Popup";
                    itemSlotRectTransform.Find("Smoke Popup").Find("itemDescription").gameObject.name = "Smoke Description";
                    itemSlotRectTransform.Find("Smoke Popup").Find("Smoke Description").gameObject.GetComponent<TextMeshProUGUI>().text = "<align=center><color=#b58500><size=12>Smoke bomb</size></color>\ncreates a temproary smoke screen across the surrounding area\n\n<color=red>Duration: </color>10 sec\n\n[j] use</align>";
                    break;
                case Item.ItemType.GreenScreenSuit:
                    itemSlotRectTransform.gameObject.name = "Green Screen Item Slot";
                    itemSlotRectTransform.Find("Popup").gameObject.name = "Green Screen Popup";
                    itemSlotRectTransform.Find("Green Screen Popup").Find("itemDescription").gameObject.name = "Green Screen Description";
                    itemSlotRectTransform.Find("Green Screen Popup").Find("Green Screen Description").gameObject.GetComponent<TextMeshProUGUI>().text = "<align=center><color=#b58500><size=12>green screen suit</size></color>\nmakes the wearer temproarily undetectable to maze guards and trap chests\n\n<color=red>Duration: </color>5 sec\n\n[j] use</align>";
                    break;
                case Item.ItemType.EMP:
                    itemSlotRectTransform.gameObject.name = "EMP Item Slot";
                    itemSlotRectTransform.Find("Popup").gameObject.name = "EMP Popup";
                    itemSlotRectTransform.Find("EMP Popup").Find("itemDescription").gameObject.name = "EMP Description";
                    itemSlotRectTransform.Find("EMP Popup").Find("EMP Description").gameObject.GetComponent<TextMeshProUGUI>().text = "<align=center><color=#b58500><size=12>EMP</size></color>\ndisables maze guards in its radius for <color=red>10</color> seconds\n\n<color=red>Duration: </color>2 sec\n\n[j] use</align>";
                    break;
            }

            itemSlotRectTransform.anchoredPosition =
                new Vector2(slotX * itemSlotCellSize, slotY * itemSlotCellSize);
            
            // Display the slot number
            currentSlotNumber++;
            itemSlotRectTransform.Find("SlotNumber").GetComponent<TextMeshProUGUI>().text = "[" + currentSlotNumber + "]";

            // Put the correct sprite in the inventory
            Image image = itemSlotRectTransform.Find("Icon").GetComponent<Image>();
            image.sprite = item.GetSprite();

            // Display which item slot is selected and display its popup
            Image selection = itemSlotRectTransform.Find("SelectedBorder").GetComponent<Image>();
            if (item.isSelected()){
                selection.color = new Color32(255,255,225,255);
                // If the current selected item was selected before refresh, do not display popup
                if(selectedItem == item.itemType){
                    itemSlotRectTransform.gameObject.GetComponent<ManageItemPopup>().alreadyDisplayed = true;
                }
                itemSlotRectTransform.gameObject.GetComponent<ManageItemPopup>().itemIsSelected = true;
                selectedItem = item.itemType;
                activeSelectedItem = item.itemType;
            }
            else{
                selection.color = new Color32(255,255,225,0);
            }

            // Show the number of items in a stack
            TextMeshProUGUI uiText = itemSlotRectTransform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
            if (item.amount > 1){
                uiText.SetText(item.amount.ToString());
            }
            else{
                uiText.SetText("");
                itemSlotRectTransform.Find("CountBackground").GetComponent<Image>().enabled = false;
            }

            // Controls where item slots are appearing 
            slotX++;
            if (slotX >= 8){
                slotX = 0;
                slotY++;
            }
        }
    }
}