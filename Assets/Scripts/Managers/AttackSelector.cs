using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSelector : MonoBehaviour
{
    public EnemyManager enemy;
    public GameObject[] attacks;
    public List<GameObject> activeAttacks = new List<GameObject>();
    public CardSelector cards;
    public DeckManager deck;
    public BuffManager buff;

    public GameObject attackParticle;
    public GameObject chargeParticle;
    public GameObject hitFx;

    private GameObject currentAttackParticle;
    private GameObject currentChargeParticle;

    public PlayerCombat pc;
    public bool inCharge;
    public bool isHeld;
    public float nextCheck;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Attack tempScript;
        for(int x = 0; x < activeAttacks.Count; x++)
        {
            tempScript = activeAttacks[x].GetComponent<Attack>();
            if (inCharge)
            {
                if(tempScript.CheckCharge())
                {
                    tempScript.Spawn(pc.attackDir, pc.onUpper);
                    SpawnParticle();

                    inCharge = false;
                    pc.EndCharge();
                    DestroyCharge();
                }
            }

            if(tempScript.CheckTimeout())
            {
                activeAttacks.RemoveAt(x);
                Destroy(tempScript.gameObject);
            }

            Collider2D hit = tempScript.CheckCollision();
            if(hit)
            {
                float dist = Vector3.Distance(hit.gameObject.transform.position, enemy.currentEnemy.transform.position);
                Vector3 between = Vector3.MoveTowards(hit.gameObject.transform.position, enemy.currentEnemy.transform.position, dist * 0.67f);
                GameObject fx = Instantiate(hitFx);
                fx.transform.position = between;
                fx.transform.Rotate(new Vector3(0, 0, 1), Random.Range(0, 360));

                if (tempScript.heavy)
                    enemy.StunEnemy(tempScript.stunDuration);

                bool wasCard = tempScript._id != 0;
                if(!wasCard)
                    enemy.Damage(tempScript.damage + buff.PlayerDamageMulti(true));
                else
                    enemy.Damage(tempScript.damage * buff.PlayerDamageMulti(false));

                if (tempScript.cursed)
                    buff.Curse(true);
            }

            tempScript.Move();
        }
    }

    public void NewAttack(bool wasCard)
    {
        if (inCharge)
            return;

        pc.movementLocked = true;

        GameObject newAttack;
        if (wasCard && cards.usingCard)
        {
            newAttack = Instantiate(attacks[cards.GetCardID()], pc.combatPlayer.transform);
            newAttack.GetComponent<Attack>()._id = cards.GetCardID();

            nextCheck = Time.time + 0.1f;
            CanCancel(false);
        }
        else
        {
            if (cards.usingCard)
                StopUsing();

            newAttack = Instantiate(attacks[0], pc.combatPlayer.transform);
            newAttack.GetComponent<Attack>()._id = 0;
        }

        newAttack.transform.parent = null;
        if (newAttack.GetComponent<Attack>().Init(isHeld))
        {
            if (newAttack.GetComponent<Attack>().limited)
                deck.RemoveLastDiscard();

            activeAttacks.Add(newAttack);
            newAttack.GetComponent<Attack>().attacker = this;
            inCharge = true;

            if (!currentChargeParticle && !pc.inAttackAnim)
            {
                currentChargeParticle = Instantiate(chargeParticle, pc.combatPlayer.transform);
            }
        }
        else
        {
            SpawnParticle();
            Destroy(newAttack.gameObject);
        }

        isHeld = true;
    }

    public void BreakHold()
    {
        if(isHeld)
        {
            isHeld = false;
        }
    }

    public void StopUsing()
    {
        cards.StopUsingCard();
    }

    public void UnlockMovement()
    {
        pc.movementLocked = false;
        cards.DropCards();
    }

    public void DestroyCharge()
    {
        if (currentChargeParticle)
            Destroy(currentChargeParticle);
    }

    public void SpawnParticle()
    {
        currentAttackParticle = Instantiate(attackParticle, pc.combatPlayer.transform);
        currentAttackParticle.transform.Translate(new Vector3(pc.attackDir * (pc.onUpper ? 1.25f : 1), 0, 0));
        currentAttackParticle.transform.parent = null;
    }

    public void CanCancel(bool cancellable)
    {
        pc.canCancel = cancellable;
    }

    public void JumpCancel()
    {
        for (int x = 0; x < activeAttacks.Count; x++)
        {
            GameObject temp = activeAttacks[x];
            if(activeAttacks[x].activeSelf == false)
            {
                activeAttacks.RemoveAt(x);
                Destroy(temp);
            }
        }

        inCharge = false;
        UnlockMovement();
        DestroyCharge();
        BreakHold();
        pc.SetIdle();
        cards.DropCards();
        cards.StopUsingCard();
    }
}
