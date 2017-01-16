using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipRenderer : MonoBehaviour
{
    [SerializeField]
    GameObject body;
    [SerializeField]
    Renderer bodyRenderer;
    [SerializeField]
    GameObject aimer;

    [SerializeField]
    LaserRenderer laser;
    [SerializeField]
    UIShipHUD hud;
    [SerializeField]
    ParticleSystem[] chargingParticles;
    [SerializeField]
    ParticleSystem[] shootParticles;
    [SerializeField]
    ParticleSystem[] destroyedParticles;
    [SerializeField]
    ParticleSystem[] laserHitParticles;

    Ship _ship;

    public void InitWithShip(Ship ship)
    {
        _ship = ship;
        _ship.isCharging.AddListener(IsCharging);
        _ship.alive.AddListener(Alive);
        _ship.shootedPos.AddListener(Shooted);

        hud.InitWithShip(ship);
        InitParticles();

        bodyRenderer.material.SetColor("_EmissionColor", ship.fleet.color * 0.8f);

        gameObject.name = string.Format("ShipRenderer-{0}", ship.fleet.name);
    }

    void InitParticles()
    {
        foreach (var p in chargingParticles)
        {
            p.gameObject.SetActive(false);
            var main = p.main;
            main.duration = GameConsts.maxChargingPower / GameConsts.chargingSpeed;
        }
        foreach (var p in destroyedParticles)
        {
            p.gameObject.SetActive(false);
        }
        foreach (var p in shootParticles)
        {
            p.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 등록한 리스너들을 모두 해제합니다.
    /// </summary>
    void RemoveAllListeners()
    {
        _ship.isCharging.RemoveListener(IsCharging);
        _ship.alive.RemoveListener(Alive);
    }

    public void Update()
    {
        if (_ship == null)
            return;
        if (_ship.alive.value)
        {
            UpdateTransform();
        }
        UpdateShooting();
    }

    /// <summary>
    /// 위치 및 각도 반영
    /// </summary>
    void UpdateTransform()
    {
        transform.position = Vector2.Lerp(transform.position, new Vector2(_ship.x, _ship.y),0.5f);
        transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(Vector3.forward * _ship.rot),0.5f);
    }

    /// <summary>
    /// 슈팅과 관련된 드로우
    /// </summary>
    void UpdateShooting()
    {
        if (_ship.isCharging.value)
        {
            aimer.transform.localRotation = Quaternion.Euler(Vector3.up * (_ship.rot - _ship.shootingRot));
        }
    }

    void Alive(bool alive)
    {
        if(!alive)
        {
            foreach (var p in destroyedParticles)
            {
                p.gameObject.SetActive(true);
            }

            RemoveAllListeners();
            body.SetActive(false);
        }
    }

    void Shooted(Vector2 pos)
    {
        foreach (var p in chargingParticles)
        {
            p.gameObject.SetActive(false);
        }
        foreach (var p in shootParticles)
        {
            p.gameObject.SetActive(false);
            p.gameObject.SetActive(true);
        }

        var rad = _ship.shootingRad;
        var direction = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad),0);
        laser.Show(transform.position + direction * 2, pos, _ship.shootingPower);


        foreach (var p in laserHitParticles)
        {
            p.transform.position = new Vector3(pos.x,pos.y,0) - direction*2;
            p.Play();
        }
    }

    void IsCharging(bool value)
    {
        if (value) // 차징 시작
        {
            foreach (var p in chargingParticles)
            {
                p.gameObject.SetActive(true);
            }
        }
    }
}
