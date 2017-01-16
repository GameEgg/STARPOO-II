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
    [SerializeField]
    Text redGraphText;
    [SerializeField]
    Text blueGraphText;
    
    [SerializeField]
    float GRAPH_HEIGHT = 30;
    [SerializeField]
    float MAX_GRAPH_WIDTH = 360;

    // Use this for initialization
    void Awake () {
        // Prevent Text color tranparent
        nameText.color = new Color(1,1,1,0);
	}
	
    public void SetGraphValue (
        double redValue, 
        double blueValue, 
        string redText,
        string blueText, 
        Color redGraphColor,
        Color blueGraphColor)
    {
        graphRed.fillAmount = (float)redValue;
        graphBlue.fillAmount = (float)blueValue;

        redGraphText.text = redText;
        blueGraphText.text = blueText;

        graphRed.color = redGraphColor;
        graphBlue.color = blueGraphColor;
    }
}
