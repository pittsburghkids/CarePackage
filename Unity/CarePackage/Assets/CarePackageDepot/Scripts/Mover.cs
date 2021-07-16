using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    public AnimatorBridge animatorBridge;

    Box box;

    public int index = 0;
    public bool autoUnpause = true;

    public void SetPackage(Box box)
    {
        this.box = box;
        box.mover = this;
        box.Initialize();

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
            bool forwardCollision = ForwardCollision(otherMover);
            bool downCollision = DownCollision(otherMover);

            Debug.LogFormat("OnCollisionEnter: Mover: {0} OtherMover: {1} ForwardCollision: {2} DownCollision: {3}",
                index, otherMover.index, forwardCollision, downCollision);

            if (forwardCollision || downCollision)
            {
                Debug.Log("Collision Pause: " + gameObject.name);
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
            // Is the other mover in front or below?
            bool forwardCollision = ForwardCollision(otherMover);
            bool downCollision = DownCollision(otherMover);

            Debug.LogFormat("OnCollisionExit: Mover: {0} OtherMover: {1} ForwardCollision: {2} DownCollision: {3}",
                index, otherMover.index, forwardCollision, downCollision);

            if ((forwardCollision || downCollision) && autoUnpause)
            {
                Unpause();
            }
        }
    }

    private bool ForwardCollision(Mover otherMover)
    {
        Vector3 floorPosition = transform.position;
        Vector3 otherFloorPosition = otherMover.transform.position;
        floorPosition.y = otherFloorPosition.y = 0;

        Vector3 collisionVector = (floorPosition - otherFloorPosition).normalized;

        float dot = Vector3.Dot(-transform.right, collisionVector);
        bool forwardCollision = (dot > .1f);

        if (forwardCollision)
        {
            Debug.DrawRay(transform.position, -transform.right, Color.red, 5);
            Debug.DrawRay(transform.position, collisionVector, Color.green, 5);
        }

        return forwardCollision;
    }

    private bool DownCollision(Mover otherMover)
    {
        bool downCollision = transform.position.y - otherMover.transform.position.y > .1f;
        return downCollision;
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