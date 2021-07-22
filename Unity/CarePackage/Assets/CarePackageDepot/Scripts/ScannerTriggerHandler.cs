using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class ScannerTriggerHandler : MonoBehaviour, ITriggerHandler
{
    const float DisplayTimeout = 5;

    [SerializeField] GameObject itemLayout = default;
    [SerializeField] GameObject errorDisplay = default;
    [SerializeField] Sprite defaultItemSprite = default;

    [SerializeField] AudioClip scanClip = default;
    [SerializeField] AudioClip errorClip = default;

    private Coroutine showScanRoutine = null;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
            audioSource.PlayOneShot(scanClip);
            itemLayout.SetActive(true);
        }
        else
        {
            Debug.Log("ScannerTriggerHandler: Scan box with no items.");
            audioSource.PlayOneShot(errorClip);
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
