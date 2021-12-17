using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatFish : MonoBehaviour
{
    public States current_state;
    public enum States
    {
        Idle    = 0b_0000_0000,     // 0
        Cat     = 0b_0000_0001,     // 1
        Fish    = 0b_0000_0010,     // 2
    }

    private Animator anim;
    private SpriteRenderer sr;
    private GameObject[] teleports;
    public GameObject player;

    private float lastthink;
    public float thinktimer;

    public float lastattack;
    public float attacktimer;

    private float catoffset = -0.67f;
    private Vector3 catdir;
    private float cattimer;
    public float catspeed;

    private Vector3 fishdir;
    public float fishspeed;

    private bool isoffset = false;
    private int numactions = 3;

    public float maxhp;
    public float currenthp;
    private bool isDead = false;
    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        teleports = GameObject.FindGameObjectsWithTag("Teleport");
        lastthink = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (currenthp <= 0)
        {
            isDead = true;
            player.GetComponent<PlayerMovement>().fadeout = true;
            Destroy(this.gameObject);
        }

        if (isDead)
            return;

        if(Time.time - lastthink >= thinktimer)
        {
            think();
            lastthink = Time.time;
        }

        if((current_state & States.Cat) == States.Cat)
        {
            if(!isoffset)
            {
                transform.Translate(Vector3.up * catoffset);
                isoffset = true;
            }

            if (cattimer <= 0)
            {
                if(isoffset)
                    transform.Translate(Vector3.up * (-1 * catoffset));

                current_state -= States.Cat;
                anim.SetInteger("State", 0);
                teleport();
            }

            transform.Translate(catdir * catspeed * Time.deltaTime);

            cattimer -= Time.deltaTime;
        }
        else if((current_state & States.Fish) == States.Fish)
        {
            transform.Translate(fishdir * fishspeed * Time.deltaTime);

            if(transform.position.y < -8)
            {
                anim.SetInteger("State", 0);
                current_state -= States.Fish;
                teleport();
            }
        }
        else
        {
            faceplayer();
        }
    }

    public void think()
    {
        if ((current_state & States.Cat) == States.Cat || (current_state & States.Fish) == States.Fish)
            return;

        int selection = Random.Range(0, numactions + 1);
        switch(selection)
        {
            case 1:
                teleport();
                break;
            case 2:
                catattack();
                break;
            case 3:
                fishattack();
                break;
        }
    }

    public void teleport()
    {
        int index;
        GameObject port;
        index = Random.Range(0, teleports.Length);
        port = teleports[index];

        while (Vector3.Distance(player.transform.position, port.transform.position) < 5)
        {
            index = Random.Range(0, teleports.Length);
            port = teleports[index];
        }

        gameObject.transform.position = port.transform.position;
    }

    public void catattack()
    {
        current_state |= States.Cat;

        transform.Translate(Vector3.up * catoffset);
        catdir = new Vector3(player.transform.position.x - gameObject.transform.position.x, 0, 0);
        catdir.Normalize();
        if (catdir.x < 0)
        {
            sr.flipX = false;
        }
        else
        {
            sr.flipX = true;
        }

        anim.SetInteger("State", 1);
        isoffset = true;
        lastattack = Time.time;
        cattimer = 3.0f;
    }

    public void fishattack()
    {
        current_state |= States.Fish;

        fishdir = new Vector3(0, -1, 0);
        gameObject.transform.position = new Vector3(player.transform.position.x, 15, 0);

        anim.SetInteger("State", 2);

        lastattack = Time.time;
    }

    public void faceplayer()
    {
        Vector3 dir = new Vector3(player.transform.position.x - gameObject.transform.position.x, 0, 0);

        if(dir.x < 0)
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if((current_state & States.Cat) == States.Cat)
            {
                anim.Play("CatSlash");

                if (isoffset)
                    transform.Translate(Vector3.up * (-1 * catoffset));

                current_state -= States.Cat;
                anim.SetInteger("State", 0);
                transform.position = player.transform.position;

                player.GetComponent<PlayerMovement>().currenthp -= 20;
            }
            else if((current_state & States.Fish) == States.Fish)
            {
                player.GetComponent<PlayerMovement>().currenthp -= 10;
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Fire"))
        {
            Destroy(collision.gameObject);
            currenthp -= 5;
        }
    }
}
