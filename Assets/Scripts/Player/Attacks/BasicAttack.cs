using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : Attack
{
    public GameObject particle;
    private BoxCollider2D col;

    public override bool Init(bool held)
    {
        damage = 5;
        chargeAmount = 0.3f;
        chargeTime = Time.time;
        speed = 15.0f;
        timeout = 2.5f;

        if (held)
        {
            damage = 1;
            chargeAmount *= 0.5f;
            this.transform.localScale *= 0.5f;
        }

        return true;
    }

    public override void Spawn(int dir, bool upper)
    {
        gameObject.SetActive(true);
        direction = dir;

        float randx = Random.Range(0.0f, 0.25f);
        float randy = Random.Range(-0.25f, 0.25f);

        transform.Translate(new Vector3(direction + randx + (upper ? (dir * 0.5f) : 0), randy, 0));
        GameObject part = Instantiate(particle, this.transform.position, new Quaternion());
        part.transform.localScale = this.transform.localScale * 1.5f;
        part.transform.Translate(new Vector3(dir * (this.transform.localScale.x * -0.4f), 0, 0), Space.World);
        part.transform.Rotate(0, 0, dir * particle.transform.rotation.eulerAngles.z);
        if (dir == -1)
        {
            part.GetComponent<SpriteRenderer>().flipX = false;
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }

        col = gameObject.GetComponent<BoxCollider2D>();
    }

    public override void Move()
    {
        if(this.gameObject.activeSelf)
            transform.Translate(direction * speed * Time.deltaTime, 0, 0);
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

    public override Collider2D CheckCollision()
    {
        if (!this.gameObject.activeSelf)
            return null;

        Collider2D[] cols = new Collider2D[5];
        col.OverlapCollider(new ContactFilter2D().NoFilter(), cols);
        foreach(Collider2D c in cols)
        {
            if (!c)
                continue;

            if(c.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                timeoutTime = Time.time - timeout;
                return col;
            }
        }

        return null;
    }
}
