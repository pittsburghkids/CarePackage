using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class CounterTriggerHandler : MonoBehaviour, ITriggerHandler
{
    private const float DisplayTimeout = 5;

    [SerializeField] GameObject countDisplay = default;
    [SerializeField] GameObject rejectDisplay = default;
    [SerializeField] TMP_Text deliveryCountText = default;
    [SerializeField] Animator doorAnimator = default;

    [SerializeField] AudioClip countClip = default;
    [SerializeField] AudioClip errorClip = default;

    private int deliveryCount = 0;
    private int lastDay = -1;

    private Coroutine showCountRoutine = null;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Reset delivery count every day.
        int currentDay = System.DateTime.Now.Day;
        if (currentDay != lastDay)
        {
            deliveryCount = 0;
            deliveryCountText.text = deliveryCount.ToString();
            lastDay = currentDay;
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        Box box = collider.gameObject.GetComponent<Box>();

        if (box.HasAddress)
        {
            // Increment delivery count.
            Debug.Log("DepotCounter: Count box with valid address.");
            deliveryCount++;
            deliveryCountText.text = deliveryCount.ToString();

            audioSource.PlayOneShot(countClip);
        }
        else
        {
            // Reject box.
            Debug.Log("DepotCounter: Reject box with no address.");
            box.mover.Reject();

            doorAnimator.SetBool("Open", true);

            countDisplay.SetActive(false);
            rejectDisplay.SetActive(true);

            audioSource.PlayOneShot(errorClip);
        }

        if (showCountRoutine != null) StopCoroutine(showCountRoutine);
        showCountRoutine = StartCoroutine(ShowScanRoutine());

    }

    private IEnumerator ShowScanRoutine()
    {
        yield return new WaitForSeconds(DisplayTimeout);

        countDisplay.SetActive(true);
        rejectDisplay.SetActive(false);

        doorAnimator.SetBool("Open", false);

        showCountRoutine = null;
    }

    public void OnTriggerExit(Collider collider)
    {

    }
}
