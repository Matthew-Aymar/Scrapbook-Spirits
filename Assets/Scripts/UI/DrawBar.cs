using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawBar : MonoBehaviour
{
    public RectTransform bar;
    private Image barImg;
    public RectTransform goopPos;
    public RectTransform goop;

    public CardSelector cards;
    public float drawTime;
    private float lastDraw;
    private float drawPercent;
    private bool canDraw;

    private Color startColor;
    private Color nextColor;
    private int whichColor; //1 = red, 2 = green, 3 = blue
    // Start is called before the first frame update
    void Start()
    {
        barImg = GetComponentInChildren<Image>();
        startColor = barImg.color;
        lastDraw = Time.time;
        whichColor = 3;
        nextColor = nextColor = new Color(0.9f, 0.9f, 1.0f);

        canDraw = true;
        drawPercent = 1.0f;
        cards.CanDraw();
        goop.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(!canDraw)
        {
            if (Time.time >= drawTime + lastDraw)
            {
                drawPercent = 1.0f;
                canDraw = true;
                cards.CanDraw();
                goop.gameObject.SetActive(false);
            }
            else
            {
                drawPercent = (Time.time - lastDraw) / drawTime;
                bar.localScale = new Vector3(drawPercent, 1, 1);

                goop.position = new Vector3(goopPos.position.x + 0.08f, goopPos.position.y, goopPos.position.z);
            }
        }
        else
        {
            barImg.color = Vector4.MoveTowards(barImg.color, nextColor, Time.deltaTime * 0.25f);
            Vector4 between = new Vector4(barImg.color.r - nextColor.r, barImg.color.g - nextColor.g, barImg.color.b - nextColor.b, barImg.color.a - nextColor.a);
            if(between.magnitude < 0.01f)
            {
                if(whichColor == 3)
                {
                    whichColor = 1;
                    nextColor = new Color(1.0f, 0.9f, 0.9f);
                }
                else if(whichColor == 2)
                {
                    whichColor = 3;
                    nextColor = new Color(0.9f, 0.9f, 1.0f);
                }
                else if(whichColor == 1)
                {
                    whichColor = 2;
                    nextColor = new Color(0.9f, 1.0f, 0.9f);
                }
            }
        }
    }

    public void ResetDrawTimer()
    {
        lastDraw = Time.time;
        canDraw = false;

        bar.localScale = new Vector3(0, 1, 1);
        barImg.color = startColor;
        drawPercent = 0;

        goop.gameObject.SetActive(true);
    }
}
