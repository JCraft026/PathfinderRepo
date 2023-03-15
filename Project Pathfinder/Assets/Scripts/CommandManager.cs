using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class CommandManager : NetworkBehaviour
{
    // Spawns an item at the in scene location
    [Command(requiresAuthority = false)]
    public void networkedSpawnItemWorld(Vector2 position, Item item)
    {
        GameObject gameObject = Instantiate(ItemAssets.Instance.pfItemWorld, position, Quaternion.identity);
        ItemWorld itemWorld = gameObject.GetComponent<ItemWorld>();
        //itemWorld.SetItem(item);
        NetworkServer.Spawn(gameObject);
        rpc_ClientSetItemSprite(itemWorld, item);
    }

    [ClientRpc]
    public void rpc_ClientSetItemSprite(ItemWorld itemWorld, Item item)
    {
        itemWorld.SetItem(item);
    }

    [Command(requiresAuthority = false)]
    public void cmd_MakeRunnerInvisible()
    {
        rpc_MakeRunnerInvisible(); // Tell the clients to make the runner invisible if they are not the runner
    }

    [ClientRpc]
    public void rpc_MakeRunnerInvisible()
    {
        if(!CustomNetworkManager.isRunner)
        {
           GameObject runner = Resources.FindObjectsOfTypeAll<GameObject>().First<GameObject>(x => x.name.Contains("Runner"));

            runner.GetComponent<SpriteRenderer>().enabled = false;
            StartCoroutine(MakeRunnerVisible());
        }
    }

    IEnumerator MakeRunnerVisible()
    {
        yield return new WaitForSeconds(5);
        GameObject runner = Resources.FindObjectsOfTypeAll<GameObject>().First<GameObject>(x => x.name.Contains("Runner"));
        runner.GetComponent<SpriteRenderer>().enabled = true;
    }

    // Tell the clients to update their events
    [Command]
    public void cmd_TransitionToYouWinYouLose(int currentEvent, int endGameEvent)
    {
        rpc_TransitionToYouWinYouLose(currentEvent, endGameEvent);
    }

    // Make sure that the clients event gets updated when the runner escapes or the guard dies
    [ClientRpc]
    public void rpc_TransitionToYouWinYouLose(int currentEvent, int endGameEvent)
    {
        HandleEvents.currentEvent = currentEvent;
        HandleEvents.endGameEvent = endGameEvent;
    }
}
