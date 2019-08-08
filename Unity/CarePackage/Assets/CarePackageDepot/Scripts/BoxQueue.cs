using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxQueue : MonoBehaviour
{
    [SerializeField] float queueDelay = 4.5f;

    private Queue<Animator> moverQueue = new Queue<Animator>();
    private float lastUnqueue = -1;

    void Update()
    {

        if (Time.time - lastUnqueue > queueDelay && moverQueue.Count > 0)
        {
            Animator animator = moverQueue.Dequeue();
            animator.speed = 1;
            lastUnqueue = Time.time;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);

        Animator animator = other.transform.parent.parent.GetComponent<Animator>();
        animator.speed = 0;
        moverQueue.Enqueue(animator);
    }

}
