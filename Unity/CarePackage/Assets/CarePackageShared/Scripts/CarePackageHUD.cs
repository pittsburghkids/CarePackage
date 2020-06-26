using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CarePackageHUD : MonoBehaviour
{
    public GameObject hud;
    public TMP_Text fpsText;

    private int samples = 30;
    private int sampleCount;
    private float sampleTotal;

    void Start()
    {
        hud.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            hud.SetActive(!hud.activeSelf);
        }

        if (hud.activeSelf)
        {
            sampleTotal += Time.deltaTime;
            sampleCount++;

            if (sampleCount == samples)
            {
                float count = sampleCount / sampleTotal;
                string label = "FPS: " + (Mathf.Round(count));
                fpsText.text = label;

                sampleTotal = 0;
                sampleCount = 0;
            }

        }
    }


}
