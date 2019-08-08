using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    GameObject package;

    public void GetPackage(GameObject package)
    {
        this.package = package;

        StartCoroutine(GetPackageRoutine(package));
    }

    IEnumerator GetPackageRoutine(GameObject package)
    {
        float startTime = Time.time;
        float endTime = startTime + .4f;

        while (Time.time < endTime)
        {
            float alpha = (Time.time - startTime) / (endTime - startTime);

            package.transform.position = Vector3.Lerp(package.transform.position, transform.position, alpha);
            yield return null;
        }

        package.transform.parent = transform;
        package.transform.localPosition = Vector3.zero;
        package.transform.localRotation = Quaternion.identity;
        package.gameObject.GetComponent<Rigidbody>().isKinematic = true;

        GetComponent<Animator>().SetTrigger("Move");
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

        yield return new WaitForSeconds(2);

        CarePackageDepot.Instance.CloseDoor();

        yield return new WaitForSeconds(1);

        Destroy(gameObject);
        Destroy(package);
    }
}

