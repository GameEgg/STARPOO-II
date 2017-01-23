using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public string cardName;
    public string text;
    public bool isGood;
    public Color textColor;
    public Color backgroundColor;
    public List<CardCondition> conditions;

    public Card(string name, string text, CardColor cardColor, List<CardCondition> conditions)
    {
        cardName = name;
        this.text = text;
        this.isGood = cardColor.isGood;
        this.textColor = cardColor.fontColor;
        backgroundColor = cardColor.color;
        this.conditions = conditions;
    }

    public bool IsSatisfyAllConditions(FleetHistory fleetHistory)
    {
        foreach (CardCondition condition in conditions)
        {
            if (!condition.IsSatisfyCondition(fleetHistory))
                return false;
        }
        return true;
    }
}

public class CardColor 
{
    public Color color;
    public Color fontColor;
    public bool isGood;

    public CardColor(Color color, Color fontColor, bool isGood) {
        this.color = color;
        this.fontColor = fontColor;
        this.isGood = isGood;
    }
}

public class CardCondition
{
    public float value;
    public bool expect;

    virtual public bool IsSatisfyCondition(FleetHistory fleetHistory)
    {
        return false;
    }
}

// percentage about HP of all ships in fleet
public class CardCondition_HP : CardCondition
{
    public CardCondition_HP(float value, bool expect)
    {
        this.value = value;
        this.expect = expect;
    }

    override public bool IsSatisfyCondition(FleetHistory fleetHistory)
    {
        float HPpercentage = (fleetHistory.restHP / (GameConsts.maxShipHp * GameSettings.shipCount)) * 100;
        if (expect == (HPpercentage > value) || (expect && (HPpercentage == value)))
            return true;

        return false;
    }
}

// percentage about HitRate of all ships in fleet
public class CardCondition_HitRate : CardCondition
{
    public CardCondition_HitRate(float value, bool expect)
    {
        this.value = value;
        this.expect = expect;
    }

    override public bool IsSatisfyCondition(FleetHistory fleetHistory)
    {
        float var = fleetHistory.hitRate * 100;
        if (expect == (var > value) || (expect && (var == value)))
            return true;
        return false;
    }
}

// percentage about TotalDamage of all ships in fleet
public class CardCondition_TotalDamage : CardCondition
{
    public CardCondition_TotalDamage(float value, bool expect)
    {
        this.value = value;
        this.expect = expect;
    }

    override public bool IsSatisfyCondition(FleetHistory fleetHistory)
    {
        float var = fleetHistory.totalDamage;
        if (expect == (var > value) || (expect && (var == value)))
            return true;
        return false;
    }
}

// percentage about DamageToEnemy of all ships in fleet
public class CardCondition_DamageToEnemy : CardCondition
{
    public CardCondition_DamageToEnemy(float value, bool expect)
    {
        this.value = value;
        this.expect = expect;
    }

    override public bool IsSatisfyCondition(FleetHistory fleetHistory)
    {
        double var = Math.Round(fleetHistory.damageToEnemy / fleetHistory.totalUseEnerge, 4) * 100;
        if (expect == (var > value) || (expect && (var == value)))
            return true;
        return false;
    }
}

// percentage about DamageToAlly of all ships in fleet
public class CardCondition_DamageToAlly : CardCondition
{
    public CardCondition_DamageToAlly(float value, bool expect)
    {
        this.value = value;
        this.expect = expect;
    }

    override public bool IsSatisfyCondition(FleetHistory fleetHistory)
    {
        double var = Math.Round(fleetHistory.damageToAlly / fleetHistory.totalUseEnerge, 4) * 100;
        if (expect == (var > value) || (expect && (var == value)))
            return true;
        return false;
    }
}

// percentage about DamageToAlly of all ships in fleet
public class CardCondition_DamageToWall : CardCondition
{
    public CardCondition_DamageToWall(float value, bool expect)
    {
        this.value = value;
        this.expect = expect;
    }

    override public bool IsSatisfyCondition(FleetHistory fleetHistory)
    {
        float damageToWall = fleetHistory.totalUseEnerge - fleetHistory.totalDamage;
        double var = Math.Round(damageToWall / fleetHistory.totalUseEnerge, 4) * 100;
        if (expect == (var > value) || (expect && (var == value)))
            return true;
        return false;
    }
}

// percentage about TotalUseEnerge of all ships in fleet
public class CardCondition_TotalUseEnerge : CardCondition
{
    public CardCondition_TotalUseEnerge(float value, bool expect)
    {
        this.value = value;
        this.expect = expect;
    }

    override public bool IsSatisfyCondition(FleetHistory fleetHistory)
    {
        float var = fleetHistory.totalUseEnerge;
        if (expect == (var > value) || (expect && (var == value)))
            return true;
        return false;
    }
}

// percentage about DamagedByEnemy of all ships in fleet
public class CardCondition_DamagedByEnemy : CardCondition
{
    public CardCondition_DamagedByEnemy(float value, bool expect)
    {
        this.value = value;
        this.expect = expect;
    }

    override public bool IsSatisfyCondition(FleetHistory fleetHistory)
    {
		float totalHP = GameConsts.maxShipHp * GameSettings.shipCount;
		float damagedHP = totalHP - fleetHistory.restHP;
		double var = Math.Round(fleetHistory.damagedByEnemy / damagedHP, 4) * 100;
        if (expect == (var > value) || (expect && (var == value)))
            return true;
        return false;
    }
}

// percentage about DamagedByAlly of all ships in fleet
public class CardCondition_DamagedByAlly : CardCondition
{
    public CardCondition_DamagedByAlly(float value, bool expect)
    {
        this.value = value;
        this.expect = expect;
    }

    override public bool IsSatisfyCondition(FleetHistory fleetHistory)
    {
        float totalHP = GameConsts.maxShipHp * GameSettings.shipCount;
		float damagedHP = totalHP - fleetHistory.restHP;
		double var = Math.Round(fleetHistory.damagedByAlly / damagedHP, 4) * 100;
        if (expect == (var > value) || (expect && (var == value)))
            return true;
        return false;
    }
}

// percentage about DamagedByWall of all ships in fleet
public class CardCondition_DamagedByWall: CardCondition
{
    public CardCondition_DamagedByWall(float value, bool expect)
    {
        this.value = value;
        this.expect = expect;
    }

    override public bool IsSatisfyCondition(FleetHistory fleetHistory)
    {
        float totalHP = GameConsts.maxShipHp * GameSettings.shipCount;
		float damagedHP = totalHP - fleetHistory.restHP;
		double var = Math.Round((damagedHP - fleetHistory.damagedByEnemy - fleetHistory.damagedByAlly) / damagedHP, 4) * 100;
        if (expect == (var > value) || (expect && (var == value)))
            return true;
        return false;
    }
}