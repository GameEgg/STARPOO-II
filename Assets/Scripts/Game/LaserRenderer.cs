using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;
public class LaserRenderer : VolumetricLineBehavior {
    [SerializeField]
    Gradient colorByPower;
    [SerializeField]
    MeshRenderer renderer;

    Vector3 start;
    Vector3 end;

    protected override void Start()
    {
        base.Start();
        colorByPower.mode = GradientMode.Blend;
        renderer.enabled = false;
    }

    public void Show(Vector3 start, Vector3 end, float power)
    {
        LineColor = colorByPower.Evaluate(power / GameConsts.maxChargingPower);
        LineWidth = 4 * GameConsts.thiknessPerPower * power;
        renderer.enabled = true;

        this.start = start;
        this.end = end;
        UpdatePos();
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        while(LineWidth > 0.01f)
        {
            LineWidth = LineWidth * Mathf.Pow(0.005f, Time.deltaTime);
            UpdatePos();
            yield return null;
        }
        renderer.enabled = false;
    }

    void UpdatePos()
    {
        transform.rotation = Quaternion.identity;
        var pos = transform.position;
        StartPos = start - pos;
        EndPos = end - pos;
    }
}
