using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourSeasons : Attack
{
    public CircleCollider2D[] cols = new CircleCollider2D[4];
    public Vector3[] initialPositions = new Vector3[4];
    public float xBoundUpper;
    public float xBoundLower;
    public float yBoundUpper;
    public float yBoundLower;

    public GameObject part;
    private int colToDestroy;
    public override bool Init(bool held)
    {
        if (held)
            return false;

        //Only one can be active at a time, but should refresh if one was hit already
        GameObject prev = GameObject.FindGameObjectWithTag("FourSeasons");
        if (prev != null)
        {
            foreach(CircleCollider2D leftOverCols in prev.GetComponent<FourSeasons>().cols)
            {
                if(leftOverCols)
                {
                    Destroy(leftOverCols.gameObject);
                }
            }
        }

        speed = 2;
        colToDestroy = -1;
        chargeAmount = 1.5f;
        chargeTime = Time.time;
        timeout = 60.0f;

        return true;
    }

    public override void Spawn(int dir, bool upper)
    {
        transform.parent = Camera.main.transform;
        transform.localPosition = new Vector3(0, 0, 11);
        transform.localScale = new Vector3(1, 1, 1);

        cols = gameObject.GetComponentsInChildren<CircleCollider2D>();
        CircleCollider2D temp;
        for (int i = 0; i < cols.Length; i++)
        {
            int rnd = Random.Range(0, cols.Length);
            temp = cols[rnd];
            cols[rnd] = cols[i];
            cols[i] = temp;
        }

        cols[0].gameObject.transform.localPosition = new Vector3(xBoundUpper, yBoundUpper, 0);
        cols[1].gameObject.transform.localPosition = new Vector3(xBoundLower, yBoundLower, 0);
        cols[2].gameObject.transform.localPosition = new Vector3(-xBoundUpper, yBoundUpper, 0);
        cols[3].gameObject.transform.localPosition = new Vector3(-xBoundLower, yBoundLower, 0);

        initialPositions[0] = cols[0].transform.position;
        initialPositions[1] = cols[1].transform.position;
        initialPositions[2] = cols[2].transform.position;
        initialPositions[3] = cols[3].transform.position;

        Instantiate(part, cols[0].transform);
        Instantiate(part, cols[1].transform);
        Instantiate(part, cols[2].transform);
        Instantiate(part, cols[3].transform);

        gameObject.SetActive(true);
    }

    public override void Move()
    {
        if(this.gameObject.activeSelf)
        {
            if (colToDestroy != -1)
            {
                Destroy(cols[colToDestroy].gameObject);
                colToDestroy = -1;
            }

            for (int x = 0; x < cols.Length; x++)
            {
                if (cols[x])
                {
                    //Wiggle a bit
                    Vector3 oldPos = cols[x].transform.position;
                    cols[x].transform.Translate(new Vector3(Random.Range(-speed * Time.deltaTime, speed * Time.deltaTime), Random.Range(-speed * Time.deltaTime, speed * Time.deltaTime), 0));
                    if (Vector3.Distance(initialPositions[x], cols[x].transform.position) > 0.5f)
                    {
                        cols[x].transform.position = oldPos;
                    }
                }
            }
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

        Collider2D[] currentColisions = new Collider2D[5];
        bool anyLeft = false;
        int count = 0;
        foreach (Collider2D col in cols)
        {
            if(col)
            {
                anyLeft = true;
                col.OverlapCollider(new ContactFilter2D().NoFilter(), currentColisions);

                foreach (Collider2D c in currentColisions)
                {
                    if (!c)
                        continue;

                    if (c.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                    {
                        colToDestroy = count;
                        return col;
                    }
                }
            }

            count++;
        }

        if (!anyLeft)
            timeoutTime = Time.time - timeout;

        return null;
    }
}
