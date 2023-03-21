using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePillar : Attack
{
    //Use as an example of a SUSTAINED attack (only 1 active + only active while button held)
    public BoxCollider2D col;
    private bool canHit;
    private float lastHit;

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

    public override void Spawn(int dir, bool upper)
    {
        gameObject.SetActive(true);
        direction = dir;

        transform.Translate(new Vector3((dir * 3.0f) + dir * (upper ? 0.25f : 0), 1.0f, 0));

        if (dir == -1)
            gameObject.GetComponent<SpriteRenderer>().flipX = true;

        col = gameObject.GetComponent<BoxCollider2D>();
    }

    public override void Move()
    {
        if (!canHit && lastHit < Time.time - 0.1f)
            canHit = true;
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
            attacker.CanCancel(true);
            Destroy(this.gameObject);
            return true;
        }
        
        return false;
    }

    public override Collider2D CheckCollision()
    {
        if (!this.gameObject.activeSelf || !canHit)
            return null;

        Collider2D[] cols = new Collider2D[5];
        col.OverlapCollider(new ContactFilter2D().NoFilter(), cols);
        foreach (Collider2D c in cols)
        {
            if (!c)
                continue;

            if (c.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                lastHit = Time.time;
                canHit = false;
                return col;
            }
        }

        return null;
    }
}
