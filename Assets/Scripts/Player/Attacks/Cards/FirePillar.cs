using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePillar : Attack
{
    //Use as an example of a SUSTAINED attack (only 1 active + only active while button held)

    public override bool Init(bool held)
    {
        if (!held)
        {
            chargeAmount = 0.25f;
            chargeTime = Time.time;
            timeout = 0.25f;
            return true;
        }
        else
        {
            GameObject[] pillars = GameObject.FindGameObjectsWithTag("FirePillar");
            foreach(GameObject original in pillars)
            {
                if (original.Equals(this.gameObject))
                    continue;
                else
                {
                    original.GetComponent<Attack>().timeoutTime = Time.time;
                }
            }

            return false;
        }
    }

    public override void Spawn(int dir)
    {
        gameObject.SetActive(true);
        direction = dir;

        transform.Translate(new Vector3(3.0f, 1.0f, 0));
    }

    public override void Move()
    {

    }

    public override bool CheckCharge()
    {
        if (chargeTime == 0)
            return false;

        if (chargeTime + chargeAmount < Time.time)
        {
            chargeTime = 0;
            timeoutTime = Time.time;
            return true;
        }

        return false;
    }

    public override bool CheckTimeout()
    {
        if (timeoutTime != 0 && timeoutTime + timeout < Time.time)
        {
            attacker.UnlockMovement();
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
