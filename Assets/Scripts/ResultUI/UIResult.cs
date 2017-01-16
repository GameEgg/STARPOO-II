using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIResult : MonoBehaviour
{
    private const float RESULT_GRAPH_HEIGHT = 40;
    private const float RESULT_GAMEOBJECT_GRAPH_HEIGHT = 80;
    private const float RESULT_GRAPH_MAX_WIDTH = 350;

    [SerializeField]
    Button btnExit;

    // Player 1
    [SerializeField]
    Text player1Name;
    [SerializeField]
    GameObject player1HitRateGraph;
    [SerializeField]
    Image player1DamageGraph_Air;
    [SerializeField]
    Image player1DamageGraph_Enemy;
    [SerializeField]
    Image player1DamageGraph_Ally;
    [SerializeField]
    Image player1DamagedGraph_Wall;
    [SerializeField]
    Image player1DamagedGraph_Ally;
    [SerializeField]
    Image player1DamagedGraph_Enemy;
    [SerializeField]
    GameObject player1RestHPGraph;

    [SerializeField]
    Text player1HitRate;
    [SerializeField]
    Text player1TotalDamage;
    [SerializeField]
    Text player1Damage_Air;
    [SerializeField]
    Text player1Damage_Enemy;
    [SerializeField]
    Text player1Damage_Ally;
    [SerializeField]
    Text player1TotalDamaged;
    [SerializeField]
    Text player1Damaged_Enemy;
    [SerializeField]
    Text player1Damaged_Ally;
    [SerializeField]
    Text player1Damaged_Wall;
    [SerializeField]
    Text player1RestHP;

    // Player 2
    [SerializeField]
    Text player2Name;
    [SerializeField]
    GameObject player2HitRateGraph;
    [SerializeField]
    Image player2DamageGraph_Air;
    [SerializeField]
    Image player2DamageGraph_Enemy;
    [SerializeField]
    Image player2DamageGraph_Ally;
    [SerializeField]
    Image player2DamagedGraph_Wall;
    [SerializeField]
    Image player2DamagedGraph_Ally;
    [SerializeField]
    Image player2DamagedGraph_Enemy;
    [SerializeField]
    GameObject player2RestHPGraph;

    [SerializeField]
    Text player2HitRate;
    [SerializeField]
    Text player2TotalDamage;

    [SerializeField]
    Text player2Damage_Air;
    [SerializeField]
    Text player2Damage_Enemy;
    [SerializeField]
    Text player2Damage_Ally;

    [SerializeField]
    Text player2TotalDamaged;
    [SerializeField]
    Text player2Damaged_Enemy;
    [SerializeField]
    Text player2Damaged_Ally;
    [SerializeField]
    Text player2Damaged_Wall;

    [SerializeField]
    Text player2RestHP;

    // Use this for initialization
    void Start () {
        btnExit.onClick.AddListener(ClickExitBtn);
        if (NetworkVariables.isClient && NetworkVariables.isNetwork)
            btnExit.gameObject.SetActive(false);

        float totalHP = GameConsts.maxShipHp * GameSettings.shipCount;
        float hitRate = 0;
        float totalEnergy = 0;
        float totalDamage = 0;
        float damageToEnemy = 0;
        float damageToAlly = 0;
        float damageToWall = 0;
        float totalDamaged = 0;
        float damagedByEnemy = 0;
        float damagedByAlly = 0;
        float damagedByWall = 0;
        float restHP = 0;

        float hitRateWidth = 0;
        float damageToEnemyWidth = 0;
        float damageToAllyWidth = 0;
        float damageToWallWidth = 0;
        float damagedByEnemyWidth = 0;
        float damagedByAllyWidth = 0;
        float damagedByWallWidth = 0;
        float restHPWidth = 0;

        InitMaxValuesInFleetHistory();

        // TODO : Need Code Refactoring about duplicate code part
        if (BattleHistory.fleetHistorys.Count >= 2)
        {
            // Player 1
            restHP = BattleHistory.fleetHistorys[0].restHP;
            totalDamage = BattleHistory.fleetHistorys[0].totalDamage;
            totalEnergy = BattleHistory.fleetHistorys[0].totalUseEnerge;
            damageToEnemy = BattleHistory.fleetHistorys[0].damageToEnemy;
            damageToAlly = BattleHistory.fleetHistorys[0].damageToAlly;
            damageToWall = totalEnergy - totalDamage;

            totalDamaged = totalHP - restHP;
            damagedByEnemy = BattleHistory.fleetHistorys[0].damagedByEnemy;
            damagedByAlly = BattleHistory.fleetHistorys[0].damagedByAlly;
            damagedByWall = totalDamaged - damagedByEnemy - damagedByAlly;

            player1Name.text = BattleHistory.fleetHistorys[0].fleet.name;
            player1HitRate.text = Math.Round(BattleHistory.fleetHistorys[0].hitRate * 100, 2).ToString() + " %";
            player1TotalDamage.text = totalEnergy.ToString();
            FillTextAboutGraphValue(
                player1Damage_Enemy,
                Math.Round(damageToEnemy * 100 / totalEnergy, 1),
                damageToEnemy);
            FillTextAboutGraphValue(
                player1Damage_Ally,
                Math.Round(damageToAlly * 100 / totalEnergy, 1),
                damageToAlly);
            FillTextAboutGraphValue(
                player1Damage_Air,
                Math.Round(damageToWall * 100 / totalEnergy, 1),
                damageToWall);
            player1TotalDamaged.text = totalDamaged.ToString();
            FillTextAboutGraphValue(
                player1Damaged_Enemy,
                Math.Round(damagedByEnemy * 100 / totalDamaged, 1),
                damagedByEnemy);
            FillTextAboutGraphValue(
                player1Damaged_Ally,
                Math.Round(damagedByAlly * 100 / totalDamaged, 1),
                damagedByAlly);
            FillTextAboutGraphValue(
                player1Damaged_Wall,
                Math.Round(damagedByWall * 100 / totalDamaged, 1),
                damagedByWall);
            player1RestHP.text = Math.Round(restHP, 4).ToString() + " ( " +
                Math.Round((restHP * 100 / (GameConsts.maxShipHp * GameSettings.shipCount)), 4).ToString() + " % )";

            hitRateWidth = CalculateHitRate(BattleHistory.fleetHistorys[0]);
            damageToEnemyWidth = CalculateDamage(damageToEnemy);
            damageToAllyWidth = CalculateDamage(damageToAlly);
            damageToWallWidth = CalculateDamage(damageToWall);
            damagedByEnemyWidth = CalculateDamaged(damagedByEnemy);
            damagedByAllyWidth = CalculateDamaged(damagedByAlly);
            damagedByWallWidth = CalculateDamaged(damagedByWall);
            player1TotalDamage.rectTransform.sizeDelta = new Vector2(damageToEnemyWidth + damageToAllyWidth + damageToWallWidth, 20);
            player1TotalDamaged.rectTransform.sizeDelta = new Vector2(damagedByEnemyWidth + damagedByAllyWidth + damagedByWallWidth, 20);
            restHPWidth = CalculateRestHP(BattleHistory.fleetHistorys[0]);

            SetGraphWitdh(player1HitRateGraph, hitRateWidth);
            SetGraphWitdh(player1DamageGraph_Enemy, damageToEnemyWidth);
            SetGraphWitdh(player1DamageGraph_Ally, damageToAllyWidth);
            SetGraphWitdh(player1DamageGraph_Air, damageToWallWidth);
            SetGraphWitdh(player1DamagedGraph_Enemy, damagedByEnemyWidth);
            SetGraphWitdh(player1DamagedGraph_Ally, damagedByAllyWidth);
            SetGraphWitdh(player1DamagedGraph_Wall, damagedByWallWidth);
            SetGraphWitdh(player1RestHPGraph, restHPWidth);

            // Player 2
            restHP = BattleHistory.fleetHistorys[1].restHP;

            totalDamage = BattleHistory.fleetHistorys[1].totalDamage;
            totalEnergy = BattleHistory.fleetHistorys[1].totalUseEnerge;
            damageToEnemy = BattleHistory.fleetHistorys[1].damageToEnemy;
            damageToAlly = BattleHistory.fleetHistorys[1].damageToAlly;
            damageToWall = totalEnergy - totalDamage;

            totalDamaged = totalHP - restHP;
            damagedByEnemy = BattleHistory.fleetHistorys[1].damagedByEnemy;
            damagedByAlly = BattleHistory.fleetHistorys[1].damagedByAlly;
            damagedByWall = totalDamaged - damagedByEnemy - damagedByAlly;

            player2Name.text = BattleHistory.fleetHistorys[1].fleet.name;
            player2HitRate.text = Math.Round(BattleHistory.fleetHistorys[1].hitRate * 100, 2).ToString() + " %";
            player2TotalDamage.text = totalEnergy.ToString();
            FillTextAboutGraphValue(
                player2Damage_Enemy,
                Math.Round(damageToEnemy * 100 / totalEnergy, 1),
                damageToEnemy);
            FillTextAboutGraphValue(
                player2Damage_Ally,
                Math.Round(damageToAlly * 100 / totalEnergy, 1),
                damageToAlly);
            FillTextAboutGraphValue(
                player2Damage_Air,
                Math.Round(damageToWall * 100 / totalEnergy, 1),
                damageToWall);

            player2TotalDamaged.text = totalDamaged.ToString();
            FillTextAboutGraphValue(
                player2Damaged_Enemy, 
                Math.Round(damagedByEnemy * 100 / totalDamaged, 1), 
                damagedByEnemy);
            FillTextAboutGraphValue(
                player2Damaged_Ally,
                Math.Round(damagedByAlly * 100 / totalDamaged, 1),
                damagedByAlly);
            FillTextAboutGraphValue(
                player2Damaged_Wall,
                Math.Round(damagedByWall * 100 / totalDamaged, 1),
                damagedByWall);
            player2RestHP.text = Math.Round(restHP, 4).ToString() + " ( " +
                Math.Round((restHP * 100 / (GameConsts.maxShipHp * GameSettings.shipCount)), 4).ToString() + " % )";

            hitRateWidth = CalculateHitRate(BattleHistory.fleetHistorys[1]);
            damageToEnemyWidth = CalculateDamage(damageToEnemy);
            damageToAllyWidth = CalculateDamage(damageToAlly);
            damageToWallWidth = CalculateDamage(damageToWall);
            damagedByEnemyWidth = CalculateDamaged(damagedByEnemy);
            damagedByAllyWidth = CalculateDamaged(damagedByAlly);
            damagedByWallWidth = CalculateDamaged(damagedByWall);
            player2TotalDamage.rectTransform.sizeDelta = new Vector2(damageToEnemyWidth + damageToAllyWidth + damageToWallWidth, 20);
            player2TotalDamaged.rectTransform.sizeDelta = new Vector2(damagedByEnemyWidth + damagedByAllyWidth + damagedByWallWidth, 20);
            restHPWidth = CalculateRestHP(BattleHistory.fleetHistorys[1]);

            SetGraphWitdh(player2HitRateGraph, hitRateWidth);
            SetGraphWitdh(player2DamageGraph_Enemy, damageToEnemyWidth);
            SetGraphWitdh(player2DamageGraph_Ally, damageToAllyWidth);
            SetGraphWitdh(player2DamageGraph_Air, damageToWallWidth);
            SetGraphWitdh(player2DamagedGraph_Enemy, damagedByEnemyWidth);
            SetGraphWitdh(player2DamagedGraph_Ally, damagedByAllyWidth);
            SetGraphWitdh(player2DamagedGraph_Wall, damagedByWallWidth);
            SetGraphWitdh(player2RestHPGraph, restHPWidth);
        }
    }

    void ClickExitBtn()
    {
        foreach (FleetHistory fleetHistory in BattleHistory.fleetHistorys)
        {
            fleetHistory.shipHistorys.Clear();
        }
        BattleHistory.fleetHistorys.Clear();

        SceneManager.LoadScene("Room");

        if (NetworkVariables.isServer && NetworkVariables.isNetwork)
        {
            NetworkManager.instance.EndGame();
            NetworkEvents.onGameEnd.Invoke();
        }
    }

    public float CalculateHitRate(FleetHistory fleetHistory)
    {
        float variable = fleetHistory.hitRate;
        
        return RESULT_GRAPH_MAX_WIDTH * variable;
    }

    public float CalculateDamage(float damage)
    {
        float variable = damage / BattleHistory.maxTotalEnergy;
        return RESULT_GRAPH_MAX_WIDTH * variable;
    }

    public float CalculateTotalEnergy(FleetHistory fleetHistory)
    {
        float variable = fleetHistory.totalUseEnerge / BattleHistory.maxTotalEnergy;
        return RESULT_GRAPH_MAX_WIDTH * variable;
    }

    public float CalculateDamaged(float damaged)
    {
        float variable = damaged / ((GameConsts.maxShipHp * GameSettings.shipCount)); // - fleetHistory.restHP
        return RESULT_GRAPH_MAX_WIDTH * variable;
    }

    public float CalculateRestHP(FleetHistory fleetHistory)
    {
        float variable = fleetHistory.restHP / (GameConsts.maxShipHp * GameSettings.shipCount);
        return RESULT_GRAPH_MAX_WIDTH * variable;
    }

    public void InitMaxValuesInFleetHistory()
    {
        foreach (FleetHistory fleetHistory in BattleHistory.fleetHistorys)
        {
            if (BattleHistory.maxTotalDamage < fleetHistory.totalDamage)
                BattleHistory.maxTotalDamage = fleetHistory.totalDamage;
            if (BattleHistory.maxTotalEnergy < fleetHistory.totalUseEnerge)
                BattleHistory.maxTotalEnergy = fleetHistory.totalUseEnerge;
        }
    }

    private void SetGraphWitdh(Image graph, float width)
    {
        graph.rectTransform.sizeDelta = new Vector2(width, RESULT_GRAPH_HEIGHT);
    }

    private void SetGraphWitdh(GameObject graph, float width)
    {
        graph.GetComponent<RectTransform>().sizeDelta = new Vector2(width, RESULT_GAMEOBJECT_GRAPH_HEIGHT);
    }

    private void FillTextAboutGraphValue(Text text, double percentage, float value)
    {
        if (percentage == 0)
        {
            text.text = "";
        }
        else
        {
            text.text = percentage.ToString() + "%\n(" + value.ToString() + ")";
        }
    }
}
