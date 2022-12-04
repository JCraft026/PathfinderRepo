using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class ItemWorld : NetworkBehaviour{
    
    private Item item;                     // The item to be referenced
    private SpriteRenderer spriteRenderer; // Get's the object's spriteRender component
    private TextMeshPro textMeshPro;       // Holds the text that shows the number of stacked items

    // Called when the itemWorld object is instantiated
    private void Awake(){
        spriteRenderer = GetComponent<SpriteRenderer>();
        textMeshPro = transform.Find("Text").GetComponent<TextMeshPro>();
    }

    // Spawns an item at the in scene location
    public static ItemWorld SpawnItemWorld(Vector2 position, Item item){
        Transform transform =
            Instantiate(ItemAssets.Instance.pfItemWorld, position, Quaternion.identity);
        ItemWorld itemWorld = transform.GetComponent<ItemWorld>();
        itemWorld.SetItem(item);
        return itemWorld;
    }

    // Drop's an item behind where the player is facing 
    public static ItemWorld DropItem(Vector2 dropPosition, Item item){
        Vector2 dropDirection = new Vector2(); // A positive/negative x/y direction based on
                                               // which direction the player is facing

        switch(MoveCharacter.Instance.facingDirection){
            case 1:
                dropDirection = new Vector2(0, 2f);
                break;
            case 2:
                dropDirection = new Vector2(2f, 0);
                break;
            case 3:
                dropDirection = new Vector2(0, -2f);
                break;
            case 4:
                dropDirection = new Vector2(-2f, 0);
                break;
        }
        ItemWorld itemWorld = SpawnItemWorld(dropPosition + dropDirection, item);
        return itemWorld;
    }

    // Assigns the right sprite and amount number to an item
    public void SetItem(Item item){
        this.item = item;
        spriteRenderer.sprite = item.GetSprite();
        if (item.amount > 1){
            textMeshPro.SetText(item.amount.ToString());
        }
        else{
            textMeshPro.SetText("");
        }
    }

    // Returns an item
    public Item GetItem(){
        return item;
    }

    // Deestroy's the object this script is in
    public void DestroySelf(){
        Destroy(gameObject);
    }
}