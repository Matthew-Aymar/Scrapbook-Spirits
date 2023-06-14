using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueInteraction : MonoBehaviour
{
    public DiaManager diaManager;
    public DialogueAnimator diaAnimator;
    public bool target;
    public string targetEnt = "name-area-instance";

    void OnEnable()
    {
        diaManager = GameObject.Find("DiaManager").GetComponent<DiaManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(target && (Input.GetButtonDown("Attack") || Input.GetButtonDown("Special")))
        {
            diaAnimator.NewValues(diaManager.GetText(targetEnt));
        }
    }

    public void InteractInput()
    {
        if(diaAnimator.enabled == false)
        {
            target = true;
            diaAnimator.NewValues(diaManager.GetText(targetEnt));
            diaAnimator.enabled = true;
        }
    }
}
