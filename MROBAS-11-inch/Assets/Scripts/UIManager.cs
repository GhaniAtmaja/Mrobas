using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using TMPro;
/*using Firebase.Extensions;
using Firebase.Storage;
using Firebase.Database;*/

public class UIManager : MonoBehaviour
{
    [Serializable]
    public class Segment
    {
        public string label;
        public string link_obj;
        public string link_icon;
    }

    [Serializable]
    public class BrainImaging
    {
        public int id;
        public Segment[] segments;
    }

    [Serializable]
    public class Patient
    {
        public string name;
        public BrainImaging[] brain_imaging;
    }

    [Serializable]
    public class Data
    {
        public Patient[] patient;
    }

    public Data data;

    //side menu
    public GameObject toolsMenu;
    public GameObject addMenu;
    public GameObject annotationMenu;
    public GameObject objectUtilityMenu;
    public GameObject transformMenu;
    public GameObject applyTransformMenu;
    public GameObject applyColorMenu;


    //scrollview
    public ScrollRect addPatientMenuScrollView;
    public ScrollRect itemLayerScrollView;
    public ScrollRect loadMenuScrollView;

    //bottom menu
    public GameObject layerMenu;
    public GameObject addPatientMenu;
    public GameObject loadMenu;
    public GameObject saveMenu;

    //prefabs
    public GameObject patientButtonPrefab;
    public GameObject itemLayerButtonPrefab;
    public GameObject loadItemButtonPrefab;
    public GameObject translateGizmoPrefab;
    public GameObject rotateGizmoPrefab;
    public GameObject scaleGizmoPrefab;

    //other
    public GameObject loadingScreen;
    public GameObject savingScreen;
    public Transform container;
    public Shader transparentShader;
    public GameObject colorPicker;
    public GameObject InformationScreen;

    GameObject[] sideMenuList;
    GameObject selectedObject;

    Vector3 prevPosition;
    Quaternion prevRotation;
    Vector3 prevScale;

    Color prevColor;
    ColorPicker picker;
    bool changeColor = false;
    //Vector3[] prevTransform = new Vector3[3];
    //Transform prevTransform;

    Vector3 position;
    //float distance = 5.0f;
    float currentDistance;
    float desiredDistance;
    float zoomRate = 1;
    float zoomDampening = 5.0f;
    float maxDistance = 1;
    float minDistance = .03f;

    public int pointIter = 1;
    public GameObject objChild;
    public Timer timer;

    //Save System
    ContainerHandler containerHandler;
    string StreamTumor;
    string StreamSkin;
    string StreamVentricle;
    GameObject SkinChild;
    GameObject TumorChild;
    GameObject VentricleChild;
    public Text saveName;
    public string[] saveFiles;

    //Coordinate Show
    [SerializeField] private TextMeshProUGUI objectCoordinateX_Text;
    [SerializeField] private TextMeshProUGUI objectCoordinateX_Placeholder;
    [SerializeField] private TextMeshProUGUI objectCoordinateY_Text;
    [SerializeField] private TextMeshProUGUI objectCoordinateY_Placeholder;
    [SerializeField] private TextMeshProUGUI objectCoordinateZ_Text;
    [SerializeField] private TextMeshProUGUI objectCoordinateZ_Placeholder;

    /*FirebaseStorage storage;
    StorageReference meshStorageReference;
    StorageReference imageStorageReference;
    FirebaseDatabase database;
    DatabaseReference databaseReference;*/

    IEnumerator Load3D(string MediaUrl, string objectName, string label)
    {
        UnityWebRequest request = UnityWebRequest.Get(MediaUrl);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            unsetLoadingScreen();

            //string to stream
            var textStream = new MemoryStream(Encoding.UTF8.GetBytes(request.downloadHandler.text));
            var obj = new OBJLoader().Load(textStream);

            objChild = obj.transform.GetChild(0).gameObject;


            objChild.transform.SetParent(container);
            objChild.name = objectName;
            objChild.AddComponent<MeshCollider>();
            objChild.layer = 6;

            if (label == "Tumor")
            {
                StreamTumor = request.downloadHandler.text;
                objChild.tag = "TumorObject";
                objChild.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));

            }
            else if (label == "Ventricle")
            {
                StreamVentricle = request.downloadHandler.text;
                objChild.tag = "VentricleObject";
                objChild.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));

            }
            else
            {
                StreamSkin = request.downloadHandler.text;
                objChild.tag = "SkinObject";
                objChild.GetComponent<MeshRenderer>().material = new Material(transparentShader);
            }

           
            //add layer item
            GameObject itemLayer = Instantiate(itemLayerButtonPrefab);
            itemLayer.name = objectName;
            itemLayer.transform.SetParent(itemLayerScrollView.content);
            itemLayer.transform.GetChild(0).GetComponent<Text>().text = objectName;

            //add event
            objChild.AddComponent<ItemLayerController>();
            itemLayer.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(objChild.GetComponent<ItemLayerController>().HideObject);
            itemLayer.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(objChild.GetComponent<ItemLayerController>().LockObject);
            itemLayer.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(objChild.GetComponent<ItemLayerController>().DeleteObject);

            float bound = objChild.GetComponent<MeshCollider>().bounds.size.x;
            //add gizmo

            //translate
            GameObject translate = Instantiate(translateGizmoPrefab);
            //translate.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            translate.transform.localScale = new Vector3(bound, bound, bound);
            translate.transform.SetParent(objChild.transform);
            translate.GetComponent<GizmoTranslateScript>().translateTarget = objChild;
            translate.SetActive(false);
            //rotation

            GameObject rotation = Instantiate(rotateGizmoPrefab);
            //rotation.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            rotation.transform.localScale = new Vector3(bound, bound, bound);
            rotation.transform.SetParent(objChild.transform);
            rotation.GetComponent<GizmoRotateScript>().rotateTarget = objChild;
            rotation.SetActive(false);
            //scale

            GameObject scale = Instantiate(scaleGizmoPrefab);
            //scale.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            scale.transform.localScale = new Vector3(bound, bound, bound);
            scale.transform.SetParent(objChild.transform);
            scale.GetComponent<GizmoScaleScript>().scaleTarget = objChild;
            scale.SetActive(false);

            /*//measure mesh volume
            float volume = VolumeOfMesh(objChild.GetComponent<MeshFilter>().mesh);
            Debug.Log((volume * 1000000).ToString() + " ml");

            //add object properties
            objChild.AddComponent<ObjectProperties>();

            //add line renderer
            LineRenderer line = objChild.AddComponent<LineRenderer>();

            //add dimesion text
            GameObject text1 = Instantiate(dimensionTextPrefab);
            text1.transform.SetParent(objChild.transform);
            GameObject text2 = Instantiate(dimensionTextPrefab);
            text2.transform.SetParent(objChild.transform);
            GameObject text3 = Instantiate(dimensionTextPrefab);
            text3.transform.SetParent(objChild.transform);

            //add object properties panel
            GameObject objectPropertiesPanel = Instantiate(objectPropertiesPanelPrefab);
            objectPropertiesPanel.transform.SetParent(objChild.transform);
            //add color picker tester
            objChild.AddComponent<ColorPickerTester>();
            objChild.GetComponent<ColorPickerTester>().picker = objectPropertiesPanel.transform.GetChild(0).GetChild(4).gameObject.GetComponent<ColorPicker>();


            //add tag
            GameObject objTag = Instantiate(objectTagPrefab);
            objTag.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = name;
            objTag.AddComponent<ObjectTag>();
            objTag.GetComponent<ObjectTag>().objName = name;
            objTag.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(objTag.GetComponent<ObjectTag>().SelectObject);
            objTag.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(objTag.GetComponent<ObjectTag>().HideObject);
            objTag.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(objTag.GetComponent<ObjectTag>().DeleteObject);
            objTag.transform.SetParent(objectTagScrollView.content);

            //set selected object
            objTag.GetComponent<ObjectTag>().SelectObject();

            //ObjectSelection objSelection = FindObjectOfType<ObjectSelection>();
            //objSelection.colorPicker.picker.CurrentColor = objChild.GetComponent<MeshRenderer>().material.color;
            //objSelection.selectedObject = objChild;
            //objSelection.colorPicker = objChild.GetComponent<ColorPickerTester>();

            //set properties
            objTag.GetComponent<ObjectTag>().ChangeObjectProperties(objChild, objectPropertiesPanel.transform.GetChild(0).gameObject);*/

            Destroy(obj);
        }
    }

    IEnumerator LoadImage(string MediaUrl, GameObject Icon)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Icon.GetComponent<RawImage>().texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }

    IEnumerator GetData(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            // error ...

        }
        else
        {
            // success...
            data = JsonUtility.FromJson<Data>(request.downloadHandler.text);

            //StartCoroutine(Load3D(data.patient[0].brain_imaging[0].segments[0].link_obj));
            //DataSnapshot patients = snapshot.Child("patients");
            //int index = 0;
            foreach (Patient patient in data.patient)
            {
                Debug.Log(patient.name);
                /*DataSnapshot models = patient.Child("model");*/
                List<string> listModels = new List<string>();

                foreach (Segment segment in patient.brain_imaging[0].segments)
                {
                    listModels.Add(segment.link_obj);

                    //add model button
                    GameObject patientButton = Instantiate(patientButtonPrefab);
                    patientButton.transform.SetParent(addPatientMenuScrollView.content);
                    patientButton.name = segment.link_obj;
                    patientButton.transform.GetChild(2).gameObject.GetComponent<Text>().text = patient.name;
                    patientButton.transform.GetChild(3).gameObject.GetComponent<Text>().text = segment.label;
                    patientButton.GetComponent<Button>().onClick.AddListener(() => Load3DObject(segment.label));

                    string refName = segment.link_icon;
                    LoadImageIcon(refName, patientButton.transform.GetChild(0).gameObject);
                }
                //listPatients.Add(patient.Key, listModels);
                //index++;
            }

            //this.transform.GetChild(1).gameObject.SetActive(false);
            unsetLoadingScreen();
        }

        // Clean up any resources it is using.
        request.Dispose();
    }
    public void Load3DObject(string label)
    {
        setLoadingScreen();

        string MediaUrl = EventSystem.current.currentSelectedGameObject.name;
        string objectName = EventSystem.current.currentSelectedGameObject.transform.GetChild(2).GetComponent<Text>().text + "_" + EventSystem.current.currentSelectedGameObject.transform.GetChild(3).GetComponent<Text>().text;
        StartCoroutine(Load3D(MediaUrl, objectName, label));
    }

    public void LoadImageIcon(string name, GameObject icon)
    {
        //Debug.Log(name);
        StartCoroutine(LoadImage(name, icon));
    }

    IEnumerator SavingFile(string StreamSkin, string StreamTumor, string StreamVentricle)
    {
        yield return new WaitForSeconds(.1f);
        containerHandler.SavePatientObject(StreamSkin, StreamTumor, StreamVentricle);

        SerializationManager.Save(saveName.text, SaveData.current);
        savingScreen.SetActive(false);
    }
    public void OnSave()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/saves/" + saveName.text + ".save") && StreamSkin != null && StreamTumor != null && StreamVentricle != null)
        {
            savingScreen.SetActive(true);
            StartCoroutine(SavingFile(StreamSkin, StreamTumor, StreamVentricle));
        }
        else
        {
            Debug.Log("File existed");
        }
    }
    IEnumerator LoadingObjects(string path)
    {
        yield return new WaitForSeconds(.1f);
        SaveData.current = (SaveData)SerializationManager.Load(path);
        ContainerData currentContainer = SaveData.current.containerData;
        SkinData currentSkinData = SaveData.current.skinData;
        TumorData currentTumorData = SaveData.current.tumorData;
        VentricleData currentVentricleData = SaveData.current.ventricleData;
        PointData[] pointData = SaveData.current.pointDatas.ToArray();

        foreach (Transform button in itemLayerScrollView.content.transform)
        {
            Destroy(button.gameObject);
        }

        if (container.gameObject != null)
        {
            Destroy(GameObject.FindWithTag("container"));
            GameObject objLoad;
            if(currentContainer.PointRefCount != 3)
            {
                objLoad = Instantiate(Resources.Load("container_only", typeof(GameObject))) as GameObject;
            }
            else
            {
                objLoad = Instantiate(Resources.Load("container_point", typeof(GameObject))) as GameObject;

                foreach (Transform child in objLoad.transform)
                {
                    //point reference
                    child.position = pointData[pointIter - 1].position;
                    pointIter++;

                    GameObject itemLayer = Instantiate(itemLayerButtonPrefab);
                    itemLayer.name = child.name;
                    itemLayer.transform.SetParent(itemLayerScrollView.content);
                    itemLayer.transform.GetChild(0).GetComponent<Text>().text = child.name;

                    itemLayer.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(child.gameObject.GetComponent<ItemLayerController>().HideObject);
                    itemLayer.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(child.gameObject.GetComponent<ItemLayerController>().LockObject);
                    itemLayer.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(child.gameObject.GetComponent<ItemLayerController>().DeleteObject);

                    float bound = child.gameObject.GetComponent<MeshCollider>().bounds.size.x;
                    //add gizmo

                    //translate
                    GameObject translate = Instantiate(translateGizmoPrefab);
                    //translate.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
                    translate.transform.localScale = new Vector3(bound, bound, bound);
                    translate.transform.SetParent(child);
                    translate.GetComponent<GizmoTranslateScript>().translateTarget = child.gameObject;
                    translate.SetActive(false);
                    //rotation

                    GameObject rotation = Instantiate(rotateGizmoPrefab);
                    //rotation.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                    rotation.transform.localScale = new Vector3(bound, bound, bound);
                    rotation.transform.SetParent(child);
                    rotation.GetComponent<GizmoRotateScript>().rotateTarget = child.gameObject;
                    rotation.SetActive(false);
                    //scale

                    GameObject scale = Instantiate(scaleGizmoPrefab);
                    //scale.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
                    scale.transform.localScale = new Vector3(bound, bound, bound);
                    scale.transform.SetParent(child);
                    scale.GetComponent<GizmoScaleScript>().scaleTarget = child.gameObject;
                    scale.SetActive(false);
                }
            }
            container = objLoad.transform;
            

            ContainerHandler containerHandler = objLoad.GetComponent<ContainerHandler>();

            containerHandler.ItemData = currentContainer;
            containerHandler.transform.position = currentContainer.position;
            containerHandler.transform.rotation = currentContainer.rotation;

            //Skin Data
            containerHandler.skinData = currentSkinData;

            var textStreamSkin = new MemoryStream(Encoding.UTF8.GetBytes(currentSkinData.StreamSkin));
            var objSkin = new OBJLoader().Load(textStreamSkin);
            SkinChild = objSkin.transform.GetChild(0).gameObject;

            SkinChild.transform.SetParent(container);
            SkinChild.name = currentSkinData.ExtraInfo;
            SkinChild.AddComponent<ItemLayerController>();
            SkinChild.AddComponent<MeshCollider>();
            SkinChild.layer = 6;

            SkinChild.tag = "SkinObject";
            SkinChild.GetComponent<MeshRenderer>().material = new Material(transparentShader);
            Debug.Log("Skin "+currentSkinData.Color);
            SkinChild.GetComponent<MeshRenderer>().material.color = currentSkinData.Color;

            GameObject itemLayerSkin = Instantiate(itemLayerButtonPrefab);
            itemLayerSkin.name = currentSkinData.ExtraInfo;
            itemLayerSkin.transform.SetParent(itemLayerScrollView.content);
            itemLayerSkin.transform.GetChild(0).GetComponent<Text>().text = currentSkinData.ExtraInfo;

            itemLayerSkin.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(SkinChild.GetComponent<ItemLayerController>().HideObject);
            itemLayerSkin.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(SkinChild.GetComponent<ItemLayerController>().LockObject);
            itemLayerSkin.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(SkinChild.GetComponent<ItemLayerController>().DeleteObject);

            float boundSkin = SkinChild.GetComponent<MeshCollider>().bounds.size.x;
            //add gizmo

            //translate
            GameObject translateSkin = Instantiate(translateGizmoPrefab);
            //translate.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            translateSkin.transform.localScale = new Vector3(boundSkin, boundSkin, boundSkin);
            translateSkin.transform.SetParent(SkinChild.transform);
            translateSkin.GetComponent<GizmoTranslateScript>().translateTarget = SkinChild;
            translateSkin.SetActive(false);
            //rotation

            GameObject rotationSkin = Instantiate(rotateGizmoPrefab);
            //rotation.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            rotationSkin.transform.localScale = new Vector3(boundSkin, boundSkin, boundSkin);
            rotationSkin.transform.SetParent(SkinChild.transform);
            rotationSkin.GetComponent<GizmoRotateScript>().rotateTarget = SkinChild;
            rotationSkin.SetActive(false);
            //scale

            GameObject scaleSkin = Instantiate(scaleGizmoPrefab);
            //scale.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            scaleSkin.transform.localScale = new Vector3(boundSkin, boundSkin, boundSkin);
            scaleSkin.transform.SetParent(SkinChild.transform);
            scaleSkin.GetComponent<GizmoScaleScript>().scaleTarget = SkinChild;
            scaleSkin.SetActive(false);

            //Tumor Data
            containerHandler.tumorData = currentTumorData;

            var textStreamTumor = new MemoryStream(Encoding.UTF8.GetBytes(currentTumorData.StreamTumor));
            var objTumor = new OBJLoader().Load(textStreamTumor);

            TumorChild = objTumor.transform.GetChild(0).gameObject;
            TumorChild.transform.SetParent(container);
            TumorChild.name = currentTumorData.ExtraInfo;
            TumorChild.AddComponent<ItemLayerController>();
            TumorChild.AddComponent<MeshCollider>();
            TumorChild.layer = 6;

            TumorChild.tag = "TumorObject";
            TumorChild.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
            Debug.Log("Tumor " + currentTumorData.Color);
            TumorChild.GetComponent<MeshRenderer>().material.color = currentTumorData.Color;

            GameObject itemLayerTumor = Instantiate(itemLayerButtonPrefab);
            itemLayerTumor.name = currentTumorData.ExtraInfo;
            itemLayerTumor.transform.SetParent(itemLayerScrollView.content);
            itemLayerTumor.transform.GetChild(0).GetComponent<Text>().text = currentTumorData.ExtraInfo;

            itemLayerTumor.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(TumorChild.GetComponent<ItemLayerController>().HideObject);
            itemLayerTumor.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(TumorChild.GetComponent<ItemLayerController>().LockObject);
            itemLayerTumor.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(TumorChild.GetComponent<ItemLayerController>().DeleteObject);

            //Ventricle Data
            containerHandler.ventricleData = currentVentricleData;

            var textStreamVentricle = new MemoryStream(Encoding.UTF8.GetBytes(currentVentricleData.StreamVentricle));
            var objVentricle = new OBJLoader().Load(textStreamVentricle);

            VentricleChild = objVentricle.transform.GetChild(0).gameObject;
            VentricleChild.transform.SetParent(container);
            VentricleChild.name = currentVentricleData.ExtraInfo;
            VentricleChild.AddComponent<ItemLayerController>();
            VentricleChild.AddComponent<MeshCollider>();
            VentricleChild.layer = 6;

            VentricleChild.tag = "VentricleObject";
            VentricleChild.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
            Debug.Log("Ventricle " + currentVentricleData.Color);
            VentricleChild.GetComponent<MeshRenderer>().material.color = currentVentricleData.Color;

            GameObject itemLayerVentricle = Instantiate(itemLayerButtonPrefab);
            itemLayerVentricle.name = currentVentricleData.ExtraInfo;
            itemLayerVentricle.transform.SetParent(itemLayerScrollView.content);
            itemLayerVentricle.transform.GetChild(0).GetComponent<Text>().text = currentVentricleData.ExtraInfo;

            itemLayerVentricle.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(VentricleChild.GetComponent<ItemLayerController>().HideObject);
            itemLayerVentricle.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(VentricleChild.GetComponent<ItemLayerController>().LockObject);
            itemLayerVentricle.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(VentricleChild.GetComponent<ItemLayerController>().DeleteObject);
            float boundVentricle = VentricleChild.GetComponent<MeshCollider>().bounds.size.x;
            //add gizmo

            //translate
            GameObject translateVentricle = Instantiate(translateGizmoPrefab);
            //translate.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            translateVentricle.transform.localScale = new Vector3(boundVentricle, boundVentricle, boundVentricle);
            translateVentricle.transform.SetParent(VentricleChild.transform);
            translateVentricle.GetComponent<GizmoTranslateScript>().translateTarget = VentricleChild;
            translateVentricle.SetActive(false);
            //rotation

            GameObject rotationVentricle = Instantiate(rotateGizmoPrefab);
            //rotation.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            rotationVentricle.transform.localScale = new Vector3(boundVentricle, boundVentricle, boundVentricle);
            rotationVentricle.transform.SetParent(VentricleChild.transform);
            rotationVentricle.GetComponent<GizmoRotateScript>().rotateTarget = VentricleChild;
            rotationVentricle.SetActive(false);
            //scale

            GameObject scaleVentricle = Instantiate(scaleGizmoPrefab);
            //scale.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            scaleVentricle.transform.localScale = new Vector3(boundVentricle, boundVentricle, boundVentricle);
            scaleVentricle.transform.SetParent(VentricleChild.transform);
            scaleVentricle.GetComponent<GizmoScaleScript>().scaleTarget = VentricleChild;
            scaleVentricle.SetActive(false);

            unsetLoadingScreen();
        }
        else
        {
            unsetLoadingScreen();
        }
    }
    public void OnLoadFile(string path)
    {
        setLoadingScreen();
        StartCoroutine(LoadingObjects(path));
        
    }
    public void GetFiles()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/saves/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves/");
        }
        saveFiles = Directory.GetFiles(Application.persistentDataPath + "/saves/");
    }
    public void DeleteFiles(string path, GameObject button)
    {
        if (File.Exists(path))
        {
            // If file found, delete it    
            File.Delete(path);
            Destroy(button);
            Debug.Log("File has been deleted");
        }
    }
    public void ShowLoadScreen()
    {
        GetFiles();

        foreach (Transform button in loadMenuScrollView.content.transform)
        {
            Destroy(button.gameObject);
        }

        for (int i = 0; i < saveFiles.Length; i++)
        {
            GameObject buttonLoadPrefab = Instantiate(loadItemButtonPrefab);
            buttonLoadPrefab.name = saveFiles[i].Replace(Application.persistentDataPath + "/saves/", ""); 
            buttonLoadPrefab.transform.SetParent(loadMenuScrollView.content);

            var index = i;
            buttonLoadPrefab.transform.GetChild(0).GetComponent<Text>().text = saveFiles[i].Replace(Application.persistentDataPath + "/saves/","");
            buttonLoadPrefab.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(()=>OnLoadFile(saveFiles[index]));
            //Delete Button
            buttonLoadPrefab.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(()=>DeleteFiles(saveFiles[index],buttonLoadPrefab));
        }
    }

    public void setLoadingScreen()
    {
        loadingScreen.SetActive(true);
        timer.startTimer = true;
    }
    public void unsetLoadingScreen()
    {
        loadingScreen.SetActive(false);
        timer.startTimer = false;
        timer.ResetTimer();
    }
    public void Awake()
    {
        container.gameObject.tag = "container";
        containerHandler = container.gameObject.GetComponent<ContainerHandler>();
        picker = colorPicker.GetComponent<ColorPicker>();
        container = FindObjectOfType<ObjectContainer>().gameObject.transform;
    }
    // Start is called before the first frame update
    void Start()
    {
        string jsonURL = "https://drive.google.com/uc?export=download&id=1H9IdgMve4k5JELTEn7O01J__5z-mcjdD";

        sideMenuList = new GameObject[7] { toolsMenu, addMenu, annotationMenu, objectUtilityMenu, transformMenu, applyTransformMenu, applyColorMenu };
        //distance = Vector3.Distance(Camera.main.transform.position, Camera.main.transform.parent.gameObject.transform.position);
        for (int i = 0; i < container.childCount; i++)
        {
            //add layer item
            GameObject obj = container.GetChild(i).gameObject;
            GameObject itemLayer = Instantiate(itemLayerButtonPrefab);
            Debug.Log("obj name" + "_" + obj.name);

            itemLayer.name = obj.name;
            itemLayer.transform.SetParent(itemLayerScrollView.content);
            itemLayer.transform.GetChild(0).GetComponent<Text>().text = obj.name;
            //add event
            itemLayer.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(obj.GetComponent<ItemLayerController>().HideObject);
            itemLayer.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(obj.GetComponent<ItemLayerController>().DeleteObject);
        }

        //set loading screen
        setLoadingScreen();

        //Get Database
        StartCoroutine(GetData(jsonURL));


        /*storage = FirebaseStorage.DefaultInstance;
        meshStorageReference = storage.GetReferenceFromUrl("gs://mrobas-ar.appspot.com/meshes");
        imageStorageReference = storage.GetReferenceFromUrl("gs://mrobas-ar.appspot.com/images");

        database = FirebaseDatabase.DefaultInstance;
        databaseReference = database.RootReference;// GetReferenceFromUrl("https://mrobas-ar-default-rtdb.firebaseio.com/patients");

        databaseReference.GetValueAsync().ContinueWithOnMainThread(task => {
            if (!task.IsFaulted && !task.IsCanceled)
            {

                DataSnapshot snapshot = task.Result;

                DataSnapshot patients = snapshot.Child("patients");
                //int index = 0;
                foreach (DataSnapshot patient in patients.Children)
                {
                    DataSnapshot models = patient.Child("model");
                    List<string> listModels = new List<string>();

                    foreach (DataSnapshot model in models.Children)
                    {
                        listModels.Add(model.Value.ToString());

                        //add model button
                        GameObject patientButton = Instantiate(patientButtonPrefab);
                        patientButton.transform.SetParent(addPatientMenuScrollView.content);
                        patientButton.name = model.Value.ToString();
                        patientButton.transform.GetChild(2).gameObject.GetComponent<Text>().text = patient.Key;
                        patientButton.transform.GetChild(3).gameObject.GetComponent<Text>().text = model.Key;
                        patientButton.GetComponent<Button>().onClick.AddListener(Load3DObject);

                        string refName = model.Value.ToString() + ".png";
                        LoadImageIcon(refName, patientButton.transform.GetChild(0).gameObject);
                    }
                    //listPatients.Add(patient.Key, listModels);
                    //index++;
                }

                //this.transform.GetChild(1).gameObject.SetActive(false);
                loadingScreen.SetActive(false);

            }
            else
            {
                Debug.Log(task.Exception);
            }
        });*/
    }

    void Update()
    {

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (selectedObject == null)
                    {
                        Debug.Log(hit.transform.gameObject);
                        selectedObject = hit.transform.gameObject;
                        prevColor = selectedObject.GetComponent<MeshRenderer>().material.color;
                        selectedObject.GetComponent<MeshRenderer>().material.color = Color.yellow;

                        objectCoordinateX_Placeholder.text = selectedObject.transform.position.x.ToString();
                        objectCoordinateY_Placeholder.text = selectedObject.transform.position.y.ToString();
                        objectCoordinateZ_Placeholder.text = selectedObject.transform.position.z.ToString();

                        //activate side menu
                        ShowObjectUtilityMenu();
                    }
                }
            }
        }

        if (changeColor)
        {
            //set color
            selectedObject.GetComponent<MeshRenderer>().material.color = picker.CurrentColor;
        }

        if (Input.touchCount == 2)//Scale
        {
            Touch touch0, touch1;
            //float distance;
            //float scaled;
            touch0 = Input.GetTouch(0);
            touch1 = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touchOnePrevPos = touch1.position - touch1.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touch0.position - touch1.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = (prevTouchDeltaMag - touchDeltaMag) * zoomRate;
            //m_text2.text = deltaMagnitudeDiff.ToString();

            if (deltaMagnitudeDiff < 0)
            {
                //placedObject.transform.localScale += new Vector3(scalingFactor, scalingFactor, scalingFactor);

                //zoom
                // affect the desired Zoom distance if we roll the scrollwheel
                desiredDistance -= Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
                //clamp the zoom min/max
                desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
                // For smoothing of the zoom, lerp distance
                currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

                position = Camera.main.transform.parent.gameObject.transform.position - (Camera.main.transform.forward * currentDistance);
                Camera.main.transform.position = position;
            }
            else if (deltaMagnitudeDiff > 0)
            {
                //placedObject.transform.localScale -= new Vector3(scalingFactor, scalingFactor, scalingFactor);

                //zoom
                // affect the desired Zoom distance if we roll the scrollwheel
                desiredDistance += Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
                //clamp the zoom min/max
                desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
                // For smoothing of the zoom, lerp distance
                currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

                position = Camera.main.transform.parent.gameObject.transform.position - (Camera.main.transform.forward * currentDistance);
                Camera.main.transform.position = position;
            }
        }

        /*//zoom
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

    public void ShowLoadMenu()
    {

        if (!loadMenu.activeSelf)
        {
            ShowLoadScreen();
            loadMenu.SetActive(true);
        }
        else
        {
            loadMenu.SetActive(false);
        }
    }

    public void ShowSaveMenu()
    {

        if (!saveMenu.activeSelf)
        {
            saveMenu.SetActive(true);
        }
        else
        {
            saveMenu.SetActive(false);
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

    public void ShowInfoScreen()
    {
        if (!InformationScreen.activeSelf)
        {
            InformationScreen.SetActive(true);
        }
        else
        {
            InformationScreen.SetActive(false);
        }
    }

    public void ShowAddMenu()
    {
        if (!addMenu.activeSelf)
        {
            foreach (GameObject menu in sideMenuList)
            {
                if (menu == addMenu) menu.SetActive(true);
                else
                {
                    menu.SetActive(false);
                }
            }
        }
        else
        {
            addMenu.SetActive(false);
        }
    }

    public void ShowAnnotationMenu()
    {
        if (!annotationMenu.activeSelf)
        {
            foreach (GameObject menu in sideMenuList)
            {
                if (menu == annotationMenu) menu.SetActive(true);
                else
                {
                    menu.SetActive(false);
                }
            }
        }
        else
        {
            annotationMenu.SetActive(false);
        }
    }

    public void ApplyCoordinate()
    {
        if(selectedObject != null)
        {
            float x_coor, y_coor, z_coor;
            //hilangin index terakhir supaya bisa di parsing stringnya
            string x = objectCoordinateX_Text.text.Remove(objectCoordinateX_Text.text.Length - 1);
            string y = objectCoordinateY_Text.text.Remove(objectCoordinateY_Text.text.Length - 1);
            string z = objectCoordinateZ_Text.text.Remove(objectCoordinateZ_Text.text.Length - 1);


            bool xparse = float.TryParse(x, out x_coor);
            bool yparse = float.TryParse(y, out y_coor);
            bool zparse = float.TryParse(z, out z_coor);

            if(xparse && yparse && zparse)
            {
                selectedObject.transform.position = new Vector3(x_coor, y_coor, z_coor);
            }
            else
            {
                Debug.Log("Coordinate is not float data type");
            }

            Debug.Log(xparse);
            Debug.Log(yparse);
            Debug.Log(zparse);


        }
        else
        {
            Debug.Log("No selected object");
        }

    }

    public void ShowObjectUtilityMenu()
    {
        if (!objectUtilityMenu.activeSelf)
        {
            foreach (GameObject menu in sideMenuList)
            {
                if (menu == objectUtilityMenu)
                {
                    menu.SetActive(true);
                    //deslect
                    selectedObject.transform.GetChild(0).gameObject.SetActive(false);
                    selectedObject.transform.GetChild(1).gameObject.SetActive(false);
                    selectedObject.transform.GetChild(2).gameObject.SetActive(false);
                }
                else
                {
                    menu.SetActive(false);
                }
            }
        }
        else
        {
            objectUtilityMenu.SetActive(false);
        }
    }

    public void ShowTransformMenu()
    {
        if (!transformMenu.activeSelf)
        {
            foreach (GameObject menu in sideMenuList)
            {
                if (menu == transformMenu) menu.SetActive(true);
                else
                {
                    menu.SetActive(false);
                }
            }
        }
        else
        {
            transformMenu.SetActive(false);
        }
    }
    public void ShowApplyTansformMenu()
    {
        if (!applyTransformMenu.activeSelf)
        {
            foreach (GameObject menu in sideMenuList)
            {
                if (menu == applyTransformMenu) menu.SetActive(true);
                else
                {
                    menu.SetActive(false);
                }
            }
        }
        else
        {
            applyTransformMenu.SetActive(false);
        }
    }

    public void ShowApplyColorMenu()
    {
        if (!applyColorMenu.activeSelf)
        {
            foreach (GameObject menu in sideMenuList)
            {
                if (menu == applyColorMenu) menu.SetActive(true);
                else
                {
                    menu.SetActive(false);
                }
            }
        }
        else
        {
            applyColorMenu.SetActive(false);
        }
    }

    public void DeselectObject()
    {
        if(selectedObject != null)
        {
            selectedObject.GetComponent<MeshRenderer>().material.color = prevColor;
            ShowToolsMenu();

            //deslect
            selectedObject.transform.GetChild(0).gameObject.SetActive(false);
            selectedObject.transform.GetChild(1).gameObject.SetActive(false);
            selectedObject.transform.GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            ShowToolsMenu();
        }

        objectCoordinateX_Placeholder.text = "Select Object to show";
        objectCoordinateY_Placeholder.text = "Select Object to show";
        objectCoordinateZ_Placeholder.text = "Select Object to show";

        selectedObject = null;
    }

    public void ShowTranslateGizmo()
    {
        for (int i = 0; i < selectedObject.transform.childCount; i++)
        {
            if (i == 0)
            {
                GameObject translate = selectedObject.transform.GetChild(i).gameObject;
                if (!translate.activeSelf)
                {
                    translate.SetActive(true);
                    translate.GetComponent<GizmoTranslateScript>().enabled = true;
                    prevPosition = selectedObject.transform.position;
                    prevRotation = selectedObject.transform.rotation;
                    prevScale = selectedObject.transform.localScale;
                    //prevColor = selectedObject.GetComponent<MeshRenderer>().material.color;

                    ShowApplyTansformMenu();
                }
                else
                {
                    translate.SetActive(false);
                }
            }
            else
            {
                selectedObject.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void ShowRotateGizmo()
    {
        for (int i = 0; i < selectedObject.transform.childCount; i++)
        {
            if (i == 1)
            {
                GameObject rotate = selectedObject.transform.GetChild(i).gameObject;
                if (!rotate.activeSelf)
                {
                    rotate.SetActive(true);
                    rotate.GetComponent<GizmoRotateScript>().enabled = true;
                    //prevTransform = selectedObject.transform;
                    prevPosition = selectedObject.transform.position;
                    prevRotation = selectedObject.transform.rotation;
                    prevScale = selectedObject.transform.localScale;
                    //prevColor = selectedObject.GetComponent<MeshRenderer>().material.color;

                    ShowApplyTansformMenu();
                }
                else
                {
                    rotate.SetActive(false);
                }
            }
            else
            {
                selectedObject.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void ShowScaleGizmo()
    {
        for (int i = 0; i < selectedObject.transform.childCount; i++)
        {
            if (i == 2)
            {
                GameObject scale = selectedObject.transform.GetChild(i).gameObject;
                if (!scale.activeSelf)
                {
                    scale.SetActive(true);
                    scale.GetComponent<GizmoScaleScript>().enabled = true;
                    //prevTransform = selectedObject.transform;
                    prevPosition = selectedObject.transform.position;
                    prevRotation = selectedObject.transform.rotation;
                    prevScale = selectedObject.transform.localScale;
                    //prevColor = selectedObject.GetComponent<MeshRenderer>().material.color;

                    ShowApplyTansformMenu();
                }
                else
                {
                    scale.SetActive(false);
                }
            }
            else
            {
                selectedObject.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void ShowColorPicker()
    {
        changeColor = true;

        colorPicker.gameObject.SetActive(true);

        ShowApplyColorMenu();
    }

    public void ApplyTransformGizmo()
    {
        for (int i = 0; i < selectedObject.transform.childCount; i++)
        {
            selectedObject.transform.GetChild(i).gameObject.SetActive(false);
        }
        ShowTransformMenu();
    }

    public void CancelTransformGizmo()
    {
        selectedObject.transform.position = prevPosition;
        selectedObject.transform.rotation = prevRotation;
        selectedObject.transform.localScale = prevScale;

        for (int i = 0; i < selectedObject.transform.childCount; i++)
        {
            selectedObject.transform.GetChild(i).gameObject.SetActive(false);
        }

        ShowTransformMenu();
    }

    public void ApplyColor()
    {
        selectedObject.GetComponent<MeshRenderer>().material.color = picker.CurrentColor;
        prevColor = picker.CurrentColor;

        changeColor = false;
        colorPicker.gameObject.SetActive(false);
        ShowObjectUtilityMenu();
    }

    public void CancelColor()
    {
        selectedObject.GetComponent<MeshRenderer>().material.color = prevColor;

        changeColor = false;
        colorPicker.gameObject.SetActive(false);
        ShowObjectUtilityMenu();
    }

    public void ShowAddPatientMenu()
    {
        if (!addPatientMenu.activeSelf)
        {
            addPatientMenu.SetActive(true);
        }
        else
        {
            addPatientMenu.SetActive(false);
        }
    }

    public void CreateLineAnnotation()
    {
        GameObject Line = new GameObject();
        Line.name = "Line";
    }

    public void CreatePointAnnotation()
    {
        if(pointIter >= 1 && pointIter < 4)
        {
            GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            point.name = "Point " + pointIter.ToString();
            point.tag = "PointObject";
            point.transform.SetParent(container);
            point.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            Destroy(point.GetComponent<SphereCollider>());
            point.AddComponent<MeshCollider>();
            //add layer item
            GameObject itemLayer = Instantiate(itemLayerButtonPrefab);
            itemLayer.name = "Point " + pointIter.ToString();
            itemLayer.transform.SetParent(itemLayerScrollView.content);
            itemLayer.transform.GetChild(0).GetComponent<Text>().text = "Point " + pointIter.ToString();
            //add event
            point.AddComponent<ItemLayerController>();
            itemLayer.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(point.GetComponent<ItemLayerController>().HideObject);
            itemLayer.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(point.GetComponent<ItemLayerController>().LockObject);
            itemLayer.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(point.GetComponent<ItemLayerController>().DeleteObject);

            float bound = point.GetComponent<MeshCollider>().bounds.size.x;
            Debug.Log(bound);
            //add gizmo
            //translate
            GameObject translate = Instantiate(translateGizmoPrefab);
            //translate.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            translate.transform.localScale = new Vector3(bound, bound, bound);
            translate.transform.SetParent(point.transform);
            translate.GetComponent<GizmoTranslateScript>().translateTarget = point;
            translate.SetActive(false);
            //rotation
            GameObject rotation = Instantiate(rotateGizmoPrefab);
            //rotation.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            rotation.transform.localScale = new Vector3(bound * 2, bound * 2, bound * 2);
            rotation.transform.SetParent(point.transform);
            rotation.GetComponent<GizmoRotateScript>().rotateTarget = point;
            rotation.SetActive(false);
            //scale
            GameObject scale = Instantiate(scaleGizmoPrefab);
            //scale.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
            scale.transform.localScale = new Vector3(bound, bound, bound);
            scale.transform.SetParent(point.transform);
            scale.GetComponent<GizmoScaleScript>().scaleTarget = point;
            scale.SetActive(false);

            pointIter++;
        }
        
    }

    public void ChangeToARScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (SceneManager.GetActiveScene().name != "ARScene") SceneManager.LoadScene("AR Scene");
    }

    public void ChangeTo3DScene()
    {
        if (SceneManager.GetActiveScene().name != "3DScene") SceneManager.LoadScene("3D Scene");
    }
}