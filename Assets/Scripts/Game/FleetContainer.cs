using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 함대들을 갖고 있습니다.
/// </summary>
public static class FleetContainer {

    public static List<Fleet> fleets = new List<Fleet>();

    /// <summary>
    /// GameSettings를 참고해 fleets 모델을 세팅합니다.
    /// </summary>
    public static void InitFleetsWithGameSettings()
    {
        fleets.Clear();

        foreach (var script in GameSettings.selectedScripts)
        {
            var fleet = new Fleet(GameSettings.shipCount,script);
            fleet.name = script.name;
            fleet.color = script.color;
            fleet.history.fleet = fleet;

            var fleetRot = (fleets.Count) * 360 / GameSettings.selectedScripts.Count;
            var numShip = fleet.myShips.Count;

            for (int j = 0; j < numShip; ++j)
            {

                int numRow = Mathf.CeilToInt(Mathf.Sqrt(numShip));
                float row = j % numRow;
                float column = Mathf.Floor(j / numRow);

                float rot = fleetRot + (row - (numRow - 1) / 2) * 6;
                float distance = column * 5;

                float rad = Mathf.PI * rot / 180;
                float farFromCenter = GameConsts.basicGroundSize * 0.5f;
                float x = -Mathf.Cos(rad) * (farFromCenter + distance);
                float y = -Mathf.Sin(rad) * (farFromCenter + distance);

                var ship = fleet.myShips[j];
                ship.x = x;
                ship.y = y;
                ship.rot = rot;
            }

            fleets.Add(fleet);
        }
    }

}
