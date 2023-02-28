using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

/*
    *This script manages items that are on the ground (-Caleb)
*/
public class ItemWorld : NetworkBehaviour
{
    [SyncVar]
    private Item item;                     // The item to be referenced
    private SpriteRenderer spriteRenderer; // Get's the object's spriteRender component
    private TextMeshPro textMeshPro;       // Holds the text that shows the number of stacked items

    //public ItemWorld Instance;

    // Called when the itemWorld object is instantiated
    private void Awake(){
        //Instance = this;
        spriteRenderer = GetComponent<SpriteRenderer>();
        try
        {
            textMeshPro = transform.Find("Text").GetComponent<TextMeshPro>();
        }
        catch
        {
            Debug.LogWarning("Failed to get textMeshPro in self, getting from gObject's child");
            textMeshPro = gameObject.GetComponentInChildren<TextMeshPro>();
        }
        if(textMeshPro == null)
        {
            Debug.LogError("Item textMeshPro is null");
        }
    }

    // Spawns an item at the in scene location
    public void spawnItemWorld(Vector2 position, Item item){
        Debug.Log("Spawning itemworld...");
        networkedSpawnItemWorld(position, item);   
    }

    // Spawns an item at the in scene location
    [Command(requiresAuthority = false)]
    public void networkedSpawnItemWorld(Vector2 position, Item item){
         GameObject.Find("ItemAssets")
            .GetComponent<CommandManager>().networkedSpawnItemWorld(position, item);
    }

    // Drop's an item behind where the player is facing 
    public static void DropItem(Vector2 dropPosition, Item item){
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
        
        GameObject.Find("ItemAssets")
            .GetComponent<CommandManager>()
            .networkedSpawnItemWorld(dropPosition + dropDirection, item);

        //ItemWorldSpawner.SpawnItemWorld(dropPosition + dropDirection, item);
        //networkedSpawnItemWorld(dropPosition + dropDirection, item);
    }

    // Assigns the right sprite and amount number to an item
    public void SetItem(Item item){
        this.item = item;
        if(item == null)
        {
            Debug.LogError("Item is null in SetItem");
        }
        spriteRenderer.sprite = item.GetSprite();
        if (item.amount > 1){
            textMeshPro.SetText(item.amount.ToString());
        }
        else{
            textMeshPro.SetText("");
        }
        Debug.Log("item set");
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