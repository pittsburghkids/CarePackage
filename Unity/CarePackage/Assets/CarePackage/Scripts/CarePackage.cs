using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarePackage : MonoBehaviour
{

    [SerializeField] GameObject packagePrefab;

    public void Delivery(string message)
    {
        if (message.StartsWith("1794"))
        {
            Instantiate(packagePrefab);
        }
    }
}
