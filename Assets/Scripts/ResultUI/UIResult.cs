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

    [SerializeField]
    Graph[] graphs;

    [SerializeField]
    Color RedTeamColor;
    [SerializeField]
    Color BlueTeamColor;

    [SerializeField]
    Text RedTeam_Name;
    [SerializeField]
    Text BlueTeam_Name;

    // Use this for initialization
    void Start () {
        btnExit.onClick.AddListener(ClickExitBtn);
        if (NetworkVariables.isClient && NetworkVariables.isNetwork)
            btnExit.gameObject.SetActive(false);

        float totalHP = GameConsts.maxShipHp * GameSettings.shipCount;
        float RedTeam_hitRate = 0;
        float RedTeam_totalEnergy = 0;
        float RedTeam_totalDamage = 0;
        float RedTeam_damageToEnemy = 0;
        float RedTeam_damageToAlly = 0;
        float RedTeam_damageToWall = 0;
        float RedTeam_totalDamaged = 0;
        float RedTeam_damagedByEnemy = 0;
        float RedTeam_damagedByAlly = 0;
        float RedTeam_damagedByWall = 0;
        float RedTeam_restHP = 0;

        float BlueTeam_hitRate = 0;
        float BlueTeam_totalEnergy = 0;
        float BlueTeam_totalDamage = 0;
        float BlueTeam_damageToEnemy = 0;
        float BlueTeam_damageToAlly = 0;
        float BlueTeam_damageToWall = 0;
        float BlueTeam_totalDamaged = 0;
        float BlueTeam_damagedByEnemy = 0;
        float BlueTeam_damagedByAlly = 0;
        float BlueTeam_damagedByWall = 0;
        float BlueTeam_restHP = 0;
        
        double percentageRed;
        double percentageBlue;
        string descriptionRed;
        string descriptionBlue;

        InitMaxValuesInFleetHistory();

        // TODO : Need Code Refactoring about duplicate code part
        if (BattleHistory.fleetHistorys.Count >= 2)
        {
            // Player 1
            RedTeam_Name.text = BattleHistory.fleetHistorys[0].fleet.name;
            RedTeam_restHP = BattleHistory.fleetHistorys[0].restHP;
            RedTeam_totalDamage = BattleHistory.fleetHistorys[0].totalDamage;
            RedTeam_totalEnergy = BattleHistory.fleetHistorys[0].totalUseEnerge;
            RedTeam_damageToEnemy = BattleHistory.fleetHistorys[0].damageToEnemy;
            RedTeam_damageToAlly = BattleHistory.fleetHistorys[0].damageToAlly;
            RedTeam_damageToWall = RedTeam_totalEnergy - RedTeam_totalDamage;
            RedTeam_totalDamaged = totalHP - RedTeam_restHP;
            RedTeam_damagedByEnemy = BattleHistory.fleetHistorys[0].damagedByEnemy;
            RedTeam_damagedByAlly = BattleHistory.fleetHistorys[0].damagedByAlly;
            RedTeam_damagedByWall = RedTeam_totalDamaged - RedTeam_damagedByEnemy - RedTeam_damagedByAlly;

            // Player 2
            BlueTeam_Name.text = BattleHistory.fleetHistorys[1].fleet.name;
            BlueTeam_restHP = BattleHistory.fleetHistorys[1].restHP;
            BlueTeam_totalDamage = BattleHistory.fleetHistorys[1].totalDamage;
            BlueTeam_totalEnergy = BattleHistory.fleetHistorys[1].totalUseEnerge;
            BlueTeam_damageToEnemy = BattleHistory.fleetHistorys[1].damageToEnemy;
            BlueTeam_damageToAlly = BattleHistory.fleetHistorys[1].damageToAlly;
            BlueTeam_damageToWall = BlueTeam_totalEnergy - BlueTeam_totalDamage;
            BlueTeam_totalDamaged = totalHP - BlueTeam_restHP;
            BlueTeam_damagedByEnemy = BattleHistory.fleetHistorys[1].damagedByEnemy;
            BlueTeam_damagedByAlly = BattleHistory.fleetHistorys[1].damagedByAlly;
            BlueTeam_damagedByWall = BlueTeam_totalDamaged - BlueTeam_damagedByEnemy - BlueTeam_damagedByAlly;           

            // HitRate
            descriptionRed = Math.Round(BattleHistory.fleetHistorys[0].hitRate * 100, 2).ToString() + " %";
            descriptionBlue = Math.Round(BattleHistory.fleetHistorys[1].hitRate * 100, 2).ToString() + " %";
            graphs[0].SetGraphValue(
                Math.Round(Convert.ToDouble(BattleHistory.fleetHistorys[0].hitRate), 2),
                Math.Round(Convert.ToDouble(BattleHistory.fleetHistorys[1].hitRate), 2),
                descriptionRed,
                descriptionBlue, 
                RedTeamColor, 
                BlueTeamColor);

            // Damage to Enemy
            percentageRed = (RedTeam_damageToEnemy == 0) ? 0 : Math.Round(RedTeam_damageToEnemy / RedTeam_totalEnergy, 4);
            percentageBlue = (BlueTeam_damageToEnemy == 0) ? 0 : Math.Round(BlueTeam_damageToEnemy / BlueTeam_totalEnergy, 4);
            descriptionRed = percentageRed * 100 + "% (" + RedTeam_damageToEnemy.ToString() + ")";
            descriptionBlue = percentageBlue * 100+ "% (" + BlueTeam_damageToEnemy.ToString() + ")";
            graphs[1].SetGraphValue(
                percentageRed,
                percentageBlue,
                descriptionRed,
                descriptionBlue,
                RedTeamColor,
                BlueTeamColor);

            // Damage to Ally
            percentageRed = (RedTeam_damageToAlly == 0) ? 0 : Math.Round(RedTeam_damageToAlly / RedTeam_totalEnergy, 4);
            percentageBlue = (BlueTeam_damageToAlly == 0) ? 0 : Math.Round(BlueTeam_damageToAlly / BlueTeam_totalEnergy, 4);
            descriptionRed = percentageRed * 100 + "% (" + RedTeam_damageToAlly.ToString() + ")";
            descriptionBlue = percentageBlue * 100 + "% (" + BlueTeam_damageToAlly.ToString() + ")";
            graphs[2].SetGraphValue(
                percentageRed,
                percentageBlue,
                descriptionRed,
                descriptionBlue,
                RedTeamColor,
                BlueTeamColor);
            Debug.Log("[Damage To Ally] Red : " + percentageRed);
            Debug.Log("[Damage To Ally] Red : " + descriptionRed);


            
            // RestHP
            percentageRed = (RedTeam_restHP == 0) ? 0 : Math.Round((RedTeam_restHP / (GameConsts.maxShipHp * GameSettings.shipCount)), 4);
            percentageBlue = (BlueTeam_restHP == 0) ? 0 : Math.Round((BlueTeam_restHP / (GameConsts.maxShipHp * GameSettings.shipCount)), 4);
            descriptionRed = (percentageRed * 100).ToString() + "% (" + Math.Round(RedTeam_restHP, 4).ToString() + ")";
            descriptionBlue = (percentageBlue * 100).ToString() + "% (" + Math.Round(BlueTeam_restHP, 4).ToString() + ")";
            graphs[3].SetGraphValue(
                percentageRed,
                percentageBlue,
                descriptionRed,
                descriptionBlue,
                RedTeamColor,
                BlueTeamColor);

            Debug.Log("[RestHP] Red : " + RedTeam_restHP);
            Debug.Log("[RestHP] Red percent : " + percentageRed);
            Debug.Log("[RestHP] Blue : " + BlueTeam_restHP);
            Debug.Log("[RestHP] Blue percent : " + percentageBlue);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
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
