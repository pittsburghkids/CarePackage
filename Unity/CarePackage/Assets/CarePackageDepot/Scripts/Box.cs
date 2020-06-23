using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField] GameObject itemPrefab = default;
    [SerializeField] Transform itemSpawnPoint = default;
    [SerializeField] GameObject confetti = default;
    [SerializeField] GameObject decal = default;

    public CarePackageDelivery carePackageDelivery;

    public void Deliver()
    {
        CarePackageDepot.Instance.SetDestinationSprite(carePackageDelivery.destinationName);
    }

    public void Open()
    {
        GetComponent<Animator>().SetTrigger("Open");
    }

    // Called by DepotStamp during animation.
    public void ShowDecal()
    {
        if (carePackageDelivery.destinationName != null)
        {
            Sprite labelSprite = CarePackage.Instance.GetSpriteForLabelName(carePackageDelivery.destinationName);

            SpriteRenderer[] spriteRenderers = decal.GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                spriteRenderer.sprite = labelSprite;
            }
        }

        decal.SetActive(true);
    }

    // Called by an animation event in the "BoxOpen" animation.
    public void Emit()
    {
        StartCoroutine(EmitRoutine());
    }

    IEnumerator EmitRoutine()
    {
        confetti.SetActive(true);

        //

        if (carePackageDelivery.itemNames != null)
        {
            foreach (string itemName in carePackageDelivery.itemNames)
            {
                if (itemName != null)
                {
                    GameObject itemInstance = Instantiate(itemPrefab);
                    itemInstance.transform.position = itemSpawnPoint.position;
                    itemInstance.name = itemName;

                    Sprite itemSprite = CarePackage.Instance.GetSpriteForItemName(itemName);
                    SpriteRenderer spriteRenderer = itemInstance.GetComponent<SpriteRenderer>();
                    spriteRenderer.sprite = itemSprite;

                    Rigidbody rigidBody = itemInstance.GetComponent<Rigidbody>();
                    rigidBody.AddForce((Vector3.up * Random.Range(3, 5f)) + (Vector3.right * Random.Range(-3f, 3f)));
                    rigidBody.AddTorque(Vector3.forward * Random.Range(-180f, 180f));

                    Destroy(itemInstance, 2f);

                    Debug.Log("Emitting: " + itemInstance.name);

                    yield return new WaitForSeconds(.2f);
                }
            }

        }

        yield return new WaitForSeconds(2.5f);

        // Close door when finished emitting.
        CarePackageDepot.Instance.CloseDoor();
    }

}
