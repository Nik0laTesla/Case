using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using TMPro;

public class AIController : MonoBehaviour
{
    [Header("Bools")]
    public bool isLevelStart;
    public bool isLevelDone;
    public bool isLevelFail;
    [SerializeField] private bool isOnGround;
    [SerializeField] private bool isLanding;
    [SerializeField] private bool isFlipping;
    [SerializeField] private bool isBoostActive;
    [SerializeField] private bool isRotationNormal;


    [SerializeField] LayerMask groundLayer;

    [Header("Particles")]
    [SerializeField] private ParticleSystem smoke;
    [SerializeField] private ParticleSystem boostParticle;


    [Header("Place")]
    public int place;
    [SerializeField] private TextMeshPro placeText;
    public GameObject crown;

    [Header("Flip Counter")]
    private Vector3 lastUp;
    private float rotateAroundX;
    private int flipCount;

    [Header("Tags")]
    string TagGround;

    [Header("Rigidbody")]
    Rigidbody RB;

    [SerializeField] private GameObject Driver;

    [Header("WheelColliders & Settings")]
    public WheelCollider[] WC;
    public GameObject[] Wheels;
    public float torque;
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
               
                if (flipCount > 1)
                {
                    isBoostActive = true;
                    StartCoroutine(SetBoost());
                }
				else
				{
                    flipCount = 0;
                }
   
                Move(speed);                              
            }

            else if (!isOnGround)
            {               
                FlipCounter();               
            }
        }
    }

	private void Update()
	{
        if (isLevelStart && !isLevelDone && !isLevelFail)
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
        }
    }

    void RayCast()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 200f,groundLayer))
        {
            if (hit.distance < 7.5f && hit.transform.CompareTag(TagGround))
            {
                smoke.Play();
                isLanding = false;
                isOnGround = true;             
            }
            else if(!isOnGround && hit.distance < 20f && hit.transform.CompareTag(TagGround))
			{
                isLanding = true;
			}
        }
        
        else
        {
            isOnGround = false;
            smoke.Stop();
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
        if (isLanding)
		{
            if (!(transform.localEulerAngles.x >=340) && !(transform.localEulerAngles.x <=20))
            {             
                RB.angularVelocity = new Vector3(10,0,0); 
            }
			
            else 
			{
                isRotationNormal = true;
                RB.angularVelocity = Vector3.zero;
            }
        }

		else
		{
            transform.RotateAround(transform.position, Vector3.right * -1, 7.5f);
		}
       
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
            torque = 900;
            boostParticle.Play();
            yield return new WaitForSeconds(flipCount);
            boostParticle.Stop();
            torque = 650;
            flipCount = 0;
        }
    }

    IEnumerator SetRandomSpeed()
    {
        while (!isLevelDone)
        {
            speed = Random.Range(0.4f, 1f);
            yield return new WaitForSeconds(1f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(TagGround))
        {
            if (!isLevelFail)
            {
                gameObject.tag = "Untagged";
                Driver.transform.SetParent(null);
                Driver.GetComponent<Animator>().enabled = false;
                Driver.GetComponent<FullBodyBipedIK>().enabled = false;
                isLevelFail = true;
                StartCoroutine(Respawn());
            }       
        }
    }

    IEnumerator Respawn()
    {
        if (!GC.isLevelFail)
        {
            yield return new WaitForSeconds(2f);
            RB.angularVelocity = Vector3.zero;
            RB.velocity = Vector3.zero;
            isLevelFail = false;
            gameObject.tag = "Player";
            Driver.transform.SetParent(transform);
            Driver.GetComponent<Animator>().enabled = true;
            Driver.GetComponent<FullBodyBipedIK>().enabled = true;
            transform.rotation = Quaternion.identity;
            transform.position += Vector3.up * 4;
            Driver.transform.localPosition = Vector3.up * 0.1f;
            Driver.transform.localRotation = Quaternion.identity;
            torque = 500;
            yield return new WaitForEndOfFrame();
            Driver.GetComponent<Animator>().enabled = false;
            yield return new WaitForEndOfFrame();
            Driver.GetComponent<Animator>().enabled = true;
        }
    }
}
