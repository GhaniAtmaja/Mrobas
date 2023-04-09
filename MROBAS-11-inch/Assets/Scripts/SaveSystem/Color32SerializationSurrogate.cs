using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class Color32SerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        Color color32 = (Color)obj;
        info.AddValue("r", color32.r);
        info.AddValue("g", color32.g);
        info.AddValue("b", color32.b);
        info.AddValue("a", color32.a);

    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Color color32 = (Color)obj;
        color32.r = (byte)info.GetValue("r", typeof(byte));
        color32.g = (byte)info.GetValue("g", typeof(byte));
        color32.b = (byte)info.GetValue("b", typeof(byte));
        color32.a = (float)info.GetValue("a", typeof(float));
        obj = color32;
        return obj;
    }
}
