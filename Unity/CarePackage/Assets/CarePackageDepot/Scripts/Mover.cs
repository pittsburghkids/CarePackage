using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    GameObject package;

    public void GetPackage(GameObject package)
    {
        this.package = package;

        package.transform.parent = transform;
        package.transform.localPosition = Vector3.zero;
        package.transform.localRotation = Quaternion.identity;
        package.gameObject.GetComponent<Rigidbody>().isKinematic = true;

    }

    public void MoverDoor()
    {
        CarePackageDepot.Instance.OpenDoor();
    }

    public void MoverComplete()
    {
        StartCoroutine(MoverCompleteRoutine());
    }

    private IEnumerator MoverCompleteRoutine()
    {
        package.GetComponent<Animator>().SetTrigger("Open");

        yield return new WaitForSeconds(2.5f);

        CarePackageDepot.Instance.CloseDoor();

        yield return new WaitForSeconds(1);

        Destroy(gameObject);
        Destroy(package);
    }
}

