using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[AddComponentMenu("")]
public class PongNetworkManager : NetworkManager
{
    public Transform PlayerOneSpawn; //Host player
    public Transform PlayerTwoSpawn; //Client player;
    GameObject Ball;

    public override void OnServerAddPlayer(Mirror.NetworkConnectionToClient clientCon)
    {
        //add player at correct spawns
        Transform playerStart = numPlayers == 0 ? PlayerOneSpawn : PlayerTwoSpawn;
        GameObject player = Instantiate(playerPrefab, playerStart.position, playerStart.rotation);
        NetworkServer.AddPlayerForConnection(clientCon, player);
        if(numPlayers == 1)
        {
            Ball = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Ball"));
            Mirror.NetworkServer.Spawn(Ball);
        }
    }

    public override void OnServerDisconnect(Mirror.NetworkConnectionToClient connection)
    {
        //destroy ball
        if(Ball != null)
        {
            NetworkServer.Destroy(Ball);
        }

        //Calls the base function and destroys the player
        base.OnServerDisconnect(connection);
    }
}
