using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    Vector3 position = Vector3.up * .24f;
    float rotationRate;
    Vector3 velocity = new Vector3(0, -.01f, 0);

    void Start()
    {
        transform.localPosition = position;
        Destroy(gameObject, 1.5f);

        rotationRate = Random.Range(-Mathf.PI, Mathf.PI);
    }

    void Update()
    {
        position += velocity;
        transform.localPosition = position;
        transform.Rotate(0, 0, rotationRate);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name.StartsWith("Flap"))
        {
            velocity = new Vector3(Random.Range(-.02f, .02f), .01f, 0);
        }

        if (collider.gameObject.name.StartsWith("Box"))
        {
            CarePackageMeter.Store(name);
        }

    }
}
