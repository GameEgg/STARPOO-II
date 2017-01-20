using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CubeColor : MonoBehaviour {

    public Color color;
    //public Renderer renderer;
	// Use this for initialization
	void Start ()
    {
        var renderer = GetComponent<Renderer>();
        renderer.sharedMaterial.color = color;
        renderer.sharedMaterial.EnableKeyword("_EMISSION");
        renderer.sharedMaterial.SetColor("_EmissionColor", color*1f);
    }
	
}
