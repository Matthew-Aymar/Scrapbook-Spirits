using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottledSpite : Attack
{
    private CircleCollider2D col;
    private GameObject enemy;

    private Vector2 targetDir;
    private Vector2 currentDir;
    private float turningSpeed;

    public override bool Init(bool held)
    {
        if (held)
            return false;

        chargeAmount = 0.3f;
        chargeTime = Time.time;
        speed = 2.5f;
        timeout = 15.0f;

        turningSpeed = 1.5f;

        enemy = GameObject.FindGameObjectWithTag("Enemy");

        return true;
    }

    public override void Spawn(int dir, bool upper)
    {
        gameObject.SetActive(true);
        direction = dir;

        transform.Translate(new Vector3(direction + (upper ? (dir * 0.5f) : 0), 0, 0));
        if (dir == -1)
        {
            currentDir = Vector3.left;
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
            currentDir = Vector3.right;

        col = gameObject.GetComponent<CircleCollider2D>();
    }

    public override void Move()
    {
        if(gameObject.activeSelf)
        {
            targetDir = new Vector2(enemy.transform.position.x - transform.position.x, enemy.transform.position.y - transform.position.y);
            targetDir.Normalize();

            currentDir = Vector3.RotateTowards(currentDir, targetDir, turningSpeed * Time.deltaTime, 0);
            currentDir.Normalize();

            transform.Translate(currentDir * Time.deltaTime * speed);
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
                timeoutTime = Time.time - timeout;
                return col;
            }
        }

        return null;
    }
}
