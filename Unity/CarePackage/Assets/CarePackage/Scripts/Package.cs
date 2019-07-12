using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Package : MonoBehaviour
{
    [SerializeField] GameObject heartParticles;

    public void Emit()
    {
        heartParticles.SetActive(true);
        Destroy(transform.parent.gameObject, 8);
    }
}
