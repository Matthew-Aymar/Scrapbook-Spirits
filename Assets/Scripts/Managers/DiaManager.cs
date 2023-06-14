using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiaManager : MonoBehaviour
{
    public DialogueTexts dia;
    public int currentPage = 0;
    public PlayerMovement player;
    public DialogueAnimator anim;
    // Start is called before the first frame update
    void Start()
    {
        dia.Init();
    }

    public string GetText(string ent)
    {
        string temp = dia.GetText(ent, currentPage);

        if (temp.Equals(""))
        {
            currentPage = 0;
            player.inText = false;
            player.textTarget.target = false;
            anim.enabled = false;
            return "";
        }
        else if (temp.Equals("|c"))
        {
            currentPage = 0;
            player.inText = false;
            player.textTarget.target = false;
            player.pc.StartTransition();
            anim.enabled = false;
            return "";
        }

        currentPage++;
        return temp;
    }
}
