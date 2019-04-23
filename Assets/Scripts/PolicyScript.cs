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

    // Use this for initialization
    int m_PolicyModifier = 0;

    public int policyModifier { get { return m_PolicyModifier; } set { m_PolicyModifier = value; } }

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void setPolicy(PolicyCard card)
    {
        Text text = GetComponentInChildren<Text>();
        switch (card.policyType)
        {
            case PolicyType.MigrationStatus:
            {
                text.text = "Migration";
                break;
            }
            case PolicyType.Occupation:
            {
                text.text = "Occupation";
                break;
            }
            case PolicyType.OccupationHierarchy:
            {
                text.text = "OccupationHierarchy";
                break;
            }
        }
    }
}
