using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerManager
{
    public List<Player> players = new List<Player>();
    
    public NetworkPlayerManager()
    {
        NetworkEvents.onPlayerJoin.AddListener(HandlePlayerJoin);
        NetworkEvents.onPlayerOut.AddListener(HandlePlayerOut);
    }

    public Player GetPlayer(byte networkId)
    {
        foreach (var p in players)
            if (p.networkId == networkId)
                return p;
        return null;
    }

    void HandlePlayerJoin(Player player)
    {
        players.Add(player);
    }

    void HandlePlayerOut(byte id)
    {
        Player target = null;
        foreach (var p in players)
        {
            if (p.networkId == id)
            {
                target = p;
                break;
            }
        }
        if (target != null)
            players.Remove(target);
    }

    void RemovePlayer()
    {

    }

    void Reset()
    {
        players.Clear();
    }

}
