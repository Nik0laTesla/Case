using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortController : MonoBehaviour
{

    [Header("Driver Positions")]
    [SerializeField] private float Driver;
    [SerializeField] private float Driver1;
    [SerializeField] private float Driver2;
    [SerializeField] private float Driver3;

    [SerializeField] private List<float> values = new List<float>();

    GameController GC;
    DriveController Drive;
    AIManager AIM;

    public static SortController instance;
    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }

        values.Add(Driver);
        values.Add(Driver1);
        values.Add(Driver2);
        values.Add(Driver3);

    }

    // Start is called before the first frame update
    void Start()
    {
        StartMethods();
    }

    private void StartMethods()
    {
        GC = GameController.instance;
        Drive = DriveController.instance;
        AIM = AIManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        Driver = Drive.transform.position.z;
        Driver1 = AIM.transform.GetChild(0).transform.position.z;
        Driver2 = AIM.transform.GetChild(1).transform.position.z;
        Driver3 = AIM.transform.GetChild(2).transform.position.z;

        values[0] = Driver;
        values[1] = Driver1;
        values[2] = Driver2;
        values[3] = Driver3;

        values.Sort();
        SetNumber();
    }

    void SetNumber()
    {
        Drive.place = Math.Abs(values.IndexOf(Driver)-4);
        AIM.transform.GetChild(0).GetComponent<AIController>().place = Math.Abs(values.IndexOf(Driver1)-4);
        AIM.transform.GetChild(1).GetComponent<AIController>().place = Math.Abs(values.IndexOf(Driver2)-4);
        AIM.transform.GetChild(2).GetComponent<AIController>().place = Math.Abs(values.IndexOf(Driver3)-4);
    }
}