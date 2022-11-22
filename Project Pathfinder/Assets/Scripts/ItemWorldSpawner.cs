using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class ItemWorldSpawner : NetworkBehaviour
{
    public Item item;

    private void Start(){
        ItemWorld.SpawnItemWorld(transform.position, item);
        Destroy(gameObject);
    }

}
