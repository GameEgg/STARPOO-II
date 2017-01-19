using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShipHUD : MonoBehaviour {
    [SerializeField]
    GameObject radarMark;
    [SerializeField]
    Animator suicideMark;
    [SerializeField]
    Animator teamkillMark;
    [SerializeField]
    Image hpBar;
    [SerializeField]
    Image chargingBar;

    Ship _ship;

    public void InitWithShip(Ship ship)
    {
        _ship = ship;
        hpBar.color = ship.fleet.color + Color.white * 0.4f;
        _ship.hp.AddListener(SetHp);
        GameEvents.onShipKilledByLaser.AddListener(ShipKilled);
        GameEvents.onSuicide.AddListener(Suicided);
    }

    void LateUpdate()
    {
        if (_ship == null)
            return;
        if (_ship.isCharging.value)
        {
            chargingBar.fillAmount = _ship.chargedPower/_ship.shootingPower;
        }
        else
        {
            chargingBar.fillAmount = _ship.delay.value/GameConsts.shootingDelay;
        }
        if(radarMark.activeSelf != _ship.scanned > 0)
            radarMark.SetActive(_ship.scanned > 0);
        transform.rotation = Camera.main.transform.rotation;
    }



    void Suicided(Ship ship, float hp)
    {
        if (ship == _ship)
            ShowSuicideEffect();
    }

    void ShipKilled(Ship killer, Ship victim)
    {
        if (victim != _ship)
            return;
        
        if (killer.fleet == victim.fleet)
            ShowTeamKillEffect();
    }

    void SetHp(float restHp)
    {
        hpBar.fillAmount = restHp / GameConsts.maxShipHp;

        if (restHp == 0)
        {
            hpBar.gameObject.SetActive(false);
            chargingBar.gameObject.SetActive(false);
            radarMark.transform.localScale = Vector3.zero;
        }
    }

    void ShowTeamKillEffect()
    {
        teamkillMark.gameObject.SetActive(true);
        teamkillMark.SetInteger("TeamIndex", FleetContainer.fleets.IndexOf(_ship.fleet));
    }

    void ShowSuicideEffect()
    {
        suicideMark.gameObject.SetActive(true);
        suicideMark.SetInteger("TeamIndex", FleetContainer.fleets.IndexOf(_ship.fleet));
    }
}
