using System.Collections;
using System.Collections.Generic;
using Mirror.Discovery;
using UnityEngine;
using TMPro;
using System.Reflection;
using System;
using UnityEngine.Events;
using System.Linq;

public class LobbyDataEntry : MonoBehaviour
{
    [SerializeField]
    public CustomNetworkManagerDAO networkManagerInterface;
    public GameObject serverEntryPrefab;
    readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>(); // Servers currently listed as joinable (Only the data, not the GUI element)
    private Dictionary<long, GameObject> serverList = new Dictionary<long, GameObject>();
    public static event UnityAction<ServerResponse, DiscoveryResponse> OnNewServer;

    public void Start() {
        OnNewServer = CreateNewLobbyEntry;

        if(networkManagerInterface.GetCustomNetworkDiscovery() == null)
        {
            Debug.LogError("networkDiscovery is null");
        }
        if(networkManagerInterface.GetCustomNetworkDiscovery().OnServerFound == null)
        {
            Debug.LogError("OnServerFound is null");
        }
        networkManagerInterface.GetCustomNetworkDiscovery().OnServerFound.AddListener(OnNewServer);
    }

    // Responsible for making server entries visible as well as managing which servers we've discovered
    public void CreateNewLobbyEntry(ServerResponse serverResponse, DiscoveryResponse discoveryResponse)
    {
        bool debugLogging=true;
        if(debugLogging)
        {
            Debug.Log("ServerName: " + discoveryResponse.serverName);
            Debug.Log("Team Available: " + discoveryResponse.teamAvailable);
            Debug.Log("Players In Game: " + discoveryResponse.playersInGame);
        }

        bool isNotCopy = 
        (discoveredServers.Values.AsEnumerable<ServerResponse>()
            .Select(x => x.serverId == serverResponse.serverId)
            .Count() == 0);
        if(isNotCopy)
        {
            //discoveredServers[serverResponse.serverId] = serverResponse; // Removed for testing

            discoveredServers.Add(serverResponse.serverId, serverResponse);

            GameObject newBox = Instantiate(serverEntryPrefab);
            GameObject newServerName = newBox.transform.GetChild(1).gameObject;
            GameObject newJoinButton = newBox.transform.GetChild(2).gameObject;
            GameObject teamAvailable = newBox.transform.GetChild(3).gameObject;
            GameObject numOfPlayers  = newBox.transform.GetChild(4).gameObject;

            newServerName.GetComponent<TMP_Text>().text = discoveryResponse.serverName;

            var backend = networkManagerInterface.GetServerBrowserBackend();
            var networkManager = networkManagerInterface.GetCustomNetworkManager();

            newJoinButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener( () => backend.JoinServer(serverResponse, networkManager, discoveryResponse.isHostRunner));

            teamAvailable.GetComponent<TMP_Text>().text = "Team: " + discoveryResponse.teamAvailable;
            numOfPlayers.GetComponent<TMP_Text>().text = "Players: " + discoveryResponse.playersInGame.ToString() + "/2";

            newBox.transform.SetParent(this.gameObject.transform);
        }
    }
}
