using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Utils  {
    public static double RoundToDouble(this float a){
        return ((double)a).Round();
    }
    public static float RoundToFloat(this double a){
        return ((float)a).Round();
    }
    public static double Round(this double a){
        return Math.Round(a * 100d) * 0.01d;
    }
    public static float Round(this float a){
        return Mathf.Round(a * 100f) * 0.01f;
    }
}
