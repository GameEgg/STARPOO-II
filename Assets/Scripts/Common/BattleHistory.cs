using System.Collections;
using System.Collections.Generic;

public static class BattleHistory {
    public static List<FleetHistory> fleetHistorys = new List<FleetHistory>();
    public static float maxTotalDamage = 0;
    public static float maxTotalEnergy = 0;

    public static void AddFleetHistory(FleetHistory history)
    {
        fleetHistorys.Add(history);
    }

    public static void AddShipHistory(FleetHistory fleetHistory, ShipHistory history)
    {
        fleetHistory.shipHistorys.Add(history);
    }
}

public class FleetHistory
{
    public Fleet fleet;
    public float restHP = GameConsts.maxShipHp * GameSettings.shipCount;
    public float hitRate = 0;
    public float totalDamage = 0;
    public float damageToEnemy = 0;
    public float damageToAlly = 0;
    public float totalUseEnerge = 0;
    public float damagedByEnemy = 0;
    public float damagedByAlly = 0;

    public float totalShootCount = 0;
    public float totalHitCount = 0;
    public float totalHittedCount = 0;

    public List<ShipHistory> shipHistorys = new List<ShipHistory>();
}

public class ShipHistory
{
    public float resetHP = GameConsts.maxShipHp;
    public float hitRate = 0;
    public float totalDamage = 0;
    public float damageToEnemy = 0;
    public float damageToAlly = 0;
    public float totalUseEnerge = 0;
    public float damagedByEnemy = 0;
    public float damagedByAlly = 0;

    public float totalShootCount = 0;
    public float totalHitCount = 0;
    public float totalHittedCount = 0;

    public bool suicide = false;
    public Ship killer = null;
    public List<Ship> killingShips = new List<Ship>();
}
