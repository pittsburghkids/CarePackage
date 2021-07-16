using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CarePackageHUD : MonoBehaviour
{

    private int samples = 30;
    private int sampleCount;
    private float sampleTotal;

    private float fps;
    private bool hudActive = false;

    Matrix4x4 matrix;
    Rect windowRect = new Rect(20, 20, 120, 50);

    void Start()
    {

#if UNITY_EDITOR
        hudActive = true;
#endif

        matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(2, 2, 1.0f));
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            hudActive = !hudActive;
        }

        //

        if (hudActive)
        {
            sampleTotal += Time.deltaTime;
            sampleCount++;

            if (sampleCount == samples)
            {
                float count = sampleCount / sampleTotal;
                fps = Mathf.Round(count);

                sampleTotal = 0;
                sampleCount = 0;
            }
        }
    }

    void OnGUI()
    {
        if (hudActive)
        {
            GUI.matrix = matrix;
            windowRect = GUILayout.Window(0, windowRect, DrawWindow, "HUD");
        }
    }

    void DrawWindow(int windowID)
    {
        GUILayout.Label("FPS: " + fps);

        if (GUILayout.Button("Loader Add Box"))
        {
            CarePackage.Instance.OnWebSocketReceived("{\"type\":\"tag.found\",\"boardName\":\"LoaderBoxA\",\"boxName\":\"BoxA\"}");
            CarePackage.Instance.OnWebSocketReceived("{\"type\":\"tag.found\",\"boardName\":\"LoaderBoxB\",\"boxName\":\"BoxB\"}");
        }

        if (GUILayout.Button("Loader Remove Box"))
        {
            CarePackage.Instance.OnWebSocketReceived("{\"type\":\"tag.removed\",\"boardName\":\"LoaderBoxA\",\"boxName\":\"BoxA\"}");
            CarePackage.Instance.OnWebSocketReceived("{\"type\":\"tag.removed\",\"boardName\":\"LoaderBoxB\",\"boxName\":\"BoxB\"}");
        }

        if (GUILayout.Button("Loader Insert Item"))
        {
            CarePackage.Instance.OnWebSocketReceived("{\"type\":\"tag.found\",\"boardName\":\"LoaderItemA\",\"itemName\":\"Basketball\"}");
        }

        if (GUILayout.Button("Loader Address Wheel"))
        {
            CarePackage.Instance.OnWebSocketReceived("{\"type\":\"encoder.change\",\"boardName\":\"LoaderAddressA\",\"destinationName\":\"House\"}");
            CarePackage.Instance.OnWebSocketReceived("{\"type\":\"encoder.change\",\"boardName\":\"LoaderAddressB\",\"destinationName\":\"House\"}");
        }

        if (GUILayout.Button("Depot Set Next [A]ddress") || Input.GetKeyDown(KeyCode.A))
        {
            CarePackage.Instance.OnWebSocketReceived("{\"type\":\"loader.address\",\"destinationName\":\"House\",\"boxName\":\"BoxA\"}");
        }

        if (GUILayout.Button("Depot Set Next [I]tems") || Input.GetKeyDown(KeyCode.I))
        {
            CarePackage.Instance.OnWebSocketReceived("{\"type\":\"loader.insert\",\"itemName\":\"Basketball\",\"boxName\":\"BoxA\"}");
            CarePackage.Instance.OnWebSocketReceived("{\"type\":\"loader.insert\",\"itemName\":\"FourLeafClover\",\"boxName\":\"BoxA\"}");
            CarePackage.Instance.OnWebSocketReceived("{\"type\":\"loader.insert\",\"itemName\":\"HeartDecoration\",\"boxName\":\"BoxA\"}");
            CarePackage.Instance.OnWebSocketReceived("{\"type\":\"loader.insert\",\"itemName\":\"Kite\",\"boxName\":\"BoxA\"}");
        }

        if (GUILayout.Button("Depot Insert [T]op Box") || Input.GetKeyDown(KeyCode.T))
        {
            CarePackage.Instance.OnWebSocketReceived("{\"type\":\"tag.found\",\"boardName\":\"DepotBoxB\",\"boxName\":\"BoxA\"}");
        }

        if (GUILayout.Button("Depot Insert [S]ide Box") || Input.GetKeyDown(KeyCode.S))
        {
            CarePackage.Instance.OnWebSocketReceived("{\"type\":\"tag.found\",\"boardName\":\"DepotBoxA\",\"boxName\":\"BoxA\"}");
        }

        if (GUILayout.Button("[R]eload Scene") || Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

    }
}