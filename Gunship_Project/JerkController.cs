using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JerkController : MonoBehaviour
{
    public float MaxVelocity = 5;
    public float MaxAcceleration = 10;
    public float JerkOffset = 2;
    public Vector3 Velocity;
    public float Acceleration = 0;
    public float Jerk = 0;

    public float MaxRotVelocity = 20;
    public float MaxRotAcceleration = 10;
    public float RotJerkOffset = 2;
    public float RotVelocity = 0;
    public float RotAcceleration = 0;
    public float RotJerk = 0 ;

    public bool isInputReading = false;
    public bool invertCons = false;

    public Rigidbody2D rigBod;


    // Start is called before the first frame update
    void Start()
    {
        Velocity = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // Linear Management
        // Adjust acceleration and velocity
        Acceleration += Jerk * Time.deltaTime;
        if (Acceleration >= MaxAcceleration)
            Acceleration = MaxAcceleration;
        if (Acceleration <= -MaxAcceleration)
            Acceleration = -MaxAcceleration;
        Velocity = rigBod.velocity;
        if (Velocity.magnitude >= MaxVelocity)
        {
            Velocity = Velocity.normalized;
            Velocity *= MaxVelocity;
            rigBod.velocity = Velocity;
        }

        // Move object forward by velocity
        rigBod.AddForce(Acceleration * transform.up);

        if (isInputReading)
        {
            if (invertCons ? Input.GetKeyDown(KeyCode.S) : Input.GetKeyDown(KeyCode.W))
            {
                Jerk = JerkOffset;

            }
            else if (invertCons ? Input.GetKeyDown(KeyCode.W) : Input.GetKeyDown(KeyCode.S))
            {
                Jerk = -JerkOffset;
            }

            if (invertCons ? Input.GetKeyUp(KeyCode.S) : Input.GetKeyUp(KeyCode.W))
            {
                Jerk = 0;
                Acceleration = 0;
            }
            else if (invertCons ? Input.GetKeyUp(KeyCode.W) : Input.GetKeyUp(KeyCode.S))
            {
                Jerk = 0;
            }
        }



        // Rotational Management
        // Adjust rotational acceleration and velocity
        RotAcceleration += (RotJerk * Time.deltaTime);

        if (RotAcceleration >= MaxRotAcceleration)
            RotAcceleration = MaxRotAcceleration;

        if (RotAcceleration <= -MaxRotAcceleration)
            RotAcceleration = -MaxRotAcceleration;


        RotVelocity = rigBod.angularVelocity;
        if (RotVelocity >= MaxRotVelocity)
            RotVelocity = MaxRotVelocity;

        if (RotVelocity <= -MaxRotVelocity)
            RotVelocity = -MaxRotVelocity;
        rigBod.angularVelocity = RotVelocity;

        // Rotate object by RotVelocity
        rigBod.AddTorque(RotAcceleration);

        if (isInputReading)
        {
            if (invertCons ? Input.GetKeyDown(KeyCode.A) : Input.GetKeyDown(KeyCode.D))
            {
                RotJerk = -RotJerkOffset;
            }
        
            if (invertCons ? Input.GetKeyDown(KeyCode.D) : Input.GetKeyDown(KeyCode.A))
            {
                RotJerk = RotJerkOffset;
            }

            if (invertCons ? Input.GetKeyUp(KeyCode.A) : Input.GetKeyUp(KeyCode.D))
            {
                RotJerk = 0;
                RotAcceleration = 0;
            }
        
            if (invertCons ? Input.GetKeyUp(KeyCode.D) : Input.GetKeyUp(KeyCode.A))
            {
                RotJerk = 0;
                RotAcceleration = 0;
            }
        }

    }
}
