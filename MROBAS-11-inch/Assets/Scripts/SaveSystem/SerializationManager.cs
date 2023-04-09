using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SerializationManager
{
    public static bool Save(string saveName, object saveData)
    {
        BinaryFormatter formatter = GetBinaryFormatter();

        if(!Directory.Exists(Application.persistentDataPath + "/saves/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves/");
        }

        string path = Application.persistentDataPath + "/saves/" + saveName + ".save";
        FileStream file = File.Create(path);

        formatter.Serialize(file, saveData);
        file.Close();

        return true;
    }



    public static object Load(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }
        BinaryFormatter formatter = GetBinaryFormatter();

        FileStream file = File.Open(path, FileMode.Open);
        try
        {
            object save = formatter.Deserialize(file);
            file.Close();
            return save;
        }
        catch
        {
            file.Close();
            return null;
        }
    }

    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        SurrogateSelector selector = new SurrogateSelector();

        Vector3SerializationSurrogate vector3Surrogate = new Vector3SerializationSurrogate();
        Vector2SerializationSurrogate vector2Surrogate = new Vector2SerializationSurrogate();
        Vector4SerializationSurrogate vector4SerializationSurrogate = new Vector4SerializationSurrogate();
        QuaternionSerializationSurrogate quaternionSurrogate = new QuaternionSerializationSurrogate();
        Color32SerializationSurrogate color32SerializationSurrogate = new Color32SerializationSurrogate();

        selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3Surrogate);
        selector.AddSurrogate(typeof(Vector2), new StreamingContext(StreamingContextStates.All), vector2Surrogate);
        selector.AddSurrogate(typeof(Vector4), new StreamingContext(StreamingContextStates.All), vector4SerializationSurrogate);
        selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), quaternionSurrogate);
        selector.AddSurrogate(typeof(Color), new StreamingContext(StreamingContextStates.All), color32SerializationSurrogate);


        formatter.SurrogateSelector = selector;

        return formatter;
    }
}
