using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    Box box;

    public int index = 0;
    public bool autoUnpause = true;

    public void SetPackage(Box box)
    {
        this.box = box;
        box.mover = this;

        box.transform.parent = transform;
        box.transform.localPosition = Vector3.zero;
        box.transform.localRotation = Quaternion.identity;
    }

    public void Reject()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetBool("Reject", true);
    }

    // This is called by an event in the animation clip "MoverLift".
    public void MoverLiftUp()
    {
        // This will set the destination background sprite.
        CarePackageDepot.Instance.SetDestinationSprite(box.carePackageDelivery.destinationName);

        // This will trigger animations for the door and lift.
        CarePackageDepot.Instance.LiftUp();
        CarePackageDepot.Instance.OpenDoor(gameObject);


        // TODO(SJG): Pass the box to OpenDoor and set destination from there.
    }

    // This is called by an event in the animation clip "MoverLift".
    public void MoverComplete()
    {
        if (box != null)
        {
            box.Open();
        }
    }

    // When we collide with the box in front of use, we need to pause.
    void OnCollisionEnter(Collision collision)
    {
        Mover otherMover = collision.gameObject.GetComponent<Mover>();

        if (otherMover != null)
        {
            // Is the other mover in front or behind?
            float dot = Vector3.Dot(transform.right, transform.position - otherMover.transform.position);
            bool frontCollision = (dot < 0);

            Debug.LogFormat("OnCollisionEnter: Mover: {0} OtherMover: {1} FrontCollision: {2}", index, otherMover.index, frontCollision);

            if (frontCollision)
            {
                Pause();
            }
        }
    }

    // If the box in front of us move ahead, we can unpause.
    void OnCollisionExit(Collision collision)
    {
        Mover otherMover = collision.gameObject.GetComponent<Mover>();

        if (otherMover != null)
        {
            // Is the other mover in front or behind?
            float dot = Vector3.Dot(transform.right, transform.position - otherMover.transform.position);
            bool frontCollision = (dot < 0);

            Debug.LogFormat("OnCollisionExit: Mover: {0} OtherMover: {1} FrontCollision: {2}", index, otherMover.index, frontCollision);

            if (frontCollision && autoUnpause)
            {
                Unpause();
            }
        }
    }

    public void Pause()
    {
        Animator animator = GetComponent<Animator>();
        animator.speed = 0;
    }

    public void Unpause()
    {
        Animator animator = GetComponent<Animator>();
        animator.speed = 1;
    }

}