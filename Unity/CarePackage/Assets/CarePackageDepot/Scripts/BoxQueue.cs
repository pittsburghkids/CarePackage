using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxQueue : MonoBehaviour
{
    [SerializeField] float queueDelay = 6f;

    private Queue<Mover> moverQueue = new Queue<Mover>();

    private bool boxClear = true;

    void Update()
    {
        if (boxClear && moverQueue.Count > 0)
        {
            Mover mover = moverQueue.Dequeue();
            mover.Unpause();

            boxClear = false;
        }
    }

    public void BoxClear()
    {
        boxClear = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Mover mover = other.transform.parent.GetComponent<Mover>();
        mover.autoUnpause = false;
        mover.Pause();

        moverQueue.Enqueue(mover);
    }

}
