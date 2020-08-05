using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CarePackageUtils : MonoBehaviour
{

    [MenuItem("CarePackage/Capture Screenshot")]
    static void CaptureScreenshot()
    {
        string date = System.DateTime.Now.ToString("yyyyMMdd-hhmmss");
        string filename = "screenshot-" + date + ".png";
        Debug.Log("Capturing screenshot: " + filename);

        ScreenCapture.CaptureScreenshot(filename);
    }

}
