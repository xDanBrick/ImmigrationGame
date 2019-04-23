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

    void Start () {
    }

    // Update is called once per frame
    void Update()
    {
    }

    

    [ClientRpc]
    public void RpcRecievePolicies(PolicyCard[] cards)
    {
        if (!isLocalPlayer)
        {
            return;
        }

        GameObject policyParent = GameObject.Find("PolicyChoices");
        for (int i = 0; i < policyParent.transform.childCount; ++i)
        {
            Transform policyButton = policyParent.transform.GetChild(i);
            policyButton.GetComponent<PolicyScript>().setPolicy(cards[i]);
        }
    }

    [Command]
    public void CmdSendChoices(int discardChoice)
    {
        RpcSendChoicesToClient(discardChoice);
    }

    [ClientRpc]
    public void RpcAddCharacters(CharacterSet characterSet)
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (netId.Value == 1)
        {
            SetCharacters(characterSet.characters, characterSet.characters2, characterSet.characters3, characterSet.characters4);
            SetCharacterNames(2, 3, 4);
        }
        else if (netId.Value == 2)
        {
            SetCharacters(characterSet.characters2, characterSet.characters, characterSet.characters3, characterSet.characters4);
            SetCharacterNames(1, 3, 4);
        }
        else if (netId.Value == 3)
        {
            SetCharacters(characterSet.characters3, characterSet.characters, characterSet.characters2, characterSet.characters4);
            SetCharacterNames(1, 2, 4);
        }
        else if (netId.Value == 4)
        {
            SetCharacters(characterSet.characters4, characterSet.characters, characterSet.characters2, characterSet.characters3);
            SetCharacterNames(1, 2, 3);
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

    void SetCharacterNames(uint index2, uint index3, uint index4)
    {
        GameObject.Find("MainPlayerText").GetComponent<Text>().text = "Player " + netId.Value.ToString();
        GameObject.Find("OtherPlayerText1").GetComponent<Text>().text = "Player " + index2.ToString();
        GameObject.Find("OtherPlayerText2").GetComponent<Text>().text = "Player " + index3.ToString();
        GameObject.Find("OtherPlayerText3").GetComponent<Text>().text = "Player " + index4.ToString();

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
        GameObject.Find("InfoText").GetComponent<Text>().text = "Vote on a policy!";
        GameObject.Find("PolicyChoices").GetComponent<PolicyCards>().OnVoting(discardChoice);
    }

    [Command]
    public void CmdSendVote(int index)
    {
        RpcVoteToClient(index);
        //GameObject.Find("ServerManager").GetComponent<ServerManager>().AddVote(index);
    }

    [ClientRpc]
    void RpcVoteToClient(int index)
    {
        GameObject.Find("PolicyChoices").transform.GetChild(index).Find("VoteCount").GetComponent<Text>().text = "Works";

    }

    [ClientRpc]
    public void RpcUpdatePolicyCard(PolicyCard choices)
    {
        if (!isLocalPlayer)
        {
            return;
        }

        Transform characters = GameObject.Find("Characters").transform;
        for (int j = 0; j < characters.childCount; ++j)
        {
            characters.GetChild(j).GetComponent<CharacterScript>().OnPolicyCard(choices);
        }
    }

    [ClientRpc]
    public void RpcBeginTurn(int index)
    {
        if(!isLocalPlayer)
        {
            return;
        }

        GameObject.Find("InfoText").GetComponent<Text>().text = "Player " + (index + 1).ToString() + ": Choose 2 policies to put to a vote";
        GameObject.Find("PolicyChoices").GetComponent<PolicyCards>().OnRoundBegin((index + 1) == netId.Value);
    }
}
