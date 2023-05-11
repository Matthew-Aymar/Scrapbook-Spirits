using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueInteraction : MonoBehaviour
{
    public DiaManager diaManager;
    public DialogueAnimator diaAnimator;
    public int page = 0;
    public string targetEnt = "name-area-instance";

    void OnEnable()
    {
        diaManager = GameObject.Find("DiaManager").GetComponent<DiaManager>();
        diaAnimator.NewValues(diaManager.dia.GetText(targetEnt, page));
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Jump") || Input.GetButtonDown("Attack") || Input.GetButtonDown("Special"))
        {
            page++;
            diaAnimator.NewValues(diaManager.dia.GetText(targetEnt, page));
        }
    }
}
