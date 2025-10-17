using System;
using UnityEngine;

[Serializable]
public struct ServerData
{
    public float posX;
    public float posY;
    public float posZ;

    public Vector3 ToVector3() => new Vector3(posX, posY, posZ);
    public static ServerData FromVector3(Vector3 v) => new ServerData { posX = v.x, posY = v.y, posZ = v.z };
    public override string ToString() => $"({posX:F3}, {posY:F3}, {posZ:F3})";
}
