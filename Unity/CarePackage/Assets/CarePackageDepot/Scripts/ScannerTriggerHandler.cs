using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScannerTriggerHandler : MonoBehaviour, ITriggerHandler
{
    const float DisplayTimeout = 2;

    [SerializeField] GameObject itemLayout = default;
    [SerializeField] GameObject errorDisplay = default;
    [SerializeField] Sprite defaultItemSprite = default;

    private Coroutine showScanRoutine = null;

    public void Start()
    {
        itemLayout.SetActive(false);
    }

    public void OnTriggerEnter(Collider collider)
    {
        Box box = collider.gameObject.GetComponent<Box>();

        itemLayout.SetActive(false);
        errorDisplay.SetActive(false);

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

                    if (sprite == null) sprite = defaultItemSprite;

                    Transform itemTransform = itemLayout.transform.GetChild(i);
                    Image itemImage = itemTransform.GetComponent<Image>();
                    itemImage.sprite = sprite;
                    itemTransform.gameObject.SetActive(true);
                }
            }

            Debug.Log("ScannerTriggerHandler: Scan box with items.");
            itemLayout.SetActive(true);
        }
        else
        {
            Debug.Log("ScannerTriggerHandler: Scan box with no items.");
            errorDisplay.SetActive(true);
        }

        if (showScanRoutine != null) StopCoroutine(showScanRoutine);
        showScanRoutine = StartCoroutine(ShowScanRoutine());
    }

    private IEnumerator ShowScanRoutine()
    {
        yield return new WaitForSeconds(DisplayTimeout);

        itemLayout.SetActive(false);
        errorDisplay.SetActive(false);

        showScanRoutine = null;
    }

    public void OnTriggerExit(Collider collider)
    {
    }
}
