using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelector : MonoBehaviour
{
    public GameObject cam;
    public GameObject card;
    public DeckManager deck;
    public GameObject player;

    public List<GameObject> cardDisplays;
    public List<int> cardHand;
    public Sprite[] icons;

    public List<Vector3> basePositions;
    public float leftBound;
    public float rightBound;

    public bool holding;
    private bool firstMove;
    private float firstMoveAmount;
    private float holdAmount;
    public int selected;

    public int maxHand;
    public int handCount;
    public float totalRot;

    public GameObject usingCard;
    private int usedCardId;

    // Start is called before the first frame update
    void Start()
    {
        handCount = cardDisplays.Count;

        int i = 0;
        foreach(GameObject card in cardDisplays)
        {
            card.transform.Find("Icon").GetComponent<SpriteRenderer>().sprite = icons[i];
            i++;
        }

        deck.StartDeck();
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
            if (holdAmount < 2.0f)
            {
                int i = 0;
                foreach (GameObject card in cardDisplays)
                {
                    card.transform.Translate(card.transform.up * Time.deltaTime * 15);
                    i++;
                }

                holdAmount += Time.deltaTime * 15;
            }
        }

        UpdateColors();
    }

    public void HoldCards()
    {
        if (firstMove || cardDisplays.Count == 0)
            return;

        holding = true;
        int i = 0;
        foreach (GameObject card in cardDisplays)
        {
            card.transform.localPosition = basePositions[i];
            i++;
        }
        holdAmount = 0;
        selected = 0;

        Time.timeScale = 0.25f;
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
            i++;
        }

        Time.timeScale = 1.0f;
    }

    public void ShowCards()
    {
        foreach(GameObject card in cardDisplays)
        {
            card.SetActive(true);
            firstMove = true;
            firstMoveAmount = 0;
        }

        player = GameObject.FindGameObjectWithTag("CombatPlayer");
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
        if (selected >= 0 && selected < cardDisplays.Count - 1)
        {
            selected++;
        }
        else if (selected == cardDisplays.Count - 1)
            selected = 0;
    }

    public void SelectLeft()
    {
        if (selected > 0 && selected < cardDisplays.Count)
        {
            selected--;
        }
        else if (selected == 0)
            selected = cardDisplays.Count - 1;
    }

    public int GetCardID()
    {
        return usedCardId;
    }

    public void NewPositions()
    {
        if (cardDisplays.Count == 0)
            return;

        float rotAmount = -totalRot / (cardDisplays.Count - 1);
        float xAmount = (Mathf.Abs(leftBound) + rightBound) / (cardDisplays.Count - 1);
        int count = 0;
        int sortCount = 0;
        float yVal = -7;
        foreach(GameObject card in cardDisplays)
        {
            float xOffset;
            float rotVal;
            if (cardDisplays.Count == 1)
            {
                xOffset = 0;
                rotVal = 0;
            }
            else
            {
                if (cardDisplays.Count == 2)
                {
                    xOffset = (xAmount * 0.5f) * count + (leftBound * 0.5f);
                    rotVal = (rotAmount * 0.5f) * count + (totalRot * 0.25f);
                }
                else
                {
                    xOffset = xAmount * count + leftBound;
                    rotVal = rotAmount * count + (totalRot * 0.5f);
                }
            }

            card.transform.rotation = new Quaternion();
            card.transform.localPosition = new Vector3(xOffset, yVal, 15 + count);
            card.transform.Rotate(new Vector3(0, 0, rotVal));

            card.transform.Find("CardBG").GetComponent<SpriteRenderer>().sortingOrder = sortCount;
            sortCount++;
            card.transform.Find("CardBase").GetComponent<SpriteRenderer>().sortingOrder = sortCount;
            sortCount++;
            card.transform.Find("Icon").GetComponent<SpriteRenderer>().sortingOrder = sortCount;
            sortCount++;

            ShowCards();

            if(count < basePositions.Count)
            {
                basePositions[count] = card.transform.localPosition;
            }
            else
            {
                basePositions.Add(card.transform.localPosition);
            }

            if(cardDisplays.Count > 2)
            {
                if (cardDisplays.Count / 2 > count)
                    yVal += 0.15f;
                else
                    yVal -= 0.15f;
            }

            count++;
        }
    }

    public void UseCard()
    {
        if(handCount > 0)
        {
            if (selected >= handCount)
                selected = handCount - 1;

            usedCardId = cardHand[selected];

            basePositions.RemoveAt(selected);
            usingCard = cardDisplays[selected];
            cardDisplays.RemoveAt(selected);
            deck.DiscardCard(cardHand[selected]);
            cardHand.RemoveAt(selected);
            handCount--;

            usingCard.transform.localPosition = new Vector3(player.gameObject.transform.localPosition.x, player.gameObject.transform.localPosition.y + 1.5f, player.gameObject.transform.localPosition.z - 1);
            SpriteRenderer back = usingCard.transform.Find("CardBase").gameObject.GetComponent<SpriteRenderer>();
            SpriteRenderer icon = usingCard.transform.Find("Icon").gameObject.GetComponent<SpriteRenderer>();
            SpriteRenderer bg = usingCard.transform.Find("CardBG").gameObject.GetComponent<SpriteRenderer>();
            back.sortingLayerName = "Combat";
            back.sortingOrder = 0;
            icon.sortingLayerName = "Combat";
            icon.sortingOrder = 1;
            bg.sortingLayerName = "Combat";
            bg.sortingOrder = -1;
            usingCard.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            usingCard.transform.rotation = new Quaternion();

            NewPositions();
        }

        Time.timeScale = 1.0f;
    }

    public void DrawCard()
    {
        if(handCount < maxHand)
        {
            GameObject newCard = Instantiate(card, cam.transform);
            cardDisplays.Add(newCard);
            cardHand.Add(deck.DrawCard());
            int i = cardHand[cardHand.Count - 1];
            newCard.transform.Find("Icon").GetComponent<SpriteRenderer>().sprite = icons[i - 1];

            handCount++;

            NewPositions();
        }
    }

    public void StopUsingCard()
    {
        if(usingCard)
            Destroy(usingCard);
    }

    public void UpdateColors()
    {
        int count = 0;
        foreach (GameObject card in cardDisplays)
        {
            SpriteRenderer[] renderers = card.GetComponentsInChildren<SpriteRenderer>();
            Color c = Color.white;
            if (count == selected && !usingCard && holding)
                c = new Color(0.9f, 0.85f, 1.0f);

            for (int x = 0; x < renderers.Length; x++)
            {
                renderers[x].color = c;
            }

            count++;
        }
    }
}
