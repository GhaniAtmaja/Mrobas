using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

public class ARExperienceManager : MonoBehaviour
{
    [SerializeField]
    private UnityEvent OnInitialized;

    [SerializeField]
    private UnityEvent OnRestarted;

    private ARPlaneManager arPlaneManager;

    private bool Initialized { get; set; }

    void Awake()
    {
        arPlaneManager = GetComponent<ARPlaneManager>();
        arPlaneManager.planesChanged += PlanesChanged;
    }

    void PlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (!Initialized)
        {
            Activate();
        }
    }

    private void Activate()
    {
        OnInitialized?.Invoke();
        Initialized = true;
        //arPlaneManager.enabled = false;
    }

    public void Restart()
    {
        OnRestarted.Invoke();
        Initialized = false;
        arPlaneManager.enabled = true;
    }
}
