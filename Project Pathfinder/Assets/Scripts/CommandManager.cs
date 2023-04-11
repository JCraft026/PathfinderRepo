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
}
