using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.SceneManagement;

public class ARImageTracking : MonoBehaviour
{
    private ARTrackedImageManager arTrackedImageManager;
    private ARAnchorManager arAnchorManager;
    private bool CanTrack { get; set; }

    string currentTracked;
    Vector3 currentPosition = new Vector3();
    Quaternion currentRotation = new Quaternion();
    int markerCounter = 1;

    public Transform container;
    public GameObject scanningScreen;
    public GameObject scanningButton;//mark button
    public List<ARAnchor> anchors = new List<ARAnchor>();
    public GameObject[] markers;
    //public GameObject prefab;
    public Dictionary<string, bool> markerStatus = new Dictionary<string, bool>();
    public Dictionary<string, GameObject> markerPoint = new Dictionary<string, GameObject>();
    public Text lockButtonText;
    public Image scanButton;
    public bool isScanning = false;
    public GameObject markerBorder;

    public Text tex;

    GameObject markerParent;
    GameObject newPrefab;

    bool isRegistered = false;

    //Error
    public float DistanceError1;
    public float DistanceError2;

    ARCanvasManager aRCanvasManager;
    void Awake()
    {

        arTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
        arAnchorManager = FindObjectOfType<ARAnchorManager>();
        container = FindObjectOfType<ObjectContainer>().gameObject.transform;
        aRCanvasManager = FindObjectOfType<ARCanvasManager>();

        for (int i = 1; i <= 3; i++) markerStatus.Add(i.ToString(), false);
    }

    private void OnEnable()
    {
        arTrackedImageManager.trackedImagesChanged += ImageChanged;
    }

    private void OnDisable()
    {
        arTrackedImageManager.trackedImagesChanged -= ImageChanged;
    }

    private void ImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        //if (!CanTrack) return;

        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            markerBorder.SetActive(true);
            UpdateImage(trackedImage);
        }
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            markerBorder.SetActive(true);
            UpdateImage(trackedImage);
        }
        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            markerBorder.SetActive(false);
        }
    }

    private void UpdateImage(ARTrackedImage trackedImage)
    {
        currentTracked = trackedImage.referenceImage.name;
        //Debug.Log(currentTracked);
        aRCanvasManager.DebugText.text = $"QrCode yang ke track sekarang adalah : {currentTracked}\n";

        if (isScanning)
        {
            markerBorder.transform.position = trackedImage.transform.position;
            markerBorder.transform.rotation = trackedImage.transform.rotation;

            currentPosition = trackedImage.transform.position;
            currentRotation = trackedImage.transform.rotation;
        }
    }

    //public void LockMarker()
    //{
    //    if (!markerStatus[currentTracked])
    //    {
    //        markerStatus[currentTracked] = true;
    //        lockButtonText.text = "Lock";
    //    }
    //    else
    //    {
    //        markerStatus[currentTracked] = false;
    //        lockButtonText.text = "Unlock";
    //    }
    //}

    //GameObject[] GetAllMarkerPoints()
    //{
    //    return GameObject.FindGameObjectsWithTag("Marker");
    //}

    //Based on ARExperience
    public void AllowTrack(bool isAllow)
    {
        CanTrack = isAllow;
    }

    public void AllowScannig()
    {
        if (!isScanning)
        {
            isScanning = true;
            scanningScreen.SetActive(true);
            scanningButton.SetActive(true);
        }
        else
        {
            isScanning = false;
            scanningScreen.SetActive(false);
            scanningButton.SetActive(false);
        }
    }

    public void MarkPosition()
    {
        if (markerCounter <= 3)
        {
            if(currentTracked == "marker1" && !markerStatus["1"])
            {
                newPrefab = Instantiate(markers[0], currentPosition, currentRotation);
                markerPoint.Add("1", newPrefab);
                markerStatus.Add("1", true);
            }
            else if(currentTracked == "marker2" && !markerStatus["2"])
            {
                newPrefab = Instantiate(markers[1], currentPosition, currentRotation);
                markerPoint.Add("2", newPrefab);
                markerStatus.Add("2", true);
            }
            else if(currentTracked == "marker3" && !markerStatus["3"])
            {
                newPrefab = Instantiate(markers[2], currentPosition, currentRotation);
                markerPoint.Add("3", newPrefab);
                markerStatus.Add("3", true);
            }
            newPrefab.tag = "Marker";
            markerCounter++;
        }

    }

    public void ResetMarker()
    {
        if (markerPoint.Count != 0)
        {
            int num = markerPoint.Count;
            for (int i = 1; i <= num; i++)
            {
                Destroy(markerPoint[i.ToString()]);
                markerPoint.Remove(i.ToString());
                markerStatus.Add(i.ToString(),false);

            }

            if (markerParent != null && isRegistered)
            {
                isRegistered = false;
                Destroy(markerParent);
            }
            markerCounter = 1;
        }
    }

    public void ObjectRegistration()
    {
        //aRCanvasManager.errorLog.SetActive(true);
        GameObject[] points = GameObject.FindGameObjectsWithTag("PointObject");

        if (markerParent == null)
        {
            markerParent = new GameObject();
            markerParent.name = "MarkerParent";
            //objPrefabs = Instantiate(prefab, Vector3.zero, Quaternion.identity);

            //3D Scene Marker
            markerParent.transform.position = points[0].transform.position;

            Vector3 v1 = points[0].transform.position - points[1].transform.position;
            Vector3 v2 = points[0].transform.position - points[2].transform.position;
            Vector3 up = Vector3.Cross(v1, v2);

            Quaternion rotation = Quaternion.LookRotation(v1, up);

            markerParent.transform.rotation = rotation;

            container.SetParent(markerParent.transform);

            //AR Scene Marker
            Vector3 vM1 = markerPoint["1"].transform.position - markerPoint["2"].transform.position;
            Vector3 vM2 = markerPoint["1"].transform.position - markerPoint["3"].transform.position;
            Vector3 upM = Vector3.Cross(vM1, vM2);

            Quaternion rotationM = Quaternion.LookRotation(vM1, upM);

            markerParent.transform.position = markerPoint["1"].transform.position;
            markerParent.transform.rotation = rotationM;

            //reset position,rotation,scale of registered object
            aRCanvasManager.originalPosition = container.transform.position;
            aRCanvasManager.originalRotation = container.transform.rotation;
            aRCanvasManager.originalScale = container.transform.localScale;

            isRegistered = true;

            //Error
            DistanceError1 = Vector3.Distance(markerPoint["2"].transform.position, points[1].transform.position) * 100;
            DistanceError2 = Vector3.Distance(markerPoint["3"].transform.position, points[2].transform.position) * 100;

            aRCanvasManager.ErrorText.text = "Error1: " + DistanceError1.ToString() + " cm\n" + "Error2: " + DistanceError2.ToString() + " cm";

            aRCanvasManager.ErrorRotateText.text = "X: " + (Mathf.Abs(rotation.x - rotationM.x)).ToString() + "\n" + "Y: " + (Mathf.Abs(rotation.y - rotationM.y)).ToString() + "\n" + "Z: " + (Mathf.Abs(rotation.z - rotationM.z)).ToString() + "\n";
            container.SetParent(null);
            DontDestroyOnLoad(container);

            /*//error
            float error = 0;
            for (int i = 0; i < markerPoint.Count; i++)
            {
                error += Vector3.Distance(markerPoint[(i + 1).ToString()].transform.position, objPrefabs.transform.GetChild(i + 1).gameObject.transform.position);
            }
            tex.text = ((error / 6) * 1000).ToString() + " mm";
            Debug.Log(((error / 6) * 1000).ToString() + " mm");*/
        }
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene("FidusialRegistration");
    }
}
