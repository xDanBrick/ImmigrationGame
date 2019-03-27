using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public struct PolicyCard
{
    public int ammount;
}
public class Characters
{
    public const int characterCount = 12;
    public static string[] names = new string[characterCount] { "Will",  "Jim", "John", "Steve", "Chris", "Ommar", "Annie", "Leanne", "Steph", "Karen", "Megan", "Alanah" };
    public static Job[] jobs = new Job[characterCount] {
        new Job(RoleType.owner, "Buiness Owner"), new Job(RoleType.management, "Buiness Manager"), new Job(RoleType.unskilled, "Buisness Worker"), new Job(RoleType.skilled, "Doctor"),
        new Job(RoleType.skilled, "Nurse"), new Job(RoleType.skilled, "Lawyer"), new Job(RoleType.skilled, "Judge"), new Job(RoleType.unskilled, "Paralegal"),
        new Job(RoleType.retired, "Retired"), new Job(RoleType.unemployed, "Unemployed"), new Job(RoleType.student, "Student"), new Job(RoleType.owner, "Shop Owner")
    };
}

public struct Character
{
    string name;
    bool isMale;
    MigrationStatus migrationStatus;
    RoleType roleType;
    string roleName;
}


public class PlayerScript : NetworkBehaviour
{
    private List<GameObject> policies = new List<GameObject>();
    private GameObject votePolicy = null;
    public bool isCurrentPlayer = false;

    [SyncVar]
    bool isVoting = false;

    [SerializeField] GameObject policyButton;

    public void OnRecievePolicies(NetworkMessage netMsg)
    {
        TableMessage tableMessage = netMsg.ReadMessage<TableMessage>();

        GameObject policyParent = GameObject.Find("PolicyChoices");
        for (int i = 0; i < policyParent.transform.childCount; ++i)
        {
            Transform policyButton = policyParent.transform.GetChild(i);
            bool positive = Random.Range(0, 2) == 0 ? true : false;
            int ammount = Random.Range(1, 20);
            string line = (positive ? "+ " : "- ") + ammount.ToString();

            if (!positive)
            {
                ammount = -ammount;
            }
            policyButton.GetComponent<PolicyScript>().policyModifier = ammount;
            policyButton.GetComponentInChildren<Text>().text = "Policy " + (i + 1).ToString() + "\n\n" + line;
        }
        //OnUpdateTurn();
    }

    void Start () {
        if(isClient)
        {
            NetworkManager.singleton.client.RegisterHandler(MsgType.Highest + 5, OnRecievePolicies);
            GameObject character = new GameObject();
            character.transform.SetParent(transform);
        }
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
