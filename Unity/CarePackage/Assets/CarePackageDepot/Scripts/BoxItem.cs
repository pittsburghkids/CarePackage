using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxItem : MonoBehaviour
{
    public float lifeTime = 1f;

    private float startTime;

    private Vector3 velocity;
    private float gravity = 4;
    private float drag = .2f;

    void Start()
    {
        velocity = ((Vector3.up * Random.Range(1.5f, 2.5f)) + (Vector3.right * Random.Range(-1f, 1f)));
        startTime = Time.time;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        float t = (Time.time - startTime) / lifeTime;
        transform.localScale = Vector3.one * (1.0f - t);

        velocity -= (Vector3.up * gravity) * Time.deltaTime;
        velocity -= velocity * drag * Time.deltaTime;

        transform.position += velocity * Time.deltaTime;
    }
}
