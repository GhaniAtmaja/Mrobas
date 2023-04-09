using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemLayerController : MonoBehaviour
{
    UIManager uIManager;
    // Start is called before the first frame update
    void Start()
    {
        uIManager = FindObjectOfType<UIManager>();
    }

    public void HideObject()
    {
        GameObject button = EventSystem.current.currentSelectedGameObject;
        if (gameObject.name == button.transform.parent.gameObject.name)
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
                button.GetComponent<Image>().color = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
                button.transform.GetChild(0).GetComponent<Image>().color = new Color32(0x32, 0x32, 0x32, 0xFF);
            }
            else
            {
                gameObject.SetActive(false);
                button.GetComponent<Image>().color = new Color32(0x32, 0x32, 0x32, 0xFF);
                button.transform.GetChild(0).GetComponent<Image>().color = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
            }
        }
    }

    public void LockObject()
    {
        GameObject button = EventSystem.current.currentSelectedGameObject;
        if (gameObject.name == button.transform.parent.gameObject.name)
        {
            if (!gameObject.GetComponent<MeshCollider>().enabled)
            {
                gameObject.GetComponent<MeshCollider>().enabled = true;
                button.GetComponent<Image>().color = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
                button.transform.GetChild(0).GetComponent<Image>().color = new Color32(0x32, 0x32, 0x32, 0xFF);
            }
            else
            {
                gameObject.GetComponent<MeshCollider>().enabled = false;
                button.GetComponent<Image>().color = new Color32(0x32, 0x32, 0x32, 0xFF);
                button.transform.GetChild(0).GetComponent<Image>().color = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
            }
        }
    }

    public void DeleteObject()
    {
        GameObject button = EventSystem.current.currentSelectedGameObject;
        if(gameObject.name == button.transform.parent.gameObject.name)
        {
            Destroy(button.transform.parent.gameObject);
            Destroy(gameObject);
            if(gameObject.tag == "PointObject")
            {
                if (uIManager.pointIter <= 1) uIManager.pointIter = 1;
                else uIManager.pointIter--;
            }
        }
    }
}
