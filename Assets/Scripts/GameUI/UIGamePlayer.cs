using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGamePlayer : MonoBehaviour
{
    [SerializeField]
    Image hpBar;
    [SerializeField]
    Image hpBarTeduri;
    [SerializeField]
    Animator hpbarAnimator;
    [SerializeField]
    Text textName;
    [SerializeField]
    UIGameShip templateShip;
    [SerializeField]
    Transform shipMarksContainer;

    Fleet _fleet;
    float fullHp;

    public void InitWithFleet(Fleet fleet)
    {
        _fleet = fleet;
        textName.text = fleet.name;
        hpBar.fillAmount = 1;

        fullHp = GameSettings.shipCount * GameConsts.maxShipHp;
        foreach (var ship in fleet.myShips)
        {
            InstantiateUIShip(ship);
        }
        hpBarTeduri.color = fleet.color + Color.white * 0.5f; ;

        GameEvents.onLaserHit.AddListener(LaserDamage);
        GameEvents.onSuicide.AddListener(SuicideDamage);
    }

    void LaserDamage(Ship shooter, Ship victim, float dmg)
    {
        if(victim.fleet == _fleet)
        {
            fullHp -= dmg;
            float maxHp = (GameSettings.shipCount) * GameConsts.maxShipHp;
            hpBar.fillAmount = fullHp / maxHp;
            hpbarAnimator.SetTrigger("Go");
        }
    }

    void SuicideDamage(Ship crazer, float dmg)
    {
        if (crazer.fleet == _fleet)
        {
            fullHp -= dmg;
            float maxHp = (GameSettings.shipCount) * GameConsts.maxShipHp;
            hpBar.fillAmount = fullHp / maxHp;
            hpbarAnimator.SetTrigger("Go");
        }
    }

    void UpdateHpbar()
    {

    }

    void OnDestroy()
    {
        GameEvents.onLaserHit.RemoveListener(LaserDamage);
    }

    void InstantiateUIShip(Ship ship)
    {
        var uiShip = Instantiate(templateShip);
        uiShip.InitWithShip(ship);

        var go = uiShip.gameObject;
        go.transform.SetParent(shipMarksContainer);
        go.transform.localScale = Vector3.one * 0.8f;
        go.transform.localPosition = Vector3.zero;
    }
}
