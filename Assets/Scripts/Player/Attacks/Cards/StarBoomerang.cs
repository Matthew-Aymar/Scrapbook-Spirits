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
    private int ampStage;
    private float ampStep;

    public GameObject AmpParticle;

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
                    original.GetComponent<StarBoomerang>().totalAmp += Time.time - original.GetComponent<StarBoomerang>().lastAmp;
                    original.GetComponent<StarBoomerang>().lastAmp = Time.time;

                    return false;
                }
            }
        }

        ampStage = 1;
        returnAmount = 0.5f;
        chargeAmount = 0.25f;
        chargeTime = Time.time;
        speed = 15.0f;
        timeout = 5.0f;
        timeoutTime = 0;
        ampStep = 1.0f;

        return true;
    }

    public override void Spawn(int dir, bool upper)
    {
        gameObject.SetActive(true);
        direction = dir;
        reversal = dir;

        transform.Translate(new Vector3(dir + dir * (upper ? 0.25f : 0), 0, 0));

        lastAmp = Time.time;

        if(dir == -1)
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
    }

    public override void Move()
    {
        if (this.gameObject.activeSelf)
        {
            if(!letGo && lastAmp < Time.time - 0.25f)
            {
                attacker.UnlockMovement();
                attacker.CanCancel(true);
                letGo = true;
                timeoutTime = Time.time;
                returnTime = Time.time;
            }
            else if(!letGo)
            {
                if(ampStage < 3 && totalAmp > ampStage * ampStep)
                {
                    transform.localScale *= 1.5f;
                    ampStage++;

                    GameObject part = Instantiate(AmpParticle, this.transform.localPosition, new Quaternion());
                    part.transform.localScale = this.transform.localScale;
                    if(direction == -1)
                        part.GetComponent<SpriteRenderer>().flipX = true;
                }
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
            attacker.CanCancel(true);
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