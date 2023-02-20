using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


/*
    *This class is responsible for spawning items onto the ground (-Caleb)
*/
public class ItemWorldSpawner : NetworkBehaviour
{

    private ItemWorld itemWorld; //

    public Item item; // Item this object is assigned to

    // On start, spawn all the objects with this script in it
    /*public override void OnStartAuthority()
    {
        Debug.LogError("THIS SHANT RUN");
        itemWorld.spawnItemWorld(transform.position, item);
        NetworkedDestroy(gameObject);
    }*/

    public static void SpawnItemWorld(Vector2 pos, Item item)
    {
        if(NetworkServer.active)
        {
            GameObject gameObject = Instantiate(ItemAssets.Instance.pfItemWorld, pos, Quaternion.identity);
            ItemWorld groundItem = gameObject.GetComponent<ItemWorld>();
            groundItem.SetItem(item);
            NetworkServer.Spawn(gameObject);
        }
        else
        {
           
            //CmdSpawnItemWorld(pos, item);
        }
    }

    [Command]
    public void NetworkedDestroy(GameObject gameObject){
        NetworkServer.Destroy(gameObject);
        Destroy(gameObject);
    }

    [Command]
    public void CmdSpawnItemWorld(Vector2 pos, Item item)
    {
        GameObject gameObject = Instantiate(ItemAssets.Instance.pfItemWorld, pos, Quaternion.identity);
        ItemWorld groundItem = gameObject.GetComponent<ItemWorld>();
        groundItem.SetItem(item);
        NetworkServer.Spawn(gameObject);
    }
}