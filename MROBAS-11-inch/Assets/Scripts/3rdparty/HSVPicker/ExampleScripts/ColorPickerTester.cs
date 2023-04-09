using UnityEngine;
using HSVPicker;

public class ColorPickerTester : MonoBehaviour 
{

    public ColorPicker picker;

    Color Color = Color.white;

	// Use this for initialization
	void Start () 
    {
        picker.onValueChanged.AddListener(color =>
        {
            Color = color;
        });

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
