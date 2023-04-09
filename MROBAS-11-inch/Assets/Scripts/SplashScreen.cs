using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ToSplashSreen());
    }

    IEnumerator ToSplashSreen()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("3D Scene");
    }
}
