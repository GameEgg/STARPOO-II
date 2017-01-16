using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct PacketHeader
{
    public byte packetType;
    public ushort size;
}

public static class PacketHeaderSerializer
{
    public static byte[] AttachHeader(byte packetType, byte[] data)
    {
        var ret = new byte[data.Length + 3];
        ret[0] = packetType;
        Buffer.BlockCopy(BitConverter.GetBytes(data.Length), 0, ret, 1, 2);
        Buffer.BlockCopy(data, 0, ret, 3, data.Length);

        return ret;
    }

    public static byte[] Serialize(PacketHeader header)
    {
        var ret = new byte[3];
        ret[0] = header.packetType;
        Buffer.BlockCopy(BitConverter.GetBytes(header.size),0,ret,1,2);
        return ret;
    }
    
    public static void Deserializer(byte[] data, ref PacketHeader header)
    {
        header.packetType = data[0];
        header.size = BitConverter.ToUInt16(data,1);
    }
}

public interface IPacketData
{
    byte[] Serialize(PacketType packetType);
}