using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController:MonoBehaviour{
    [Header("Wheels Rotation")]
    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public Transform rearRightWheel;
    public Transform rearLeftWheel;
    [Header("Wheels Turn")]
    public Transform lfWheelContainer;
    public Transform rfWheelContainer;

	public Text mySpeedText;

    public float power = 300;
    public float maxSpeed = 50;
    public float carGrip = 70;
    public float turnSpeed = 3;

    public float mySpeed;
    public float throttle;

    private Rigidbody carRigidbody;

    private Transform[] wheelTransform;
    private Transform carTransform;


    private Vector3 accel;
    private Vector3 myRight;
    private Vector3 velocity;
    private Vector3 flatVelocity;
    private Vector3 relativeVelocity;
    private Vector3 direction;
    private Vector3 flatDirection;
    private Vector3 carUp;
    private Vector3 carFwd;
    private Vector3 carRight;
    private Vector3 tempVec;
    private Vector3 engineForce;
    private Vector3 turnVelocity;
    private Vector3 imp;
    private Vector3 rotationAmount;

    private AudioSource audio;

    private float maxSpeedToTurn = .2f;
    private float deadZone = .001f;
    private float rev;
    private float actualTurn;
    private float carMass;
    private float actualGrip;
    private float horizontal;
    private float slideSpeed;
	private float setSpeed;

	private bool isReversing = false;
    // Use this for initialization
    void Start () {
        Init();
	}

    void Init()
    {
        carTransform = transform;
        carRigidbody = GetComponent<Rigidbody>();
        carUp = carTransform.up;
        carMass = GetComponent<Rigidbody>().mass;

        carRight = Vector3.right;
        carFwd = Vector3.forward;
        carRigidbody.centerOfMass = new Vector3(0, -.7f, .35f);
		setSpeed = maxSpeed;
        wheelTransform = new Transform[4];
        SetUpWheels();
        audio = GetComponent<AudioSource>();
    }
   
    void Update ()
    {
        // call the function to start processing all vehicle physics
        CarPhysicsUpdate();
        // call the function to see what input we are using and apply it
        CheckInput();
	}

    private void LateUpdate()
    {
        // this function makes the visual 3D wheels rotate and turn
        RotateVisualWheels();
        // this funtion creates and maintains engine sound;
        EngineSound();

		if(mySpeed > 0)
			mySpeedText.text = "Speed: " + (mySpeed*2).ToString ("F0");
    }

    private void CheckInput()
    {
        horizontal = Input.GetAxis("Horizontal");


        throttle = Input.GetAxis("Trigger");

		if (throttle < 0 && !isReversing) 
		{
			if (mySpeed < 0) 
			{
				throttle = 0;

			}
				
		}
    }


    private void CarPhysicsUpdate()
    {

		
        myRight = carTransform.right;
        
		if (throttle > 0)
			velocity = carRigidbody.velocity;
		else
			velocity = carRigidbody.velocity *.5f;

		tempVec = new Vector3(velocity.x, 0, velocity.z);
        
		flatVelocity = tempVec;
        
		direction = carTransform.TransformDirection(carFwd);
        
		tempVec = new Vector3(direction.x, 0, direction.z);
        
		flatDirection = Vector3.Normalize(tempVec);
        
		relativeVelocity = carTransform.InverseTransformDirection(flatVelocity);
        
		slideSpeed = Vector3.Dot(myRight, flatVelocity);


		mySpeed = flatVelocity.magnitude;

        rev = Mathf.Sign(Vector3.Dot(flatVelocity, flatDirection));

        engineForce = (flatDirection * (power * throttle) * carMass);
        
		actualTurn = horizontal;
        
		//if (rev < 0.1f)
          //  actualTurn -= actualTurn;

		turnVelocity = (((carUp * turnSpeed) * actualTurn) * carMass) * 350;

		
        actualGrip = Mathf.Lerp(100, carGrip, mySpeed*.1f);
        
		imp = myRight * (-slideSpeed * carMass * actualGrip);


    }

    private void SetUpWheels()
    {
        if ((null == frontLeftWheel || null == frontRightWheel || null == rearLeftWheel || null == rearRightWheel))
        {
            Debug.LogError("One or more of the wheel transforms have not been plugged in on the car");
            Debug.Break();
        }
        else
        {
            wheelTransform[0] = frontLeftWheel;
            wheelTransform[1] = rearLeftWheel;
            wheelTransform[2] = frontRightWheel;
            wheelTransform[3] = rearRightWheel;
        }
    }

    private void RotateVisualWheels()
    {
        lfWheelContainer.localEulerAngles = new Vector3(lfWheelContainer.localEulerAngles.x, horizontal * 30, lfWheelContainer.localEulerAngles.z);
        rfWheelContainer.localEulerAngles = new Vector3(rfWheelContainer.localEulerAngles.x, horizontal * 30, rfWheelContainer.localEulerAngles.z);

        rotationAmount = carRight * (relativeVelocity.z * 1.6f * Time.deltaTime * Mathf.Rad2Deg);

        wheelTransform[0].Rotate(rotationAmount);
        wheelTransform[1].Rotate(rotationAmount);
        wheelTransform[2].Rotate(rotationAmount);
        wheelTransform[3].Rotate(rotationAmount);
    }

    private void SlowVelocity()
    {
        carRigidbody.AddForce(-flatVelocity * .8f);
    }

    private void EngineSound()
    {
        audio.pitch = .30f + mySpeed * .025f;

        if (mySpeed > 30)
            audio.pitch = .25f + mySpeed * .015f;
        if (mySpeed > 80)
            audio.pitch = .20f + mySpeed * .013f;
        if (mySpeed > 120)
            audio.pitch = .15f + mySpeed * .011f;

        if (audio.pitch > 2)
            audio.pitch = 2;
    }

    private void FixedUpdate()
    {
        if (mySpeed < maxSpeed)
            carRigidbody.AddForce(engineForce * Time.deltaTime);

        if (mySpeed > maxSpeedToTurn)
            carRigidbody.AddTorque(turnVelocity * Time.deltaTime);
        else if (mySpeed < maxSpeedToTurn)
            return;

        carRigidbody.AddForce(imp * Time.deltaTime);
    }
}
