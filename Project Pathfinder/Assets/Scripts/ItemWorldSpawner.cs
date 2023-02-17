using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class ItemWorldSpawner : NetworkBehaviour
{

    private ItemWorld itemWorld; //

    public Item item; // Item this object is assigned to

    // On start, spawn all the objects with this script in it
    public override void OnStartAuthority()
    {
        Debug.LogError("THIS SHANT RUN");
        itemWorld.spawnItemWorld(transform.position, item);
        NetworkedDestroy(gameObject);
    }

    [Command]
    public void NetworkedDestroy(GameObject gameObject){
        NetworkServer.Destroy(gameObject);
        Destroy(gameObject);
    }
}