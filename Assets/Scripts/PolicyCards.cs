using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PolicyCards : MonoBehaviour {

    private List<GameObject> policies = new List<GameObject>();
    public int discardIndex = -1;
    private int votePolicyIndex = -1;
    private int yearsPast = 0;

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

    void setChildColor(int index, Color color)
    {
        Transform child = transform.GetChild(index);
        child.Find("PolicyBorder").GetComponent<Image>().color = color;
        child.Find("PolicyImage").GetComponent<Image>().color = color;
    }

    public void OnVoting(int discardChoice)
    {
        setChildColor(discardChoice, Color.red);

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
        GameObject.Find("YearsText").GetComponent<Text>().text = "Years Past: " + yearsPast.ToString();
        ++yearsPast;

        if (isTurn)
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
            setChildColor(i, Color.white);
        }

        Transform voteCount = GameObject.Find("PolicyChoices").transform;
        for(int i = 0; i < voteCount.childCount; ++i)
        {
            voteCount.GetChild(i).Find("VoteCount").GetComponent<Text>().text = "0";
        }

        discardIndex = -1;
        votePolicyIndex = -1;
        policies.Clear();
    }

    public void SelectPolicy(int index)
    {
        GameObject.Find("MenuSound").GetComponent<AudioSource>().Play();
        if (currentState != SelectionState.waiting)
        {
            if (currentState == SelectionState.votingOnPolicy)
            {
                if (index != discardIndex)
                {
                    if (votePolicyIndex != -1)
                    {
                        setChildColor(votePolicyIndex, Color.white);
                    }
                    votePolicyIndex = index;
                    setChildColor(index, Color.green);
                }
            }
            else
            {
                GameObject policyButton = transform.GetChild(index).gameObject;
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
                    }
                    else
                    {
                        return;
                    }
                }
                setChildColor(index, add ? Color.white : Color.green);
            }
        }
    }

    public void OnPolicySet()
    {
        GameObject.Find("MenuSound").GetComponent<AudioSource>().Play();
        if (currentState != SelectionState.waiting)
        {
            if (currentState == SelectionState.votingOnPolicy)
            {
                PlayerScript.GetLocalPlayer().SendVote(votePolicyIndex);

                GameObject.Find("InfoText").GetComponent<Text>().text = "Waiting on other Players";

                setChildColor(votePolicyIndex, Color.white);

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
                                setChildColor(i, Color.white);
                                isChosen = true;
                            }
                        }
                        if (!isChosen)
                        {
                            discardIndex = i;
                        }
                    }

                    PlayerScript.GetLocalPlayer().SendChoices(discardIndex);
                    policies.Clear();
                }
            }
        }
    }
}
