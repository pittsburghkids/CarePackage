using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Package : MonoBehaviour
{
    [SerializeField] GameObject heartParticles;

    public void Emit()
    {
        //heartParticles.SetActive(true);
        Destroy(transform.parent.gameObject, 8);

        List<string> itemNames = CarePackage.Instance.GetItemsInBox(transform.parent.gameObject.name);

        foreach (string itemName in itemNames)
        {
            Debug.Log(itemName);
        }

        CarePackage.Instance.EmptyBox(transform.parent.gameObject.name);
    }
}
