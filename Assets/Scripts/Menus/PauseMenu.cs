using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pause_menu;
    public GameObject selection;
    public GameObject cardmenu;
    private bool menu_shown = false;
    private bool card_shown = false;
    private int selection_num = 0;
    private Vector3 originalpos;
    // Start is called before the first frame update
    void Start()
    {
        originalpos = selection.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (menu_shown)
        {
            if (Input.GetButtonDown("Interact"))
            {
                if (selection_num > 0)
                    selection_num--;
                else
                    selection_num = 3;
            }

            if (Input.GetButtonDown("Down"))
            {
                if (selection_num < 3)
                    selection_num++;
                else
                    selection_num = 0;
            }

            selection.transform.position = new Vector3(selection.transform.position.x, originalpos.y - (0.525f * selection_num), selection.transform.position.z);

            if (Input.GetButtonDown("Start"))
            {
                if (selection_num == 0)
                {
                    menu_shown = false;
                    pause_menu.SetActive(false);
                    selection.SetActive(false);
                }
                else if (selection_num == 1)
                {
                    cardmenu.SetActive(true);
                    pause_menu.SetActive(false);
                    selection.SetActive(false);
                    card_shown = true;
                }
                else if (selection_num == 2)
                {

                }
                else if (selection_num == 3)
                {
                    Application.Quit(0);
                }
            }
        }

        if (Input.GetButtonDown("Pause") && !menu_shown)
        {
            menu_shown = true;
            pause_menu.SetActive(true);
            selection.SetActive(true);
        }
        else if(Input.GetButtonDown("Pause") && !card_shown)
        {
            menu_shown = false;
            pause_menu.SetActive(false);
            selection.SetActive(false);
        }
        else if(Input.GetButtonDown("Pause"))
        {
            card_shown = false;
            cardmenu.SetActive(false);
            pause_menu.SetActive(true);
            selection.SetActive(true);
        }
    }
}
