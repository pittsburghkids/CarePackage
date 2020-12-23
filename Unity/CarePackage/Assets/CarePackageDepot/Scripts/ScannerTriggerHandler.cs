using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScannerTriggerHandler : MonoBehaviour, ITriggerHandler
{
    [SerializeField] Animator animator = default;
    [SerializeField] GameObject itemLayout = default;

    public void Start()
    {
        itemLayout.SetActive(false);
    }

    public void OnTriggerEnter(Collider collider)
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

        itemLayout.SetActive(true);
    }

    public void OnTriggerExit(Collider collider)
    {
        itemLayout.SetActive(false);
    }
}
