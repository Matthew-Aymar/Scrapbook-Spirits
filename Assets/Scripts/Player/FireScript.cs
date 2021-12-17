using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireScript : MonoBehaviour
{
    public Transform pos1, pos2, pos3, pos4;
    public Transform atk1, atk2;
    public SpriteRenderer sr;
    public Animator anim;
    public GameObject ball;
    private bool inatk;

    public void SetPos(int p)
    {
        if (inatk)
            return;

        switch (p)
        {
            case 1:
                gameObject.transform.position = pos1.position;
                sr.flipX = false;
                break;
            case 2:
                gameObject.transform.position = pos2.position;
                sr.flipX = true;
                break;
            case 3:
                gameObject.transform.position = pos1.position;
                sr.flipX = false;
                break;
            case 4:
                gameObject.transform.position = pos2.position;
                sr.flipX = true;
                break;
        }
    }

    public void FireAttack()
    {
        inatk = true;
        if (sr.flipX)
            gameObject.transform.position = atk2.position;
        else
            gameObject.transform.position = atk1.position;

        anim.Play("Base Layer.Wisp_Fire_Attack", -1);
    }

    public void Shoot()
    {
        GameObject temp;
        temp = Instantiate(ball, this.transform.position, this.transform.rotation);

        if(sr.flipX)
        {
            temp.GetComponent<FireBall>().dir = Vector2.left;
        }
        else
        {
            temp.GetComponent<FireBall>().dir = Vector2.right;
        }

        temp.GetComponent<SpriteRenderer>().flipX = sr.flipX;
    }

    public void FinishAtk()
    {
        inatk = false;
    }
}
