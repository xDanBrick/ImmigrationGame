using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProsperityScroller : MonoBehaviour {

    float startY = 0.0f;
	// Use this for initialization
	void Start () {
        startY = transform.localPosition.y;

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.Translate(0.0f, 0.25f, 0.0f);
        if(transform.localPosition.y > startY + 70)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, startY);
        }
    }
}
