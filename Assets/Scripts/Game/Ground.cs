using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class Ground : GameSyncObject
{
    [SerializeField]
    LineRenderer line;
    [SerializeField]
    EdgeCollider2D collider;

    public override void GameInit()
    {
        DrawGround(GameVariables.groundSize.value);
        GameVariables.groundSize.AddListener(DrawGround);
    }

    void DrawGround(float r)
    {
        var max = line.numPositions = 100;
        var colPoints = new Vector2[100];
        for (int i = 0; i < max; ++i)
        {
            var th = Mathf.PI * 2 * i / (max-1);
            var p = new Vector2(Mathf.Cos(th) * r, Mathf.Sin(th) * r);
            line.SetPosition(i, p);
            colPoints[i] = p;
        }
        collider.points = colPoints;
    }
	
    void OnDestroy()
    {
        GameVariables.groundSize.RemoveListener(DrawGround);
    }

    void Reset()
    {
        line = GetComponent<LineRenderer>();
    }
}
