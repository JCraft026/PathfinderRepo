using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Inventory : MonoBehaviour
{
    private Inventory     inventory;         // Reference to the inventory's functions
    private Transform     itemSlotContainer; // Area where the UI inventory lies
    private Transform     itemSlotTemplate;  // Template for each item slot that instantiates
    private MoveCharacter rigidBody;         // The player's rigidbody coordinates

    // Run when the UI activates
    private void Awake(){
        itemSlotContainer = transform.Find("ItemSlotContainer");
        itemSlotTemplate  = itemSlotContainer.Find("ItemSlotTemplate");
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
