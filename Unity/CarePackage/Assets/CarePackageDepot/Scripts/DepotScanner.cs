using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DepotScanner : MonoBehaviour
{
    public GameObject scannerOverlay;

    public Animator Animator
    {
        get
        {
            return GetComponent<Animator>();
        }
    }

    public void Scan()
    {
        StartCoroutine(ScanRoutine());
    }

    private IEnumerator ScanRoutine()
    {
        scannerOverlay.SetActive(true);
        yield return new WaitForSeconds(1);
        scannerOverlay.SetActive(false);
    }
}
