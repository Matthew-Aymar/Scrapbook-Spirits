using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSwipe : Attack
{
    public override bool Init(bool held)
    {
        chargeAmount = 0.5f;
        chargeTime = Time.time;
        speed = 15.0f;
        timeout = 0.35f;

        return true;
    }

    public override void Spawn(int dir)
    {
        gameObject.SetActive(true);
        direction = dir;

        transform.Translate(new Vector3(direction * 1.5f, 0.5f, 0));
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
            attacker.UnlockMovement();
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
