using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public enum objectType {container,tumor,skin,point,ventricle};

[System.Serializable]
public class ContainerData
{
    public string id;
    public objectType objectType;
    public int PointRefCount;
    public Vector3 position;
    public Quaternion rotation;
}
