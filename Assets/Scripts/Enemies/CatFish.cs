using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatFish : MonoBehaviour
{
    public GameObject manager;
    public float xBoundLower;
    public float xBoundUpper;
    private bool onUpper;
    private GameObject player;
    private Animator anim;
    private SpriteRenderer sr;

    public GameObject catWarning;
    public GameObject catCollider;

    public float walkSpeed;
    public float attackSpeed;
    private float nextAttack;
    public int numAttacks;

    private bool inAttack;
    private float chargeEnd;
    private int selectedAttack;

    private int attackDir;
    private bool inOffset;

    private float wanderDir;

    private float catSpeed;
    // Start is called before the first frame update
    void Start()
    {
        nextAttack = Time.time + Random.Range(attackSpeed * 0.8f, attackSpeed * 1.2f);
        player = GameObject.FindGameObjectWithTag("CombatPlayer");
        sr = gameObject.GetComponent<SpriteRenderer>();
        anim = gameObject.GetComponent<Animator>();
        manager = GameObject.FindGameObjectWithTag("CombatManager");

        int r = Random.Range(0, 2);
        if (r == 0)
            r = -5;
        else
            r = 5;
        Vector3 startPos = new Vector3(r, -2.0f, 9);
        this.transform.localPosition = startPos;
    }

    // Update is called once per frame
    void Update()
    {
        if(!inAttack && nextAttack < Time.time)
        {
            selectedAttack = Random.Range(0, 100);
            if (selectedAttack < 10) //Wander
                Wander();
            else if (selectedAttack < 20) //Teleport
                Teleport();
            else if (selectedAttack < 60) //Attack 1
                CatAttack();
            else //Attack 2
                Teleport();

        }
        else if(inAttack)
        {
            if (selectedAttack < 10) //Wander
                Wander();
            else if (selectedAttack < 20) //Teleport
                Teleport();
            else if (selectedAttack < 60) //Attack 1
                CatAttack();
            else //Attack 2
                FishAttack();
        }
        else
        {
            attackDir = (player.transform.localPosition.x <= transform.localPosition.x) ? -1 : 1;
            if (attackDir == -1)
                sr.flipX = true;
            else
                sr.flipX = false;
        }

        if(!onUpper)
        {
            if (transform.localPosition.x < -xBoundLower)
                transform.localPosition = new Vector3(-xBoundLower, transform.position.y, 9);
            else if (transform.localPosition.x > xBoundLower)
                transform.localPosition = new Vector3(xBoundLower, transform.position.y, 9);
        }
        else
        {
            if (transform.localPosition.x < -xBoundUpper)
                transform.localPosition = new Vector3(-xBoundUpper, transform.position.y, 9);
            else if (transform.localPosition.x > xBoundUpper)
                transform.localPosition = new Vector3(xBoundUpper, transform.position.y, 9);
        }
    }

    private void CatAttack()
    {
        if(!inAttack)
        {
            //Initial Attack
            chargeEnd = Time.time + 3.0f;
            inAttack = true;
            catSpeed = Random.Range(walkSpeed * 1.25f, walkSpeed * 1.5f);
        }
        else if(chargeEnd < Time.time)
        {
            //Launch Attack / Reset
            anim.SetInteger("State", 0);
            nextAttack = Time.time + Random.Range(attackSpeed * 0.8f, attackSpeed * 1.2f);

            inAttack = false;
            if(inOffset)
            {
                transform.Translate(Vector3.down);
                inOffset = false;
            }

            catWarning.SetActive(false);
            manager.GetComponent<EnemyManager>().RemoveWarning(catWarning);
            Teleport();
        }
        else
        {
            //During Charge
            anim.SetInteger("State", 1);

            if(!catWarning.activeSelf && chargeEnd - Time.time < 1.65f && !inOffset)
            {
                catWarning.SetActive(true);
                manager.GetComponent<EnemyManager>().ShowWarnings();
            }

            if (!inOffset && chargeEnd - Time.time < 0.4f)
            {
                inOffset = true;
                transform.Translate(Vector3.up);
                anim.Play("CatSlash");
                catWarning.SetActive(false);
            }
            else if(!inOffset)
                transform.Translate(new Vector3(attackDir, 0, 0) * catSpeed * Time.deltaTime);
        }
    }

    private void FishAttack()
    {
        if (!inAttack)
        {
            //Initial Attack
            chargeEnd = Time.time + 1.0f;
            inAttack = true;
        }
        else if (chargeEnd < Time.time)
        {
            nextAttack = Time.time + Random.Range(attackSpeed * 0.8f, attackSpeed * 1.2f);
            inAttack = false;
        }
        else
        {

        }
    }

    private void Wander()
    {
        if (!inAttack)
        {
            //Initial Attack
            chargeEnd = Time.time + 3.0f;
            inAttack = true;
            wanderDir = Random.Range(-1.0f, 1.0f);
        }
        else if (chargeEnd < Time.time)
        {
            nextAttack = Time.time + Random.Range(attackSpeed * 0.8f, attackSpeed * 1.2f);
            inAttack = false;
        }
        else
        {
            transform.Translate(new Vector2(wanderDir, 0) * walkSpeed * Time.deltaTime);
        }
    }

    private void Teleport()
    {
        if (!inAttack)
        {
            Vector3 playerPos = player.transform.localPosition;

            Vector3[] positions = new Vector3[6];
            positions[0] = new Vector3(-xBoundLower, -2.0f, 9);
            positions[1] = new Vector3(xBoundLower, -2.0f, 9);
            positions[2] = new Vector3(0, -2.0f, 9);
            positions[3] = new Vector3(0, 2.5f, 9);
            positions[4] = new Vector3(-xBoundUpper, 2.5f, 9);
            positions[5] = new Vector3(xBoundUpper, 2.5f, 9);

            int r = Random.Range(0, 6);
            if(Vector2.Distance(playerPos, positions[r]) < 1.0f)
            {
                if (r == 0)
                    r = positions.Length - 1;
                else
                    r--;
            }

            transform.localPosition = positions[r];
            if (r >= 3)
            {
                onUpper = true;
                transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
            }
            else
            {
                onUpper = false;
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }

            nextAttack = Time.time + Random.Range(attackSpeed * 0.8f, attackSpeed * 1.2f);
        }
    }
}