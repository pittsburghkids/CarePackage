using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class DepotCounter : MonoBehaviour
{
    [SerializeField] TMP_Text deliveryCountText = default;

    private int deliveryCount = 0;
    private int lastDay = -1;

    void Start()
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

    public void Count()
    {
        // Increment delivery count.
        deliveryCount++;
        deliveryCountText.text = deliveryCount.ToString();
    }
}
