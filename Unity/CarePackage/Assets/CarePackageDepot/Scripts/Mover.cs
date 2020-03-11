using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    GameObject package;

    public int index = 0;
    public bool autoUnpause = true;

    public void SetPackage(GameObject package)
    {
        this.package = package;

        package.transform.parent = transform;
        package.transform.localPosition = Vector3.zero;
        package.transform.localRotation = Quaternion.identity;
    }

    public void MoverLiftUp()
    {
        CarePackageDepot.Instance.LiftUp();
        CarePackageDepot.Instance.OpenDoor();
    }

    public void MoverComplete()
    {
        StartCoroutine(MoverCompleteRoutine());
    }

    private IEnumerator MoverCompleteRoutine()
    {
        if (package != null)
        {
            package.GetComponent<Animator>().SetTrigger("Open");
        }

        yield return new WaitForSeconds(2.5f);

        CarePackageDepot.Instance.CloseDoor();

        yield return new WaitForSeconds(1);

        Destroy(gameObject);
        Destroy(package);
    }

    void OnCollisionEnter(Collision collision)
    {
        Mover otherMover = collision.gameObject.GetComponent<Mover>();

        if (otherMover != null)
        {
            if (otherMover.index < index)
            {
                Debug.Log(collision.gameObject.name);
                Pause();
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        Mover otherMover = collision.gameObject.GetComponent<Mover>();

        if (otherMover != null)
        {
            if (otherMover.index < index && autoUnpause)
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