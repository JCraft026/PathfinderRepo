using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Inventory : MonoBehaviour
{
    private Inventory     inventory;         // Reference to the inventory's members and functions
    private Transform     itemSlotContainer, // Area where the UI inventory lies
                          itemSlotTemplate,  // Template for each item slot that instantiates
                          keySlotContainer,  // Area where the Key UI lies
                          keySlot0,          // Slot holding the first key
                          keySlot1,          // Slot holding the second key
                          keySlot2,          // Slot holding the third key
                          keySlot3;          // Slot holding the fourth key
    private MoveCharacter rigidBody;         // The player's rigidbody coordinates
    private bool          hasKey0 = false,   // Boolean variable describing whether key 0 is present
                          hasKey1 = false,   // Boolean variable describing whether key 1 is present
                          hasKey2 = false,   // Boolean variable describing whether key 2 is present
                          hasKey3 = false;   // Boolean variable describing whether key 3 is present

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
        
        //  
        foreach (Item item in inventory.GetItemList()){
            RectTransform itemSlotRectTransform =
                Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            itemSlotRectTransform.gameObject.SetActive(true);

            itemSlotRectTransform.anchoredPosition =
                new Vector2(slotX * itemSlotCellSize, slotY * itemSlotCellSize);
            
            // Put the correct sprite in the inventory
            Image image = itemSlotRectTransform.Find("Icon").GetComponent<Image>();
            image.sprite = item.GetSprite();

            // Display which item slot is selected
            Image selection = itemSlotRectTransform.Find("SelectedBorder").GetComponent<Image>();
            if (item.isSelected()){
                selection.color = new Color32(255,255,225,255);
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