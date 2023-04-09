using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;

public class Progress : MonoBehaviour
{
    [SerializeField] private VideoPlayer video;
    Slider tracking;
    // Start is called before the first frame update
    void Start()
    {
        tracking = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        tracking.value = (float)video.frame / (float)video.frameCount;
    }
}
