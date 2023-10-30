using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarLogic : MonoBehaviour
{
    public ConnectorNode conNode;
    [SerializeField] Vector3 moveLoc;
    [SerializeField] float moveDistance = 0;
    [SerializeField] float travelDist = 0;

    public JerkController controller;

    private bool findNewPoint = false;

    Camera cam;

    private void OnDestroy()
    {
        TimeSlowLogic.Instance.SlowMoStart();
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponentInParent<JerkController>();
        cam = Camera.main;
        moveLoc = cam.ScreenToWorldPoint(new Vector3(Random.Range(0, cam.pixelWidth), Random.Range(0, cam.pixelHeight), 0));
        moveLoc.z = 0;
        moveDistance = Vector3.Distance(transform.position, moveLoc);
    }

    // Update is called once per frame
    void Update()
    {
        if (conNode != null) // Follow Mode
        {
            transform.up = conNode.transform.position - transform.position;
            transform.position = conNode.transform.position - (transform.up * 1.5f);
        }
        else // Driver Mode
        {
            /* 
            Vector3 tempMovVec = cam.WorldToScreenPoint(moveLoc);
            if (tempMovVec.x < 0 || tempMovVec.y < 0 ||
                tempMovVec.x > cam.pixelWidth ||
                tempMovVec.y > cam.pixelHeight)
            {
                controller.Acceleration = 0;
                findNewPoint = true;
            } 
            */

            if (findNewPoint)
            {
                moveLoc = new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), 0);
                moveDistance = Vector3.Distance(transform.position, moveLoc);
                travelDist = 0;
                findNewPoint = false;
            }
            else
            {
                float RotValue = 0.0f;
                float angleDiff = Vector3.Angle(transform.up, moveLoc - transform.position);
                travelDist += controller.Velocity.magnitude * Time.deltaTime;
                float movVal = 0.0f, distDiff = moveDistance - travelDist;
                if (angleDiff > 90)
                {
                    RotValue = 20.0f;
                }
                else if (angleDiff > 1)
                {
                    // turn right
                    if (Vector3.Dot(moveLoc, transform.right) > 0)
                    {
                        RotValue = -10f;
                    }
                    else
                    {
                        RotValue = 10f;
                    }
                }
                else
                {
                    RotValue = 0.0f;
                    // rigBod.angularDrag = 1000;
                }

                if (distDiff > (moveDistance * 0.05f))
                {
                    movVal = 2.0f;
                }
                else
                {
                    movVal = 0;
                    // controller.Velocity = Vector3.zero;
                    controller.Acceleration = 0;
                    findNewPoint = true;
                }
                controller.RotJerk = RotValue;
                controller.Jerk = movVal;
            }
        }    
    }
}
