using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Meditate : Attack
{
    public BuffManager buffer;
    public Image fade;

    public override bool Init(bool held)
    {
        if (held)
            return false;

        chargeAmount = 0.5f;
        chargeTime = Time.time;
        timeout = 5.0f;

        fade = GameObject.FindGameObjectWithTag("Fade").GetComponent<Image>();
        fade.color = new Color(0, 0, 0, 0);
        buffer = GameObject.FindGameObjectWithTag("Buffer").GetComponent<BuffManager>();
        return true;
    }

    public override void Spawn(int dir, bool upper)
    {
        transform.parent = Camera.main.transform;
        transform.localPosition = new Vector3(0, 0, 10);
        gameObject.SetActive(true);

        buffer.Meditate();

        Time.timeScale = 0.0f;
        fade.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
    }

    public override void Move()
    {
        return;
    }

    public override bool CheckCharge()
    {
        if (chargeTime == 0)
            return false;

        fade.color = new Color(0, 0, 0, 0.5f - (chargeTime + chargeAmount - Time.time));

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
            attacker.StopUsing();
            Destroy(this.gameObject);
            return true;
        }
        return false;
    }

    public void FinishAnimation()
    {
        fade.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        timeoutTime = Time.time - timeout;
        Time.timeScale = 1.0f;
    }

    public override Collider2D CheckCollision()
    {
        return null;
    }
}
