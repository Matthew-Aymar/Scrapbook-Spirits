using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawBar : MonoBehaviour
{
    public GameObject clock;
    private Image clockImg;
    public GameObject small;

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
        clockImg = clock.GetComponent<Image>();
        startColor = clockImg.color;
        lastDraw = Time.time;
        whichColor = 3;
        nextColor = nextColor = new Color(0.9f, 0.9f, 1.0f);

        canDraw = true;
        drawPercent = 1.0f;
        cards.CanDraw();
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

                small.transform.eulerAngles = new Vector3(0, 0, -50); 
            }
            else
            {
                drawPercent = (Time.time - lastDraw) / drawTime;

                small.transform.eulerAngles = new Vector3(0, 0, (360 * drawPercent) - 50);
            }
        }
        else
        {
            clockImg.color = Vector4.MoveTowards(clockImg.color, nextColor, Time.deltaTime * 0.25f);
            Vector4 between = new Vector4(clockImg.color.r - nextColor.r, clockImg.color.g - nextColor.g, clockImg.color.b - nextColor.b, clockImg.color.a - nextColor.a);
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

        clockImg.color = startColor;
        drawPercent = 0;
    }
}
