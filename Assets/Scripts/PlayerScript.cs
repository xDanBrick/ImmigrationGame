using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public enum PolicyType
{
    Occupation, OccupationHierarchy, MigrationStatus, Unique
}

public enum JobTier
{
    tierOne, tierTwo, tierThree
}

public struct PolicyCard
{
    public int ammount;
    public PolicyType policyType;
    public int policyIndex;
    public string professionName;
    public string professionName2;
    public JobTier tierOne;
    public JobTier tierTwo;
    public int prosperityCount;
}

public class GameStuff
{
    public static int playerIndex = 0;
}

public class Characters
{
    public const int characterCount = 12;
    public static string[] professions = new string[7] { "Business", "Healthcare", "Legal", "Shop", "Retired", "Unemployed", "Student" };
    public static string[] names = new string[characterCount] { "Will",  "Jim", "John", "Steve", "Chris", "Ommar", "Annie", "Leanne", "Steph", "Karen", "Megan", "Alanah" };
    public static string[] tierOneJobs = new string[2] {
        "Buiness Manager", "Buisness Worker"
    };
    public static string[] tierTwoJobs = new string[3] {
        "Buiness Owner", "Paralegal", "Shop Owner"
    };
    public static string[] tierThreeJobs = new string[4] {
        "Doctor", "Nurse", "Lawyer", "Judge",
    };
    public static string[] nonWorkJobs = new string[2] {
        "Retired", "Unemployed"
    };
}

public struct Character
{
    public string name;
    public bool isMale;
    public MigrationStatus migrationStatus;
    public string roleName;
    public JobTier jobTier;
}


public class PlayerScript : NetworkBehaviour
{
    private List<GameObject> policies = new List<GameObject>();
    private GameObject votePolicy = null;
    public bool isCurrentPlayer = false;
    private uint playerIndex = 0;

    [SyncVar]
    bool isVoting = false;

    [SerializeField] GameObject policyButton;

    //public void OnRecievePolicies(NetworkMessage netMsg)
    //{
    //    //CharacterSet characterSet = netMsg.ReadMessage<CharacterSet>();
    //    //Transform characters = GameObject.Find("Characters").transform;
    //    //for (int i = 0; i < characters.childCount; ++i)
    //    //{
    //    //    //characters.GetChild(i).GetComponent<CharacterScript>().InitCharacter(characterSet.characters[i]);
    //    //}

    //    //GameObject policyParent = GameObject.Find("PolicyChoices");
    //    //for (int i = 0; i < policyParent.transform.childCount; ++i)
    //    //{
    //    //    Transform policyButton = policyParent.transform.GetChild(i);
    //    //    bool positive = Random.Range(0, 2) == 0 ? true : false;
    //    //    int ammount = Random.Range(1, 20);
    //    //    string line = (positive ? "+ " : "- ") + ammount.ToString();

    //    //    if (!positive)
    //    //    {
    //    //        ammount = -ammount;
    //    //    }
    //    //    policyButton.GetComponent<PolicyScript>().policyModifier = ammount;
    //    //    policyButton.GetComponentInChildren<Text>().text = "Policy " + (i + 1).ToString() + "\n\n" + line;
    //    //}
    //    //OnUpdateTurn();
    //}

    public class ConnectInfo : MessageBase
    {
        public int index;
    }

    //void OnConnect(NetworkMessage msg)
    //{
    //    ConnectInfo info = msg.ReadMessage<ConnectInfo>();
    //    playerIndex = info.index;
    //    //Debug.Log(playerIndex);
    //}

    void Start () {
    }

    // Update is called once per frame
    void Update()
    {
        if(!isLocalPlayer)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            
            if(isVoting)
            {
                if (votePolicy)
                {
                    votePolicy.GetComponent<Image>().color = Color.white;
                    CmdSendVote(new PolicyCard());
                    votePolicy = null;
                }
                
            }
            else
            {
                OnPolicySet();
                isVoting = true;
            }
            
        }
    }

    public void OnPolicySet()
    {
        if (policies.Count == 2)
        {
            int discardIndex = -1;
            GameObject policyParent = GameObject.Find("PolicyChoices");
            for (int i = 0; i < policyParent.transform.childCount; ++i)
            {
                bool isChosen = false;
                for (int j = 0; j < policies.Count; ++j)
                {
                    if(policyParent.transform.GetChild(i).gameObject == policies[j])
                    {
                        isChosen = true;
                        policyParent.transform.GetChild(i).GetComponent<Image>().color = Color.white;
                    }
                }
                if(!isChosen)
                {
                    discardIndex = i;
                }
            }
            CmdSendChoices(discardIndex);
            policies.Clear();
        }
    }

    [Command]
    void CmdSendChoices(int discardChoice)
    {
        //GameObject.Find("PlayerText").GetComponent<Text>().text = "Choices Recieved On Server";
        RpcSendChoicesToClient(discardChoice);
    }

    [ClientRpc]
    public void RpcAddCharacters(CharacterSet characterSet)
    {
        if (!isLocalPlayer)
        {
            return;
        }

        Debug.Log(netId.Value);

        if (netId.Value == 1)
        {
            SetCharacters(characterSet.characters, characterSet.characters2, characterSet.characters3, characterSet.characters4);
        }
        else if (netId.Value == 2)
        {
            SetCharacters(characterSet.characters2, characterSet.characters, characterSet.characters3, characterSet.characters4);
        }
        else if (netId.Value == 3)
        {
            SetCharacters(characterSet.characters3, characterSet.characters, characterSet.characters2, characterSet.characters4);
        }
        else if (netId.Value == 4)
        {
            SetCharacters(characterSet.characters4, characterSet.characters, characterSet.characters2, characterSet.characters3);
        }
        //if(playerId == playerIndex)
        //{
        //    Transform characters = GameObject.Find("Characters").transform;
        //    for (int j = 0; j < characters.childCount; ++j)
        //    {
        //        characters.GetChild(j).GetComponent<CharacterScript>().InitCharacter(characterSet.characters[j]);
        //    }

        //    //GameObject policyParent = GameObject.Find("PolicyChoices");
        //    //for (int i = 0; i < policyParent.transform.childCount; ++i)
        //    //{
        //    //    Transform policyButton = policyParent.transform.GetChild(i);
        //    //    bool positive = Random.Range(0, 2) == 0 ? true : false;
        //    //    int ammount = Random.Range(1, 20);
        //    //    string line = (positive ? "+ " : "- ") + ammount.ToString();

        //    //    if (!positive)
        //    //    {
        //    //        ammount = -ammount;
        //    //    }
        //    //    policyButton.GetComponent<PolicyScript>().policyModifier = ammount;
        //    //    policyButton.GetComponentInChildren<Text>().text = "Policy " + (i + 1).ToString() + "\n\n" + line;
        //    //}
        //}
        //else
        //{
        //    Transform characters = GameObject.Find("OtherCharacters").transform.GetChild(otherPlayerIndex);
        //    for (int j = 0; j < characters.childCount; ++j)
        //    {
        //        characters.GetChild(j).GetComponent<CharacterScript>().InitCharacter(characterSet.characters[j]);
        //    }
        //}
    }

    void SetCharacters(Character[] mainCharacters, Character[] character1, Character[] character2, Character[] character3)
    {
        Transform characters = GameObject.Find("Characters").transform;
        for (int j = 0; j < characters.childCount; ++j)
        {
            characters.GetChild(j).GetComponent<CharacterScript>().InitCharacter(mainCharacters[j]);
        }
        Transform otherCharacters = GameObject.Find("OtherCharacters").transform.GetChild(0);
        for (int j = 0; j < otherCharacters.childCount; ++j)
        {
            otherCharacters.GetChild(j).GetComponent<CharacterScript>().InitCharacter(character1[j]);
        }
        otherCharacters = GameObject.Find("OtherCharacters").transform.GetChild(1);
        for (int j = 0; j < otherCharacters.childCount; ++j)
        {
            otherCharacters.GetChild(j).GetComponent<CharacterScript>().InitCharacter(character2[j]);
        }
        otherCharacters = GameObject.Find("OtherCharacters").transform.GetChild(2);
        for (int j = 0; j < otherCharacters.childCount; ++j)
        {
            otherCharacters.GetChild(j).GetComponent<CharacterScript>().InitCharacter(character3[j]);
        }
    }

    [ClientRpc]
    void RpcSendChoicesToClient(int discardChoice)
    {
        GameObject.Find("PolicyChoices").transform.GetChild(discardChoice).GetComponent<Image>().color = Color.red;
    }

    [Command]
    void CmdSendVote(PolicyCard choices)
    {
        RpcSendVotesToClient(choices);
    }

    [ClientRpc]
    void RpcSendVotesToClient(PolicyCard choices)
    {
        GameObject.Find("PlayerText").GetComponent<Text>().text = "Votes Recieved";
    }

    public void SelectPolicy(int index)
    {
        GameObject policyButton = GameObject.Find("PolicyChoices").transform.GetChild(index).gameObject;
        if (isVoting)
        {
            if(votePolicy)
            {
                votePolicy.GetComponent<Image>().color = Color.white;
            }
            votePolicy = policyButton;
            policyButton.GetComponent<Image>().color = Color.green;
        }
        else
        {
            
            bool add = false;
            for (int i = 0; i < policies.Count; ++i)
            {
                if (policies[i] == policyButton)
                {
                    policies.Remove(policyButton);
                    add = true;
                    break;
                }
            }
            if (!add)
            {
                if (policies.Count < 2)
                {
                    policies.Add(policyButton);
                    Transform characters = GameObject.Find("Characters").transform;
                    for (int i = 0; i < characters.childCount; ++i)
                    {
                        PolicyCard card = new PolicyCard();
                        card.ammount = 5;
                        characters.GetChild(i).GetComponent<CharacterScript>().OnPolicyCard(card);
                    }
                }
                else
                {
                    return;
                }
            }
            policyButton.GetComponent<Image>().color = add ? Color.white : Color.green;
        }
    }
}
