using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Belt : MonoBehaviour
{
    [SerializeField] float speed = -.1f;
    private float offset = 0;

    // Update is called once per frame
    void Update()
    {
        offset = (offset + (Time.deltaTime * speed)) % 1.0f;
        GetComponent<MeshRenderer>().material.SetTextureOffset("_BaseMap", Vector2.up * offset);
    }
}
