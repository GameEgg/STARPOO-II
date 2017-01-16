using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RoomEvents
{
    public static SelectedScriptEvent onScriptSelected = new SelectedScriptEvent();
    public static SelectedScriptEvent onScriptCanceled = new SelectedScriptEvent();
}