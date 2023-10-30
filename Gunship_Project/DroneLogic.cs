using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneLogic : MonoBehaviour
{
    [SerializeField] Vector3 moveLoc;
    [SerializeField] float moveDistance = 0;
    [SerializeField] float travelDist = 0;
    Camera cam;
    public JerkController controller;
    public GameObject PrefabTarget;
    private GameObject tarMark;
    private bool RangeDel = false;
    public Rigidbody2D rigBod;

    private void OnDestroy()
    {
        Destroy(tarMark.gameObject);
        if (RangeDel == false)
        {
            TimeSlowLogic.Instance.SlowMoStart();
        }
        DroneSpawner.Instance.droneCount--;
        Destroy(this.gameObject);
    }

    enum DroneState
    {
        dRot = 0,
        dMov = 1,
        dNew = 2,
        dDie = -1
    }

    private float dieDmgTimer = 0.25f;
    private float dieDmg = 15f;

    private DroneState dStat = 0;

    public void EnableDie()
    {
        dStat = DroneState.dDie;
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponentInParent<JerkController>();
        cam = Camera.main;
        moveLoc = cam.ScreenToWorldPoint(new Vector3(Random.Range(0, cam.pixelWidth), Random.Range(0, cam.pixelHeight), 0));
        moveLoc.z = 0;

        tarMark = Instantiate(PrefabTarget);
        Vector3 tempVec = moveLoc;
        tempVec.z = -11;
        tarMark.transform.position = tempVec;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject PlayerObj = GameObject.FindGameObjectWithTag("Player");

        if (PlayerObj != null)
        {
            if (Vector3.Distance(transform.position, PlayerObj.transform.position) > 35)
            {
                RangeDel = true;
                Destroy(this.gameObject);
                return;
            }
        }

        float RotValue = 0.0f;
        float angleDiff = Vector3.Angle(transform.up, moveLoc - transform.position);
        switch (dStat)
        {
            case DroneState.dRot:

                if (angleDiff > 5)
                {
                    // turn right
                    if (Vector3.Dot(moveLoc, transform.right) > 0)
                    {
                        RotValue = -1.0f;
                    }
                    else
                    {
                        RotValue = 1.0f;
                    }
                }
                else
                {
                    // turn right
                    RotValue = 0;
                    controller.RotAcceleration = 0;
                    moveDistance = Vector3.Distance(transform.position, moveLoc);
                    moveLoc = transform.position + (transform.up * moveDistance);
                    Vector3 tempVec2 = moveLoc;
                    tempVec2.z = 0;
                    tarMark.transform.position = tempVec2;

                    if (Random.Range(0, 100) < 30)
                    {
                        moveDistance *= 1.25f;
                    }
                    dStat = DroneState.dMov;
                }
                controller.RotJerk = RotValue;
                break;
            case DroneState.dMov:
                travelDist += controller.Velocity.magnitude * Time.deltaTime;
                float movVal = 0.0f, distDiff = moveDistance - travelDist;

                if (angleDiff > 0.5f)
                {
                    // turn right
                    if (Vector3.Dot(moveLoc, transform.right) > 0)
                    {
                        if (rigBod.angularVelocity > 0)
                        {
                            controller.RotAcceleration = 0;
                            // rigBod.angularVelocity = 0;
                        }
                        RotValue = -20f;
                    }
                    else
                    {
                        if (rigBod.angularVelocity < 0)
                        {
                            controller.RotAcceleration = 0;
                            // rigBod.angularVelocity = 0;
                        }
                        RotValue = 20f;
                    }
                }
                else
                {
                    RotValue = 0.0f;
                    controller.RotAcceleration = 0;
                }
                if (distDiff > (moveDistance * 0.05f))
                {
                    movVal = 1.0f;
                }
                else
                {
                    // turn right
                    movVal = 0;
                    // controller.Velocity = Vector3.zero;
                    controller.Acceleration = 0;
                    dStat = DroneState.dNew;
                }
                controller.RotJerk = RotValue;
                controller.Jerk = movVal;
                break;
            case DroneState.dNew:
                moveLoc = cam.ScreenToWorldPoint(new Vector3(Random.Range(0, cam.pixelWidth), Random.Range(0, cam.pixelHeight), 0));
                moveLoc.z = 0;
                Vector3 tempVec = moveLoc;
                tempVec.z = -11;
                tarMark.transform.position = tempVec;
                moveDistance = 0;
                travelDist = 0;
                rigBod.angularDrag = 0.1f;
                dStat = DroneState.dRot;
                break;
            case DroneState.dDie:
                dieDmgTimer -= Time.deltaTime;
                if (dieDmgTimer <= 0)
                {
                    GetComponentInChildren<HealthBar>().DealDamage((int)dieDmg);
                    GetComponentInChildren<Rigidbody2D>().AddForce((Vector3.zero - transform.position) * -500);
                    dieDmgTimer = 0.25f;
                }
                break;
            default:
                break;
        }
    }
}
