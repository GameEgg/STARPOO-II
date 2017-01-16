using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameUpdator : MonoBehaviour
{
    [SerializeField]
    Transform gamePhysicsField;
    [SerializeField]
    PhysicsMaterial2D shipPhysicsMaterial;
    [SerializeField]
    UIGame gameUI;

    List<ShipUpdator> shipUpdators = new List<ShipUpdator>();
    List<AILauncher> aiLaunchers = new List<AILauncher>();

    public void Init()
    {
        GameVariables.gameFrame = 0;
        InitAILaunchers();
        CreateShipUpdators();
        GameEvents.onShipDestroyed.AddListener(ShipDestroyed);
        GameEvents.onLaserHit.AddListener(ShipDamaged);
    }

    void InitAILaunchers()
    {
        foreach(var fleet in FleetContainer.fleets)
        {
            var go = new GameObject();
            go.name = string.Format("AILauncher-{0}", fleet.name);
            go.transform.SetParent(transform);

            var aiLauncher = go.AddComponent<AILauncher>();
            aiLauncher.Init(fleet);

            aiLaunchers.Add(aiLauncher);
        }
    }

    void OnDestroy()
    {
        GameEvents.onShipDestroyed.RemoveListener(ShipDestroyed);
    }

    void ShipDestroyed(Ship ship)
    {
        foreach (var fleet in FleetContainer.fleets)
        {
            if (fleet.myShips.Count == 0)
            {
                GameVariables.updateBlocked = true;
                StartCoroutine(GameOverSequence());
            }
        }
        if(GameVariables.groundChangeTimer.value == 0)
            GameVariables.groundChangeTimer.value = GameConsts.basicGroundTimer * 0.5f;
        else
            GameVariables.groundChangeTimer.value = GameConsts.basicGroundTimer;
    }

    void ShipDamaged(Ship ship1, Ship ship2, float damage)
    {
        GameVariables.groundChangeTimer.value = GameConsts.basicGroundTimer;
    }

    IEnumerator GameOverSequence()
    {
        BGMManager.instance.FadeStop();
        yield return StartCoroutine(gameUI.PlayGameOverUI());

        SceneManager.LoadScene("Result");
    }

    void FixedUpdate()
    {
        if (GameVariables.updateBlocked)
            return;

        GameVariables.gameFrame++;

        // 1. 저번 FixedUpdate 이후에 이루어진 물리 변화를 Ship에 반영
        foreach (var updators in shipUpdators)
        {
            updators.ApplyPhysicsToShip();
        }

        // 2. AIScript 실행
        foreach (var aiLauncher in aiLaunchers)
        {
            aiLauncher.UpdateExportedValues();
            aiLauncher.RunJSUpdate();
        }

        // 3. AIScript 로부터 온 명령을 실행
        foreach (var updators in shipUpdators)
        {
            updators.GameUpdate();
        }

        GameVariables.groundChangeTimer.value = Mathf.Max(GameVariables.groundChangeTimer.value - Time.deltaTime,0);
        if(GameVariables.groundChangeTimer.value == 0)
        {
            GameVariables.groundSize.value *= Mathf.Pow(GameConsts.groundSizeMultiplyer,Time.deltaTime);
        }
    }

    void CreateShipUpdators()
    {
        foreach (var fleet in FleetContainer.fleets)
        {
            foreach (var ship in fleet.myShips)
            {
                var go = new GameObject();
                go.transform.SetParent(gamePhysicsField);

                var shipUpdator = go.AddComponent<ShipUpdator>();
                shipUpdator.InitWithShip(ship,shipPhysicsMaterial);
                shipUpdators.Add(shipUpdator);
            }
        }
    }
}
