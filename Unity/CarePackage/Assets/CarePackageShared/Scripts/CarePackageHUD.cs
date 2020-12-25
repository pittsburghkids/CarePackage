using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CarePackageHUD : MonoBehaviour
{

    private int samples = 30;
    private int sampleCount;
    private float sampleTotal;

    private float fps;
    private bool hudActive = true;

    Matrix4x4 matrix;
    Rect windowRect = new Rect(20, 20, 120, 50);

    void Start()
    {
        hudActive = false;
        matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(2, 2, 1.0f));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            hudActive = !hudActive;
        }

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
    }

}
