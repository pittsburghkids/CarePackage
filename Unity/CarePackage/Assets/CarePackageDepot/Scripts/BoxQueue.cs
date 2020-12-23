using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxQueue : MonoBehaviour
{
    public bool autoClear = false;

    private Queue<Mover> moverQueue = new Queue<Mover>();
    private bool boxClear = true;

    void Update()
    {
        if (boxClear && moverQueue.Count > 0)
        {
            Mover mover = moverQueue.Dequeue();

            mover.Unpause();
            mover.autoUnpause = true;

            boxClear = false;
        }
    }

    public void BoxClear()
    {
        boxClear = true;
    }

    private void OnTriggerEnter(Collider collider)
    {

        Box box = collider.gameObject.GetComponent<Box>();
        Mover mover = box.Mover;

        mover.autoUnpause = false;
        mover.Pause();

        moverQueue.Enqueue(mover);
    }

    private void OnTriggerExit(Collider other)
    {
        if (autoClear)
        {
            BoxClear();
        }
    }

}
