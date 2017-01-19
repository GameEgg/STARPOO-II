using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Jurassic.Library;

public static class Utils  {

    public static double GetDouble(this ObjectInstance obj, string key){

        if (obj[key] is Int32)
        {

            return (double)(int)obj[key];
        }
        else {
            return (double)obj[key];
        }
    }
}
