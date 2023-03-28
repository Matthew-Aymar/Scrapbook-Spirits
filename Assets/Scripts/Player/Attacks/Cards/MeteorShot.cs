using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorShot : Attack
{
    private CircleCollider2D col;
    public GameObject part;

    public override bool Init(bool held)
    {
        if (held)
            return false;

        damage = 50;
        chargeAmount = 1.5f;
        chargeTime = Time.time;
        speed = 30.0f;
        timeout = 10.0f;
        heavy = true;
        stunDuration = 3.0f;

        return true;
    }

    public override void Spawn(int dir, bool upper)
    {
        gameObject.SetActive(true);
        direction = dir;

        transform.Translate(new Vector3(direction + (upper ? (dir * 0.5f) : 0), 0, 0));
        if (dir == -1)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }

        col = gameObject.GetComponent<CircleCollider2D>();

        gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0.25f * direction * speed, 0.65f * speed), ForceMode2D.Impulse);
    }

    public override void Move()
    {
        if (gameObject.activeSelf)
        {

        }
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
        if (!this.gameObject.activeSelf)
            return null;

        Collider2D[] cols = new Collider2D[5];
        col.OverlapCollider(new ContactFilter2D().NoFilter(), cols);
        foreach (Collider2D c in cols)
        {
            if (!c)
                continue;

            if (c.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                GameObject p = Instantiate(part, transform);
                p.transform.Rotate(new Vector3(0, 0, Random.Range(0, 360)));
                p.transform.parent = null;
                timeoutTime = Time.time - timeout;
                return col;
            }
        }

        return null;
    }
}
