using System.Collections;
using System.Collections.Generic;
using Mirror.Discovery;
using UnityEngine;

public class LobbyDataEntry : MonoBehaviour
{
    [SerializeField]
    public CustomNetworkManagerDAO networkManagerInterface;
    readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>(); // Servers currently listed as joinable (Only the data, not the GUI element)

    // Responsible for making server entries visible as well as managing which servers we've discovered
    public void CreateNewLobbyEntry(DiscoveryResponse discoveryResponse, ServerResponse serverResponse, bool debugLogging=true)
    {
        // UNFINISHED, still needs to create lobby entries but it will (probably) save them successfully
        if(debugLogging)
        {
            Debug.Log("ServerName: " + discoveryResponse.serverName);
            Debug.Log("Team Available: " + discoveryResponse.teamAvailable);
            Debug.Log("Players In Game: " + discoveryResponse.playersInGame);
        }
        discoveredServers[serverResponse.serverId] = serverResponse;
    }
}
