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
        return Math.Round(a,2);
    }
    public static double Floor(this double a){
        return Math.Floor(a);
    }
    public static float Round(this float a){
        return (float)Math.Round(a,2);
    }
}
