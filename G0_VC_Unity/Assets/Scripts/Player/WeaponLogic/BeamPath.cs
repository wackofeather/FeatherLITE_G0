using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


[System.Serializable]
public class BeamPath : INetworkSerializable, IEquatable<BeamPath>
{



    public List<BounceHit> points = new List<BounceHit>();

    public int PathPoints()
    {
        return points.Count;
    }


    public Vector3 GetPosition(float progress)
    {
        int basePoint = (int)progress;
        if (basePoint >= points.Count - 1) return points[basePoint].point;
        float betweenProgress = progress - basePoint;
        return Vector3.Lerp(points[basePoint].point, points[basePoint + 1].point, betweenProgress);
    }


    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        // Serialize the count of the list
        int count = points.Count;
        serializer.SerializeValue(ref count);

        if (serializer.IsReader)
        {
            // Clear the list when deserializing to avoid duplicates
            points.Clear();
            for (int i = 0; i < count; i++)
            {
                BounceHit bounceHit = new BounceHit(Vector3.zero, 0, 0); // Default constructor
                bounceHit.NetworkSerialize(serializer); // Deserialize each item
                points.Add(bounceHit);
            }
        }
        else
        {
            // Serialize each item in the list
            foreach (var point in points)
            {
                point.NetworkSerialize(serializer);
            }

        }

    }

    // Implement IEquatable<BeamPath>
    public bool Equals(BeamPath other)
    {
        if (other == null || other.points.Count != points.Count)
        {
            return false;
        }

        for (int i = 0; i < points.Count; i++)
        {
            if (!points[i].Equals(other.points[i]))
            {
                return false;
            }
        }

        return true;
    }

    public override bool Equals(object obj)
    {
        return obj is BeamPath other && Equals(other);
    }

    public override int GetHashCode()
    {
        int hash = 17;
        foreach (var point in points)
        {
            hash = hash * 31 + point.GetHashCode();
        }
        return hash;


    }
}

[System.Serializable]
public class BounceHit : INetworkSerializable, IEquatable<BounceHit>
{
    public BounceHit(Vector3 _point, int _hitType, ulong _ID)
    {
        point = _point;
        hitType = _hitType;
        playerHitID = _ID;
    }
    public Vector3 point;
    public int hitType;

    public ulong playerHitID;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref point);
        serializer.SerializeValue(ref hitType);
        serializer.SerializeValue(ref playerHitID);
    }

    public bool Equals(BounceHit other)
    {
        return other != null &&
               point == other.point &&
               hitType == other.hitType &&
               playerHitID == other.playerHitID;
    }

    public override bool Equals(object obj)
    {
        return obj is BounceHit other && Equals(other);
    }

    public override int GetHashCode()
    {
        return point.GetHashCode() ^ hitType.GetHashCode() ^ playerHitID.GetHashCode();
    }
}