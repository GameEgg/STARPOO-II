using Jurassic;
using Jurassic.Library;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 스크립트를 실행하고, 결과로써 배에 내려진 명령들을 적용합니다.
/// </summary>
public class AILauncher : MonoBehaviour {

    ScriptEngine engine;
    Fleet _fleet;

    ArrayInstance myShips;
    ArrayInstance enemyShips;
    List<ShipJSObject> allEnemyShips = new List<ShipJSObject>();
    List<MyShipJSObject> allMyShips = new List<MyShipJSObject>();

    public void Init(Fleet fleet)
    {
        _fleet = fleet;


        engine = new ScriptEngine();

        ExportArrays(engine);
        ExportGlobalValues(engine);
        ExportGlobalFunctions(engine);
        engine.Execute(_fleet.script.code);
    }

    /// <summary>
    /// JSContext로 변수들을 내보냅니다.
    /// </summary>
    /// <param name="engine"></param>
    void ExportGlobalValues(ScriptEngine engine)
    {
        engine.SetGlobalValue("dt", engine.Number.Construct(0.02));
        engine.SetGlobalValue("shipMaxHp", engine.Number.Construct(GameConsts.maxShipHp));
        engine.SetGlobalValue("shipMaxSpeed", engine.Number.Construct(GameConsts.maxShipSpd));
        engine.SetGlobalValue("shipMaxRotSpeed", engine.Number.Construct(GameConsts.maxShipRotSpd));
        engine.SetGlobalValue("chargingSpeed", engine.Number.Construct(GameConsts.chargingSpeed));
        engine.SetGlobalValue("shootingDelay", engine.Number.Construct(GameConsts.shootingDelay));
        engine.SetGlobalValue("maxChargingPower", engine.Number.Construct(GameConsts.maxChargingPower));
        engine.SetGlobalValue("groundRadius", engine.Number.Construct(GameVariables.groundSize.value));
    }

    /// <summary>
    /// JSContext로 함수들을 내보냅니다.
    /// </summary>
    /// <param name="engine"></param>
    void ExportGlobalFunctions(ScriptEngine engine) {
        engine.SetGlobalFunction("log", new Action<string>(Log));
        engine.SetGlobalFunction("polar", new Func<ObjectInstance, ObjectInstance>(Polar));
        engine.SetGlobalFunction("polarFrom", new Func<ObjectInstance, ObjectInstance, ObjectInstance>(PolarFrom));
        engine.SetGlobalFunction("cos", new Func<double, double>(Cos));
        engine.SetGlobalFunction("sin", new Func<double, double>(Sin));
        engine.SetGlobalFunction("d2r", new Func<double, double>(D2R));
        engine.SetGlobalFunction("r2d", new Func<double, double>(R2D));
        engine.SetGlobalFunction("dist", new Func<ObjectInstance, ObjectInstance, double>(Distance));
    }

    /// <summary>
    /// JSContext로 배열들을 내보냅니다.
    /// </summary>
    /// <param name="engine"></param>
    void ExportArrays(ScriptEngine engine)
    {
        myShips = engine.Array.New();
        enemyShips = engine.Array.New();
        
        engine.SetGlobalValue("myShips", myShips);
        engine.SetGlobalValue("enemyShips", enemyShips);
        
        foreach (var fleet in FleetContainer.fleets) {
            foreach (var ship in fleet.myShips)
            {
                if(fleet == _fleet)
                    allMyShips.Add(new MyShipJSObject(engine, ship));
                else
                    allEnemyShips.Add(new ShipJSObject(engine, ship));
            }
        }
    }

    /// <summary>
    /// 내보냈던 값들 중 변화된 점을 jsContext에 반영합니다.
    /// </summary>
    public void UpdateExportedValues()
    {
        int i;

        //update myShips
        if (myShips.Length != _fleet.myShips.Count)
        {
            i = 0;
            foreach (var shipObj in allMyShips)
            {
                if (shipObj.ship.alive.value)
                {
                    myShips[i++] = shipObj;
                }
            }
            myShips.Length = (uint)i;
        }
        for(i = 0; i < myShips.Length; ++i)
        {
            ((MyShipJSObject)myShips[i]).UpdateProperties();
        }

        //update enemyShips
        if (enemyShips.Length != _fleet.enemyShips.Count)
        {
            i = 0;
            foreach (var enemy in allEnemyShips)
            {
                if (enemy.scanned)
                {
                    enemyShips[i++] = enemy;
                }
            }
            enemyShips.Length = (uint)i;
        }
        for (i = 0; i < enemyShips.Length; ++i)
        {
            ((ShipJSObject)enemyShips[i]).UpdateProperties();
        }


        engine.SetGlobalValue("groundRadius", engine.Number.Construct(GameVariables.groundSize.value));
    }

    /// <summary>
    /// JSUpdate를 실행합니다.
    /// </summary>
    public void RunJSUpdate()
    {
        engine.CallGlobalFunction("update");
    }

    #region JSFunctions
    public void Log(string str)
    {
        Debug.Log("JS LOG : " + str);
    }
    public double Cos(double v)
    {
        return Mathf.Cos(Mathf.Deg2Rad * (float)v);
    }
    public double Sin(double v)
    {
        return Mathf.Sin(Mathf.Deg2Rad * (float)v);
    }
    public double D2R(double v)
    {
        return (Mathf.Deg2Rad * (float)v);
    }
    public double R2D(double v)
    {
        return (Mathf.Rad2Deg * (float)v);
    }
    public double Distance(ObjectInstance a, ObjectInstance b)
    {
        double x1 = (double)a["x"];
        double y1 = (double)a["y"];

        double x2 = (double)b["x"];
        double y2 = (double)b["y"];

        float x = (float)x1 - (float)x2;
        float y = (float)y1 - (float)y2;

        return Mathf.Sqrt(x * x + y * y);
    }

    public ObjectInstance Polar(ObjectInstance target)
    {
        ObjectInstance ret = PolarFrom(null, target);
        return ret;
    }
    public ObjectInstance PolarFrom(ObjectInstance center, ObjectInstance target)
    {
        //		Debug.Log(target["x"]);
        //Debug.Log((float)target["x"]);
        float x, y;
        //		float x = (float)(double)target["x"];//(float)((PropertyDescriptor)target["x"]).Value;
        //		float y = (float)(double)target["y"];//(float)((PropertyDescriptor)target["y"]).Value;
        if (target["x"] is Int32)
        {

            x = (int)target["x"];
            y = (int)target["y"];
        }
        else {
            x = (float)(double)target["x"];
            y = (float)(double)target["y"];
        }
        ObjectInstance ret = engine.Object.Construct();
        if (center == null)
        {
            var rot = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
            rot %= 360;
            var r = Vector2.Distance(new Vector2(x, y), Vector2.zero);
            ret["r"] = (double)r;
            ret["rot"] = (double)rot;
        }
        else
        {
            if (center["x"] is int)
            {

                x -= (int)center["x"];
                y -= (int)center["y"];
            }
            else {
                x -= (float)(double)center["x"];
                y -= (float)(double)center["y"];
            }
            var rot = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
            if (center.HasProperty("rot"))
            {
                if (center["rot"] is int)
                {
                    rot -= (int)center["rot"];
                }
                else {
                    rot -= (float)(double)center["rot"];
                }
                rot %= 360;

                if (rot > 180)
                {
                    rot -= 360;
                }
                if (rot < -180)
                {
                    rot += 360;
                }

            }

            var r = Vector2.Distance(new Vector2(x, y), Vector2.zero);
            ret["r"] = (double)r;
            ret["rot"] = (double)rot;
        }

        return ret;
    }
    #endregion
}

public class MyShipJSObject : ShipJSObject
{
    public MyShipJSObject(ScriptEngine engine, Ship ship)
        : base(engine,ship)
    {
        PopulateFunctions();
    }

    [JSFunction(Name = "shoot")]
    public void Shoot(double power)
    {
        _ship.Shoot((float)power);
    }
    [JSFunction(Name = "setSpeed")]
    public void SetSpeed(double speed)
    {
        _ship.SetSpeed((float)speed);
    }
    [JSFunction(Name = "setRotSpeed")]
    public void SetRotSpeed(double rotSpeed)
    {
        _ship.SetRotSpeed((float)rotSpeed);
    }

    public override void UpdateProperties()
    {
        base.UpdateProperties();
        this["spd"] = (double)_ship.spd;
        this["rotSpd"] = (double)_ship.rotSpd;
    }
}

public class ShipJSObject : ObjectInstance
{
    public bool scanned { get { return _ship.scanned > 0; } }
    public Ship ship { get { return _ship; } }
    protected Ship _ship;
    public ShipJSObject(ScriptEngine engine, Ship ship)
        : base(engine)
    {
        _ship = ship;
        this["ammo"] = 666;
        UpdateProperties();
    }

    public virtual void UpdateProperties()
    {
        this["id"] = (double)_ship.id;
        this["hp"] = (double)_ship.hp.value;
        this["x"] = (double)_ship.x;
        this["y"] = (double)_ship.y;
        this["rot"] = (double)_ship.rot;
        this["rad"] = (double)_ship.rad;
        this["delay"] = (double)_ship.delay.value;
        this["isCharging"] = _ship.isCharging.value;
        this["chargedPower"] = (double)_ship.chargedPower;
        this["shootingRot"] = (double)_ship.shootingRot;
        this["shootingRad"] = (double)_ship.shootingRad;
        this["shootingPower"] = (double)_ship.shootingPower;
    }
}