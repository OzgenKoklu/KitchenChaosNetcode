using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : INetworkSerializable, IEquatable<PlayerData> 
{

    public ulong clientId;
    public int colorId;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref colorId);
    }

    bool IEquatable<PlayerData>.Equals(PlayerData other)
    {
        return clientId == other.clientId && colorId == other.colorId;
    }
}
