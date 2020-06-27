using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class DepotScanner : MonoBehaviour
{
    [SerializeField] GameObject itemLayout = default;

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
    }

    public void StartScan(Collider collider)
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
                    Sprite sprite = CarePackage.Instance.GetSpriteForIconName(itemName);

                    Transform itemTransform = itemLayout.transform.GetChild(i);
                    Image itemImage = itemTransform.GetComponent<Image>();
                    itemImage.sprite = sprite;
                    itemTransform.gameObject.SetActive(true);
                }
            }
        }

        itemLayout.SetActive(true);
    }

    public void StopScan(Collider collider)
    {
        itemLayout.SetActive(false);
    }
}
