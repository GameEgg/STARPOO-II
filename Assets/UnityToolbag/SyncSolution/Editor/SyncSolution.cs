﻿using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace UnityToolbag
{
    public static class SyncSolution
    {
        [MenuItem("Assets/Sync Solution #&s")]
        public static void Sync()
        {
            var editor = Type.GetType("UnityEditor.SyncVS, UnityEditor");
            var SyncSolution = editor.GetMethod("SyncSolution", BindingFlags.Public | BindingFlags.Static);
            SyncSolution.Invoke(null, null);
            Debug.Log("Solution synced!");
        }
    }
}
