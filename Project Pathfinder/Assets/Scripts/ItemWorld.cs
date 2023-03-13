using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using System.Linq;
using System;

/*
    *This script manages items that are on the ground (-Caleb)
*/
public class ItemWorld : NetworkBehaviour
{
    [SyncVar]
    private Item item;                     // The item to be referenced
    private SpriteRenderer spriteRenderer; // Get's the object's spriteRender component

    //public ItemWorld Instance;

    // Called when the itemWorld object is instantiated
    private void Awake(){
        //Instance = this;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Spawns an item at the in scene location
    public void spawnItemWorld(Vector2 position, Item item){
        Debug.Log("Spawning itemworld...");
        networkedSpawnItemWorld(position, item);   
    }

    // Spawns an item at the in scene location
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
        if(item.itemType == Item.ItemType.Keys_0
            || item.itemType == Item.ItemType.Keys_1
            || item.itemType == Item.ItemType.Keys_2
            || item.itemType == Item.ItemType.Keys_3)
            {
                spriteRenderer.sprite = item.GetSprite();
            }
        else
            spriteRenderer.sprite = ItemAssets.Instance.ChestClosed;
        Debug.Log("item set");
    }

    // Returns an item
    public Item GetItem(){
        return item;
    }

    // Change to the open chest sprite and disable picking the item up anymore
    public void OpenChest(){
        spriteRenderer.sprite = ItemAssets.Instance.ChestOpened;
        gameObject.GetComponent<Collider2D>().enabled = false;
        //Destroy(gameObject);
    }

    // Quick way to give the illusion that the key was picked up and is now gone
    public void PickUpKey()
    {
        spriteRenderer.enabled = false;
        gameObject.GetComponent<Collider2D>().enabled = false;
    }

    // Spawn a bunch of chests around the map
    public static void SpawnChests(int numberToSpawn)
    {
        // All of the walls that our chests can spawn against
        List<GameObject> topWalls = Resources.FindObjectsOfTypeAll<GameObject>()
                                        .Where<GameObject>(x => x.name.Contains("Wall_TB") || x.name.Contains("Wall_LR")).ToList();

        // Shuffle the indexes of the walls for extra randomness
        topWalls = ShuffleList<GameObject>(topWalls);

        // If our list is null ensure we get a clear error for why client disconnected
        if(topWalls == null)
        {
            throw(new Exception("topWalls list is null in SpawnChests()"));
        }

        int chestsSpawned = 0; // Number of chests we've spawned so far
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        // Spawn the chests
        while(chestsSpawned < numberToSpawn)
        {
            int wallIndex = UnityEngine.Random.Range(0, topWalls.Count); // Random index for the walls

            Debug.Log("wallIndex: " + wallIndex);

            // Ensure that if there is a null reference in the list we get a clear error message for why the client disconnected
            if(topWalls[wallIndex] == null)
            {
                throw(new Exception("Null wallIndex in SpawnChests() (wallIndex = " + wallIndex + ")"));
            }
            
            Vector2 chestPos; // Rough/initial position of the chest based on the wall position

            //Adjust the initial spawn point for a chest depending on which wall type it is spawning on
            if(topWalls[wallIndex].name.Contains("Wall_TB"))
                chestPos = new Vector2(topWalls[wallIndex].transform.position.x, topWalls[wallIndex].transform.position.y - 5);
            else
                chestPos = new Vector2(topWalls[wallIndex].transform.position.x - 5, topWalls[wallIndex].transform.position.y);

            // If the chest is spawned outside the boundaries of the map move it back into the map
            while(chestPos.y >= 50 || chestPos.y <= -50 || chestPos.x >= 52 || chestPos.x <= -52)
            {
                // Adjust y position
                if(chestPos.y > 50)
                {
                    chestPos.y -= 1;
                }
                else if(chestPos.y <= -50)
                {
                    chestPos.y += 1;
                }

                // Adjust x position
                if(chestPos.x >= 52)
                {
                    chestPos.x -= 1;
                }
                else if(chestPos.x <= -52)
                {
                    chestPos.x += 1;
                }
            }

            // Clear the wall sprite
            if(chestPos.y > 0)
            {
                chestPos.y -= 5;
            }

            else
            {
                chestPos.y += 5;
            }

            // Command the server to spawn the chest
            GameObject.Find("ItemAssets")
            .GetComponent<CommandManager>()
            .networkedSpawnItemWorld(chestPos, Item.getRandomItem());
            chestsSpawned += 1;
            topWalls.Remove(topWalls[wallIndex]);   // Keep one wall from having multiple chests spawned at it
            topWalls = ShuffleList<GameObject>(topWalls);
        }
    }

    public static void SpawnKeys()
    {
        var cmdManager =  GameObject.Find("ItemAssets")
                            .GetComponent<CommandManager>();

        
        // All of the walls that our chests can spawn against
        List<GameObject> topWalls = Resources.FindObjectsOfTypeAll<GameObject>()
                                        .Where<GameObject>(x => x.name.Contains("Wall_TB") || x.name.Contains("Wall_LR")).ToList();

        // Shuffle the indexes of the walls for extra randomness
        topWalls = ShuffleList<GameObject>(topWalls);

        // If our list is null ensure we get a clear error for why client disconnected
        if(topWalls == null)
        {
            throw(new Exception("topWalls list is null in SpawnChests()"));
        }

        for(int iters = 0; iters < 4; iters++)
        {
            int wallIndex = UnityEngine.Random.Range(0, topWalls.Count); // Random index for the walls

             Vector2 keyPos; // Rough/initial position of the chest based on the wall position

                //Adjust the initial spawn point for a chest depending on which wall type it is spawning on
                if(topWalls[wallIndex].name.Contains("Wall_TB"))
                    keyPos = new Vector2(topWalls[wallIndex].transform.position.x, topWalls[wallIndex].transform.position.y - 5);
                else
                    keyPos = new Vector2(topWalls[wallIndex].transform.position.x - 5, topWalls[wallIndex].transform.position.y);

                // If the chest is spawned outside the boundaries of the map move it back into the map
                while(keyPos.y >= 50 || keyPos.y <= -50 || keyPos.x >= 52 || keyPos.x <= -52)
                {
                    // Adjust y position
                    if(keyPos.y > 50)
                    {
                        keyPos.y -= 1;
                    }
                    else if(keyPos.y <= -50)
                    {
                        keyPos.y += 1;
                    }

                    // Adjust x position
                    if(keyPos.x >= 52)
                    {
                        keyPos.x -= 1;
                    }
                    else if(keyPos.x <= -52)
                    {
                        keyPos.x += 1;
                    }
                }

                // Clear the wall sprite
                if(keyPos.y > 0)
                {
                    keyPos.y -= 5;
                }

                else
                {
                    keyPos.y += 5;
                }


            cmdManager.networkedSpawnItemWorld(keyPos, Item.GetKey(iters));
            topWalls.RemoveAt(wallIndex);
        }
    }

    //Fisher-Yates shuffle for shuffling lists.
    public static List<T> ShuffleList<T>(List<T> list)  
    {  
        System.Random rng = new System.Random();
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            T value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }
        return list;
    }
}