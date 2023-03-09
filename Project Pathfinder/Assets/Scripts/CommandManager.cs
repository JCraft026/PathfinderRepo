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
        var custNetMan = CustomNetworkManagerDAO.GetNetworkManagerGameObject().GetComponent<CustomNetworkManager>();

        if(!CustomNetworkManager.isRunner)
        {
            GameObject runner = GameObject.FindObjectsOfType<GameObject>().First<GameObject>(x => x.name.Contains("Runner"));
            Debug.Log("Disabling runner sprite on client");
            runner.GetComponent<SpriteRenderer>().enabled = false;
            StartCoroutine(MakeRunnerVisible());
        }
    }

    IEnumerator MakeRunnerVisible()
    {
        yield return new WaitForSeconds(5);
        GameObject runner = GameObject.FindObjectsOfType<GameObject>().First<GameObject>(x => x.name.Contains("Runner"));
        runner.GetComponent<SpriteRenderer>().enabled = true;
    }
}
