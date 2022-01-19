using System.Collections;
using System.Collections.Generic;
using System.Collections;
using RootMotion.FinalIK;
using RootMotion.Demos;
using UnityEngine;
using TMPro;


public class DriveController : MonoBehaviour
{
    [Header("Bools")]
    public bool isLevelStart;
    public bool isLevelDone;
    public bool isLevelFail;
    [SerializeField] private bool isOnGround;
    [SerializeField] private bool isAccelerating;
    [SerializeField] private bool isFlipping;
    [SerializeField] private bool isBoostActive;
    [SerializeField] private bool isRandomIndexSetted;

    [Header("Rigidbody")]
    [SerializeField] private Rigidbody RB;

    [Header("Tags")]
    string TagGround;
    string TagFinishTrigger;

    [Header("Flip Counter")]
    private Vector3 lastUp;
    private float rotateAroundX;
    private int flipCount;

    [Header("Driver")]
    public GameObject Driver;

    [Header("Place")]
    public int place;
    [SerializeField] private TextMeshPro placeText;
    public GameObject crown;

    [Header("Particles")]
    [SerializeField] private ParticleSystem smokeParticle;
    [SerializeField] private ParticleSystem boostParticle;

    [Header("WheelColliders & Settings")]
    public WheelCollider[] WC;
    public GameObject[] Wheels;
    public float torque = 900;
    [SerializeField] private float speed;
    [SerializeField] private float finalTorque;
    private Quaternion quat;
    private Vector3 position;

    GameController GC;
    CameraController Camera;

    public static DriveController instance;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
    }

    private void Start()
    {
        GC = GameController.instance;
        Camera = CameraController.instance;
        StartMethods();
    }

    void StartMethods()
    {
        GetTags();
        GetRB();
    }

    void GetTags()
    {
        TagGround = GC.TagGround;
        TagFinishTrigger = GC.TagFinishTrigger;
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
                if (flipCount > 1)
                {
                    isBoostActive = true;
                    StartCoroutine(SetBoost());
                }
                else
                {
                    flipCount = 0;
                }

                if (isAccelerating)
                {

                    speed = Input.GetAxis("Fire1");
                    Move(speed);
                }
                else
                {
                    Move(speed);
                }
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

        else if (!isLevelStart && isLevelDone)
        {
            smokeParticle.Stop();

            RB.mass = 2500;

            if (RB.angularVelocity.magnitude > 0)
            {
                RB.angularVelocity = new Vector3(-25f, 0, 0);
            }
        }
    }

    private void Update()
    {
        if (isLevelStart && !isLevelFail && !isLevelDone)
        {
            if (place == 1)
            {
                crown.SetActive(true);
            }
            else
            {
                crown.SetActive(false);
            }

            placeText.text = place.ToString();
            
            RayCast();

            if (isOnGround)
            {
                smokeParticle.Play();

                isRandomIndexSetted = false;
                Driver.GetComponent<FullBodyBipedIK>().enabled = true;
                Driver.GetComponent<Animator>().SetBool("isFlipping", false);
                if (Input.GetMouseButton(0))
                {

                    isAccelerating = true;
                }

                else
                {
                    isAccelerating = false;
                }
            }

            else if (!isOnGround)
            {
                smokeParticle.Stop();
                isAccelerating = false;

                if (RB.angularVelocity.magnitude > 0)
                {
                    RB.angularVelocity = new Vector3(-0.25f, 0, 0); 

                }

                if (Input.GetMouseButton(0))
                {
                    isFlipping = true;

                    if (!isRandomIndexSetted)
                    {
                        isRandomIndexSetted = true;
                        Driver.GetComponent<Animator>().SetInteger("randomAnimIndex", Random.Range(0, 3)); 

                    }

                    Driver.GetComponent<FullBodyBipedIK>().enabled = false;
                    Driver.GetComponent<Animator>().SetBool("isFlipping", true);
                }

                else
                {
                    isFlipping = false;
                }
            }

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

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag(TagGround))
		{
            gameObject.tag = "Untagged";
            Driver.transform.SetParent(null);
            Driver.GetComponent<Animator>().enabled = false;
            Driver.GetComponent<FullBodyBipedIK>().enabled = false;
            GC.LevelFailActions();
        }

        else if (other.CompareTag(TagFinishTrigger))
		{
            if(place == 1)
			{
                GC.LevelDoneActions();
			}
			
            else
			{
                Camera.isLevelFinishFail = true;
                GC.LevelFailActions();
			}
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
            torque = 1000;
            boostParticle.Play();
            yield return new WaitForSeconds(flipCount * 1.5f);
            boostParticle.Stop();
            torque = 600;
            flipCount = 0;
        }
    }
}
