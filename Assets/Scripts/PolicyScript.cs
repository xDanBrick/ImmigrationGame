using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PolicyScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public void OnPointerEnter(PointerEventData eventData)
    {
        //transform.Find("PolicyImage").GetComponent<Image>().enabled = false;
        //transform.Find("PolicyText").GetComponent<Text>().enabled = false;
        //transform.Find("PolicyText 2").GetComponent<Text>().enabled = true;
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //transform.Find("PolicyImage").GetComponent<Image>().enabled = true;
        //transform.Find("PolicyText").GetComponent<Text>().enabled = true;
        //transform.Find("PolicyText 2").GetComponent<Text>().enabled = false;
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
                        text.text = "If you have undocumented migration status with positive prosperity, gain documented status";
                    }
                    //+n Prosperity if you are illegal, -N prosperity for legal. (vice versa) - 2 Cards.
                    else if (card.policyIndex == 1)
                    {
                        text.text = "+" + card.prosperityCount.ToString() + " Prosperity if you are undocumented, -" + card.prosperityCount.ToString() + "prosperity for documented or local";
                    }
                    //Lose job if illegal with -n prosperity, legals get +N prosperity. (after 3 turns) - 2 Cards
                    else if (card.policyIndex == 2)
                    {
                        text.text = "Lose job if undocumented with -" + card.prosperityCount.ToString() + " prosperity, legals get +" + card.prosperityCount.ToString() + " prosperity";
                    }

                    break;
                }
            case PolicyType.Occupation:
                {
                    //If you are of X profession +-n. - one card for each profession = 7 cards (any duplicates?)
                    if (card.policyIndex == 0)
                    {
                        text.text = "If you are of " + card.professionName + " profession +" + card.prosperityCount.ToString();
                    }
                    //If you are of -N prosperity as X profession +n else -n. - 7 cards
                    else if (card.policyIndex == 1)
                    {
                        text.text = "If you are of positive prosperity as " + card.professionName + " profession +" + card.prosperityCount.ToString() + " else -" + card.prosperityCount.ToString() + " prospertity";
                    }
                    //If your prosperity is +n you get a promotion in your job, (if you are legal) (after n turns) - 7 cards.
                    else if (card.policyIndex == 2)
                    {
                        text.text = "If your prosperity is +" + card.prosperityCount.ToString() + " you get a promotion in your job, if documented";
                    }
                    //Gain +N prosperity for X profession, lose -n prosperity for Z profession. - 4P2 = 12cards, only the first four occupation group.
                    else if (card.policyIndex == 3)
                    {
                        text.text = "Gain +" + card.prosperityCount.ToString() + " prosperity for " + card.professionName + " profession, lose -" + card.prosperityCount.ToString() + " prosperity for " + card.professionName2 + " profession";
                    }
                    break;
                }
            case PolicyType.OccupationHierarchy:
                {
                    //Tier X workers can gain +n prosperity but tier Z workers will lose -n Prosperity (Vice Versa.) - 3P2 = 6 cards.
                    if (card.policyIndex == 0)
                    {
                        text.text = "Tier " + (card.tierOne + 1).ToString() + " workers can gain +" + card.prosperityCount.ToString() + " prosperity but " + card.tierTwo.ToString() + " workers will lose -" + card.prosperityCount.ToString() + " prosperity";
                    }
                    //Tier X workers can change jobs or promote, if positive prosperity. - 3 Cards.
                    else if (card.policyIndex == 1)
                    {
                        text.text = card.tierOne.ToString() + " workers can change jobs or promote, if positive prosperity";
                    }
         
                    break;
                }
        }
    }
}
