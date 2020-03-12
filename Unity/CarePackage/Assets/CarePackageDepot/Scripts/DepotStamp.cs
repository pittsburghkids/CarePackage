using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DepotStamp : MonoBehaviour
{
    public Animator Animator
    {
        get
        {
            return GetComponent<Animator>();
        }
    }

    private Box lastBox;

    public void Stamp(Collider collider)
    {
        Animator.SetTrigger("Activate");

        Box box = collider.gameObject.GetComponent<Box>();
        lastBox = box;
    }

    public void ApplyStamp()
    {
        lastBox?.ShowDecal();
    }
}
