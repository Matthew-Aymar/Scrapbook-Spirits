using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public GameObject start_text;
    private SpriteRenderer start_sr;
    private bool lower = true;
    public GameObject start_menu;
    public GameObject selection;
    public GameObject controls;
    private Vector3 originalpos;
    private int selection_num = 0;

    public bool menu_shown = false;
    private bool controls_shown = false;
    // Start is called before the first frame update
    void Start()
    {
        start_sr = start_text.GetComponent<SpriteRenderer>();
        originalpos = selection.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(lower)
        {
            if (start_sr.color.a > 0.25f)
            {
                start_sr.color = new Color(1, 1, 1, start_sr.color.a - Time.deltaTime * 0.5f);
            }
            else
            {
                lower = false;
            }
        }
        else
        {
            if (start_sr.color.a < 1.0f)
            {
                start_sr.color = new Color(1, 1, 1, start_sr.color.a + Time.deltaTime * 0.5f);
            }
            else
            {
                lower = true;
            }
        }
        

        if(menu_shown)
        {
            if(Input.GetButtonDown("Interact"))
            {
                if (selection_num > 0)
                    selection_num--;
                else
                    selection_num = 2;
            }

            if(Input.GetButtonDown("Down"))
            {
                if (selection_num < 2)
                    selection_num++;
                else
                    selection_num = 0;
            }

            selection.transform.position = new Vector3(selection.transform.position.x, originalpos.y - (0.525f * selection_num), selection.transform.position.z);

            if(Input.GetButtonDown("Start"))
            {
                if (controls_shown)
                {
                    selection.SetActive(true);
                    start_menu.SetActive(true);
                    controls.SetActive(false);
                    controls_shown = false;
                }
                else
                {
                    if (selection_num == 0)
                    {
                        SceneManager.LoadScene("SampleScene");
                    }
                    else if (selection_num == 1)
                    {
                        selection.SetActive(false);
                        start_menu.SetActive(false);
                        controls.SetActive(true);
                        controls_shown = true;
                    }
                    else if (selection_num == 2)
                    {
                        Application.Quit(0);
                    }
                }
            }
        }

        if (Input.GetButtonDown("Start") && !menu_shown)
        {
            menu_shown = true;
            start_menu.SetActive(true);
            start_text.SetActive(false);
            selection.SetActive(true);
        }
    }
}
