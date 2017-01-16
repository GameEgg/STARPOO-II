using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GameSettings의 값에 따라 게임 뷰를 로드합니다.
public class GameLoader : MonoBehaviour {

    [SerializeField]
    ShipRenderer shipViewPrefab;
    [SerializeField]
    Transform gameRenderer;
    [SerializeField]
    GameUpdator gameUpdator;
    [SerializeField]
    AudioClip bgm;

    [SerializeField]
    LogWindow[] logWindows; 

    /// <summary>
    /// FleetContainer의 fleets를 참고로 ShipRenderer들을 생성합니다.
    /// </summary>
    void CreateShipRenderersAndSetShipId()
    {
        for(int i = 0; i < FleetContainer.fleets.Count; ++i)
        {
            var fleet = FleetContainer.fleets[i];
            var numShip = fleet.myShips.Count;

            for (int j = 0; j < numShip; ++j)
            {
                var ship = fleet.myShips[j];
                ship.id = i * 1000 + j;
                
                var shipView = Instantiate(shipViewPrefab);
                shipView.InitWithShip(ship);
                shipView.transform.SetParent(gameRenderer);
            }
            logWindows[i].Init(fleet.script);
        }
    }

    /// <summary>
    /// 게임 변수들을 초기화합니다.
    /// </summary>
    void InitGameVariables()
    {
        GameVariables.groundSize.value = GameConsts.basicGroundSize;
        GameVariables.groundChangeTimer.value = GameConsts.basicGroundTimer + GameConsts.preDelay;
        GameVariables.updateBlocked = true;
    }

    /// <summary>
    /// GameSyncObject들을 찾아, Init합니다.
    /// </summary>
    void InitGameSyncObjects()
    {
        foreach(var go in FindObjectsOfType<GameSyncObject>())
        {
            go.GameInit();
        }
    }

    void TestCode()
    {
        var scripts = ScriptLoader.LoadAIScripts();
        if (FleetContainer.fleets.Count == 0)
        {
            var script1 = scripts[0];
            var script2 = scripts[1];
            GameSettings.selectedScripts.Add(script1);
            GameSettings.selectedScripts.Add(script2);
            GameSettings.selectedScripts[0].color = Color.red;
            GameSettings.selectedScripts[1].color = Color.blue;
            FleetContainer.InitFleetsWithGameSettings();
        }
    }

    void InitPhysicsConsts()
    {
        Physics2D.queriesStartInColliders = false;
        Physics2D.changeStopsCallbacks = true;
        Physics2D.showColliderContacts = true;
        Physics2D.alwaysShowColliders = true;
    }

    IEnumerator Start()
    {
        TestCode();
        InitPhysicsConsts();
        InitGameVariables();
        InitGameSyncObjects();
        CreateShipRenderersAndSetShipId();
        gameUpdator.Init();
        BGMManager.instance.Stop();

        yield return new WaitForSeconds(3.5f);

        GameVariables.updateBlocked = false;

        BGMManager.instance.Play(bgm);
    }
}
