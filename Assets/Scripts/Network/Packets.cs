using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public enum PacketType
{
    pJoin, pNetworkId, pConnect, pChat, pDisconnect, pSendScript, pCancelScript, pPlayGame, pShipAmount, pGameEnd
}


public class pScript : IPacketData
{
    public byte index;
    public byte networkId;
    public string code;
    public string name;

    public byte[] Serialize(PacketType packetType)
    {
        var serializedName = System.Text.Encoding.UTF8.GetBytes(name);
        var serializedCode = System.Text.Encoding.UTF8.GetBytes(code);
        var data = new byte[3 + serializedName.Length + serializedCode.Length];
        data[0] = index;
        data[1] = networkId;
        data[2] = (byte)(3 + serializedName.Length);
        Buffer.BlockCopy(serializedName, 0, data, 3, serializedName.Length);
        Buffer.BlockCopy(serializedCode, 0, data, data[2], serializedCode.Length);

        data = PacketHeaderSerializer.AttachHeader((byte)packetType, data);
        return data;
    }

    public pScript() { }

    public pScript(byte[] data)
    {
        index = data[0];
        networkId = data[1];
        var codeStartIndex = data[2];
        var serializedName = new byte[codeStartIndex-3];
        var serializedCode = new byte[data.Length - codeStartIndex];
        Buffer.BlockCopy(data, 3, serializedName, 0, serializedName.Length);
        Buffer.BlockCopy(data, codeStartIndex, serializedCode, 0, serializedCode.Length);

        name = System.Text.Encoding.UTF8.GetString(serializedName);
        code = System.Text.Encoding.UTF8.GetString(serializedCode);
    }
}

public class pEmpty : IPacketData
{
    public byte[] Serialize(PacketType packetType)
    {
        var data = PacketHeaderSerializer.AttachHeader((byte)packetType, new byte[1]);
        return data;
    }
}

public class pByte : IPacketData
{
    public byte value;
    public pByte(byte[] data)
    {
        value = data[0];
    }
    public pByte(byte data)
    {
        value = data;
    }

    public byte Deserialize(byte[] data)
    {
        return data[0];
    }

    public byte[] Serialize(PacketType packetType)
    {
        byte[] data = new byte[1];
        data[0] = value;
        data = PacketHeaderSerializer.AttachHeader((byte)packetType, data);
        return data;
    }
}

public class pString : IPacketData
{
    public string myString;

    public pString(string data)
    {
        myString = data;
    }

    public pString(byte[] data)
    {
        myString = Deserialize(data);
    }

    public string Deserialize(byte[] data)
    {
        return System.Text.Encoding.UTF8.GetString(data, 0, data.Length);
    }

    public byte[] Serialize(PacketType packetType)
    {
        var data = System.Text.Encoding.UTF8.GetBytes(myString);
        data = PacketHeaderSerializer.AttachHeader((byte)packetType, data);
        return data;
    }
}

public class pChatMessage : IPacketData
{
    public ChatMessage chatMessage;

    public pChatMessage(ChatMessage message)
    {
        chatMessage = message;
    }

    public pChatMessage(byte[] data)
    {
        chatMessage = Deserialize(data);
    }

    public ChatMessage Deserialize(byte[] data)
    {
        var chatMessage = new ChatMessage();
        chatMessage.playerId = data[0];
        chatMessage.message = System.Text.Encoding.UTF8.GetString(data, 1, data.Length-1);
        return chatMessage;
    }

    public byte[] Serialize(PacketType packetType)
    {
        var messageData = System.Text.Encoding.UTF8.GetBytes(chatMessage.message);
        var data = new byte[messageData.Length + 1];
        data[0] = chatMessage.playerId;
        Buffer.BlockCopy(messageData, 0, data, 1, messageData.Length);
        data = PacketHeaderSerializer.AttachHeader((byte)packetType, data);
        return data;
    }
}

public class pPlayer : IPacketData
{
    public Player player;

    public pPlayer(Player p)
    {
        player = p;
    }

    public pPlayer(byte[] data)
    {
        player = Deserialize(data);
    }

    public Player Deserialize(byte[] data)
    {
        Debug.Log("pPlayer : => " + STRING.Byte(data));
        var player = new Player();
        player.networkId = data[0];
        player.name = System.Text.Encoding.UTF8.GetString(data, 1, data.Length - 1);

        return player;
    }

    public byte[] Serialize(PacketType packetType)
    {
        var nameByte = System.Text.Encoding.UTF8.GetBytes(player.name);
        byte[] data = new byte[nameByte.Length + 1];
        data[0] = player.networkId;
        Buffer.BlockCopy(nameByte, 0, data, 1, nameByte.Length);

        data = PacketHeaderSerializer.AttachHeader((byte)packetType, data);
        return data;
    }
}