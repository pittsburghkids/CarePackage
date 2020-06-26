using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class DepotScanner : MonoBehaviour
{
    public SpriteRenderer destinationSpriteRenderer;
    private Sprite defaultDestinationSprite;

    [SerializeField] GameObject itemLayout;

    public Animator Animator
    {
        get
        {
            return GetComponent<Animator>();
        }
    }

    public void Start()
    {
        itemLayout.SetActive(false);

        defaultDestinationSprite = destinationSpriteRenderer.sprite;
    }

    public void Scan(Collider collider)
    {
        Box box = collider.gameObject.GetComponent<Box>();


        // Clear old sprites.
        for (int i = 0; i < itemLayout.transform.childCount; i++)
        {
            Transform itemTransform = itemLayout.transform.GetChild(i);
            itemTransform.gameObject.SetActive(false);
        }

        // Set item sprites.
        if (box.carePackageDelivery.itemNames != null)
        {
            for (int i = 0; i < box.carePackageDelivery.itemNames.Count; i++)
            {
                if (i < itemLayout.transform.childCount)
                {
                    string itemName = box.carePackageDelivery.itemNames[i];
                    Sprite sprite = CarePackage.Instance.GetSpriteForItemName(itemName);

                    Transform itemTransform = itemLayout.transform.GetChild(i);
                    Image itemImage = itemTransform.GetComponent<Image>();
                    itemImage.sprite = sprite;
                    itemTransform.gameObject.SetActive(true);
                }
            }
        }

        //

        Sprite destinationSprite = CarePackage.Instance.GetSpriteForLabelName(box.carePackageDelivery.destinationName);
        if (destinationSprite != null)
        {
            destinationSpriteRenderer.sprite = destinationSprite;
        }
        else
        {
            destinationSpriteRenderer.sprite = defaultDestinationSprite;
        }

        StartCoroutine(ScanRoutine());
    }

    private IEnumerator ScanRoutine()
    {
        itemLayout.SetActive(true);

        // destinationSpriteRenderer.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        // destinationSpriteRenderer.gameObject.SetActive(false);

        itemLayout.SetActive(false);
    }
}
