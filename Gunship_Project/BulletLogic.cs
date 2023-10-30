using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLogic : MonoBehaviour
{
    float DestroyTimer = 4;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        DestroyTimer -= Time.deltaTime;
        if (DestroyTimer <= 0)
            Destroy(this.gameObject);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Drone")
        {
            collision.gameObject.GetComponentInChildren<HealthBar>().DealDamage(20);
            collision.gameObject.GetComponentInChildren<Rigidbody2D>().AddForce(transform.up * 500);
            Destroy(this.gameObject);
        }
        if (collision.gameObject.tag == "Bomb")
        {
            collision.gameObject.GetComponentInChildren<HealthBar>().DealDamage(20);
            Destroy(this.gameObject);
        }
        if (collision.gameObject.tag == "TrainCar")
        {
            collision.gameObject.GetComponentInChildren<HealthBar>().DealDamage(20);
            Destroy(this.gameObject);
        }
        if (collision.gameObject.tag == "KnockBull")
        {
            Destroy(collision.gameObject);
            Destroy(this.gameObject);
        }

    }
}
