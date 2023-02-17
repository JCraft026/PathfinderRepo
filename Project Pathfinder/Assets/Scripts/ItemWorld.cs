using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class ItemWorld : NetworkBehaviour{
    
    private Item item;                     // The item to be referenced
    private SpriteRenderer spriteRenderer; // Get's the object's spriteRender component
    private TextMeshPro textMeshPro;       // Holds the text that shows the number of stacked items

    //public ItemWorld Instance;

    // Called when the itemWorld object is instantiated
    private void Awake(){
        //Instance = this;
        spriteRenderer = GetComponent<SpriteRenderer>();
        textMeshPro = transform.Find("Text").GetComponent<TextMeshPro>();
    }

    // Spawns an item at the in scene location
    public void spawnItemWorld(Vector2 position, Item item){
        Debug.Log("AM I working?");
        networkedSpawnItemWorld(position, item);   
    }

    // Spawns an item at the in scene location
    [Command]
    public void networkedSpawnItemWorld(Vector2 position, Item item){
        Debug.Log("Please am I here?");
        GameObject gameObject = Instantiate(ItemAssets.Instance.pfItemWorld, position, Quaternion.identity);
        ItemWorld itemWorld = gameObject.GetComponent<ItemWorld>();
        itemWorld.SetItem(item);
        NetworkServer.Spawn(gameObject);
    }

    // Drop's an item behind where the player is facing 
    public void DropItem(Vector2 dropPosition, Item item){
        Vector2 dropDirection = new Vector2(); // A positive/negative x/y direction based on which direction the player is facing

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
        networkedSpawnItemWorld(dropPosition + dropDirection, item);
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