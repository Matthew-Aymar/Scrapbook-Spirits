using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthText : MonoBehaviour
{
    public PlayerCombat player;
    public Image text;

    private Animator anim;

    private int currentStage = 0;

    private int lastHP;
    private bool jumped;
    private Vector2 startSize;
    // Start is called before the first frame update
    void Start()
    {
        lastHP = player.health;
        anim = text.gameObject.GetComponent<Animator>();
        startSize = text.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if(player.health != lastHP)
        {
            textJump();
            lastHP = player.health;

            currentStage = (int)(player.health / (player.maxHealth / 5));
            switch(currentStage)
            {
                case 4:
                    anim.Play("LivelyText");
                    break;
                case 3:
                    anim.Play("HealthyText");
                    break;
                case 2:
                    anim.Play("OkayText");
                    break;
                case 1:
                    anim.Play("HurtingText");
                    break;
                case 0:
                    anim.Play("BleakText");
                    break;
            }
        }

        if(jumped)
        {
            text.transform.localScale = Vector3.MoveTowards(text.transform.localScale, startSize, Time.deltaTime * 2);
            if(Vector3.Distance(text.transform.localScale, startSize) < 0.01f)
            {
                jumped = false;
                text.color = Color.white;
            }
        }
    }

    public void textJump()
    {
        text.transform.localScale = startSize;
        text.transform.localScale *= 1.25f;
        text.color = new Color(1.0f, 0.5f, 0.5f, 1.0f);
        jumped = true;
    }
}
