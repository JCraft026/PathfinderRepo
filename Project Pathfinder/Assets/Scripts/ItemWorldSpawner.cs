using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class ItemWorldSpawner : NetworkBehaviour
{
    public Item item; // Item this object is assigned to

    // On start, spawn all the objects with this script in it
    private void Start(){
        ItemWorld.SpawnItemWorld(transform.position, item);
        Destroy(gameObject);
    }

}
