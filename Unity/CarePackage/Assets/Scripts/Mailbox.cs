using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mailbox : MonoBehaviour
{
    public GameObject letterSchoolPrefab;
    public GameObject letterHospitalPrefab;
    public GameObject letterStadiumPrefab;
    public GameObject letterAirportPrefab;

    public GameObject heartsSchoolPrefab;
    public GameObject heartsHospitalPrefab;
    public GameObject heartsStadiumPrefab;
    public GameObject heartsAirportPrefab;

    public void Message(string message)
    {
        if (message == "1778646020")
        {
            Instantiate(letterSchoolPrefab);
            Instantiate(heartsSchoolPrefab);
        }
        if (message == "1778530052")
        {
            Instantiate(letterHospitalPrefab);
            Instantiate(heartsHospitalPrefab);
        }
        if (message == "1778588420")
        {
            Instantiate(letterStadiumPrefab);
            Instantiate(heartsStadiumPrefab);
        }
        if (message == "1778588676")
        {
            Instantiate(letterAirportPrefab);
            Instantiate(heartsAirportPrefab);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Instantiate(letterSchoolPrefab);
            Instantiate(heartsSchoolPrefab);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Instantiate(letterHospitalPrefab);
            Instantiate(heartsHospitalPrefab);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Instantiate(letterStadiumPrefab);
            Instantiate(heartsStadiumPrefab);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Instantiate(letterAirportPrefab);
            Instantiate(heartsAirportPrefab);
        }
    }
}
