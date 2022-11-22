using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Inventory : MonoBehaviour
{
    private Inventory inventory;
    private Transform itemSlotContainer;
    private Transform itemSlotTemplate;
    private MoveCharacter rigidBody;

    private void Awake(){
        itemSlotContainer = transform.Find("ItemSlotContainer");
        itemSlotTemplate  = itemSlotContainer.Find("ItemSlotTemplate");
    }

    public void SetPlayer(MoveCharacter rigidBody){
        this.rigidBody = rigidBody;
    }

    public void SetInventory(Inventory inventory){
        this.inventory = inventory;
        inventory.OnItemListChanged += Inventory_OnItemListChanged;
        RefreshInventoryItems();
    }

    private void Inventory_OnItemListChanged(object sender, System.EventArgs e){
        RefreshInventoryItems();
    }

    public void RefreshInventoryItems(){
        foreach (Transform child in itemSlotContainer){
            if (child == itemSlotTemplate) continue;
            Destroy(child.gameObject);
        }

        int x = 0;
        int y = 0;
        float itemSlotCellSize = 55.0f;
        foreach (Item item in inventory.GetItemList()){
            RectTransform itemSlotRectTransform =
                Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            itemSlotRectTransform.gameObject.SetActive(true);

            itemSlotRectTransform.anchoredPosition =
                new Vector2(x * itemSlotCellSize, y * itemSlotCellSize);
            
            Image image = itemSlotRectTransform.Find("Icon").GetComponent<Image>();
            image.sprite = item.GetSprite();

            Image selection = itemSlotRectTransform.Find("SelectedBorder").GetComponent<Image>();

            if (item.isSelected()){
                selection.color = new Color32(255,255,225,255);
            }
            else{
                selection.color = new Color32(255,255,225,0);
            }

            TextMeshProUGUI uiText = itemSlotRectTransform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
            if (item.amount > 1){
                uiText.SetText(item.amount.ToString());
            }
            else{
                uiText.SetText("");
            }

            x++;
            if (x >= 8){
                x = 0;
                y++;
            }
        }
    }
}
