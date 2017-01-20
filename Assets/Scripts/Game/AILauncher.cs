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

    int randomI = 0;
    double[] randoms = {0.67875,0.32222,0.65738,0.38630,0.46512,0.96310,0.73474,0.68538,0.56789,0.92037,0.86292,0.30085,0.72147,0.19404,0.30749,0.23272,0.45703,0.51853,0.80288,0.21540,0.24340,0.42498,0.61466,0.51448,0.08059,0.38226,0.74283,0.12995,0.49975,0.39035,0.18740,0.79624,0.96973,0.98041,0.94837,0.04450,0.75611,0.62129,0.76015,0.34358,0.59329,0.57857,0.88833,0.95905,0.01246,0.85224,0.33290,0.62534,0.48244,0.17672,0.82828,0.87360,0.26881,0.81760,0.90564,0.17268,0.37158,0.77488,0.03382,0.78815,0.52516,0.04855,0.47839,0.45444,0.99514,0.63861,0.97378,0.70270,0.78151,0.27949,0.78556,0.00178,0.88428,0.23677,0.17931,0.61725,0.03787,0.51043,0.35831,0.28354,0.50784,0.83492,0.11927,0.53180,0.29422,0.47580,0.93105,0.83087,0.39699,0.43307,0.05923,0.44635,0.55057,0.70674,0.50380,0.11263,0.28613,0.29681,0.26476,0.36090};

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
        engine.SetGlobalFunction("cartesian", new Func<ObjectInstance, ObjectInstance>(Cartesian));
        engine.SetGlobalFunction("polar", new Func<ObjectInstance, ObjectInstance>(Polar));
        engine.SetGlobalFunction("polarFrom", new Func<ObjectInstance, ObjectInstance, ObjectInstance>(PolarFrom));
        engine.SetGlobalFunction("cos", new Func<double, double>(Cos));
        engine.SetGlobalFunction("sin", new Func<double, double>(Sin));
        engine.SetGlobalFunction("d2r", new Func<double, double>(D2R));
        engine.SetGlobalFunction("r2d", new Func<double, double>(R2D));
        engine.SetGlobalFunction("dist", new Func<ObjectInstance, ObjectInstance, double>(Distance));
        engine.SetGlobalFunction("random", new Func<double>(Random));
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
            Debug.Log("!! : " + enemyShips.Length);
            i = 0;
            foreach (var enemy in allEnemyShips)
            {
                if (enemy.scanned)
                {
                    enemyShips[i++] = enemy;
                }
            }
            enemyShips.Length = (uint)i;
            Debug.Log("i : "+i);
            Debug.Log("~ : "+enemyShips.Length);
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
        GameEvents.onLog.Invoke(_fleet.script,str);
    }
    public double Random(){
        return randoms[randomI++%randoms.Length];
    }
    public double Cos(double v)
    {
        return Math.Cos(D2R(v));
    }
    public double Sin(double v)
    {
        return Math.Sin(D2R(v));
    }
    public double D2R(double v)
    {
        return (v * Math.PI / 180);
    }
    public double R2D(double v)
    {
        return (v * 180 / Math.PI);
    }
    public double Distance(ObjectInstance a, ObjectInstance b)
    {
        double x1 = (double)a["x"];
        double y1 = (double)a["y"];

        double x2 = (double)b["x"];
        double y2 = (double)b["y"];

        double x = x1 - x2;
        double y = y1 - y2;

        return Math.Sqrt(x * x + y * y);
    }

    public ObjectInstance Cartesian(ObjectInstance target)
    {
        double r = target.GetDouble("r");
        double rot = target.GetDouble("rot");

        double x = r * Cos(rot);
        double y = r * Sin(rot);

        ObjectInstance ret = engine.Object.Construct();
        ret["x"] = x;
        ret["y"] = y;

        return ret;
    }

    public ObjectInstance Polar(ObjectInstance target)
    {
        ObjectInstance ret = PolarFrom(null, target);
        return ret;
    }
    public ObjectInstance PolarFrom(ObjectInstance center, ObjectInstance target)
    {
        var x = target.GetDouble("x");
        var y = target.GetDouble("y");

        ObjectInstance ret = engine.Object.Construct();
        if (center == null)
        {
            var rot = R2D(Math.Atan2(y, x));
            rot %= 360;

            var r = Vector2.Distance(new Vector2((float)x, (float)y), Vector2.zero);
            ret["r"] = (double)r;
            ret["rot"] = rot;
        }
        else
        {
            x -= center.GetDouble("x");
            y -= center.GetDouble("y");

            var rot = R2D(Math.Atan2(y, x));
            if (center.HasProperty("rot"))
            {
                rot -= center.GetDouble("rot");

                rot %= 360;

                if (rot > 181)
                {
                    rot -= 360;
                }
                else if (rot < -181)
                {
                    rot += 360;
                }
            }

            var r = Vector2.Distance(new Vector2((float)x, (float)y), Vector2.zero);
            ret["r"] = (double)r;
            ret["rot"] = rot;
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
        //_ship.Shoot((float)power);
        _ship.Shoot((float)power);
    }
    [JSFunction(Name = "setSpeed")]
    public void SetSpeed(double speed)
    {
        //_ship.SetSpeed((float)speed);
        _ship.SetSpeed((float)speed);
    }
    [JSFunction(Name = "setRotSpeed")]
    public void SetRotSpeed(double rotSpeed)
    {
        //_ship.SetRotSpeed((float)rotSpeed);
        _ship.SetRotSpeed((float)rotSpeed);
    }

    public override void UpdateProperties()
    {
        base.UpdateProperties();
        ExportValue("spd",_ship.spd);
        ExportValue("rotSpd",_ship.rotSpd);
        ExportValue("isDetected",(_ship.scanned > 0));
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
        ExportValue("id", _ship.id);
        ExportValue("hp", _ship.hp.value);
        ExportValue("x", _ship.x);
        ExportValue("y", _ship.y);
        ExportValue("rot", _ship.rot);
        ExportValue("rad", _ship.rad);
        ExportValue("delay", _ship.delay.value);
        ExportValue("isCharging", _ship.isCharging.value);
        ExportValue("chargedPower", _ship.chargedPower);
        ExportValue("shootingRot", _ship.shootingRot);
        ExportValue("shootingRad", _ship.shootingRad);
        ExportValue("shootingPower", _ship.shootingPower);
    }

    protected void ExportValue(string key, float value){
        this[key] = (double)value;
    }
    protected void ExportValue(string key, bool value){
        this[key] = value;
    }
}