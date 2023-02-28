using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSwipe : Attack
{
    public override bool Init(bool held)
    {
        if (held)
            return false;

        chargeAmount = 0.5f;
        chargeTime = Time.time;
        speed = 15.0f;
        timeout = 0.35f;

        return true;
    }

    public override void Spawn(int dir, bool upper)
    {
        gameObject.SetActive(true);
        direction = dir;

        transform.Translate(new Vector3((dir * 1.5f) + dir * (upper ? 0.25f : 0), 0.5f, 0));

        if (dir == -1)
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
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
            attacker.CanCancel(true);
            attacker.UnlockMovement();
            attacker.BreakHold();
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
