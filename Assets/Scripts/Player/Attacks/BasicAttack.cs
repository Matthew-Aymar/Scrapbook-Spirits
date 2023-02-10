using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : Attack
{
    public GameObject particle;

    public override bool Init(bool held)
    {
        chargeAmount = 0.3f;
        chargeTime = Time.time;
        speed = 15.0f;
        timeout = 2.5f;

        if (held)
        {
            chargeAmount *= 0.5f;
            this.transform.localScale *= 0.5f;
        }

        return true;
    }

    public override void Spawn(int dir)
    {
        gameObject.SetActive(true);
        direction = dir;

        float randx = Random.Range(0.0f, 0.25f);
        float randy = Random.Range(-0.25f, 0.25f);

        transform.Translate(new Vector3(direction + randx, randy, 0));
        GameObject part = Instantiate(particle, this.transform.position, particle.transform.rotation);
        part.transform.localScale = this.transform.localScale * 1.5f;
        part.transform.Translate(new Vector3(this.transform.localScale.x * -0.4f, 0, 0), Space.World);
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

    public override bool CheckCollision()
    {
        return false;
    }
}
