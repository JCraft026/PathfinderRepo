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

    #region Steam Generator Commands

    // Spawn a steam generator over the network
    [Command(requiresAuthority = false)]
    public void NetworkedSpawnGenerator(Vector2 generatorPos, int generatorIndex)
    {
            // Get the steam generator prefab from the network manager
        var generatorPrefab = CustomNetworkManagerDAO.GetNetworkManagerGameObject()
                            .GetComponent<CustomNetworkManager>().spawnPrefabs
                            .Find(x => x.name.Contains("SteamGenerator"));

        if(generatorPrefab == null)
        {
            Debug.LogError("CommandManager: NetworkedSpawnGenerator, generatorPrefab is null");
        }
                            
        var generator = Instantiate(generatorPrefab, generatorPos, Quaternion.identity);

        // Set Generator name
        switch (generatorIndex)
        {
            case 0:
                generator.gameObject.name = "MSG1";
                break;
            case 1:
                generator.gameObject.name = "MSG2";
                break;
            case 2:
                generator.gameObject.name = "MSG3";
                break;
        }

        if(generator == null)
        {
            Debug.LogError("CommandManager: NetworkedSpawnGenerator, GENERATOR IS NULL");
        }
        NetworkServer.Spawn(generator);

        if(generator.GetComponent<GeneratorController>() == null)
        {
            Debug.LogError("CommandManager: NetworkedSpawnGenerator, GeneratorController.cs is null");
        }
        generator.GetComponent<Animator>().SetBool("IsBusted", false);
    }
    #endregion Steam Generator Commands
}
