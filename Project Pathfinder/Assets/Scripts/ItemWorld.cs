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
    public AudioClip ChestOpenSound;
    public AudioClip KeyPickupSound;
    public AudioSource audioSource;
    public static System.Random randomNum  = new System.Random();
                                           // Random number generator

    // Index cooresponding to a WallStatus item in a WallStatus[][] array
    public struct MazeCellIndex{
        public MazeCellIndex(int x, int y){
            inner  = x;
            outer  = y;
        }
        public int inner; // Inner maze cell index (Cooresponds to "i" index in RenderMaze.cs)
        public int outer; // Outer maze cell index (Cooresponds to "j" index in RenderMaze.cs)
    }

    //public ItemWorld Instance;

    // Called when the itemWorld object is instantiated
    private void Awake(){
        //Instance = this;
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
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
    }

    // Returns an item
    public Item GetItem(){
        return item;
    }

    // Change to the open chest sprite and disable picking the item up anymore
    public void OpenChest(){
        spriteRenderer.sprite = ItemAssets.Instance.ChestOpened;
        gameObject.GetComponent<Collider2D>().enabled = false;
        audioSource.Play();
        //Destroy(gameObject);
    }

    // Command the chest be open on client and server
    [Command(requiresAuthority=false)]
    public void cmd_OpenChest(){
        rpc_OpenChest();
    }

    // Open the chest for both client and server
    [ClientRpc]
    public void rpc_OpenChest()
    {
        this.OpenChest();
    }

    // Quick way to give the illusion that the key was picked up and is now gone
    public void PickUpKey()
    {
        spriteRenderer.enabled = false;
        gameObject.GetComponent<Collider2D>().enabled = false;
        audioSource.Play();
    }

    // Launches the RPC to pickup a key
    [Command(requiresAuthority=false)]
    public void cmd_PickUpKey()
    {
        rpc_PickUpKey();
    }

    // RPC runs the code to disable the key sprite and collider
    [ClientRpc]
    public void rpc_PickUpKey()
    {
        this.PickUpKey();
    }
    // Spawn a specified amount of chests throughout the maze
    public static void SpawnChests(int numberToSpawn)
    {
        WallStatus[,] mazeData      = CustomNetworkManagerDAO.GetNetworkManagerGameObject().GetComponent<CustomNetworkManager>().parsedMazeJson;
                                                        // WallStatus data for each cell in the maze
        List<MazeCellIndex> cellIndexList = CreateMazeCellIndexList(mazeData.GetLength(0), mazeData.GetLength(1));
                                                        // List of structs containing the WallStatus indexes for each of the cells
        WallStatus currentCell      = new WallStatus(); // Wall status values for the current selected cell
        string currentCellName;                         // GameObject name of the current cell
        Vector3 currentCellPosition;                    // Scene position of the current cell
        int chestsSpawned           = 0;                // Total chests spawned

        // Loop spawning chests until the specified amount has spawned
        while(chestsSpawned < numberToSpawn){
            var randomCellIndex = randomNum.Next(0, cellIndexList.Count);
                                                   // Random index that cooresponds to a WallStatus index of a cell
            currentCell         = mazeData[cellIndexList[randomCellIndex].inner, cellIndexList[randomCellIndex].outer];
            currentCellName     = "mcf(" + (cellIndexList[randomCellIndex].inner-(int)(Utilities.GetMazeWidth()/2)) + "," + (cellIndexList[randomCellIndex].outer-(int)(Utilities.GetMazeHeight()/2)) + ")";
            currentCellPosition = GameObject.Find(currentCellName).transform.position;
            List<int> cellWalls = new List<int>(); // List of the current cells valid walls where chests can spawn against
            Vector3 chestPosition;                 // Scene position of the chest to spawn

            // Fill the list of valid chest spawn walls contained within the cell
            if(currentCell.HasFlag(WallStatus.TOP)){
                cellWalls.Add(GenerateMazeConstants.TOP);
            }
            if(currentCell.HasFlag(WallStatus.LEFT)){
                cellWalls.Add(GenerateMazeConstants.LEFT);
            }
            if(currentCell.HasFlag(WallStatus.RIGHT)){
                cellWalls.Add(GenerateMazeConstants.RIGHT);
            }
            
            // If the current cell has at least one valid wall to spawn a chest against, spawn a chest
            if(cellWalls.Count > 0){
                // Select a random valid wall to spawn the chest against
                var randomWallIndex = randomNum.Next(0, cellWalls.Count);

                // Set the chest spawn position
                switch (cellWalls[randomWallIndex])
                {
                    case GenerateMazeConstants.TOP:
                        chestPosition = currentCellPosition + new Vector3(0, (Utilities.GetCellSize() / 1.55f) - 2f, 0);
                        break;
                    case GenerateMazeConstants.LEFT:
                        chestPosition = currentCellPosition + new Vector3((-Utilities.GetCellSize() / 2) + 1.5f, 0, 0);
                        break;
                    case GenerateMazeConstants.RIGHT:
                        chestPosition = currentCellPosition + new Vector3((+Utilities.GetCellSize() / 2) - 1.5f, 0, 0);
                        break;
                    default:
                        chestPosition = new Vector3(0, 0, 0);
                        break;
                }

                // Spawn the chest
                GameObject.Find("ItemAssets").GetComponent<CommandManager>().networkedSpawnItemWorld(chestPosition, Item.getChestItem(true));
                
                // Increment the amount of chests spawned
                chestsSpawned += 1;
            }

            // Remove the current cell from the list of potential cells to spawn a chest at
            cellIndexList.RemoveAt(randomCellIndex);
        }
    }

    // Spawn all four keys in random locations throughout the maze
    public static void SpawnKeys()
    {
        WallStatus[,] mazeData      = CustomNetworkManagerDAO.GetNetworkManagerGameObject().GetComponent<CustomNetworkManager>().parsedMazeJson;
                                                        // WallStatus data for each cell in the maze
        List<MazeCellIndex> cellIndexList = CreateMazeCellIndexList((int)Utilities.GetMazeWidth(), (int)Utilities.GetMazeHeight());
                                                        // List of structs containing the WallStatus indexes for each of the cells
        string currentCellName;                         // GameObject name of the current cell
        Vector3 currentCellPosition;                    // Scene position of the current cell

        // Spawn all four keys
        for (int keysSpawned = 0; keysSpawned < 4; keysSpawned++){
            var randomCellIndex = randomNum.Next(0, cellIndexList.Count);
                                 // Random index that cooresponds to a WallStatus index of a cell
            currentCellName     = "mcf(" + (cellIndexList[randomCellIndex].inner-(int)(Utilities.GetMazeWidth()/2)) + "," + (cellIndexList[randomCellIndex].outer-(int)(Utilities.GetMazeHeight()/2)) + ")";
            currentCellPosition = GameObject.Find(currentCellName).transform.position;
            Vector3 keyPosition; // Scene position of the key to spawn
            
            // Set the key's spawn position
            keyPosition = currentCellPosition;

            // Spawn the key
            GameObject.Find("ItemAssets").GetComponent<CommandManager>().networkedSpawnItemWorld(keyPosition, Item.GetKey(keysSpawned));
            
            // Remove the current cell from the list of potential cells to spawn a key at
            cellIndexList.RemoveAt(randomCellIndex);
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

    // Fill a list of indexes cooresponding to items in a WallStatus[,] array
    public static List<MazeCellIndex> CreateMazeCellIndexList(int outerBoundaryLength, int innerBoundaryLength){
        List<MazeCellIndex> cellIndexList = new List<MazeCellIndex>(); // List containing all the WallStatus indexes in the MazeData array

        for (int j = 0; j < outerBoundaryLength; j++){
            for (int i = 0; i < innerBoundaryLength; i++){
                cellIndexList.Add(new MazeCellIndex(i, j));
            }
        }

        return cellIndexList;
    }
}