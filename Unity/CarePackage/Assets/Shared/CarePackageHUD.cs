using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CarePackageHUD : MonoBehaviour
{
    public TMP_Text fpsText;

    IEnumerator Start()
    {
        while (true)
        {
            float count = (1 / Time.deltaTime);
            string label = "FPS: " + (Mathf.Round(count));
            fpsText.text = label;

            yield return new WaitForSeconds(0.5f);
        }
    }

}
