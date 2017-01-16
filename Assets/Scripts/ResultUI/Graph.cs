using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Graph : MonoBehaviour
{
    [SerializeField]
    Text nameText;
    [SerializeField]
    Image graphRed;
    [SerializeField]
    Image graphBlue;

    // Use this for initialization
    void Awake () {
        nameText.color = new Color(1,1,1,0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
