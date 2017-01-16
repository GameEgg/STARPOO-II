using System.Collections;
using System.Collections.Generic;
//using System;
using UnityEngine;
using System.IO;

public class PacketQueue {

    struct PacketInfo
    {
        public int offset;
        public int size;
    }
    public struct PacketByteData
    {
        public PacketHeader header;
        public byte[] data;
        public PacketByteData(PacketHeader header, byte[] data)
        {
            this.header = header;
            this.data = data;
        }
    }

    enum Phase
    {
        WriteHeader,WriteData
    }

    Phase phase;
    int writingOffset;
    byte[] writingData;
    PacketHeader writingDataHeader;


    int offset;
    MemoryStream streamBuffer;
    List<PacketInfo> offsetList;

    Queue<PacketByteData> receivedPacketData = new Queue<PacketByteData>();

    Object lockObj = new Object();

    public PacketQueue()
    {
        writingOffset = 0;
        writingData = new byte[1024*20];
        phase = Phase.WriteHeader;
        writingDataHeader = new PacketHeader();
        streamBuffer = new MemoryStream();
        offsetList = new List<PacketInfo>();
    }

    public void EnqueueAndAnalyze(byte[] data, int size)
    {
        var localOffset = 0;
        while (true)
        {
            if (phase == Phase.WriteHeader)
            {
                var complete = ReadData(data, size, 3, ref localOffset);
                if(complete)
                {
                    PacketHeaderSerializer.Deserializer(writingData, ref writingDataHeader);
                    //Debug.Log("Analyze packetType, datasize - "+((PacketType)writingDataHeader.packetType).ToString()  + ", "+ writingDataHeader.size);
                    phase = Phase.WriteData;
                    writingOffset = 0;
                }
            }
            else if(phase == Phase.WriteData)
            {
                var complete = ReadData(data, size, writingDataHeader.size, ref localOffset);
                if (complete)
                {
                    var dataonly = new byte[writingDataHeader.size];
                    System.Buffer.BlockCopy(writingData, 0, dataonly, 0, dataonly.Length);
                    receivedPacketData.Enqueue(new PacketByteData(writingDataHeader, dataonly));
                    phase = Phase.WriteHeader;
                    writingOffset = 0;
                }
            }
            if (localOffset >= size)
                break;
        }
        //Debug.Log("Analyze complete - " + localOffset + "/" + size);
    }

    bool ReadData(byte[] data, int size, int totalSize, ref int localOffset)
    {
        var writeSize = (int)Mathf.Min(size - localOffset, totalSize - writingOffset);

        System.Buffer.BlockCopy(data, localOffset, writingData, writingOffset, writeSize);

        localOffset += writeSize;
        writingOffset += writeSize;

        return writingOffset >= totalSize;
    }


    public int ReceivedItemCount()
    {
        return receivedPacketData.Count;
    }

    public PacketByteData DequeueAnalyzed()
    {
        return receivedPacketData.Dequeue();
    }
    
    public int Enqueue(byte[] data, int size = -1)
    {
        if (size == -1)
            size = data.Length;
        
        var info = new PacketInfo();

        info.offset = offset;
        info.size = size;
        
        lock (lockObj)
        {
            offsetList.Add(info);

            streamBuffer.Position = offset;
            streamBuffer.Write(data, 0, size);
            streamBuffer.Flush();
            offset += size;
        }

        return size;
    }
    
    
    public int Dequeue(ref byte[] buffer, int size)
    {
        if(offsetList.Count <= 0){
            return -1;
        }

        int recvSize = 0;

        lock(lockObj){
            var info = offsetList[0];

            var dataSize = Mathf.Min(size,info.size);
            streamBuffer.Position = info.offset;
            recvSize = streamBuffer.Read(buffer,0,dataSize);

            if(recvSize > 0){
                offsetList.RemoveAt(0);
            }

            if(offsetList.Count == 0){
                Clear();
                offset = 0;
            }
        }

        return recvSize;
    }
    
    void Clear()
    {
        var buffer = streamBuffer.GetBuffer();
        System.Array.Clear(buffer, 0, buffer.Length);
        streamBuffer.Position = 0;
        streamBuffer.SetLength(0);
    }
}
