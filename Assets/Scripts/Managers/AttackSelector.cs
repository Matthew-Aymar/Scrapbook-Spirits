using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSelector : MonoBehaviour
{
    public GameObject[] attacks;
    public List<GameObject> activeAttacks = new List<GameObject>();
    public CardSelector cards;

    public PlayerCombat pc;
    public bool inCharge;
    private bool isHeld;
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
                    tempScript.Spawn(1);

                    inCharge = false;
                }
            }

            if(tempScript.CheckTimeout())
            {
                activeAttacks.RemoveAt(x);
                Destroy(tempScript.gameObject);
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
        if (!wasCard)
        {
            newAttack = Instantiate(attacks[0], pc.combatPlayer.transform);
        }
        else
        {
            newAttack = Instantiate(attacks[cards.GetCardID()], pc.combatPlayer.transform);
            nextCheck = Time.time + 0.1f;
        }

        newAttack.transform.parent = null;
        if (newAttack.GetComponent<Attack>().Init(isHeld))
        {
            activeAttacks.Add(newAttack);
            newAttack.GetComponent<Attack>().attacker = this;
            inCharge = true;
        }
        else
        {
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

    public void UnlockMovement()
    {
        pc.movementLocked = false;
    }
}
