using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    void Start()
    {
        // Load depot scene if needed.
        if (!SceneManager.GetSceneByName("CarePackageDepot").isLoaded)
        {
            SceneManager.LoadSceneAsync("CarePackageDepot", LoadSceneMode.Additive);
        }

        // Load meter scene if needed.
        if (!SceneManager.GetSceneByName("CarePackageMeter").isLoaded)
        {
            SceneManager.LoadSceneAsync("CarePackageMeter", LoadSceneMode.Additive);
        }

        // Configure external displays.
        Debug.Log("DISPLAYS:" + Display.displays.Length);

        // // if (Display.displays.Length > 1)
        // // {
        // Display.displays[1].Activate();
        // //}
    }
}
