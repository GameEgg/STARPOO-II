using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPhysics;

public class ShipUpdator : GameSyncObject {
    public Ship ship
    {
        get { return _ship; }
    }

    Ship _ship;
    CircleCollider2D _collider;
    CircleCollider2D _radar;
    Rigidbody2D _rigidbody;

    Body _body;
    DCollider _dcol;

    List<Ship> scannedEnemies = new List<Ship>();

    /// <summary>
    /// 물리 연산들을 Ship에 적용합니다.
    /// </summary>
    public void ApplyPhysicsToShip()
    {
        _ship.x = transform.position.x.Round();
        _ship.y = transform.position.y.Round();
        //_ship.rot = transform.rotation.eulerAngles.z;
    }
    
    public override void GameUpdate()
    {
        if (_ship.alive.value)
        {
            UpdateShooting();
            UpdateRotation();
            UpdatePosition();
            CheckDead();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 슈팅에 대한 업데이트
    /// </summary>
    void UpdateShooting()
    {
        if (_ship.isCharging.value)
        {
            if(_ship.chargedPower >= _ship.shootingPower)
            {
                Shoot();
            }
            else
            {
                _ship.chargedPower += GameConsts.chargingSpeed * Time.deltaTime;
            }
        }
        else
        {
            _ship.delay.value = Mathf.Max(0, _ship.delay.value - Time.deltaTime);
        }
    }

    /// <summary>
    /// 슈팅합니다.
    /// </summary>
    void Shoot()
    {
        _ship.isCharging.value = false;
        _ship.chargedPower = 0;
        _ship.delay.value = GameConsts.shootingDelay;
        GameEvents.onLaserShoot.Invoke(ship, ship.shootingPower);

        var rad = _ship.shootingRad;
        var direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        var hit = Physics2D.CircleCast(
            transform.position,
            _ship.shootingPower * GameConsts.thiknessPerPower,
            direction,
            GameConsts.basicGroundSize * 2,
            ~LayerMask.GetMask("Radar"));

        if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Ship"))
        {
            var target = hit.rigidbody.gameObject.GetComponent<ShipUpdator>().ship;
            if (target.hp.value <= 0)
                Debug.Log("hit target ship, but target hp already is 0");
            var dmg = Mathf.Min(target.hp.value,_ship.shootingPower);
            target.hp.value -= dmg;
            GameEvents.onLaserHit.Invoke(_ship, target, dmg);
            if (target.hp.value <= 0)
                GameEvents.onShipKilledByLaser.Invoke(ship, target);
        }
        _ship.shootedPos.value = hit.point;
    }

    /// <summary>
    /// 위치정보 업데이트
    /// </summary>
    void UpdatePosition()
    {
        var speed = !_ship.isCharging.value ? _ship.spd :
            Mathf.Min(GameConsts.maxShipSpd * GameConsts.chargingMoveSlow, _ship.spd);
        
        var position = transform.position + (transform.rotation * Vector3.right * Time.deltaTime * speed);
        transform.position = position;
        //_rigidbody.MovePosition(position);

        //var position = transform.position + (_ship.rot * Vector3.right * Time.deltaTime * speed);
        //Debug.Log(_body.Position);
        //_body.Position = new Vector2d(FInt.Create(position.x.RoundToDouble()),FInt.Create(position.y.RoundToDouble()));
        //_body.PositionChanged = true;
        //Debug.Log(_body.Position);
    }

    /// <summary>
    /// 각도 업데이트
    /// </summary>
    void UpdateRotation()
    {
        _ship.rot += _ship.rotSpd * Time.deltaTime;

        _ship.rot %= 360;
        if (_ship.rot > 181)
        {
            _ship.rot -= 360;
        }
        if (_ship.rot < -181)
        {
            _ship.rot += 360;
        }
        transform.rotation = Quaternion.Euler(Vector3.forward * _ship.rot);
    }

    /// <summary>
    /// 참조중인 함선이 고인이 됐는지 체크하고 고인이라면 장비를 정지합니다.
    /// </summary>
    void CheckDead()
    {
        var suicide = Vector2.Distance(Vector2.zero, _rigidbody.transform.position) > GameVariables.groundSize.value;

        if (suicide)
        {
            var restHp = _ship.hp.value;
            _ship.hp.value -= restHp;
            GameEvents.onSuicide.Invoke(ship, restHp);
        }
        if (_ship.hp.value <= 0)
        {
            _ship.alive.value = false;
            _rigidbody.transform.position = Vector2.up * 500;//레이더에서 해제하려면 OnTriggerExit 특성상 위치를 이동시킬 필요가 있습니다.
            
            foreach(var scannedEnemy in scannedEnemies)
            {
                scannedEnemy.scanned--;
                if (scannedEnemy.scanned == 0)
                    ship.fleet.RadarOut(scannedEnemy);
            }
            scannedEnemies.Clear();
            _radar.gameObject.SetActive(false);

            ship.fleet.myShips.Remove(ship);
            GameEvents.onShipDestroyed.Invoke(_ship);
        }
    }

    /// <summary>
    /// 함선 업데이터 동작을 위한 사전작업
    /// </summary>
    /// <param name="ship">참조할 함선 데이터</param>
    /// <param name="physicsMaterial">물리연산시 사용할 함선의 physicsMaterial</param>
    public void InitWithShip(Ship ship, PhysicsMaterial2D physicsMaterial)
    {
        _ship = ship;

        gameObject.layer = LayerMask.NameToLayer("Ship");
        gameObject.name = string.Format("ShipUpdator-{0}", ship.fleet.name);

        _rigidbody = gameObject.AddComponent<Rigidbody2D>();
        _rigidbody.gravityScale = 0;
        _rigidbody.angularDrag = 1000;
        _rigidbody.sharedMaterial = physicsMaterial;

        _collider = gameObject.AddComponent<CircleCollider2D>();
        _collider.radius = GameConsts.shipCollisionRadius;

        InitRadar();

        transform.localPosition = new Vector2(ship.x, ship.y);
        UpdateRotation();
    }
    
    /// <summary>
    /// 레이더 동작을 위한 사전작업
    /// </summary>
    void InitRadar()
    {
        var radarGo = new GameObject();
        radarGo.name = "Radar";
        radarGo.transform.SetParent(transform);
        radarGo.transform.localPosition = Vector2.zero;
        radarGo.transform.localScale = Vector2.one;
        radarGo.layer = LayerMask.NameToLayer("Radar");
        _radar = radarGo.AddComponent<CircleCollider2D>();
        _radar.isTrigger = true;
        _radar.radius = GameConsts.radarCollisionRadius;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer("Ship"))
            return;
        
        var target = col.gameObject.GetComponent<ShipUpdator>().ship;

        if (target.fleet != ship.fleet && !scannedEnemies.Contains(target))
        {
            scannedEnemies.Add(target);
            target.scanned++;
            if (target.scanned == 1)
                ship.fleet.RadarIn(target);
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer("Ship"))
            return;
        var target = col.gameObject.GetComponent<ShipUpdator>().ship;

        if (target.fleet != _ship.fleet && scannedEnemies.Contains(target))
        {
            scannedEnemies.Remove(target);
            target.scanned--;
            if (target.scanned == 0)
                ship.fleet.RadarOut(target);
        }
    }
}
