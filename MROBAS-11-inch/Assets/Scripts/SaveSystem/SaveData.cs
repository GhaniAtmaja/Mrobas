using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    private static SaveData _current;

    public ContainerData containerData;
    public SkinData skinData;
    public TumorData tumorData;
    public VentricleData ventricleData;
    public List<PointData> pointDatas;

    public static SaveData current
    {
        get
        {
            if (_current == null)
            {
                _current = new SaveData();
                 
            }
            return _current;
        }
        set
        { 
            if (value != null)
            {
                _current = value;
            }
        }
    }
    
}
