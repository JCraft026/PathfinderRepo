using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CommandManager : NetworkBehaviour
{
    // Spawns an item at the in scene location
    [Command(requiresAuthority = false)]
    public void networkedSpawnItemWorld(Vector2 position, Item item){
        if(isServer)
        {
            GameObject gameObject = Instantiate(ItemAssets.Instance.pfItemWorld, position, Quaternion.identity);
            ItemWorld itemWorld = gameObject.GetComponent<ItemWorld>();
            itemWorld.SetItem(item);
            NetworkServer.Spawn(gameObject);
        }
    }
}
