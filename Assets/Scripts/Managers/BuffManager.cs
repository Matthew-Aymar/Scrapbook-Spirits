using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    //Player Buffs
    private int flameLevel;
    private int meditation;

    //Player Debuffs
    private bool p_cursed;
    private int p_douses;

    //Enemy Buffs

    //Enemy Debuffs
    private bool e_cursed;
    private int e_douses;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float PlayerDamageMulti(bool isBasic)
    {
        if(isBasic)
        {
            return flameLevel;
        }
        else
        {
            float multi = 1;
            if(p_douses > 0)
                multi /= p_douses;

            if (e_cursed)
            {
                multi *= 3;
                e_cursed = false;
            }

            return multi;
        }
    }

    public float EnemyDamageMulti()
    {
        float multi = 1;
        if(e_douses > 0)
            multi /= e_douses;

        if (p_cursed)
        {
            multi *= 3;
            p_cursed = false;
        }

        return multi;
    }

    public int GetMeditationStacks() { return meditation; }

    public void Brighten() { flameLevel++; }
    public void Meditate() { meditation++; }

    //public void debuff(bool enemy) { if (enemy) ; else ;}
    public void Curse(bool enemy) { if (enemy) e_cursed = true; else p_cursed = true; }
    public void Douse(bool enemy) { if (enemy) e_douses++; else p_douses++; }
}
