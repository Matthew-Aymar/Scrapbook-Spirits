using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthText : MonoBehaviour
{
    public PlayerCombat player;
    public TMP_Text text;

    private int lastHP;
    private bool jumped;
    private float speed;
    // Start is called before the first frame update
    void Start()
    {
        lastHP = player.health;
    }

    // Update is called once per frame
    void Update()
    {
        if(player.health != lastHP)
        {
            textJump();
            text.text = "" + player.health;
            lastHP = player.health;
        }

        if(jumped)
        {
            Vector2 newPos = Vector2.MoveTowards(text.gameObject.transform.localPosition, new Vector2(), Time.deltaTime * speed);
            Vector3 newScale = Vector3.MoveTowards(text.gameObject.transform.localScale, new Vector3(1, 1, 1), Time.deltaTime * speed / 10);
            if (Vector2.Distance(newPos, new Vector2()) <= 0.01f)
            {
                jumped = false;
                newPos = new Vector2();
                text.color = Color.black;
                newScale = new Vector3(1, 1, 1);
            }

            text.gameObject.transform.localScale = newScale;
            text.gameObject.transform.localPosition = newPos;
        }
    }

    public void textJump()
    {
        text.transform.localScale *= 1.5f;
        text.gameObject.transform.localPosition = new Vector3();

        Vector3 jumpDir = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0);
        jumpDir.Normalize();

        float randomMag = Random.Range(0.05f, 0.1f);
        jumpDir *= randomMag;

        speed = randomMag * 750;

        text.gameObject.transform.Translate(jumpDir);
        text.color = Color.red;
        jumped = true;
    }
}
