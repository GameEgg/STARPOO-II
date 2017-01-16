using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Threading;
using System.Net.Sockets;
using System.Net;

public class TransferTCP {
    
    static TransferTCP _instance;
    public static TransferTCP instance{
        get{
            if(_instance == null){
                _instance = new TransferTCP();
            }
            return _instance;
        }
    }

    public PacketQueue sendToServerQueue;
    public PacketQueue sendToClientsQueue;
    public PacketQueue recvQueue;
    public ClientEvent onNewClientJoin = new ClientEvent();
    public UnityEvent onServerClosed = new UnityEvent();
    public ByteEvent onClientDisconnected = new ByteEvent();



    public Thread thread;
    bool threadRunning = true;
    Socket socketAsServer;
    Socket socketAsClient;

    bool acceptClient;
    List<Client> clients;
    byte[] buffer = new byte[1024*20];


    public TransferTCP()
    {
        sendToServerQueue = new PacketQueue();
        sendToClientsQueue = new PacketQueue();
        recvQueue = new PacketQueue();
        clients = new List<Client>();
    }

    void Dispatch(){
        while(thread.IsAlive && threadRunning)
        {
            Debug.Log("isServer = "+ NetworkVariables.isServer);
            if (NetworkVariables.isServer){
                if(acceptClient){
                    AcceptClient();
                }

                for(int i = clients.Count - 1; i >= 0; --i){
                    var client = clients[i];
                    if (!IsSocketConnected(client.socket))
                    {
                        Debug.LogWarning("클라이언트 한 명 나감 : " + client.networkId);
                        clients.RemoveAt(i);
                        onClientDisconnected.Invoke(client.networkId);
                    }
                    else {
                        DispatchReceive(client.socket);
                    }
                }
            }
            if (NetworkVariables.isClient)
            {
                if (!IsSocketConnected(socketAsClient))
                {
                    Debug.LogWarning("서버 닫힘");
                    onServerClosed.Invoke();
                }
                else {
                    DispatchReceive(socketAsClient);
                }
            }
            DispatchSend();

            Thread.Sleep(5);
        }
        Debug.Log("end thread");
    }

    bool IsSocketConnected(Socket s)
    {
        return !((s.Poll(1000, SelectMode.SelectRead) && (s.Available == 0)) || !s.Connected);
    }

    void DispatchSend(){
        //if(socket.Poll(0,SelectMode.SelectWrite)){
        int sendSize;
        if (NetworkVariables.isServer)
        {
            while (true)
            {
                sendSize = sendToClientsQueue.Dequeue(ref buffer, buffer.Length);
                if (sendSize <= 0)
                    break;
                foreach (var client in clients)
                {
                    client.socket.Send(buffer, sendSize, SocketFlags.None);
                }
            }

            foreach (var client in clients)
            {
                SendBuffer(client.socket, client.sendQueue);
            }
        }
        if(NetworkVariables.isClient)
        {
            SendBuffer(socketAsClient, sendToServerQueue);
        }
    }

    void SendBuffer(Socket socket, PacketQueue queue)
    {
        while (true)
        {
            var sendSize = queue.Dequeue(ref buffer, buffer.Length);
            if (sendSize <= 0)
                break;
            socket.Send(buffer, sendSize, SocketFlags.None);
        }
    }

    void DispatchReceive(Socket socket)
    {
        while (socket.Poll(0,SelectMode.SelectRead))
        {
            var buffer = new byte[1400];
            var recvSize = socket.Receive(buffer, buffer.Length, SocketFlags.None);
            if(recvSize == 0)
            {
                return;
            }
            else 
            {
                recvQueue.EnqueueAndAnalyze(buffer,recvSize);
            }
        }
    }

    public void StartServer()
    {
        socketAsServer = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        socketAsServer.Bind(new IPEndPoint(IPAddress.Any,NetworkConsts.port));
        socketAsServer.Listen(NetworkConsts.maxConnection);
        threadRunning = true;

        thread = new Thread(new ThreadStart(Dispatch));
        thread.Start();
        acceptClient = true;
    }

    public void StopServer(){
        foreach(var client in clients)
        {
            client.socket.Close();
        }
        clients.Clear();
        socketAsServer.Close();
        socketAsServer = null;
        threadRunning = false;
        thread = null;
        acceptClient = false;
    }

    public void PauseAcceptConnecting(){
        acceptClient = false;
        socketAsServer.Close();
    }

    public void ResumeAcceptConnecting(){
        acceptClient = true;
        socketAsServer = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        socketAsServer.Bind(new IPEndPoint(IPAddress.Any,NetworkConsts.port));
        socketAsServer.Listen(NetworkConsts.maxConnection);
    }

    public bool Connect(string address)
    {
        socketAsClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socketAsClient.NoDelay = true;
        socketAsClient.SendTimeout = 2000;
        socketAsClient.ReceiveTimeout = 2000;
        try {
            socketAsClient.Connect(address, NetworkConsts.port);
        }
        catch
        {
            socketAsClient.Close();
            socketAsClient = null;
            return false;
        }

        //socketAsClient.SendBufferSize = 0;

        if (thread == null)
        {
            threadRunning = true;
            thread = new Thread(new ThreadStart(Dispatch));
            Debug.Log("start thread");
            thread.Start();
        }
        Debug.Log("start client");
        return true;
    }

    public void Disconnect()
    {
        socketAsClient.Close();
        threadRunning = false;
        thread = null;
    }

    void AcceptClient(){
        if(socketAsServer.Poll(0,SelectMode.SelectRead))
        {
            var socket = socketAsServer.Accept();
            var client = new Client(socket);
            client.networkId = NetworkIdGenerator.Generate();
            clients.Add(client);
            onNewClientJoin.Invoke(client);
            Debug.Log("client connected! : "+client.networkId);
            
            if(!acceptClient){
                client.sendQueue.Enqueue(new pEmpty().Serialize(PacketType.pKick));
            }

        }
    }
}
