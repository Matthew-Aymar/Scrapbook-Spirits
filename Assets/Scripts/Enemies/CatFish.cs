using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatFish : MonoBehaviour
{
    public GameObject manager;
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
    // Start is called before the first frame update
    void Start()
    {
        nextAttack = Time.time + Random.Range(attackSpeed * 0.8f, attackSpeed * 1.2f);
        player = GameObject.FindGameObjectWithTag("CombatPlayer");
        sr = gameObject.GetComponent<SpriteRenderer>();
        anim = gameObject.GetComponent<Animator>();
        manager = GameObject.FindGameObjectWithTag("CombatManager");
    }

    // Update is called once per frame
    void Update()
    {
        if(!inAttack && nextAttack < Time.time)
        {
            selectedAttack = Random.Range(0, numAttacks);
            switch (selectedAttack)
            {
                case 0:
                    CatAttack();
                    break;
                case 1:
                    FishAttack();
                    break;
            }
        }
        else if(inAttack)
        {
            switch (selectedAttack)
            {
                case 0:
                    CatAttack();
                    break;
                case 1:
                    FishAttack();
                    break;
            }
        }
        else
        {
            attackDir = (player.transform.localPosition.x <= transform.localPosition.x) ? -1 : 1;
            if (attackDir == 1)
                sr.flipX = true;
            else
                sr.flipX = false;
        }
    }

    private void CatAttack()
    {
        if(!inAttack)
        {
            //Initial Attack
            chargeEnd = Time.time + 3.0f;
            inAttack = true;
        }
        else if(chargeEnd < Time.time)
        {
            //Launch Attack / Reset
            anim.SetInteger("State", 0);
            nextAttack = Time.time + Random.Range(attackSpeed * 0.8f, attackSpeed * 1.2f);

            inAttack = false;

            catWarning.SetActive(false);
            manager.GetComponent<EnemyManager>().RemoveWarning(catWarning);
        }
        else
        {
            //During Charge
            anim.SetInteger("State", 1);
            transform.Translate(new Vector3(attackDir, 0, 0) * walkSpeed * Time.deltaTime);

            if(!catWarning.activeSelf && chargeEnd - Time.time < 1.25f)
            {
                catWarning.SetActive(true);
                manager.GetComponent<EnemyManager>().ShowWarnings();
            }
        }
    }

    private void FishAttack()
    {

    }
}
