using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    private const float emitDuration = 2.0f;
    private const float delayDuration = .25f;

    [SerializeField] GameObject itemPrefab = default;
    [SerializeField] Transform itemSpawnPoint = default;
    [SerializeField] GameObject confetti = default;
    [SerializeField] GameObject decal = default;

    public CarePackageDelivery carePackageDelivery;

    public bool HasAddress
    {
        get
        {
            return !(string.IsNullOrEmpty(carePackageDelivery.destinationName));
        }
    }

    public Mover Mover
    {
        get
        {
            // TODO(SJG) Cache this.
            return transform.parent.GetComponent<Mover>();
        }
    }

    public void Deliver()
    {
        CarePackageDepot.Instance.SetDestinationSprite(carePackageDelivery.destinationName);
    }

    public void Open()
    {
        GetComponent<Animator>().SetTrigger("Open");
    }

    public void Reject()
    {
        // TODO(SJG) Move this to mover?
        Mover.GetComponent<Animator>().SetBool("Reject", true);
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

        // Emit items one at a time.
        if (carePackageDelivery != null && carePackageDelivery.itemNames != null)
        {
            List<string> itemNames = carePackageDelivery.itemNames;

            int itemCount = Mathf.Min(itemNames.Count, 16);
            float itemDelay = emitDuration / itemCount;

            for (int i = 0; i < itemCount; i++)
            {
                string itemName = itemNames[i];

                if (itemName != null)
                {
                    GameObject itemInstance = Instantiate(itemPrefab);
                    itemInstance.transform.position = itemSpawnPoint.position;
                    itemInstance.name = itemName;

                    Sprite itemSprite = CarePackage.Instance.GetSpriteForItemName(itemName);
                    SpriteRenderer spriteRenderer = itemInstance.GetComponent<SpriteRenderer>();
                    spriteRenderer.sprite = itemSprite;

                    Debug.Log("Emitting: " + itemInstance.name);

                    yield return new WaitForSeconds(itemDelay);
                }
            }
        }

        // Additional post-item wait time.
        yield return new WaitForSeconds(delayDuration);

        // Close box and door when finished emitting.
        GetComponent<Animator>().SetTrigger("Close");
        CarePackageDepot.Instance.CloseDoor();
    }

}
