using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffManager : MonoBehaviour
{
    public Sprite[] icons;
    public GameObject iconObj;

    public GameObject playerBuffs;
    public GameObject enemyBuffs;

    private bool shouldUpdate;

    //Player Buffs
    public int flameLevel;
    public int meditation;

    //Player Debuffs
    public int p_cursed;
    public int p_douses;

    //Enemy Buffs

    //Enemy Debuffs
    public int e_cursed;
    public int e_douses;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(shouldUpdate)
            UpdateIcons();
    }

    public void UpdateIcons()
    {
        int[] p_buffs = new int[] { p_cursed, p_douses, flameLevel, meditation };
        //int[] e_buffs = new int[] { e_cursed, e_douses };

        foreach (Transform child in playerBuffs.transform) 
        {
            Destroy(child.gameObject);
        }

        int totalBuffs = 0;
        for(int x = 0; x < p_buffs.Length; x++)
        {
            if(p_buffs[x] > 0)
            {
                GameObject newIcon = Instantiate(iconObj, playerBuffs.transform);
                newIcon.GetComponent<Image>().sprite = icons[x];
                newIcon.GetComponentInChildren<TMP_Text>().text = "";

                newIcon.transform.localPosition = new Vector3(60 * totalBuffs, 0, 0);

                if (p_buffs[x] > 1)
                    newIcon.GetComponentInChildren<TMP_Text>().text = "" + p_buffs[x];

                totalBuffs++;
            }
        }

        shouldUpdate = false;
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

            if (e_cursed == 1)
            {
                multi *= 3;
                e_cursed = 0;
            }

            return multi;
        }
    }

    public float EnemyDamageMulti()
    {
        float multi = 1;
        if(e_douses > 0)
            multi /= e_douses;

        if (p_cursed == 1)
        {
            multi *= 3;
            p_cursed = 0;
        }

        return multi;
    }

    public int GetMeditationStacks() { shouldUpdate = true; return meditation;}

    public void Brighten() { flameLevel++; shouldUpdate = true; }
    public void Meditate() { meditation++; shouldUpdate = true; }

    //public void debuff(bool enemy) { if (enemy) ; else ;}
    public void Curse(bool enemy) { if (enemy) e_cursed = 1; else p_cursed = 1; shouldUpdate = true; }
    public void Douse(bool enemy) { if (enemy) e_douses++; else p_douses++; shouldUpdate = true; }
}
