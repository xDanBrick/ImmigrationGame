using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PolicyScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.Find("PolicyImage").GetComponent<Image>().enabled = false;
        transform.Find("PolicyText").GetComponent<Text>().enabled = false;
        transform.Find("PolicyText 2").GetComponent<Text>().enabled = true;
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.Find("PolicyImage").GetComponent<Image>().enabled = true;
        transform.Find("PolicyText").GetComponent<Text>().enabled = true;
        transform.Find("PolicyText 2").GetComponent<Text>().enabled = false;
    }

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void setPolicy(PolicyCard card)
    {
        Text text = transform.Find("PolicyText").GetComponent<Text>();
        switch (card.policyType)
        {
            case PolicyType.MigrationStatus:
                {
                    //If you have illegal migration status with N prosperity, gain Documented status - 2 Cards.
                    if (card.policyIndex == 0)
                    {
                        text.text = "If you have illegal migration status with " + card.prosperityCount.ToString() + " prosperity, gain Documented status";
                    }
                    //+n Prosperity if you are illegal, -N prosperity for legal. (vice versa) - 2 Cards.
                    else if (card.policyIndex == 1)
                    {
                        text.text = "+" + card.prosperityCount.ToString() + " Prosperity if you are undocumented, -" + card.prosperityCount.ToString() + "prosperity for documented or local";
                    }
                    //Lose job if illegal with -n prosperity, legals get +N prosperity. (after 3 turns) - 2 Cards
                    else if (card.policyIndex == 3)
                    {
                        text.text = "Lose job if illegal with -" + card.prosperityCount.ToString() + " prosperity, legals get +" + card.prosperityCount.ToString() + " prosperity";
                    }

                    break;
                }
            case PolicyType.Occupation:
                {
                    //If you are of X profession +-n. - one card for each profession = 7 cards (any duplicates?)
                    if (card.policyIndex == 0)
                    {
                        text.text = "If you are of " + card.professionName + " profession +-n. - one card for each profession";
                    }
                    //If you are of -N prosperity as X profession +n else -n. - 7 cards
                    else if (card.policyIndex == 1)
                    {
                        text.text = "If you are of -N prosperity as X profession +n else -n";
                    }
                    //If your prosperity is +n you get a promotion in your job, (if you are legal) (after n turns) - 7 cards.
                    else if (card.policyIndex == 2)
                    {
                        text.text = "If your prosperity is +n you get a promotion in your job, (if you are legal) (after n turns)";
                    }
                    //Gain +N prosperity for X profession, lose -n prosperity for Z profession. - 4P2 = 12cards, only the first four occupation group.
                    else if (card.policyIndex == 3)
                    {
                        text.text = "Gain +N prosperity for X profession, lose -n prosperity for Z profession. - 4P2 = 12cards, only the first four occupation group";
                    }
                    break;
                }
            case PolicyType.OccupationHierarchy:
                {
                    //Tier X workers can gain +n prosperity but tier Z workers will lose -n Prosperity (Vice Versa.) - 3P2 = 6 cards.
                    if (card.policyIndex == 0)
                    {
                        text.text = "Tier X workers can gain +n prosperity but tier Z workers will lose -n Prosperity";
                    }
                    //Tier X workers can change jobs or promote, if positive prosperity. - 3 Cards.
                    else if (card.policyIndex == 1)
                    {
                        text.text = "Tier X workers can change jobs or promote, if positive prosperity";
                    }
                    //Tier X workers have a chance to lose their jobs, -n prosperity. - 3 Cards.
                    else if (card.policyIndex == 2)
                    {
                        text.text = "Tier X workers have a chance to lose their jobs, -n prosperity";
                    }
                    //Tier X workers gain +1 prosperity for the next 2 rounds. - 3 Cards.
                    else if (card.policyIndex == 3)
                    {
                        text.text = "Tier X workers gain +1 prosperity for the next 2 rounds";
                    }
                    break;
                }
        }
    }
}
