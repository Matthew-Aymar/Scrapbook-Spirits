using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckBuilder : MonoBehaviour
{
    public CardSelector cards;
    public InventoryManager inv;
    public GameObject cardDisplay;
    public GameObject textCount;
    public Transform LibraryPos;
    public Transform DeckPos;

    public TMP_Text cardCount;

    public Vector3 startPos = new Vector3(-225, 225, 0);
    public float colOffset = 5; //yOffset each column

    public List<GameObject> cardObjs;
    private int selected;
    private int yPos; //for scrolling purposes
    private int relativeToMiddle = -1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int row = selected / 3;
        int col = selected % 3;

        if (cardObjs.Count > 0)
        {
            if (Input.GetButtonDown("Left"))
            {
                if (col > 0)
                    selected--;
                else
                {
                    if (row == cardObjs.Count / 3 && cardObjs.Count % 3 != 0)
                    {
                        selected += cardObjs.Count % 3 - 1;
                    }
                    else
                        selected += 2;
                }
            }

            if (Input.GetButtonDown("Right"))
            {
                if (row == cardObjs.Count / 3 && cardObjs.Count % 3 != 0)
                {
                    if (col < cardObjs.Count % 3 - 1)
                        selected++;
                    else
                    {
                        selected -= cardObjs.Count % 3 - 1;
                    }
                }
                else if (col >= 2)
                {
                    selected -= 2;
                }
                else
                    selected++;
            }

            if (Input.GetButtonDown("Interact"))
            {
                if(row > 0)
                {
                    selected -= 3;
                    relativeToMiddle--;
                    if (relativeToMiddle < -1)
                    {
                        ScrollCards(false);
                    }
                }   
            }

            if (Input.GetButtonDown("Down"))
            {
                if(row < (cardObjs.Count - 1) / 3)
                {
                    if((row + 1) == cardObjs.Count / 3 && cardObjs.Count % 3 != 0)
                    {
                        selected += 3;
                        if (selected >= cardObjs.Count)
                            selected = cardObjs.Count - 1;
                    }
                    else
                        selected += 3;

                    relativeToMiddle++;
                    if (relativeToMiddle > 1)
                    {
                        ScrollCards(true);
                    }
                }
            }

            int count = 0;
            foreach (GameObject card in cardObjs)
            {
                int r = count / 3;
                int c = count % 3;

                Image[] images = card.GetComponentsInChildren<Image>();
                foreach (Image i in images)
                {
                    if (r == row && c == col)
                    {
                        i.color = new Color(0.9f, 0.85f, 1.0f);
                    }
                    else
                        i.color = Color.white;
                }

                count++;
            }

            if(Input.GetButtonDown("Jump"))
            {
                RemoveFromMenu(selected);
            }
        }
    }

    public void CreateMenu()
    {
        int totalCards = 0;
        int rowCount = 0;
        int colCount = 0;

        Vector3 cardPos = startPos;

        for(int x = 0; x < inv.CardCollection.Length; x++)
        {
            if(inv.CardCollection[x] > 0)
            {
                GameObject card = Instantiate(cardDisplay, LibraryPos);
                card.transform.localPosition = cardPos;

                card.transform.Find("Icon").GetComponent<Image>().sprite = cards.icons[x % 9];

                if(inv.CardCollection[x] > 1)
                {
                    GameObject text = Instantiate(textCount, card.transform);
                    text.transform.localPosition = new Vector3(90, -100, 0);
                    text.GetComponent<TMP_Text>().text = "x" + inv.CardCollection[x];
                }

                totalCards += inv.CardCollection[x];
                colCount++;
                if(colCount > 2)
                {
                    rowCount++;
                    colCount = 0;
                }

                cardPos.x = startPos.x + (225 * colCount);

                cardPos.y = startPos.y + (-225 * rowCount);
                cardPos.y += colOffset * colCount;

                cardObjs.Add(card);
            }
        }

        cardCount.text = "Total Cards: " + totalCards;
    }

    public void RemoveFromMenu(int i)
    {
        inv.CardCollection[i]--;
        if (inv.CardCollection[i] >= 1)
        {
            if (inv.CardCollection[i] == 1)
                cardObjs[i].GetComponentInChildren<TMP_Text>().text = "";
            else
                cardObjs[i].GetComponentInChildren<TMP_Text>().text = "x" + inv.CardCollection[i];
        }
        else if (inv.CardCollection[i] == 0)
        {
            for (int x = i; x < cardObjs.Count - 1; x++)
            {
                inv.CardCollection[i] = inv.CardCollection[i + 1];
            }

            GameObject g = cardObjs[i];
            cardObjs.RemoveAt(i);
            Destroy(g);

            Vector3 newPos;
            for (int x = i; x < cardObjs.Count; x++)
            {
                newPos = cardObjs[x].transform.localPosition;

                if (cardObjs[x].transform.localPosition.x <= -225)
                {
                    newPos.x = 225;
                    newPos.y += 225;
                    newPos.y += colOffset * 2;
                }
                else
                {
                    newPos.x -= 225;
                    newPos.y -= colOffset;
                }

                cardObjs[x].transform.localPosition = newPos;
            }
        }

        if (selected >= cardObjs.Count)
            selected = cardObjs.Count - 1;
    }

    public void ScrollCards(bool down)
    {
        int rowCount;
        int colCount;
        Vector3 newPos = new Vector3();

        if (down)
            yPos++;
        else
            yPos--;

        for (int x = 0; x < cardObjs.Count; x++)
        {
            rowCount = x / 3;
            colCount = x % 3;
            newPos.x = startPos.x + (225 * colCount);
            newPos.y = startPos.y + (-225 * rowCount);
            newPos.y += colOffset * colCount;

            newPos.y += 225 * yPos;

            cardObjs[x].transform.localPosition = newPos;
        }

        if (down)
            relativeToMiddle = 1;
        else
            relativeToMiddle = -1;
    }
}
