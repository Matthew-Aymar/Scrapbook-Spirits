using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarBoomerang : Attack
{
    //Use as an example of an AMPING attack (effect changes as held + launches when released)

    private float returnTime;
    private float returnAmount;
    private float reversal;

    public bool letGo;
    public float lastAmp;
    public float totalAmp;
    public override bool Init(bool held)
    {
        if (held)
        {
            GameObject[] stars = GameObject.FindGameObjectsWithTag("StarBoomerang");
            foreach (GameObject original in stars)
            {
                if (original.Equals(this.gameObject))
                    continue;
                else if(!original.GetComponent<StarBoomerang>().letGo)
                {
                    original.GetComponent<StarBoomerang>().totalAmp = Time.time - original.GetComponent<StarBoomerang>().lastAmp;
                    original.GetComponent<StarBoomerang>().lastAmp = Time.time;

                    return false;
                }
            }
        }

        returnAmount = 0.5f;
        chargeAmount = 0.25f;
        chargeTime = Time.time;
        speed = 15.0f;
        timeout = 5.0f;
        timeoutTime = 0;

        return true;
    }

    public override void Spawn(int dir)
    {
        gameObject.SetActive(true);
        direction = dir;
        reversal = dir;

        float randx = Random.Range(0.0f, 0.25f);
        float randy = Random.Range(-0.25f, 0.25f);

        transform.Translate(new Vector3(direction + randx, randy, 0));

        lastAmp = Time.time;
    }

    public override void Move()
    {
        if (this.gameObject.activeSelf)
        {
            if(!letGo && lastAmp < Time.time - 0.25f)
            {
                attacker.UnlockMovement();
                letGo = true;
                timeoutTime = Time.time;
                returnTime = Time.time;
            }

            if(letGo)
            {
                if (returnTime + returnAmount < Time.time)
                {
                    reversal -= direction * Time.deltaTime * 2.0f;
                }

                transform.Translate(reversal * speed * Time.deltaTime, 0, 0, Space.World);
            }

            transform.Rotate(new Vector3(0, 0, 1), 180 * Time.deltaTime * direction * Mathf.Abs(reversal));
        }
    }

    public override bool CheckCharge()
    {
        if (chargeTime == 0)
            return false;

        if (chargeTime + chargeAmount < Time.time)
        {
            chargeTime = 0;
            return true;
        }

        return false;
    }

    public override bool CheckTimeout()
    {
        if (timeoutTime != 0 && timeoutTime + timeout < Time.time)
        {
            Destroy(this.gameObject);
            return true;
        }
        return false;
    }

    public override bool CheckCollision()
    {
        return false;
    }
}
