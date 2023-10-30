using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleLogic : MonoBehaviour
{
    public float maxSpeed = 10.0f;
    bool isIn = false;
    GameObject playerObj;
    // Transforms to act as start and end markers for the journey.
    public Transform startMarker;
    public Transform endMarker;

    // Movement speed in units per second.
    public float speed = 0.5f;

    // Time when the movement started.
    private float startTime;

    // Total distance between the markers.
    private float journeyLength = 1f;

    void Start()
    {

    }

    // Move to the target end position.
    void Update()
    {
        if (isIn) 
        {
            // Distance moved equals elapsed time times speed..
            float distCovered = (Time.time - startTime) * speed;

            // Fraction of journey completed equals current distance divided by total distance.
            float fractionOfJourney = distCovered / journeyLength;

            // Set our position as a fraction of the distance between the markers.
            playerObj.transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fractionOfJourney);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null) 
        {
            if (collision.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude < maxSpeed)
            {
                startMarker = collision.gameObject.transform;
                endMarker = this.gameObject.transform;

                // Keep a note of the time the movement started.
                startTime = Time.time;

                // Calculate the journey length.
                journeyLength = Vector3.Distance(startMarker.position, endMarker.position);

                collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                playerObj = collision.gameObject;
                isIn = true;
                playerObj.GetComponent<PlayerBallLogic>().roundOver = true;
            }
            else
            {
                // Change angle of ball's travel, do so in this case by added a force maybe using the direction vector between the ball and the hole
                // To avoid issue of accidentally accelerating the ball either set a max velocity of the ball or calculate using positive/negative signs
                // on coordinate quadrants to determine when the ball has passed the hole :3
            }
        }
    }
}
