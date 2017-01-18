using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;


public struct ChatMessage
{
    public byte playerId;
    public string message;
    public ChatMessage(byte id, string msg)
    {
        playerId = id;
        message = msg;
    }
}

public struct NetworkClient
{
    public PacketQueue sendQueue;
    public Socket socket;
    public byte networkId;
    public NetworkClient(Socket socket)
    {
        this.socket = socket;
        sendQueue = new PacketQueue();
        networkId = 0;
    }
}

public class Player
{
    public string name = "default name";
    public byte networkId = 0;
}

public class AIScript
{
    public string name;
    public Player author;
    public string code;
    public string path;
    public Color color;

    public AIScript() { }
    public AIScript(AIScript origin) {
        name = origin.name;
        author = origin.author;
        code = origin.code;
        path = origin.path;
        color = origin.color;
    }
}

public class Fleet
{
    public string name;
    public Color color;
    public List<Ship> myShips = new List<Ship>();
    public List<Ship> enemyShips = new List<Ship>();
    public AIScript script;
    public FleetHistory history = new FleetHistory();

    public Fleet(int numShip, AIScript ai)
    {
        this.script = ai;
        for (int i = 0; i < numShip; ++i)
        {
            myShips.Add(new Ship(this));
        }
        BattleHistory.AddFleetHistory(history);
    }

    public void RadarIn(Ship enemyShip)
    {
        enemyShips.Add(enemyShip);
        enemyShips.Sort();
    }

    public void RadarOut(Ship enemyShip)
    {
        enemyShips.Remove(enemyShip);
    }
}

public class Ship : IComparable<Ship>
{
    public int id;
    public O<float> hp = new O<float>(GameConsts.maxShipHp);
    public float x;
    public float y;
    public float spd;
    public float rot;
    public float rad
    {
        get
        {
            return Mathf.Deg2Rad * rot;
        }
    }
    public float rotSpd;
    public O<float> delay = new O<float>(GameConsts.preDelay);
    public O<bool> isCharging = new O<bool>(false);
    public float chargedPower;
    public float shootingPower;
    public float shootingRot;
    public float shootingRad
    {
        get
        {
            return Mathf.Deg2Rad * shootingRot;
        }
    }

    public O<Vector2> shootedPos = new O<Vector2>(Vector2.zero);
    public O<bool> alive = new O<bool>(true);
    public Fleet fleet{ get { return _fleet; }}
    public int scanned = 0;
    Fleet _fleet;
    public ShipHistory history = new ShipHistory();

    public Ship(Fleet fleet)
    {
        _fleet = fleet;
        BattleHistory.AddShipHistory(fleet.history, history);
    }

    public void SetSpeed(float speed)
    {
        speed = Mathf.Max(0, Mathf.Min(GameConsts.maxShipSpd, speed));
        spd = speed;
    }

    public void SetRotSpeed(float rotSpeed)
    {
        rotSpeed = Mathf.Max(-GameConsts.maxShipRotSpd, Mathf.Min(GameConsts.maxShipRotSpd, rotSpeed));
        rotSpd = rotSpeed;
    }

    public void Shoot(float power)
    {
        if (delay.value > 0)
            return;
        if (isCharging.value)
            return;
        if (power <= 0)
            return;
        if (power > GameConsts.maxChargingPower)
            power = GameConsts.maxChargingPower;

        isCharging.value = true;
        chargedPower = 0;
        shootingPower = power;
        shootingRot = rot;
    }

    public int CompareTo(Ship other)
    {
        return other.id - id;
    }
}

public class Game
{

}