using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    Vector3 position = Vector3.up * .24f;
    float rotationRate;
    Vector3 velocity = new Vector3(0, -.3f, 0);

    void Start()
    {
        transform.localPosition = position;
        Destroy(gameObject, 2f);

        rotationRate = Random.Range(-Mathf.PI / 2, Mathf.PI / 2);
    }

    void Update()
    {
        position += velocity * Time.deltaTime;
        transform.localPosition = position;
        transform.Rotate(0, 0, rotationRate);
    }

    void OnTriggerEnter(Collider collider)
    {
        velocity = new Vector3(Random.Range(-.5f, .5f), .5f, 0);
    }
}
