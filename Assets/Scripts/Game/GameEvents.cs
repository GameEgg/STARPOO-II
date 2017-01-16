using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    /// <summary>
    /// 공격자 , 희생자 , 데미지
    /// </summary>
    public static ShipShipFloatEvent onLaserHit = new ShipShipFloatEvent();

    /// <summary>
    /// 자살한 함선 , 자살로 잃은 남은 hp
    /// </summary>
    public static ShipFloatEvent onSuicide = new ShipFloatEvent();

    /// <summary>
    /// 킬러 , 희생자
    /// </summary>
    public static ShipShipEvent onShipKilledByLaser = new ShipShipEvent();

    /// <summary>
    /// 발사 함선, 차징 에너지
    /// </summary>
    public static ShipFloatEvent onLaserShoot = new ShipFloatEvent();

    /// <summary>
    /// 파괴된 함선
    /// </summary>
    public static ShipEvent onShipDestroyed = new ShipEvent();
}
