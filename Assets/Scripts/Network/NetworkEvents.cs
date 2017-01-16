using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class NetworkEvents
{
    /// <summary>
    /// 새 플레이어가 입장하면 발생
    /// </summary>
    public static PlayerEvent onPlayerJoin = new PlayerEvent();

    /// <summary>
    /// 플레이어가 접속을 종료하면 발생. 인자는 id
    /// </summary>
    public static ByteEvent onPlayerOut = new ByteEvent();

    /// <summary>
    /// 방장이 접속을 종료하면 발생.
    /// </summary>
    public static UnityEvent onServerClosed = new UnityEvent();

    /// <summary>
    /// 방장이 게임 시작했을 떄 발생
    /// </summary>
    public static UnityEvent onPlayGame = new UnityEvent();

    /// <summary>
    /// 방장이 결과 화면에서 로비로 돌아가기를 선택하면 발생
    /// </summary>
    public static UnityEvent onGameEnd = new UnityEvent();

    /// <summary>
    /// 스크립트가 제출되면 발생
    /// </summary>
    public static ScriptEvent onScriptIn = new ScriptEvent();

    /// <summary>
    /// 방장 혹은 스크립트 주인이 스크립트를 취소하면 발생
    /// </summary>
    public static ByteEvent onScriptOut = new ByteEvent();

    /// <summary>
    /// 방장 혹은 스크립트 주인이 스크립트를 취소하면 발생
    /// </summary>
    public static ChatMessageEvent onChat = new ChatMessageEvent();

    /// <summary>
    /// 방장이 배의 수를 변경하면 발생
    /// </summary>
    public static ByteEvent onShipAmountChanged = new ByteEvent();
}
