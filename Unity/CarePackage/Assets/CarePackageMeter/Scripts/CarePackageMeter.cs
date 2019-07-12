using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarePackageMeter : MonoBehaviour
{
    [SerializeField] Transform rootTransform;

    [SerializeField] GameObject heartPrefab;
    [SerializeField] GameObject starPrefab;

    [SerializeField] Animator doorAnimator;

    string currentBox = null;
    float currentBoxTime = 0;

    public bool boxPresent = false;

    public static void Store(string message)
    {
        Debug.Log("STORE: " + message);
    }

    public void InsertBox(string message)
    {
        int result;
        if (int.TryParse(message, out result))
        {
            if (currentBox != message)
            {
                Debug.Log("InsertBox: " + message);
                currentBox = message;
                currentBoxTime = Time.time;

                boxPresent = true;
                doorAnimator.SetBool("Open", boxPresent);
            }

            if (currentBox == message)
            {
                currentBoxTime = Time.time;
            }
        }
    }

    public void InsertItem(string message)
    {
        if (message == "1778588676")
        {
            GameObject heartInstance = Instantiate(heartPrefab, rootTransform);
            heartInstance.name = message;
        }

        if (message == "1778530052")
        {
            GameObject heartInstance = Instantiate(heartPrefab, rootTransform);
            heartInstance.name = message;
        }

        if (message == "1778588420")
        {
            GameObject starInstance = Instantiate(heartPrefab, rootTransform);
            starInstance.name = message;
        }

        if (message == "1778646020")
        {
            GameObject starInstance = Instantiate(heartPrefab, rootTransform);
            starInstance.name = message;
        }
    }

    void Update()
    {
        if (currentBox != null && (Time.time - currentBoxTime) > .5f)
        {
            boxPresent = false;
            doorAnimator.SetBool("Open", boxPresent);

            Debug.Log("Box removed");
            currentBox = null;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GameObject heartInstance = Instantiate(heartPrefab, rootTransform);
            heartInstance.name = "1778588676";
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GameObject starInstance = Instantiate(starPrefab, rootTransform);
            starInstance.name = "1778588420";
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            boxPresent = !boxPresent;
            doorAnimator.SetBool("Open", boxPresent);
        }

    }
}