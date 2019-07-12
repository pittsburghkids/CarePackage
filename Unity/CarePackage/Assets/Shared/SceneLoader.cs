using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadSceneAsync("CarePackage", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("CarePackageMeter", LoadSceneMode.Additive);

        Debug.Log("DISPLAYS:" + Display.displays.Length);

        // if (Display.displays.Length > 1)
        // {
        Display.displays[1].Activate();
        //}
    }
}
