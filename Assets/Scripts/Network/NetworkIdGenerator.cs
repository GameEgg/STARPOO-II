using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NetworkIdGenerator
{
    static byte nextId = 10;

    public static byte Generate()
    {
        byte ret = nextId++;
        do
        {
            ret = (byte)((nextId++)%90 + 10);
        }
        while (IsDuplicatedId(ret));
        return ret;
    }

    static bool IsDuplicatedId(byte id)
    {
        foreach (var p in NetworkManager.instance.playerContainer.players)
            if (p.networkId == id)
                return true;
        return false;
    }
}
