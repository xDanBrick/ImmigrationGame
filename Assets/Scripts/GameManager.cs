using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CharacterSet : MessageBase
{
    public Character[] characters;
}

public class GameManager : NetworkManager {

    private List<PlayerScript> players = new List<PlayerScript>();
    private List<Character> characters = new List<Character>();

    private int currentPlayerIndex = 0;

    private GameObject playerText;
    [SerializeField] GameObject policyButton;
	// Use this for initialization
	void Start () {
        playerText = GameObject.Find("PlayerText");
        
    }
	
	// Update is called once per frame
	void Update () {
    }

    public void EndTurn()
    {
        if (currentPlayerIndex == players.Count - 1)
        {
            currentPlayerIndex = 0;
        }
        else
        {
            currentPlayerIndex++;
        }

        OnUpdateTurn();
    }

    void CreatePlayerDeck()
    {
        Debug.Log("Player Deck Created");
        List<string> names = new List<string>(Characters.names);
        List<string> tierOne = new List<string>(Characters.tierOneJobs);
        List<string> tierTwo = new List<string>(Characters.tierTwoJobs);
        List<string> tierThree = new List<string>(Characters.tierThreeJobs);

        for(int i = 0; i < 12; ++i)
        {
            int randomIndex = Random.Range(0, names.Count);
            Character character = new Character();
            character.name = names[randomIndex];
            names.Remove(character.name);
            character.isMale = randomIndex < 6;
            character.migrationStatus = (MigrationStatus)Random.Range(0, 4);
            switch (character.migrationStatus)
            {
                case MigrationStatus.local:
                    {
                        character.roleName = tierThree[Random.Range(0, tierThree.Count)];
                        break;
                    }
                case MigrationStatus.student:
                    {
                        character.roleName = "Student";
                        break;
                    }
                case MigrationStatus.undocumented:
                    {
                        character.roleName = tierOne[Random.Range(0, tierOne.Count)];
                        break;
                    }
                case MigrationStatus.working:
                    {
                        character.roleName = tierTwo[Random.Range(0, tierTwo.Count)];
                        break;
                    }
            }
            characters.Add(character);
        }
    }

    void OnUpdateTurn()
    {
        playerText.GetComponent<Text>().text = "Player " + (currentPlayerIndex + 1).ToString();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {

        Debug.Log("A client connected to the server: " + conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {

        NetworkServer.DestroyPlayersForConnection(conn);

        if (conn.lastError != NetworkError.Ok)
        {

            if (LogFilter.logError) { Debug.LogError("ServerDisconnected due to error: " + conn.lastError); }

        }

        Debug.Log("A client disconnected from the server: " + conn);

    }

    public override void OnServerReady(NetworkConnection conn)
    {

        NetworkServer.SetClientReady(conn);

        Debug.Log("Client is set to the ready state (ready to receive state updates): " + conn);

    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {

        var player = (GameObject)GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        CharacterSet msg = new CharacterSet();
        msg.characters = new Character[3];
        for (int i = 0; i < 3; ++i)
        {
            msg.characters[i] = characters[i];
        }

        characters.RemoveRange(0, 3);

        player.GetComponent<PlayerScript>().RpcOnRecievePolicies(msg, conn.connectionId);
        Debug.Log("Player Control Id " + conn.connectionId);
        //conn.Send(MsgType.Highest + 5, msg);

        Debug.Log("Client has requested to get his player added to the game");

    }

    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {

        if (player.gameObject != null)

            NetworkServer.Destroy(player.gameObject);

    }

    public override void OnServerError(NetworkConnection conn, int errorCode)
    {

        Debug.Log("Server network error occurred: " + (NetworkError)errorCode);

    }

    public override void OnStartHost()
    {
        CreatePlayerDeck();
        Debug.Log("Host has started");

    }

    public override void OnStartServer()
    {

        Debug.Log("Server has started");

    }

    public override void OnStopServer()
    {

        Debug.Log("Server has stopped");

    }

    public override void OnStopHost()
    {

        Debug.Log("Host has stopped");

    }

    // Client callbacks

    public override void OnClientConnect(NetworkConnection conn)

    {

        base.OnClientConnect(conn);

        Debug.Log("Connected successfully to server, now to set up other stuff for the client...");

    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {

        StopClient();

        if (conn.lastError != NetworkError.Ok)

        {

            if (LogFilter.logError) { Debug.LogError("ClientDisconnected due to error: " + conn.lastError); }

        }

        Debug.Log("Client disconnected from server: " + conn);

    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {

        Debug.Log("Client network error occurred: " + (NetworkError)errorCode);

    }

    public override void OnClientNotReady(NetworkConnection conn)
    {

        Debug.Log("Server has set client to be not-ready (stop getting state updates)");

    }

    public override void OnStartClient(NetworkClient client)
    {

        Debug.Log("Client has started");

    }

    public override void OnStopClient()
    {

        Debug.Log("Client has stopped");

    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {

        base.OnClientSceneChanged(conn);

        Debug.Log("Server triggered scene change and we've done the same, do any extra work here for the client...");

    }

}
