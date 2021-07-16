using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxQueue : MonoBehaviour
{
    public bool ClearBoxes { get; set; } = true;

    [SerializeField] private bool autoClear = false;
    private Queue<Mover> moverQueue = new Queue<Mover>();

    void Update()
    {
        if (ClearBoxes && moverQueue.Count > 0)
        {
            Mover mover = moverQueue.Dequeue();

            Debug.Log("Unpausing Mover: " + mover.gameObject.name);

            mover.Unpause();
            mover.autoUnpause = true;

            ClearBoxes = false;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        Box box = collider.gameObject.GetComponent<Box>();
        Mover mover = box.mover;

        mover.autoUnpause = false;

        Debug.Log("Pausing Mover: " + mover.gameObject.name);
        mover.Pause();

        moverQueue.Enqueue(mover);
    }

    private void OnTriggerExit(Collider other)
    {
        if (autoClear)
        {
            ClearBoxes = true;
        }
    }

}
