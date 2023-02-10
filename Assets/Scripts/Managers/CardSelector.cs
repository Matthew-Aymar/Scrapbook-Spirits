using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelector : MonoBehaviour
{
    public GameObject[] cardDisplays;
    public Sprite[] icons;

    public Vector3[] basePositions;
    public bool holding;
    private bool firstMove;
    private float firstMoveAmount;
    private float holdAmount;
    private int selected;
    // Start is called before the first frame update
    void Start()
    {
        basePositions = new Vector3[cardDisplays.Length];
        int i = 0;
        foreach(GameObject card in cardDisplays)
        {
            card.transform.Find("Icon").GetComponent<SpriteRenderer>().sprite = icons[i];
            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(firstMove)
        {
            if(firstMoveAmount < 1.25f)
            {
                int i = 0;
                foreach (GameObject card in cardDisplays)
                {
                    card.transform.Translate(card.transform.up * Time.deltaTime * 5);
                    i++;
                }

                firstMoveAmount += Time.deltaTime * 5;
            }
            else
            {
                int i = 0;
                foreach (GameObject card in cardDisplays)
                {
                    basePositions[i] = card.transform.localPosition;
                    i++;
                }
                firstMove = false;
            }
        }
        else if(holding)
        {
            if(holdAmount < 2.0f)
            {
                int i = 0;
                foreach (GameObject card in cardDisplays)
                {
                    card.transform.Translate(card.transform.up * Time.deltaTime * 15);
                    i++;
                }

                holdAmount += Time.deltaTime * 15;
            }
            else
            {
                cardDisplays[selected].transform.Find("card_base").GetComponent<SpriteRenderer>().color = new Color(0.9f, 0.85f, 1f, 1);
            }
        }
    }

    public void HoldCards()
    {
        if (firstMove)
            return;

        holding = true;
        int i = 0;
        foreach (GameObject card in cardDisplays)
        {
            card.transform.localPosition = basePositions[i];
            i++;
        }
        holdAmount = 0;
        selected = 1;
    }

    public void DropCards()
    {
        if (firstMove)
            return;

        holding = false;
        int i = 0;
        foreach (GameObject card in cardDisplays)
        {
            card.transform.localPosition = basePositions[i];
            cardDisplays[selected].transform.Find("card_base").GetComponent<SpriteRenderer>().color = Color.white;
            i++;
        }
    }

    public void ShowCards()
    {
        foreach(GameObject card in cardDisplays)
        {
            card.SetActive(true);
            firstMove = true;
            firstMoveAmount = 0;
        }
    }

    public void HideCards()
    {
        foreach (GameObject card in cardDisplays)
        {
            card.SetActive(false);
        }
    }

    public void SelectRight()
    {
        if (selected < cardDisplays.Length - 1)
        {
            cardDisplays[selected].transform.Find("card_base").GetComponent<SpriteRenderer>().color = Color.white;
            selected++;
        }
    }

    public void SelectLeft()
    {
        if(selected > 0)
        {
            cardDisplays[selected].transform.Find("card_base").GetComponent<SpriteRenderer>().color = Color.white;
            selected--;
        }
    }

    public int GetCardID()
    {
        return selected + 1;
    }
}
