using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirghterBurn : Attack
{
    public GameObject part;
    public GameObject player;
    public BuffManager buffer;

    public override bool Init(bool held)
    {
        if (held)
            return false;

        chargeAmount = 0.5f;
        chargeTime = Time.time;
        timeout = 0.0f;

        player = GameObject.FindGameObjectWithTag("CombatPlayer");
        buffer = GameObject.FindGameObjectWithTag("Buffer").GetComponent<BuffManager>();
        return true;
    }

    public override void Spawn(int dir, bool upper)
    {
        gameObject.SetActive(true);
        Instantiate(part, player.transform);
        buffer.Brighten();
    }

    public override void Move()
    {
        return;
    }

    public override bool CheckCharge()
    {
        if (chargeTime == 0)
            return false;

        if (chargeTime + chargeAmount < Time.time)
        {
            chargeTime = 0;
            timeoutTime = Time.time;
            attacker.CanCancel(true);
            attacker.UnlockMovement();
            attacker.BreakHold();
            attacker.StopUsing();
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

    public override Collider2D CheckCollision()
    {
        return null;
    }
}
