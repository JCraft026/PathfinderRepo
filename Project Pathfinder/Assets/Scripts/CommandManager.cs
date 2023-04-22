using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class CommandManager : NetworkBehaviour
{
    // Causes runner to take Chaser dash collision damage
    [Command(requiresAuthority = false)]
    public void cmd_TakeDashDamage(int activeGuardId)
    {
        rpc_TakeDashDamage(activeGuardId);
    }

    [ClientRpc]
    public void rpc_TakeDashDamage(int activeGuardId)
    {
        var runner = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner"));
                                 // Runner game object
        CameraShake cameraShake; // Camera shaker

        // If the runner is going to die from the next hit, set the appropriate end game event
        if(runner.GetComponent<ManageRunnerStats>().health <= 1){
            HandleEvents.endGameEvent = HandleEventsConstants.RUNNER_CAPTURED;
        }

        // Subtract runner damage hp
        runner.GetComponent<ManageRunnerStats>().TakeDamage(1);

        // Shake the cooresponding camera of the active character
        if(CustomNetworkManager.isRunner){
            cameraShake = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("CameraHolder(R)")).transform.GetChild(0).GetComponent<CameraShake>();
            StartCoroutine(cameraShake.Shake(.15f, .7f));
        }
        else{
            if(activeGuardId == ManageActiveCharactersConstants.CHASER){
                cameraShake = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("CameraHolder(C)")).transform.GetChild(0).GetComponent<CameraShake>();
                StartCoroutine(cameraShake.Shake(.15f, .7f));
            }
        }
    }

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

        // Set the pickup sound for the item
        switch(item.itemType)
        {
            case Item.ItemType.Keys_0:
            case Item.ItemType.Keys_1:
            case Item.ItemType.Keys_2:
            case Item.ItemType.Keys_3:
                itemWorld.audioSource.clip = itemWorld.KeyPickupSound;
            break;
            default:
                itemWorld.audioSource.clip = itemWorld.ChestOpenSound;
            break;
        }
    }

    [Command(requiresAuthority = false)]
    public void cmd_MakeRunnerInvisible()
    {
        rpc_MakeRunnerInvisible(); // Tell the clients to make the runner invisible if they are not the runner
    }

    [ClientRpc]
    public void rpc_MakeRunnerInvisible()
    {
        GameObject runner = Resources.FindObjectsOfTypeAll<GameObject>().First<GameObject>(x => x.name.Contains("Runner"));

        if(CustomNetworkManager.isRunner)
        {
            runner.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 30);
        }
        if(!CustomNetworkManager.isRunner){
            runner.GetComponent<SpriteRenderer>().enabled = false;
        }
        StartCoroutine(MakeRunnerVisible());
    }

    IEnumerator MakeRunnerVisible()
    {
        yield return new WaitForSeconds(5);
        GameObject runner = Resources.FindObjectsOfTypeAll<GameObject>().First<GameObject>(x => x.name.Contains("Runner"));
        if(CustomNetworkManager.isRunner)
        {
            runner.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        }
        if(!CustomNetworkManager.isRunner)
        {
            runner.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    // Spawn a steam generator over the network
    [Command(requiresAuthority = false)]
    public void NetworkedSpawnGenerator(Vector2 generatorPos, int generatorIndex)
    {
        rpc_NetworkedSpawnGenerator(generatorPos, generatorIndex);
    }

    [ClientRpc]
    public void rpc_NetworkedSpawnGenerator(Vector2 generatorPos, int generatorIndex)
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
        //NetworkServer.Spawn(generator);

        if(generator.GetComponent<GeneratorController>() == null)
        {
            Debug.LogError("CommandManager: NetworkedSpawnGenerator, GeneratorController.cs is null");
        }
        generator.GetComponent<Animator>().SetBool("IsBusted", false);
    }

    // Call an RPC to set steam booleans true or false
    [Command(requiresAuthority = false)]
    public void cmd_SetSteam(string parameterToSet, bool setting, string generatorName){
        rpc_SetSteam(parameterToSet, setting, generatorName);
        Debug.Log("Called Command to set steam");
    }

    // Set steam booleans true or false
    [ClientRpc]
    public void rpc_SetSteam(string parameterToSet, bool setting, string generatorName)
    {
        Debug.Log("Called RPC to set steam");

        if(!CustomNetworkManager.isRunner){
            if(setting == true){
                Debug.Log("M" + generatorName + "(Enabled)");
                GameObject.Find("M" + generatorName + "(Enabled)").GetComponent<SpriteRenderer>().enabled  = false;
                GameObject.Find("M" + generatorName + "(Disabled)").GetComponent<SpriteRenderer>().enabled = true;
                GameObject.Find("PopupMessageManager").GetComponent<ManagePopups>().ProcessPopup("<color=red>steam generator disabled!</color>", 5f);
            }
            else{
                GameObject.Find("M" + generatorName + "(Enabled)").GetComponent<SpriteRenderer>().enabled  = true;
                GameObject.Find("M" + generatorName + "(Disabled)").GetComponent<SpriteRenderer>().enabled = false;
            }
        }

        // If generator became busted play a break sound
        if(setting == true)
            GameObject.Find(generatorName).GetComponent<AudioSource>().Play();
        
        GameObject.Find(generatorName).GetComponent<Animator>().SetBool(parameterToSet, setting);
    }

    // Call an RPC to set objects to avtice or inactive
    [Command(requiresAuthority = false)]
    public void cmd_objectEnable(string target, bool setting, string generatorName)
    {
        rpc_objectEnable(target, setting, generatorName);
        Debug.Log("Called Command to set object");
    }

    // Set objects to avtice or inactive
    [ClientRpc]
    public void rpc_objectEnable(string target, bool setting, string generatorName){
        Debug.Log("Called RPC to set object");
        GameObject.Find(generatorName).GetComponent<GeneratorController>().local_objectEnable(target, setting);
    }    
    // Tell the clients to update their events
    [Command (requiresAuthority = false)]
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

    [Command (requiresAuthority = false)]
    public void cmd_DestroyWall(string wallName)
    {
        rpc_DestroyWall(wallName);
    }

    [ClientRpc]
    public void rpc_DestroyWall(string wallName)
    {
        GameObject wall = GameObject.Find(wallName);
        var crackedWallManager = Resources.FindObjectsOfTypeAll<GameObject>()
                                    .FirstOrDefault<GameObject>(x => x.GetComponent<ManageCrackedWalls>() != null)
                                    .GetComponent<ManageCrackedWalls>();
        crackedWallManager.DestroyCrackedWall(wall);

    }

    // Call an RPC to set the generator health
    [Command(requiresAuthority = false)]
    public void cmd_SetGeneratorHealth(string generatorName, int health){
        rpc_SetGeneratorHealth(generatorName, health);
    }

    // Set the generator health
    [ClientRpc]
    public void rpc_SetGeneratorHealth(string generatorName, int health)
    {
        var generator = GameObject.Find(generatorName);
        generator.GetComponent<GeneratorController>().healthPoints = health;
        // If the generator is taking damage play the damage sound
        if(health > 0)
            generator.GetComponent<AudioSource>().Play();
    }

    // Call an RPC to cause the runner to take attack damage
    [Command(requiresAuthority = false)]
    public void cmd_TakeAttackDamage(){
        rpc_TakeAttackDamage();
    }

    // Cause the runner to take attack damage
    [ClientRpc]
    public void rpc_TakeAttackDamage()
    {
        Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(gObject => gObject.name.Contains("Runner")).GetComponent<ManageRunnerStats>().TakeDamage(2);
    }
}
