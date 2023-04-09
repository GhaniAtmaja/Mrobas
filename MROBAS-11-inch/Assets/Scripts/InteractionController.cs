using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    GameObject selectedObject;

    void Start()
    {

    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                Debug.Log(hit.transform.gameObject);
                selectedObject = hit.transform.gameObject;
                selectedObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
            }
            else
            {
                selectedObject.GetComponent<MeshRenderer>().material.color = Color.white;
                selectedObject = null;
            }
        }
    }
}
