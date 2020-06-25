using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DepotScanner : MonoBehaviour
{
    public SpriteRenderer destinationSpriteRenderer;
    private Sprite defaultDestinationSprite;

    public Animator Animator
    {
        get
        {
            return GetComponent<Animator>();
        }
    }

    public void Start()
    {
        defaultDestinationSprite = destinationSpriteRenderer.sprite;
    }

    public void Scan(Collider collider)
    {
        Box box = collider.gameObject.GetComponent<Box>();

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
        destinationSpriteRenderer.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        destinationSpriteRenderer.gameObject.SetActive(false);
    }
}
