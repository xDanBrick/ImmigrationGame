using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBG : MonoBehaviour {

    public float scrollSpeed;
    public float tileSizeZ;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = GetComponent<RectTransform>().localPosition;
    }

    void Update()
    {
        float newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileSizeZ);
        GetComponent<RectTransform>().localPosition = startPosition + Vector3.up * newPosition;
    }
}
