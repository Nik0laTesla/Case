using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Bools")]
    public bool isLevelStart;
    public bool isLevelDone;
    public bool isLevelFail;

    [Header("Settings")]
    public Vector3 offset;
    private Vector3 targetPosition;
    public float smooth;
    private Transform Target;
    private Vector3 finalPos;


    DriveController DC;
    FinishTrigger Finish;
    

    public static CameraController instance;


	private void Awake()
	{
		if (!instance)
		{
            instance = this;
		}
	}
	
    // Start is called before the first frame update
	void Start()
    {
        StartMethods();
    }

    void StartMethods()
	{
        DC = DriveController.instance;
        Finish = FinishTrigger.instance;

        Target = DC.transform;
        finalPos = Finish.transform.position;
        finalPos.y += 15f;
        finalPos.x += 15f;
	}

    // Update is called once per frame
    void LateUpdate()
    {
        if(isLevelStart && !isLevelDone && !isLevelFail)
		{
            targetPosition = Target.transform.position + offset;

            transform.position = Vector3.Lerp(transform.position,targetPosition,smooth);
		}
		else if(isLevelDone)
        {
            
            transform.LookAt(Target);
            transform.position = Vector3.Lerp(transform.position,finalPos,Time.deltaTime);
		}
    }
}
