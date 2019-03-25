using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolicyScript : MonoBehaviour {

    // Use this for initialization
    int m_PolicyModifier = 0;

    public int policyModifier { get { return m_PolicyModifier; } set { m_PolicyModifier = value; } }

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnPolicyChange()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>().SelectPolicy(gameObject);
    }

    public void Vote()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>().SelectVote();
    }
}
