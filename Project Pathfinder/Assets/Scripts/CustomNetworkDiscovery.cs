using UnityEngine;
using UnityEngine.Events;
using Mirror;
using System;
using System.Net;
using Mirror.Discovery;

/*
    Documentation: https://mirror-networking.gitbook.io/docs/components/network-discovery
    API Reference: https://mirror-networking.com/docs/api/Mirror.Discovery.NetworkDiscovery.html
*/

[Serializable]
public class ServerFoundUnityEvent : UnityEvent<ServerResponse> {};

// Sent by a client to request a discovery response from games being hosted
public class DiscoveryRequest : NetworkMessage
{
    // Add properties for whatever information you want sent by clients
    // in their broadcast messages that servers will consume.
}

// Sent by a host as a reply to a discovery request, contains information about the game
public class DiscoveryResponse : NetworkMessage
{
    // Add properties for whatever information you want the server to return to
    // clients for them to display or consume for establishing a connection.
    public string serverName;       // Name of the server set by the host
    public Team teamAvailable;    // The team (runner/guards) available for the client to play as
    public int playersInGame;       // Number of players in the game
    public bool isHostRunner;       // Used to determine which team the client will join as in the server browser
}

// Used to contain an event for OnServerFound
public class CustomServerFoundUnityEvent : UnityEvent<ServerResponse, DiscoveryResponse> {};

//This class is responsible for advertising and discovering games waiting for players
public class CustomNetworkDiscovery : NetworkDiscoveryBase<DiscoveryRequest, DiscoveryResponse>
{
    [Tooltip("Invoked when a server is found")]
    public CustomNetworkManagerDAO networkManagerDao;         // Allows us to easily communicate with the network manager and its components
    public CustomServerFoundUnityEvent OnServerFound = new(); // Contains a delegate function to run when a server is found (see LobbyDataEntry class for more information)
    #region Server

    /// <summary>
    /// Reply to the client to inform it of this server
    /// </summary>
    /// <remarks>
    /// Override if you wish to ignore server requests based on
    /// custom criteria such as language, full server game mode or difficulty
    /// </remarks>
    /// <param name="request">Request coming from client</param>
    /// <param name="endpoint">Address of the client that sent the request</param>
    protected override void ProcessClientRequest(DiscoveryRequest request, IPEndPoint endpoint)
    {
        base.ProcessClientRequest(request, endpoint);
    }

    /// <summary>
    /// Process the request from a client, send a response containing info about the game being hosted
    /// </summary>
    /// <remarks>
    /// Override if you wish to provide more information to the clients
    /// such as the name of the host player
    /// </remarks>
    /// <param name="request">Request coming from client</param>
    /// <param name="endpoint">Address of the client that sent the request</param>
    /// <returns>A message containing information about this server</returns>
    protected override DiscoveryResponse ProcessRequest(DiscoveryRequest request, IPEndPoint endpoint) 
    {
        var isHostRunner = networkManagerDao.GetCustomNetworkManager().IsHostRunner;

        // Build the discovery response
        return new DiscoveryResponse()
        {
            serverName = networkManagerDao.GetServerBrowserBackend().serverName,
            playersInGame = NetworkManager.singleton.numPlayers,
            isHostRunner = isHostRunner,
            teamAvailable = isHostRunner ? Team.Guards : Team.Runner,
        };

    }

    // Tell all potential clients to remove this server from their list of joinable games
    public void CustomStopDiscovery()
    {
        StopDiscovery();
    }

    #endregion

    #region Client

    /// <summary>
    /// Create a message that will be broadcasted on the network to discover servers
    /// </summary>
    /// <remarks>
    /// Override if you wish to include additional data in the discovery message
    /// such as desired game mode, language, difficulty, etc... </remarks>
    /// <returns>An instance of ServerRequest with data to be broadcasted</returns>
    protected override DiscoveryRequest GetRequest()
    {
        return new DiscoveryRequest();
    }

    /// <summary>
    /// Process the answer from a server
    /// </summary>
    /// <remarks>
    /// A client receives a reply from a server, this method processes the
    /// reply and raises an event
    /// </remarks>
    /// <param name="response">Response that came from the server</param>
    /// <param name="endpoint">Address of the server that replied</param>
    protected override void ProcessResponse(DiscoveryResponse response, IPEndPoint endpoint)
    {
        /* To get these to display on the front-end of the server browser we can override OnServerFound(ServerResponse)
           to a delagate function used to create a lobby entry with the information from the response.
           If you have any questions about this strategy let me know (-Caleb) */
        OnServerFound.Invoke(new ServerResponse(){
            EndPoint = endpoint,
            serverId = RandomLong(),
            uri = new("kcp://"+endpoint.Address.ToString()/*+endpoint.Port.ToString()*/)
        }, response);
    }

    #endregion

    
}
