using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PolicyCards : MonoBehaviour {

    private List<GameObject> policies = new List<GameObject>();
    public int discardIndex = -1;
    private int votePolicyIndex = -1;

    enum SelectionState
    {
        choosePolicies, waiting, votingOnPolicy
    }

    SelectionState currentState = SelectionState.waiting;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
       
    }

    public void OnVoting(int discardChoice)
    {
        transform.GetChild(discardChoice).GetComponent<Image>().color = Color.red;

        discardIndex = discardChoice;
        if(currentState == SelectionState.waiting)
        {
            currentState = SelectionState.votingOnPolicy;
        }
        else if(currentState == SelectionState.choosePolicies)
        {
            currentState = SelectionState.waiting;
        }
    }

    public void OnRoundBegin(bool isTurn)
    {
        if(isTurn)
        {
            currentState = SelectionState.choosePolicies;
        }
        else
        {
            currentState = SelectionState.waiting;
        }

        //Reset Colors
        for (int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).GetComponent<Image>().color = Color.white;
        }
    }

    public void SelectPolicy(int index)
    {
        if (currentState != SelectionState.waiting)
        {
            GameObject policyButton = transform.GetChild(index).gameObject;
            if (currentState == SelectionState.votingOnPolicy)
            {
                if (index != discardIndex)
                {
                    if (votePolicyIndex != -1)
                    {
                        transform.GetChild(votePolicyIndex).GetComponent<Image>().color = Color.white;
                    }
                    votePolicyIndex = index;
                    policyButton.GetComponent<Image>().color = Color.green;
                }
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

    public void OnPolicySet()
    {
        if (currentState != SelectionState.waiting)
        {
            if (currentState == SelectionState.votingOnPolicy)
            {
                //for (int i = 0; i < transform.childCount; ++i)
                //{
                //    transform.GetChild(i).GetComponent<Image>().color = Color.white;
                //}
                var players = GameObject.FindGameObjectsWithTag("Player");
                for (int i = 0; i < players.Length; ++i)
                {
                    players[i].GetComponent<PlayerScript>().RpcSendVote(votePolicyIndex);
                }
                //GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>().RpcSendVote(votePolicyIndex);
                //Debug.Log("VoteSent");
                discardIndex = -1;
                votePolicyIndex = -1;
                currentState = SelectionState.waiting;
            }
            else
            {
                if (policies.Count == 2)
                {
                    int discardIndex = -1;
                    for (int i = 0; i < transform.childCount; ++i)
                    {
                        bool isChosen = false;
                        for (int j = 0; j < policies.Count; ++j)
                        {
                            if (transform.GetChild(i).gameObject == policies[j])
                            {
                                isChosen = true;
                                transform.GetChild(i).GetComponent<Image>().color = Color.white;
                            }
                        }
                        if (!isChosen)
                        {
                            discardIndex = i;
                        }
                    }
                    GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>().CmdSendChoices(discardIndex);
                    policies.Clear();
                }
            }
        }
    }
}
