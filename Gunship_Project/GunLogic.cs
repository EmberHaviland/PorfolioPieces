using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunLogic : MonoBehaviour
{
    [SerializeField] float MaxFireRate = 10;
    [SerializeField] float FireRate = 0;
    [SerializeField] float RampRate = 4;
    [SerializeField] float KnockBack = 4f;
    [SerializeField] Rigidbody2D RigBod;

    public GameObject bulletPrefab;
    float timeHold = 0.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timeHold += Time.deltaTime;
        if (timeHold > (1 / FireRate))
        {
            Instantiate(bulletPrefab, transform.position, transform.rotation);
            RigBod.AddForce(RigBod.gameObject.transform.up * -KnockBack);
            timeHold -= (1 / FireRate);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            FireRate += RampRate * Time.deltaTime;
            if (FireRate > MaxFireRate)
                FireRate = MaxFireRate;
        }
        else
        {
            FireRate -= RampRate * 2 * Time.deltaTime;
            if (FireRate < 0)
                FireRate = 0;
        }


    }
}
