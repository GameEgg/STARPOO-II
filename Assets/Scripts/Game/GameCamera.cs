﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 카메라 조작을 담당합니다.
/// </summary>
public class GameCamera : GameSyncObject {

    /// <summary>
    /// -1은 기본 카메라. 그 위로는 각 배들.
    /// </summary>
    int currentIndex = -1;
    Ship _ship;
    Vector3 originPos;
    float delay = 0;

    public override void GameInit()
    {
        base.GameInit();
        originPos = transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveCameraIndex(currentIndex + 1);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveCameraIndex(currentIndex - 1);
        }
    }

    void LateUpdate()
    {
        if (delay > 0)
        {
            delay -= Time.deltaTime;
            return;
        }

        if(currentIndex == -1)
        {
            transform.position = originPos;
        }
        else if(_ship != null)
        {
            if (!_ship.alive.value)
            {
                StartCoroutine(MoveToOrigin());
            }
            else{
                var forward = new Vector3(Mathf.Cos(_ship.shootingRad), Mathf.Sin(_ship.shootingRad), 0.1f);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(forward, -Vector3.forward),0.5f);
                transform.position = Vector3.Lerp(transform.position, new Vector3(_ship.x, _ship.y, -1) - forward*5, 0.5f);
            }
        }   
    }

    void MoveCameraIndex(int index)
    {
        if(index == -1)
        {
            transform.rotation = Quaternion.identity;
        }
        else if(index == -2)
        {
            int i = 0;
            foreach (var fleet in FleetContainer.fleets)
            {
                i += fleet.myShips.Count;
            }
            currentIndex = i-1;
            var f = FleetContainer.fleets[FleetContainer.fleets.Count - 1];
            _ship = f.myShips[f.myShips.Count - 1];
            return;
        }
        else
        {
            int i = 0;
            _ship = null;
            foreach (var fleet in FleetContainer.fleets)
            {
                foreach(var ship in fleet.myShips)
                {
                    if(index == i)
                    {
                        _ship = ship;
                        break;
                    }
                    i++;
                }
                if (_ship != null)
                    break;
            }
            if(_ship == null)
            {
                MoveCameraIndex(-1);
                return;
            }
        }
        currentIndex = index;
    }

    IEnumerator MoveToOrigin()
    {
        yield return new WaitForSeconds(0.7f);
        MoveCameraIndex(-1);
    }
}