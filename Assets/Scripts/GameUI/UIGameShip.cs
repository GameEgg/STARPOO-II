using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameShip : MonoBehaviour
{
    [SerializeField]
    Image hpBar;
    [SerializeField]
    Image hpBarTeduri;
    [SerializeField]
    GameObject teamKilMark;
    [SerializeField]
    GameObject deadMark;
    [SerializeField]
    GameObject suicideMark;

    Ship _ship;

    public void InitWithShip(Ship ship)
    {
        _ship = ship;
        Init();
        _ship.hp.AddListener(SetHp);
        hpBarTeduri.color = hpBar.color = ship.fleet.color + Color.white * 0.5f;
        GameEvents.onShipKilledByLaser.AddListener(ShipKilled);
        GameEvents.onSuicide.AddListener(Suicided);
    }
    
    void Init()
    {
        SetHp(GameConsts.maxShipHp);
        deadMark.SetActive(false);
        teamKilMark.SetActive(false);
        suicideMark.SetActive(false);
    }

    void OnDestroy()
    {
        _ship.hp.RemoveListener(SetHp);
        GameEvents.onShipKilledByLaser.RemoveListener(ShipKilled);
    }

    void Suicided(Ship ship, float hp)
    {
        if(ship == _ship)
            ShowSuicideEffect();
    }

    void ShipKilled(Ship killer, Ship victim)
    {
        if (victim != _ship)
            return;

        deadMark.SetActive(true);
        if (killer.fleet == victim.fleet)
            ShowTeamKillEffect();
    }

    /// <summary>
    /// 체력(%)을 보여줍니다.
    /// </summary>
    /// <param name="restHpPercent">남은 체력</param>
    void SetHp(float restHp)
    {
        hpBar.fillAmount = restHp / GameConsts.maxShipHp;

        if(restHp <= 0)
            deadMark.SetActive(true);
    }

    /// <summary>
    /// 팀킬당했으면 호출해주세요.
    /// </summary>
    void ShowTeamKillEffect()
    {
        teamKilMark.SetActive(true);
    }

    /// <summary>
    /// 자살했으면 호출해주세요.
    /// </summary>
    void ShowSuicideEffect()
    {
        suicideMark.SetActive(true);
    }
}
