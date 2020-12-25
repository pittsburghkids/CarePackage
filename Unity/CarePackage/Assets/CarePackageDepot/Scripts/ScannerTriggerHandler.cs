using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScannerTriggerHandler : MonoBehaviour, ITriggerHandler
{
   [SerializeField] GameObject itemLayout = default;
   [SerializeField] GameObject errorDisplay = default;

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
        if (box.HasItems)
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

            Debug.Log("ScannerTriggerHandler: Scan box with items.");
            itemLayout.SetActive(true);
        } else {
            Debug.Log("ScannerTriggerHandler: Scan box with no items.");
            errorDisplay.SetActive(true);
        }
    }

    // TODO(SJG): Instead of doing this on exit, set a timer that resets when a new box enters.
    public void OnTriggerExit(Collider collider)
    {
        itemLayout.SetActive(false);
        errorDisplay.SetActive(false);
    }
}
