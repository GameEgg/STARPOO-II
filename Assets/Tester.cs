using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start ()
    {
        yield return new WaitForSeconds(0.5f);
        //pScriptTest();
        //StartCoroutine( testChatUI());
        //pScriptTest();
        /*
        
        GameVariables.me = new Player();
        DontDestroyOnLoad(gameObject);

        NetworkManager.instance.StartServer();
        yield return new WaitForSeconds(2);
        GameVariables.me.name = "TESTER";
        GameVariables.me.networkId = 0;
        NetworkManager.instance.Connect();
        */
    }

    IEnumerator testChatUI()
    {

        NetworkVariables.isClient = true;
        NetworkVariables.isNetwork = true;
        for (int i = 0; i < 20; ++i)
        {
            var p = new Player();
            p.networkId = NetworkIdGenerator.Generate();
            p.name = "name + "+p.networkId;
            NetworkEvents.onPlayerJoin.Invoke(p);
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(0.5f);
    }

    void pScriptTest()
    {
        var script1 = new pScript();
        script1.code = "function update(){}";
        script1.name = "gameegg";
        script1.networkId = 3;
        script1.index = 1;

        var binary = script1.Serialize(PacketType.pSendScript);

        var binDataOnly = new byte[binary.Length - 3];
        System.Buffer.BlockCopy(binary, 3, binDataOnly, 0, binDataOnly.Length);

        var script2 = new pScript(binDataOnly);

        Debug.Log("script name (should be gameegg) : " + script2.name);
        Debug.Log("script code (should be function update(){}) : " + script2.code);
        Debug.Log("script networkId (should be 3) : " + script2.networkId);
        Debug.Log("script index (should be 1) : " + script2.index);
    }

    void pPlayerTest()
    {
        var player = new Player();
        player.name = "tester";
        player.networkId = 77;
        var p = new pPlayer(player);

        var b = p.Serialize(PacketType.pConnect);

        var b2 = new byte[b.Length - 3];
        System.Buffer.BlockCopy(b, 3, b2, 0, b2.Length);

        var p2 = new pPlayer(b2);

        Debug.Log("tester name(should be tester) : " + p2.player.name);
        Debug.Log("tester id(should be 77) : " + p2.player.networkId);

    }

    void PacketHeaderSerializerTest()
    {

        var h = new PacketHeader();
        h.packetType = (byte)PacketType.pJoin;
        h.size = 1024 * 20;

        Debug.Log(h.size);
        Debug.Log(h.packetType);
        Debug.Log("----------should be same------>");
        var p = PacketHeaderSerializer.Serialize(h);
        var b = new byte[1024];
        System.Buffer.BlockCopy(p, 0, b, 0, 3);
        h.size = 1024 * 7;
        PacketHeaderSerializer.Deserializer(b, ref h);
        Debug.Log(h.size);
        Debug.Log(h.packetType);
    }
}
