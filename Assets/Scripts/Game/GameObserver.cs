using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObserver : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameEvents.onLaserHit.AddListener(LaserHit);
        GameEvents.onLaserShoot.AddListener(LaserShoot);
        GameEvents.onSuicide.AddListener(Suicide);
        GameEvents.onShipKilledByLaser.AddListener(ShipKilledByLaser);
    }
	
    void LaserHit (Ship attacker, Ship victim, float damage)
    {
        attacker.history.totalDamage += damage;
        attacker.fleet.history.totalDamage += damage;

        victim.history.totalHittedCount += 1;
        victim.history.resetHP = (victim.history.resetHP < damage) ? 0 : victim.history.resetHP - damage;
        victim.fleet.history.totalHittedCount += 1;
        victim.fleet.history.restHP = (victim.fleet.history.restHP < damage) ? 0 : victim.fleet.history.restHP - damage;

        if (attacker.fleet != victim.fleet)
        {
            attacker.history.totalHitCount += 1;
            attacker.history.hitRate = attacker.history.totalHitCount / attacker.history.totalShootCount;
            attacker.fleet.history.totalHitCount += 1;
            attacker.fleet.history.hitRate = attacker.fleet.history.totalHitCount / attacker.fleet.history.totalShootCount;

            attacker.history.damageToEnemy += damage;
            attacker.fleet.history.damageToEnemy += damage;
            victim.history.damagedByEnemy += damage;
            victim.fleet.history.damagedByEnemy += damage;
        }
        else
        {
            attacker.history.damageToAlly += damage;
            attacker.fleet.history.damageToAlly += damage;
            victim.history.damagedByAlly += damage;
            victim.fleet.history.damagedByAlly += damage;
        }
    }

    void LaserShoot (Ship shooter, float shootingPower)
    {
        shooter.history.totalUseEnerge += shootingPower;
        shooter.history.totalShootCount += 1;
        shooter.history.hitRate = shooter.history.totalHitCount / shooter.history.totalShootCount;

        shooter.fleet.history.totalUseEnerge += shootingPower;
        shooter.fleet.history.totalShootCount += 1;
        shooter.fleet.history.hitRate = shooter.fleet.history.totalHitCount / shooter.fleet.history.totalShootCount;
    }

    void Suicide (Ship target, float damage)
    {
        target.history.resetHP -= damage;
        target.fleet.history.restHP -= damage;
        target.history.suicide = true;
    }

    void ShipKilledByLaser (Ship killer, Ship victim)
    {
        killer.history.killingShips.Add(victim);
        victim.history.killer = killer;
    }

    void OnDestroy()
    {
        GameEvents.onLaserHit.RemoveListener(LaserHit);
        GameEvents.onLaserShoot.RemoveListener(LaserShoot);
        GameEvents.onSuicide.RemoveListener(Suicide);
        GameEvents.onShipKilledByLaser.RemoveListener(ShipKilledByLaser);
    }
}
