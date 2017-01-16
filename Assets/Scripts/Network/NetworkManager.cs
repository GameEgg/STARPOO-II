using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Net.Sockets;

public class NetworkManager : MonoBehaviour {
    static NetworkManager _instance;
    public static NetworkManager instance
    {
        get{
            if(_instance == null){
                var go = new GameObject();
                DontDestroyOnLoad(go);
                go.name = "NetworkManager";

                _instance = go.AddComponent<NetworkManager>();
            }
            return _instance;
        }
    }


    public NetworkPlayerManager playerContainer = new NetworkPlayerManager();
    public NetworkPacketHandler packetHandler = new NetworkPacketHandler();
    bool goMain = false;


    public void Chat(string message, byte networkId = 0)
    {
        if (networkId == 0)
            networkId = GameVariables.me.networkId;

        var chat = new ChatMessage(networkId, message);
        var pChat = new pChatMessage(chat);
        if (NetworkVariables.isServer)
        {
            NetworkEvents.onChat.Invoke(chat);
        }
        SendPacket(pChat.Serialize(PacketType.pChat));
    }

    public void EndGame()
    {
        var packet = new pEmpty();
        SendPacket(packet.Serialize(PacketType.pGameEnd));
    }

    public void PlayGame()
    {
        var packet = new pEmpty();
        SendPacket(packet.Serialize(PacketType.pPlayGame));
    }

    public void ChangeShipAmount(byte amount)
    {
        var packet = new pByte(amount);
        SendPacket(packet.Serialize(PacketType.pShipAmount));
    }

    public void SendScript(AIScript script)
    {
        var packet = new pScript();
        packet.networkId = GameVariables.me.networkId;
        packet.name = script.name;
        packet.code = script.code;
        
        SendPacket(packet.Serialize(PacketType.pSendScript));
    }

    public void CancelScript(byte index)
    {
        SendPacket(new pByte(index).Serialize(PacketType.pCancelScript));
    }
    

    void SendPacket(byte[] serializedPacket)
    {
        if (NetworkVariables.isServer)
        {
            TransferTCP.instance.sendToClientsQueue.Enqueue(serializedPacket);
        }
        if (NetworkVariables.isClient)
        {
            TransferTCP.instance.sendToServerQueue.Enqueue(serializedPacket);
        }
    }

    public void StopAll()
    {
        if (NetworkVariables.isServer)
            StopServer();
        if (NetworkVariables.isClient)
            Disconnect();
        playerContainer.players.Clear();
    }

    public void StartServer()
    {
        NetworkVariables.isNetwork = true;
        NetworkVariables.isServer = true;
        SceneManager.LoadScene("Room");

        GameVariables.me.networkId = 1;
        NetworkEvents.onPlayerJoin.Invoke(GameVariables.me);
        TransferTCP.instance.onNewClientJoin.AddListener(NewClientJoin);
        TransferTCP.instance.onClientDisconnected.AddListener(OnClientDisconnected);
        TransferTCP.instance.StartServer();
    }

    public void StopServer()
    {
        NetworkVariables.isNetwork = false;
        NetworkVariables.isServer = false;
        TransferTCP.instance.StopServer();
        TransferTCP.instance.onNewClientJoin.RemoveListener(NewClientJoin);
        TransferTCP.instance.onClientDisconnected.RemoveListener(OnClientDisconnected);
    }

    public void Connect(string url = "localhost")
    {
        NetworkVariables.isNetwork = true;
        NetworkVariables.isClient = true;
        SceneManager.LoadScene("Room");
        var success = TransferTCP.instance.Connect(url);

        if(success){
            TransferTCP.instance.onServerClosed.AddListener(OnServerClosed);
            NetworkEvents.onGameEnd.AddListener(OnGameEnd);
        }
        else
        {
            NoticeUI.instance.ShowMessage("Fail to connect");
            NetworkVariables.isNetwork = false;
            NetworkVariables.isClient = false;
            goMain = true;
        }

    }

    public void Disconnect()
    {
        NetworkVariables.isNetwork = false;
        NetworkVariables.isClient = false;
        TransferTCP.instance.Disconnect();
        TransferTCP.instance.onServerClosed.RemoveListener(OnServerClosed);
        NetworkEvents.onGameEnd.RemoveListener(OnGameEnd);
    }

    void OnGameEnd()
    {
        SceneManager.LoadScene("Room");
    }

    void OnServerClosed()
    {
        Debug.LogWarning("알림 : 서버가 종료되었습니다.");
        NoticeUI.instance.ShowMessage("Connection closed by server");
        Disconnect();
        goMain = true;
    }

    void OnClientDisconnected(byte id)
    {
        NetworkEvents.onPlayerOut.Invoke(id);
        TransferTCP.instance.sendToClientsQueue.Enqueue(new pByte(id).Serialize(PacketType.pDisconnect));
    }

    void Update()
    {
        if(TransferTCP.instance.thread != null)
            Debug.Log("thread alive : "+TransferTCP.instance.thread.IsAlive);
        if (TransferTCP.instance.recvQueue.ReceivedItemCount() > 0)
        {
            var item = TransferTCP.instance.recvQueue.DequeueAnalyzed();
            packetHandler.Handle(item.header.packetType, item.data);
        }
        if (goMain) {
            SceneManager.LoadScene("Main");
            goMain = false;
        }
    }

    void NewClientJoin(Client client)
    {
        var networkId = new pByte(client.networkId);
        Debug.Log("Send generated id : " + client.networkId);
        client.sendQueue.Enqueue(networkId.Serialize(PacketType.pNetworkId));
        foreach(var p in playerContainer.players)
        {
            var pPlayer = new pPlayer(p);
            client.sendQueue.Enqueue(pPlayer.Serialize(PacketType.pJoin));
        }
    }

    void OnDestroy()
    {
        StopAll();
    }

}
