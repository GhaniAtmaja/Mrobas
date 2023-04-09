using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ContainerHandler : MonoBehaviour
{
    public objectType objectType;
    public ContainerData ItemData;
    public SkinData skinData;
    public TumorData tumorData;
    public VentricleData ventricleData;
    public List<PointData> pointDatas;
    public void Awake()
    {
        ItemData = new ContainerData();
        skinData = new SkinData();
        tumorData = new TumorData();
        ventricleData = new VentricleData();
        pointDatas = new List<PointData>();
    }
    public void Start()
    {

        if (string.IsNullOrEmpty(ItemData.id))
        {
            //container
            ItemData.id = System.DateTime.Now.ToLongDateString() + System.DateTime.Now.ToLongTimeString() + Random.Range(0, int.MaxValue).ToString();
            ItemData.objectType = objectType.container;
        }
    }
    public void SavePatientObject(string StreamSkin, string StreamTumor, string StreamVentricle)
    {
        SaveData.current.pointDatas = new List<PointData>();
        int pointref = 0;
        foreach (Transform child in this.gameObject.transform)
        {
            if (child.gameObject.tag == "SkinObject")
            {
                skinData.id = System.DateTime.Now.ToLongDateString() + System.DateTime.Now.ToLongTimeString() + Random.Range(0, int.MaxValue).ToString() + "_skin";
                skinData.objectType = objectType.skin;
                skinData.StreamSkin = StreamSkin;

                Color skinColor = child.gameObject.GetComponent<MeshRenderer>().material.color;

                skinData.Color = skinColor;
                skinData.ExtraInfo = child.gameObject.name;
                SaveData.current.skinData = skinData;
                //SaveMeshSkin(skinMesh);
            }
            else if (child.gameObject.tag == "TumorObject")
            {
                tumorData.id = System.DateTime.Now.ToLongDateString() + System.DateTime.Now.ToLongTimeString() + Random.Range(0, int.MaxValue).ToString() + "_tumor";
                tumorData.objectType = objectType.tumor;
                tumorData.StreamTumor = StreamTumor;

                Color tumorColor = child.gameObject.GetComponent<MeshRenderer>().material.color;

                tumorData.Color = tumorColor;
                tumorData.ExtraInfo = child.gameObject.name;
                SaveData.current.tumorData = tumorData;
                //SaveMeshTumor(tumorMesh);
            }
            else if (child.gameObject.tag == "VentricleObject")
            {
                ventricleData.id = System.DateTime.Now.ToLongDateString() + System.DateTime.Now.ToLongTimeString() + Random.Range(0, int.MaxValue).ToString() + "_ventricle";
                ventricleData.objectType = objectType.ventricle;
                ventricleData.StreamVentricle = StreamVentricle;

                Color ventricleColor = child.gameObject.GetComponent<MeshRenderer>().material.color;

                ventricleData.Color = ventricleColor;
                ventricleData.ExtraInfo = child.gameObject.name;
                SaveData.current.ventricleData = ventricleData;
                //SaveMeshTumor(tumorMesh);
            }
            else
            {
                PointData pointData = new PointData();
                pointData.id = System.DateTime.Now.ToLongDateString() + System.DateTime.Now.ToLongTimeString() + Random.Range(0, int.MaxValue).ToString() + "_point";
                pointData.objectType = objectType.point;
                pointData.position = child.position;
                Debug.Log("Child_position " + pointData.position);
                pointDatas.Add(pointData);
                pointref++;
            }
        }
        ItemData.PointRefCount = pointref;
        SaveData.current.pointDatas = pointDatas;
        SaveData.current.containerData = ItemData;
    }
    public void Update()
    {
        ItemData.position = transform.position;
        ItemData.rotation = transform.rotation;
    }
}
