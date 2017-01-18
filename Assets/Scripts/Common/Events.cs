using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Net.Sockets;

public class AIScriptEvent : UnityEvent<AIScript> { }
public class ShipFloatEvent : UnityEvent<Ship, float> { }
public class ShipShipFloatEvent : UnityEvent<Ship, Ship, float> { }
public class ShipEvent : UnityEvent<Ship> { }
public class ShipShipEvent : UnityEvent<Ship,Ship> { }

public class PlayerEvent : UnityEvent<Player> { }
public class PlayersEvent : UnityEvent<List<Player>> { }
public class IntEvent : UnityEvent<int> { }
public class ByteEvent : UnityEvent<byte> { }
public class ScriptEvent : UnityEvent<AIScript> { }
public class SelectedScriptEvent : UnityEvent<UIScriptListItem> { }
public class ChatMessageEvent : UnityEvent<ChatMessage> { }
public class NetworkErrorEvent : UnityEvent<NetworkError> { }

public class ClientEvent : UnityEvent<NetworkClient> { }
public class ScriptStringEvent : UnityEvent<AIScript, string> { };