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

    public void Stamp()
    {
        Animator.SetTrigger("Activate");
    }
}
