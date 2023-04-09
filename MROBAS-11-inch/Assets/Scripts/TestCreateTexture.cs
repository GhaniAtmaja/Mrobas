using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestCreateTexture : MonoBehaviour
{
    public RawImage img;
    public GameObject obj;
    // Start is called before the first frame update
    void Start()
    {
        //public static Texture2D RuntimePreviewGenerator.GenerateModelPreview(Transform model, int width = 64, int height = 64, bool shouldCloneModel = false);
        Texture2D tex = RuntimePreviewGenerator.GenerateModelPreview(obj.transform, 64, 64, false);
        img.texture = tex;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
