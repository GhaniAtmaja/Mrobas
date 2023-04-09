using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectContainer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int numMusicPlayers = FindObjectsOfType<ObjectContainer>().Length;
        if (numMusicPlayers > 2)
        {
            Destroy(this.gameObject);
        }
        // if more then one music player is in the scene
        //destroy ourselves
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
