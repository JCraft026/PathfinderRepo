using System.Collections;
using System.Collections.Generic;
using Mirror.Discovery;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Net;
using System;

/*
    *This class acts as a frontend for the server browser's list of servers
    *NOTE: This will not currently clear out servers that are no longer available to join as of yet
*/
public class LobbyDataEntry : MonoBehaviour
{
    [SerializeField]
    public CustomNetworkManagerDAO networkManagerInterface; // Allows us to communicate more easily for the network manager and its component pieces
    public GameObject serverEntryPrefab;                    // Prefab to display on the game menu
    readonly Dictionary<IPEndPoint, ServerResponse> discoveredServers = new Dictionary<IPEndPoint, ServerResponse>();
                                                            // Servers currently listed as joinable (Only the data, not the GUI element)
    private Dictionary<long, GameObject> serverList = new Dictionary<long, GameObject>();
                                                            // List of server browser entries being displayed
    public static event UnityAction<ServerResponse, DiscoveryResponse> OnNewServer;
                                                            // Delegate that runs when a server is found (to the NetworkDiscovery) in this class' Start() function
    readonly Dictionary<IPEndPoint, DateTime> serverUpdatedTimes = new Dictionary<IPEndPoint, DateTime>();
    public void Start() {
        OnNewServer = CreateNewLobbyEntry; // Assign the delegate function for finding new servers

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
        // Vomit all the information about the incoming responses into the log file
        Debug.Log("Create new lobby entry");
        Debug.Log("ServerName: " + discoveryResponse.serverName);
        Debug.Log("Team Available: " + discoveryResponse.teamAvailable);
        Debug.Log("Players In Game: " + discoveryResponse.playersInGame);
        Debug.Log("ServerID: " + serverResponse.serverId);
        Debug.Log("ServerIPEndpoint: " + serverResponse.EndPoint);
        Debug.Log("ServerURI: " + serverResponse.uri);

        // If the incoming responses are not already being displayed, display them
        if(!discoveredServers.ContainsKey(serverResponse.EndPoint))
        {
            // Add the server to our list of open games
            discoveredServers.Add(serverResponse.EndPoint, serverResponse);

            // Create the server entry
            GameObject newBox = Instantiate(serverEntryPrefab);
            GameObject newServerName = newBox.transform.GetChild(1).gameObject;
            GameObject newJoinButton = newBox.transform.GetChild(2).gameObject;
            GameObject teamAvailable = newBox.transform.GetChild(3).gameObject;
            GameObject numOfPlayers  = newBox.transform.GetChild(4).gameObject;

            newServerName.GetComponent<TMP_Text>().text = discoveryResponse.serverName;

            var backend = networkManagerInterface.GetServerBrowserBackend();
            var networkManager = networkManagerInterface.GetCustomNetworkManager();

            // Set the onClick for the join button (makes it so that when we click the join button we join the server)
            newJoinButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener( () => backend.JoinServer(serverResponse, networkManager, discoveryResponse.isHostRunner));
            
            // Set server info displays
            teamAvailable.GetComponent<TMP_Text>().text = "Team: " + discoveryResponse.teamAvailable;
            numOfPlayers.GetComponent<TMP_Text>().text = "Players: " + discoveryResponse.playersInGame.ToString() + "/2";
            //TODO: Put a timer on the server boxes life (6ish seconds)
            serverUpdatedTimes.Add(serverResponse.EndPoint, DateTime.Now);
            StartCoroutine(CheckServerLifeSpan(newBox, serverResponse.EndPoint));

            // Make the new server box a child of the scrollview object
            newBox.transform.SetParent(this.gameObject.transform);
        }

        //TODO: Reset the timer on the servers lifespan if we catch its active discovery packet again
        else
        {
            serverUpdatedTimes[serverResponse.EndPoint] = DateTime.Now;
        }
    }

    public void DeleteOldEntry(GameObject lobbyEntry)
    {
        Destroy(lobbyEntry);
    }

    IEnumerator CheckServerLifeSpan(GameObject box, IPEndPoint serverIp)
    {
        
        while((DateTime.Now - serverUpdatedTimes[serverIp]).Ticks < 60000000)   //60000000 is the number of ticks in 6 seconds
        {
            //Debug.Log("Checking server box");
            yield return null;
        }
        serverUpdatedTimes.Remove(serverIp);
        discoveredServers.Remove(serverIp);
        Destroy(box);
    }
}
