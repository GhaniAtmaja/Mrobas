using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using TMPro;

public class ARCanvasManager : MonoBehaviour
{
    //side menu
    public GameObject toolsMenu;
    public GameObject registrationMenu;
    public GameObject manualRegistrationMenu;
    public GameObject qrRegistrationMenu;
    public GameObject annotationMenu;
    public GameObject objectUtilityMenu;
    public GameObject transformMenu;
    public GameObject applyTransformMenu;
    public GameObject applyColorMenu;
    public GameObject showErrorMenu;
    public GameObject showErrorRotateMenu;


    //scrollview
    public ScrollRect addPatientMenuScrollView;
    public ScrollRect itemLayerScrollView;

    //bottom menu
    public GameObject layerMenu;
    public GameObject addPatientMenu;

    //prefabs
    public GameObject patientButtonPrefab;
    public GameObject itemLayerButtonPrefab;
    public GameObject translateGizmoPrefab;
    public GameObject rotateGizmoPrefab;
    public GameObject scaleGizmoPrefab;

    //other
    public GameObject loadingScreen;
    public Transform container;
    public Shader transparentShader;
    public GameObject colorPicker;
    public GameObject joystick;

    //Debug
    public TextMeshProUGUI DebugText;
    public TextMeshProUGUI ErrorText;
    public TextMeshProUGUI ErrorRotateText;


    GameObject[] sideMenuList;
    //GameObject selectedObject;

    Vector3 prevPosition;
    Quaternion prevRotation;
    Vector3 prevScale;

    public Vector3 originalPosition;
    public Quaternion originalRotation;
    public Vector3 originalScale;


    //Color prevColor;
    //ColorPicker picker;
    //Color currentColor;
    //bool changeColor = false;
    //Vector3[] prevTransform = new Vector3[3];
    //Transform prevTransform;

    //Vector3 position;
    //float distance = 5.0f;
    //float currentDistance;
    //float desiredDistance;
    //float zoomRate = 40;
    //float zoomDampening = 5.0f;
    //float maxDistance = 1;
    //float minDistance = .03f;

    GameObject containerGizmo;
    int manualStatus = 0;
    // Start is called before the first frame update
    void Start()
    {
        sideMenuList = new GameObject[6] { toolsMenu, registrationMenu, qrRegistrationMenu, manualRegistrationMenu, applyTransformMenu, joystick };

        container = FindObjectOfType<ObjectContainer>().gameObject.transform;

        for(int i = 0; i < container.childCount; i++)
        {
            //add layer item
            GameObject obj = container.GetChild(i).gameObject;
            GameObject itemLayer = Instantiate(itemLayerButtonPrefab);
            Debug.Log(obj.name);
            itemLayer.name = obj.name;
            itemLayer.transform.SetParent(itemLayerScrollView.content);
            itemLayer.transform.GetChild(0).GetComponent<Text>().text = obj.name;
            //add event
            itemLayer.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(obj.GetComponent<ItemLayerController>().HideObject);
            itemLayer.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(obj.GetComponent<ItemLayerController>().DeleteObject);
        }

        containerGizmo = new GameObject();
        containerGizmo.name = "containerGizmo";
        containerGizmo.transform.SetParent(container);

        //set loading screen
        //loadingScreen.SetActive(true);
    }

    void Update()
    {
        //Translate Object
        float vL = joystick.transform.GetChild(0).gameObject.GetComponent<bl_Joystick>().Vertical;
        float hL = joystick.transform.GetChild(0).gameObject.GetComponent<bl_Joystick>().Horizontal;

        float vR = joystick.transform.GetChild(1).gameObject.GetComponent<bl_Joystick>().Vertical;

        if (manualStatus == 1) //Move
        {
            Vector3 translateL = (new Vector3(hL, 0, vL) * Time.deltaTime) * 0.01f / 2;
            Vector3 translateR = (new Vector3(0, vR, 0) * Time.deltaTime) * 0.01f / 2;

            container.Translate(translateL);
            container.Translate(translateR);
        }
        if (manualStatus == 2) //rotate
        {
            container.Rotate(vL * 1f * 0.1f * 2, 0, hL * -1f * 0.1f * 2);
            container.Rotate(0, vR * 1f * 0.1f * 2, 0);

        }
 
        /* if (Input.GetMouseButtonDown(0))
         {
             RaycastHit hit;
             Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
             if (Physics.Raycast(ray, out hit, 100.0f))
             {
                 if (selectedObject == null)
                 {
                     if (hit.transform.gameObject.tag == "MedicalObject")
                     {
                         Debug.Log(hit.transform.gameObject);
                         selectedObject = hit.transform.gameObject;
                         prevColor = selectedObject.GetComponent<MeshRenderer>().material.color;
                         selectedObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
                         //activate side menu
                         ShowObjectUtilityMenu();
                     }
                 }
                 *//*else
                 {
                     selectedObject.GetComponent<MeshRenderer>().material.color = Color.white;
                     ShowToolsMenu();
                     selectedObject = null;
                 }*//*

             }
         }

         if (changeColor)
         {
             //set color
             selectedObject.GetComponent<MeshRenderer>().material.color = picker.CurrentColor;
         }

         // affect the desired Zoom distance if we roll the scrollwheel
         desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
         //clamp the zoom min/max
         desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
         // For smoothing of the zoom, lerp distance
         currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

         position = Camera.main.transform.parent.gameObject.transform.position - (Camera.main.transform.forward * currentDistance);
         Camera.main.transform.position = position;*/
    }

    public void ShowLayerMenu()
    {
        if (!layerMenu.activeSelf)
        {
            layerMenu.SetActive(true);
        }
        else
        {
            layerMenu.SetActive(false);
        }
    }

    public void ShowErrorMenu()
    {
        if (!showErrorMenu.activeSelf)
        {
            showErrorMenu.SetActive(true);
        }
        else
        {
            showErrorMenu.SetActive(false);
        }
    }
    public void ShowErroRotaterMenu()
    {
        if (!showErrorRotateMenu.activeSelf)
        {
            showErrorRotateMenu.SetActive(true);
        }
        else
        {
            showErrorRotateMenu.SetActive(false);
        }
    }

    public void ShowToolsMenu()
    {
        if (!toolsMenu.activeSelf)
        {
            foreach (GameObject menu in sideMenuList)
            {
                if (menu == toolsMenu) menu.SetActive(true);
                else
                {
                    menu.SetActive(false);
                }
            }
        }
        else
        {
            toolsMenu.SetActive(false);
        }
    }

    public void ShowRegistrationMenu()
    {
        if (!registrationMenu.activeSelf)
        {
            foreach (GameObject menu in sideMenuList)
            {
                if (menu == registrationMenu) menu.SetActive(true);
                else
                {
                    menu.SetActive(false);
                }
            }
        }
        else
        {
            registrationMenu.SetActive(false);
        }
    }

    public void ShowQRRegistrationMenu()
    {
        if (!qrRegistrationMenu.activeSelf)
        {
            foreach (GameObject menu in sideMenuList)
            {
                if (menu == qrRegistrationMenu) menu.SetActive(true);
                else
                {
                    menu.SetActive(false);
                }
            }
        }
        else
        {
            qrRegistrationMenu.SetActive(false);
        }
    }

    public void ShowManualRegistrationMenu()
    {
        if (!manualRegistrationMenu.activeSelf)
        {
            foreach (GameObject menu in sideMenuList)
            {
                if (menu == manualRegistrationMenu) menu.SetActive(true);
                else
                {
                    menu.SetActive(false);
                }
            }
        }
        else
        {
            manualRegistrationMenu.SetActive(false);
        }
    }

    public void AddGizmo()
    {
        float bound = 0.05f;// objChild.GetComponent<MeshCollider>().bounds.size.x;
        //add gizmo
        //translate
        GameObject translate = Instantiate(translateGizmoPrefab);
        //translate.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
        translate.transform.localScale = new Vector3(bound, bound, bound);
        translate.transform.SetParent(containerGizmo.transform);
        translate.GetComponent<GizmoTranslateScript>().translateTarget = containerGizmo;
        translate.SetActive(false);
        //rotation
        GameObject rotation = Instantiate(rotateGizmoPrefab);
        //rotation.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        rotation.transform.localScale = new Vector3(bound, bound, bound);
        rotation.transform.SetParent(containerGizmo.transform);
        rotation.GetComponent<GizmoRotateScript>().rotateTarget = containerGizmo;
        rotation.SetActive(false);
        //scale
        GameObject scale = Instantiate(scaleGizmoPrefab);
        //scale.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
        scale.transform.localScale = new Vector3(bound, bound, bound);
        scale.transform.SetParent(containerGizmo.transform);
        scale.GetComponent<GizmoScaleScript>().scaleTarget = containerGizmo;
        scale.SetActive(false);
    }

    public void DeleteGizmo()
    {
        for (int i = 0; i < containerGizmo.transform.childCount; i++)
        {
            Destroy(containerGizmo.transform.GetChild(i).gameObject);
        }
    }

    public void ShowApplyTansformMenu()
    {
        if (!applyTransformMenu.activeSelf)
        {
            foreach (GameObject menu in sideMenuList)
            {
                if (menu == applyTransformMenu || menu==joystick) menu.SetActive(true);
                else
                {
                    menu.SetActive(false);
                }
            }
        }
        else
        {
            applyTransformMenu.SetActive(false);
            joystick.SetActive(false);
        }
    }

    public void ShowTranslateGizmo()
    {
        for (int i = 0; i < containerGizmo.transform.childCount; i++)
        {
            if (i == 0)
            {
                GameObject translate = containerGizmo.transform.GetChild(i).gameObject;
                if (!translate.activeSelf)
                {
                    translate.SetActive(true);
                    translate.GetComponent<GizmoTranslateScript>().enabled = true;
                    prevPosition = container.transform.position;
                    prevRotation = container.transform.rotation;
                    prevScale = container.transform.localScale;
                    //prevColor = selectedObject.GetComponent<MeshRenderer>().material.color;
                    manualStatus = 1;

                    ShowApplyTansformMenu();
                    //ShowJoystick();
                }
                else
                {
                    translate.SetActive(false);
                }
            }
            else
            {
                containerGizmo.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void ShowRotateGizmo()
    {
        for (int i = 0; i < containerGizmo.transform.childCount; i++)
        {
            if (i == 1)
            {
                GameObject rotate = containerGizmo.transform.GetChild(i).gameObject;
                if (!rotate.activeSelf)
                {
                    rotate.SetActive(true);
                    rotate.GetComponent<GizmoRotateScript>().enabled = true;
                    //prevTransform = selectedObject.transform;
                    prevPosition = container.transform.position;
                    prevRotation = container.transform.rotation;
                    prevScale = container.transform.localScale;
                    //prevColor = selectedObject.GetComponent<MeshRenderer>().material.color;
                    manualStatus = 2;

                    ShowApplyTansformMenu();
                    //ShowJoystick();
                }
                else
                {
                    rotate.SetActive(false);
                }
            }
            else
            {
                containerGizmo.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void ShowScaleGizmo()
    {
        for (int i = 0; i < containerGizmo.transform.childCount; i++)
        {
            if (i == 2)
            {
                GameObject scale = containerGizmo.transform.GetChild(i).gameObject;
                if (!scale.activeSelf)
                {
                    scale.SetActive(true);
                    scale.GetComponent<GizmoScaleScript>().enabled = true;
                    //prevTransform = selectedObject.transform;
                    prevPosition = container.transform.position;
                    prevRotation = container.transform.rotation;
                    prevScale = container.transform.localScale;
                    //prevColor = selectedObject.GetComponent<MeshRenderer>().material.color;
                    manualStatus = 3;

                    ShowApplyTansformMenu();
                    //ShowJoystick();
                }
                else
                {
                    scale.SetActive(false);
                }
            }
            else
            {
                containerGizmo.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void ApplyTransformGizmo()
    {
        for (int i = 0; i < containerGizmo.transform.childCount; i++)
        {
            containerGizmo.transform.GetChild(i).gameObject.SetActive(false);
        }

        ShowManualRegistrationMenu();
        //ShowJoystick();
    }

    public void CancelTransformGizmo()
    {
        container.transform.position = prevPosition;
        container.transform.rotation = prevRotation;
        container.transform.localScale = prevScale;

        for (int i = 0; i < containerGizmo.transform.childCount; i++)
        {
            containerGizmo.transform.GetChild(i).gameObject.SetActive(false);
        }

        ShowManualRegistrationMenu();
        //ShowJoystick();
    }

    public void BackOriginalTransformGizmo()
    {
        if(originalPosition != null &&
           originalRotation != null &&
           originalScale != null)
        {
            container.transform.position = originalPosition;
            container.transform.rotation = originalRotation;
            container.transform.localScale = originalScale;
        }

        for (int i = 0; i < containerGizmo.transform.childCount; i++)
        {
            containerGizmo.transform.GetChild(i).gameObject.SetActive(false);
        }

        ShowManualRegistrationMenu();
        //ShowJoystick();
    }

    public void ChangeToARScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (SceneManager.GetActiveScene().name != "ARScene") SceneManager.LoadScene("AR Scene");
    }

    public void ChangeTo3DScene()
    {
        Destroy(containerGizmo);
        if (SceneManager.GetActiveScene().name != "3DScene") SceneManager.LoadScene("3D Scene");
    }
}
