using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkPacketHandler
{
    delegate void Handler(byte[] data);
    Dictionary<byte, Handler> handlers;

    public NetworkPacketHandler()
    {
        handlers = new Dictionary<byte, Handler>();
        handlers.Add((byte)PacketType.pJoin, HandlePJoin);
        handlers.Add((byte)PacketType.pNetworkId, HandlePNetworkId);
        handlers.Add((byte)PacketType.pConnect, HandlePConnect);
        handlers.Add((byte)PacketType.pChat, HandlePChat);
        handlers.Add((byte)PacketType.pDisconnect, HandlePDisconnect);
        handlers.Add((byte)PacketType.pSendScript, HandlePSendScript);
        handlers.Add((byte)PacketType.pCancelScript, HandlePCancelScript);
        handlers.Add((byte)PacketType.pPlayGame, HandlePPlayGame);
        handlers.Add((byte)PacketType.pGameEnd, HandlePGameEnd);
        handlers.Add((byte)PacketType.pShipAmount, HandlePShipAmount);
        handlers.Add((byte)PacketType.pKick, HandlePKick);
    }

    public void Handle(byte packetType, byte[] data)
    {
        Debug.LogWarning("got network event : " + ((PacketType)packetType).ToString());
        handlers[packetType](data);
    }

    void HandlePKick(byte[] data)
    {
        SceneManager.LoadScene("Main");
        NoticeUI.instance.ShowMessage("Fail to connect");
    }

    void HandlePShipAmount(byte[] data)
    {
        var packet = new pByte(data);
        NetworkEvents.onShipAmountChanged.Invoke(packet.value);
    }

    void HandlePGameEnd(byte[] data)
    {
        NetworkEvents.onGameEnd.Invoke();
    }

    void HandlePPlayGame(byte[] data)
    {
        NetworkEvents.onPlayGame.Invoke();
    }

    void HandlePCancelScript(byte[] data)
    {
        var packet = new pByte(data);
        NetworkEvents.onScriptOut.Invoke(packet.value);
    }

    void HandlePSendScript(byte[] data)
    {
        var packet = new pScript(data);
        var aiScript = new AIScript();
        aiScript.author = NetworkManager.instance.playerContainer.GetPlayer(packet.networkId);
        aiScript.code = packet.code;
        aiScript.name = packet.name;
        aiScript.color = (packet.index == 0) ? Color.red : Color.blue;

        NetworkEvents.onScriptIn.Invoke(aiScript);
    }

    void HandlePDisconnect(byte[] data)
    {
        var packet = new pByte(data);
        if (NetworkVariables.isServer)
        {
            TransferTCP.instance.sendToClientsQueue.Enqueue(packet.Serialize(PacketType.pChat));
        }
        if(NetworkVariables.isClient)
        {
            NetworkEvents.onPlayerOut.Invoke(packet.value);
        }
    }

    void HandlePChat(byte[] data)
    {
        var packet = new pChatMessage(data);
        NetworkEvents.onChat.Invoke(packet.chatMessage);
        if (NetworkVariables.isServer)
        {
            TransferTCP.instance.sendToClientsQueue.Enqueue(packet.Serialize(PacketType.pChat));
        }
    }

    void HandlePJoin(byte[] data)
    {
        var packet = new pPlayer(data);
        NetworkEvents.onPlayerJoin.Invoke(packet.player);
    }

    void HandlePConnect(byte[] data)
    {
        var packet = new pPlayer(data);
        NetworkEvents.onPlayerJoin.Invoke(packet.player);

        TransferTCP.instance.sendToClientsQueue.Enqueue(packet.Serialize(PacketType.pJoin));
    }

    void HandlePNetworkId(byte[] data)
    {
        var packet = new pByte(data);
        GameVariables.me.networkId = packet.value;
        var pConnect = new pPlayer(GameVariables.me);
        TransferTCP.instance.sendToServerQueue.Enqueue(pConnect.Serialize(PacketType.pConnect));
    }
}


public static class STRING
{
    public static string Byte(byte[] data)
    {
        string ret = "";
        foreach(var b in data)
        {
            if (ret.Length != 0)
                ret += ", ";
            ret += b;
        }

        return ret;
    }
}
