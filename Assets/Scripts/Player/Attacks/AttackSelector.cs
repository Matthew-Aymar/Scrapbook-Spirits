using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSelector : MonoBehaviour
{
    public GameObject[] attacks;
    public List<GameObject> activeAttacks = new List<GameObject>();

    public PlayerCombat pc;
    private bool inCharge;
    private bool isHeld;
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
                    pc.SetIdle();
                    pc.movementLocked = false;
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

    public void newAttack(bool wasCard)
    {
        if (inCharge)
            return;

        if(!wasCard)
        {
            pc.movementLocked = true;
            GameObject newAttack = Instantiate(attacks[0], pc.combatPlayer.transform);
            activeAttacks.Add(newAttack);
            newAttack.transform.parent = null;
            newAttack.GetComponent<Attack>().Init(isHeld);

            inCharge = true;
            isHeld = true;
        }
    }

    public void breakHold()
    {
        isHeld = false;
    }
}
