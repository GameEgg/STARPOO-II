using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class ScriptLoader {

    public static List<AIScript> LoadAIScripts()
    {
        if (GameVariables.me == null)
        {
            GameVariables.me = new Player();
            GameVariables.me.name = "Room Tester";
        }

        List<AIScript> list = new List<AIScript>();
        string[] paths = Directory.GetFiles(Directory.GetCurrentDirectory() + @"/Script/", "*.js");
        foreach (var path in paths)
        {
            var script = new AIScript();
            script.name = Path.GetFileNameWithoutExtension(path);
            script.code = File.ReadAllText(path);
            script.author = GameVariables.me;

            list.Add(script);
        }

        return list;
    }

}
