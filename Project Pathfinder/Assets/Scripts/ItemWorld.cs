using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class ItemWorld : NetworkBehaviour{
    
    private Item item;
    private SpriteRenderer spriteRenderer;
    private TextMeshPro textMeshPro;

    public static ItemWorld SpawnItemWorld(Vector2 position, Item item){
        Transform transform =
            Instantiate(ItemAssets.Instance.pfItemWorld, position, Quaternion.identity);
        // NetworkServer.Spawn(transform);
        ItemWorld itemWorld = transform.GetComponent<ItemWorld>();
        itemWorld.SetItem(item);
        return itemWorld;
    }

    public static ItemWorld DropItem(Vector2 dropPosition, Item item){
        Vector2 dropDirection = new Vector2();
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

    private void Awake(){
        spriteRenderer = GetComponent<SpriteRenderer>();
        textMeshPro = transform.Find("Text").GetComponent<TextMeshPro>();
    }

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

    public Item GetItem(){
        return item;
    }

    public void DestroySelf(){
        Destroy(gameObject);
    }
}
