using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameVariables
{
    public static Player me;
    public static Observable<float> groundSize = new Observable<float>();
    public static Observable<float> groundChangeTimer = new Observable<float>();
    public static int gameFrame = 0;
    public static bool updateBlocked;
}
