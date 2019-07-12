using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Letter : MonoBehaviour
{
    public void SetSortingLayer(int layer)
    {

        SortingGroup sortingGroup = GetComponent<SortingGroup>();

        if (sortingGroup)
        {
            sortingGroup.sortingOrder = layer;
        }

    }

    public void Remove()
    {
        Destroy(gameObject);
    }
}
