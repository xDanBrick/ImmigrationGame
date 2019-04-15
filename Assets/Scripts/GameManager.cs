using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public struct CharacterSet
{
    public Character[] characters, characters2, characters3, characters4;
}

public class GameManager : NetworkManager {

    private List<GameObject> players = new List<GameObject>();
    private List<Character> characters = new List<Character>();
    private List<PolicyCard> policyDeck = new List<PolicyCard>();
    private CharacterSet charactersSet = new CharacterSet();

    private int currentPlayerIndex = 0;

    private GameObject playerText;
    [SerializeField] GameObject policyButton;
	// Use this for initialization
	void Start () {
        playerText = GameObject.Find("PlayerText");
        
    }
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            
        }
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

    void AddPolicyCard(PolicyType type, int index, string profName, string profName2, JobTier tier, JobTier tier2)
    {
        PolicyCard card = new PolicyCard();
        card.policyType = type;
        card.professionName = profName;
        card.professionName2 = profName2;
        card.tierOne = tier;
        card.tierTwo = tier2;
        card.policyIndex = index;
        policyDeck.Add(card);
    }

    void AddPolicyCard(PolicyType type, int index)
    {
        PolicyCard card = new PolicyCard();
        card.policyType = type;
        card.policyIndex = index;
        policyDeck.Add(card);
    }

    void CreatePolicyDeck()
    {

        //CATEGORY ONE
        //+- Based on proffession
        for(int i = 0; i < 7; ++i)
        {
            for(int j = 0; j < 3; ++j)
            {
                AddPolicyCard(PolicyType.Occupation, j, Characters.professions[i], "", JobTier.tierOne, JobTier.tierOne);
            }
        }
        for (int i = 0; i < 4; ++i)
        {
            for (int j = 0; j < 4; ++j)
            {
                if(j != i)
                {
                    AddPolicyCard(PolicyType.Occupation, 3, Characters.professions[i], Characters.professions[j], JobTier.tierOne, JobTier.tierOne);
                }
            }
        }

        //CATEGORY TWO
        for (int i = 0; i < 3; ++i)
        {
            for (int j = 0; j < 3; ++j)
            {
                if (j != i)
                {
                    AddPolicyCard(PolicyType.OccupationHierarchy, 0, "", "", (JobTier)i, (JobTier)j);
                }
            }

            AddPolicyCard(PolicyType.OccupationHierarchy, 1, "", "", (JobTier)i, 0);
            AddPolicyCard(PolicyType.OccupationHierarchy, 2, "", "", (JobTier)i, 0);
            AddPolicyCard(PolicyType.OccupationHierarchy, 3, "", "", (JobTier)i, 0);
        }

        //CATEGORY THREE AND FOUR
        for(int i = 0; i < 4; ++i)
        {
            AddPolicyCard(PolicyType.MigrationStatus, i);
            AddPolicyCard(PolicyType.MigrationStatus, i);
            AddPolicyCard(PolicyType.Unique, i);
        }
    }

    public override void OnServerConnect(NetworkConnection conn)
    {

        Debug.Log("A client connected to the server: " + conn);
        
        //if (conn.connectionId == 4)
        //{

        //}
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

        if (conn.connectionId == 0)
        {
            charactersSet.characters = new Character[3];
            for (int i = 0; i < 3; ++i)
            {
                charactersSet.characters[i] = characters[i];
            }
        }
        else if (conn.connectionId == 1)
        {
            charactersSet.characters2 = new Character[3];
            for (int i = 0; i < 3; ++i)
            {
                charactersSet.characters2[i] = characters[i];
            }
        }
        else if (conn.connectionId == 2)
        {
            charactersSet.characters3 = new Character[3];
            for (int i = 0; i < 3; ++i)
            {
                charactersSet.characters3[i] = characters[i];
            }
        }
        else if (conn.connectionId == 3)
        {
            charactersSet.characters4 = new Character[3];
            for (int i = 0; i < 3; ++i)
            {
                charactersSet.characters4[i] = characters[i];
            }

            for (int i = 0; i < players.Count; ++i)
            {
                players[i].GetComponent<PlayerScript>().RpcAddCharacters(charactersSet);
            }
        }
        characters.RemoveRange(0, 3);

    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {

        var player = GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

        players.Add(player);

        //PlayerScript.ConnectInfo info = new PlayerScript.ConnectInfo();
        //info.index = currentPlayerIndex++;

        //conn.Send(MsgType.Highest + 5, info);

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
        CreatePolicyDeck();
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
        Debug.Log(conn);

        GameStuff.playerIndex = conn.connectionId;
        //PlayerScript.ConnectInfo info = new PlayerScript.ConnectInfo();
        //info.index = currentPlayerIndex - 1;
        //conn.Send(MsgType.Highest + 5, info);
    
        //GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>().setId(conn.connectionId);
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
        Debug.Log(client.connection);
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
