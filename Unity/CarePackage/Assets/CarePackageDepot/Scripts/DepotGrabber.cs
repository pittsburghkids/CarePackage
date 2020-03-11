using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DepotGrabber : MonoBehaviour
{
    public Animator Animator
    {
        get
        {
            return GetComponent<Animator>();
        }
    }
}
