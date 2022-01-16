using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [Header("Bools")]
    public bool isLevelStart;
    public bool isLevelDone;
    public bool isLevelFail;
    [SerializeField] private bool isOnGround;
    [SerializeField] private bool isAccelerating;
    [SerializeField] private bool isFlipping;
    [SerializeField] private bool isBoostActive;

    [Header("Flip Counter")]
    private Vector3 lastUp;
    private float rotateAroundX;
    private int flipCount;

    [Header("Tags")]
    string TagGround;

    [Header("Rigidbody")]
    Rigidbody RB;

    [Header("WheelColliders & Settings")]
    public WheelCollider[] WC;
    public GameObject[] Wheels;
    public float torque = 600;
    [SerializeField] private float speed;
    [SerializeField] private float finalTorque;
    private Quaternion quat;
    private Vector3 position;

    GameController GC;
    // Start is called before the first frame update
    void Start()
    {
        GC = GameController.instance;
        StartMethods();


        StartCoroutine(SetRandomSpeed());
		
    }

    private void StartMethods()
	{
        GetTags();
        GetRB();
    }

    void GetTags()
    {
        TagGround = GC.TagGround;
    }

    void GetRB()
    {
        RB = gameObject.GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (isLevelStart && !isLevelDone && !isLevelFail)
        {

            WheelRotation();

            if (isOnGround)
            {
             
                    if (flipCount > 0)
                    {
                        isBoostActive = true;
                        StartCoroutine(SetBoost());
                    }

                //speed = Input.GetAxis("Fire1");
               // speed = 0.7f;
                Move(speed);                              
            }

            else if (!isOnGround)
            {
                if (isFlipping)
                {
                    transform.RotateAround(transform.position, Vector3.right * -1, 3.5f);
                    FlipCounter();
                }
            }
        }
    }

	private void Update()
	{
        RayCast();

        if (isOnGround)
        {
           // isRandomIndexSetted = false;
           // Driver.GetComponent<FullBodyBipedIK>().enabled = true;
            //Driver.GetComponent<Animator>().SetBool("isFlipping", false);



        }

    }

    void RayCast()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up, transform.TransformDirection(Vector3.down), out hit, 7.5f))
        {
            if (hit.transform.CompareTag(TagGround))
            {
                isOnGround = true;
            }
        }

        else
        {
            isOnGround = false;
        }
    }

    void Move(float acceleration)
    {
        acceleration = Mathf.Clamp(acceleration, -1, 1);
        finalTorque = acceleration * torque;

        for (int i = 0; i < 2; i++)
        {
            WC[i].motorTorque = finalTorque;
        }
    }

    void WheelRotation()
    {
        for (int i = 0; i < 2; i++)
        {
            WC[i].GetWorldPose(out position, out quat);
            Wheels[i].transform.position = position;
            Wheels[i].transform.rotation = quat;
        }
    }

    private void FlipCounter()
    {
        var rotationDiffrence = Vector3.SignedAngle(transform.up, lastUp, transform.right);
        rotateAroundX += Mathf.Abs(rotationDiffrence);
        flipCount = Mathf.RoundToInt(rotateAroundX / 360);
        lastUp = transform.up;
    }
    
    IEnumerator SetBoost()
    {
        if (isBoostActive)
        {
            isBoostActive = false;
            torque = 800;
            yield return new WaitForSeconds(flipCount);
            torque = 550;
            flipCount = 0;
        }
    }

    IEnumerator SetRandomSpeed()
    {
        while (!isLevelDone)
        {
            speed = Random.Range(0.4f, 1f);
            yield return new WaitForSeconds(0.5f);
            Debug.Log("Random atýyo");
        }
    }
}
