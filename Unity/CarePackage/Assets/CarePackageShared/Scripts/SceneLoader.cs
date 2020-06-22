using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public bool loadDepot;
    public bool loadLoader;

    void Start()
    {
        // Show development console.
        Debug.LogError("Force the developer console to appear.");

        // Process command line arguments.
        string[] commandLineArguments = System.Environment.GetCommandLineArgs();

        Debug.Log("Command line argument count: " + commandLineArguments.Length);

        for (int i = 0; i < commandLineArguments.Length; i++)
        {
            string commandLineArgument = commandLineArguments[i];
            Debug.Log("Command line argument: " + commandLineArgument);

            if (commandLineArguments[i] == "-depot")
            {
                LoadDepotScene();
                return;
            }

            if (commandLineArguments[i] == "-loader")
            {
                LoadLoaderScene();
                return;
            }
        }

        // If no args were passed, use class bools.
        if (loadDepot)
        {
            LoadDepotScene();
            return;
        }
        if (loadLoader)
        {
            LoadLoaderScene();
            return;
        }

        // Default scene to load with no args.
        LoadDepotScene();
    }

    void LoadDepotScene()
    {
        // Load depot scene if needed.
        if (!SceneManager.GetSceneByName("CarePackageDepot").isLoaded)
        {
            SceneManager.LoadSceneAsync("CarePackageDepot", LoadSceneMode.Additive);
        }
    }

    void LoadLoaderScene()
    {
        // Load meter scene if needed.
        if (!SceneManager.GetSceneByName("CarePackageLoader").isLoaded)
        {
            SceneManager.LoadSceneAsync("CarePackageLoader", LoadSceneMode.Additive);
            SceneManager.LoadSceneAsync("CarePackageLoader", LoadSceneMode.Additive);
        }

    }
}
