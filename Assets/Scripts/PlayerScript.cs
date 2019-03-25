using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public struct PolicyCard
{
    public int ammount;
}

public class PlayerScript : NetworkBehaviour
{
    private List<GameObject> policies = new List<GameObject>();

    public bool isCurrentPlayer = false;

    [SyncVar]
    bool isVoting = false;

    [SerializeField] GameObject policyButton;

    public void OnTableMessage(NetworkMessage netMsg)
    {
        TableMessage tableMessage = netMsg.ReadMessage<TableMessage>();

        GameObject policyParent = GameObject.Find("PolicyChoices");
        for (int i = 0; i < policyParent.transform.childCount; ++i)
        {
            Transform policyButton = policyParent.transform.GetChild(i);
            policyButton.gameObject.SetActive(true);
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
            NetworkManager.singleton.client.RegisterHandler(MsgType.Highest + 5, OnTableMessage);
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
            Debug.Log(isVoting);
            if(isVoting)
            {
                CmdSendVote(new PolicyCard());
                GameObject voteChoices = GameObject.Find("PolicyVoteChoices");
                for (int i = 0; i < voteChoices.transform.childCount; ++i)
                {
                    voteChoices.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            else
            {
                OnPolicySet();
            }
            
        }
    }

    public void OnPolicySet()
    {
        if (policies.Count == 2)
        {
            PolicyCard[] choices = new PolicyCard[policies.Count];
            for (int i = 0; i < policies.Count; ++i)
            {
                choices[i].ammount = policies[i].GetComponent<PolicyScript>().policyModifier;
            }

            GameObject policyParent = GameObject.Find("PolicyChoices");
            for (int i = 0; i < policyParent.transform.childCount; ++i)
            {
                policyParent.transform.GetChild(i).gameObject.SetActive(false);
            }
            isCurrentPlayer = true;
            CmdSendChoices(choices);
            policies.Clear();
        }
    }

    [Command]
    void CmdSendChoices(PolicyCard[] choices)
    {
        GameObject.Find("PlayerText").GetComponent<Text>().text = "Choices Recieved On Server";
        RpcSendChoicesToClient(choices);
        isVoting = true;
    }

    [ClientRpc]
    void RpcSendChoicesToClient(PolicyCard[] choices)
    {
        if(!isCurrentPlayer)
        {
            GameObject voteChoices = GameObject.Find("PolicyVoteChoices");
            for (int i = 0; i < voteChoices.transform.childCount; ++i)
            {
                GameObject policyButton = voteChoices.transform.GetChild(i).gameObject;
                policyButton.SetActive(true);
                policyButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-100 + (i * 200), 200.0f);
                policyButton.GetComponentInChildren<Text>().text = "Policy " + (i + 1).ToString() + "\n\n" + choices[i].ammount.ToString();
            }
        }


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

    public void SelectVote()
    {

    }

    public void SelectPolicy(GameObject gameObject)
    {
        bool add = false;
        for(int i = 0; i < policies.Count; ++i)
        {
            if (policies[i] == gameObject)
            {
                policies.Remove(gameObject);
                add = true;
                break;
            }
        }
        if(!add)
        {
            if (policies.Count < 2)
            {
                policies.Add(gameObject);
                Transform characters = GameObject.Find("Characters").transform;
                for(int i = 0; i < characters.childCount; ++i)
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
        gameObject.GetComponent<Image>().color = add ? Color.white : Color.red;
    }
}
