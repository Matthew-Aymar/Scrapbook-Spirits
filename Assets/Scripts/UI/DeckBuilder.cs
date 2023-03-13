using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DeckBuilder : MonoBehaviour
{
    public DeckManager deck;

    public CardText cardText;
    public CardSelector cards;

    public InventoryManager inv;
    public GameObject cardDisplay;
    public GameObject textCount;
    public Transform LibraryPos;
    public Transform DeckPos;

    public TMP_Text cardCount;
    public TMP_Text deckCount;

    public Vector3 startPos = new Vector3(-225, 225, 0);
    public float colOffset = 5; //yOffset each column

    public List<GameObject> cardObjs;
    private int selected = -1;
    private int selectedCardID;
    private int yPos; //for scrolling purposes
    private int relativeToMiddle = -1;

    private List<int> tempDeck = new List<int>(20);

    public bool inDeck = false;
    public GameObject deckSelected;
    public GameObject libSelected;
    // Start is called before the first frame update
    void Start()
    {
        deckCount.text = "0/20";
    }

    // Update is called once per frame
    void Update()
    {
        if (!inDeck)
            LibraryUpdate();
        else
            DeckUpdate();
    }

    public void DeckUpdate()
    {
        if(tempDeck.Count > 0)
        {
            if (Input.GetButtonDown("Left"))
            {
                selected--;
            }

            if (Input.GetButtonDown("Right"))
            {
                selected++;
            }

            if (Input.GetButtonDown("Interact"))
            {
                selected -= 10;
            }

            if (Input.GetButtonDown("Down"))
            {
                selected += 10;
            }

            if(Input.GetButtonDown("Jump"))
            {
                RemoveFromDeck();
            }

            if (selected < 0)
                selected = 0;
            else if (selected >= tempDeck.Count)
                selected = tempDeck.Count - 1;
        }

        int count = 0;
        foreach (Transform card in DeckPos.transform)
        {
            Image[] images = card.GetComponentsInChildren<Image>();
            foreach (Image i in images)
            {
                if (selected == count)
                {
                    i.color = new Color(0.9f, 0.85f, 1.0f);
                }
                else
                    i.color = Color.white;
            }

            count++;
        }

        if (Input.GetButtonDown("TabLeft"))
        {
            inDeck = false;
            deckSelected.SetActive(false);
            libSelected.SetActive(true);

            selected = 0;
        }
    }

    public void LibraryUpdate()
    {
        int lastSelect = selected;
        if (selected == -1)
            selected = 0;

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
                if (row > 0)
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
                if (row < (cardObjs.Count - 1) / 3)
                {
                    if ((row + 1) == cardObjs.Count / 3 && cardObjs.Count % 3 != 0)
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

            if (Input.GetButtonDown("Jump"))
            {
                RemoveFromMenu(selected);
            }
        }

        if (selected != lastSelect)
        {
            Sprite s = cardObjs[selected].transform.Find("Icon").GetComponent<Image>().sprite;
            selectedCardID = Array.IndexOf(cards.icons, s);

            cardText.SetText(selectedCardID);
        }

        if (Input.GetButtonDown("TabRight"))
        {
            inDeck = true;
            deckSelected.SetActive(true);
            libSelected.SetActive(false);

            selected = 0;
        }
    }

    public void CreateMenu()
    {
        int totalCards = 0;
        int rowCount = 0;
        int colCount = 0;

        Vector3 cardPos = startPos;

        if (cardObjs.Count != 0)
        {
            for(int x = 0; x < cardObjs.Count; x++)
            {
                Destroy(cardObjs[x]);
            }

            cardObjs.Clear();
        }

        for(int x = 0; x < inv.CardCollection.Length; x++)
        {
            if(inv.CardCollection[x] > 0)
            {
                GameObject card = Instantiate(cardDisplay, LibraryPos);
                card.transform.localPosition = cardPos;

                card.transform.Find("Icon").GetComponent<Image>().sprite = cards.icons[x % inv.totalCardTypes];

                GameObject text = Instantiate(textCount, card.transform);
                text.transform.localPosition = new Vector3(90, -100, 0);
                if (inv.CardCollection[x] > 1)
                {
                    text.GetComponent<TMP_Text>().text = "x" + inv.CardCollection[x];
                }
                else
                    text.GetComponent<TMP_Text>().text = "";

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

                if (selectedCardID == 0)
                    selectedCardID = x % inv.totalCardTypes;
            }
        }

        if (tempDeck.Count != 0)
            totalCards += tempDeck.Count;

        cardCount.text = "Total Cards: " + totalCards;
    }

    public void RemoveFromDeck()
    {
        GameObject temp = DeckPos.GetChild(selected).gameObject;
        Sprite s = temp.transform.Find("Icon").GetComponent<Image>().sprite;
        selectedCardID = Array.IndexOf(cards.icons, s);
        tempDeck.RemoveAt(selected);
        Destroy(temp);

        int count = 0;
        foreach(Transform card in DeckPos)
        {
            if(count >= selected)
            {
                if (count <= 10)
                    card.transform.localPosition = new Vector3((count * 60) - 350, -150, 0);
                else
                    card.transform.localPosition = new Vector3(((count - 10) * 60) - 350, -250, 0);
            }

            count++;
        }

        deckCount.text = tempDeck.Count + "/20";
        inv.CardCollection[selectedCardID]++;
        CreateMenu();
    }

    public void RemoveFromMenu(int i)
    {
        if(AddToTempDeck())
        {
            inv.CardCollection[selectedCardID]--;
            if (inv.CardCollection[selectedCardID] >= 1)
            {
                if (inv.CardCollection[selectedCardID] == 1)
                    cardObjs[i].GetComponentInChildren<TMP_Text>().text = "";
                else
                    cardObjs[i].GetComponentInChildren<TMP_Text>().text = "x" + inv.CardCollection[selectedCardID];
            }
            else if (inv.CardCollection[selectedCardID] <= 0)
            {

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

            if(cardObjs.Count > 0)
            {
                if (selected >= cardObjs.Count)
                    selected = cardObjs.Count - 1;

                Sprite s = cardObjs[selected].transform.Find("Icon").GetComponent<Image>().sprite;
                selectedCardID = Array.IndexOf(cards.icons, s);

                cardText.SetText(selectedCardID);
            }
        }
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

    public void SortDeck()
    {
        Dictionary<int, List<GameObject>> deckObjs = new Dictionary<int, List<GameObject>>();

        foreach(Transform c in DeckPos)
        {
            Sprite s = c.Find("Icon").GetComponent<Image>().sprite;
            int DeckCardID = Array.IndexOf(cards.icons, s);
            
            if(deckObjs.ContainsKey(DeckCardID))
            {
                deckObjs.GetValueOrDefault(DeckCardID).Add(c.gameObject);
            }
            else
            {
                deckObjs.Add(DeckCardID, new List<GameObject>());
                deckObjs.GetValueOrDefault(DeckCardID).Add(c.gameObject);
            }
        }

        int count = 1;
        for (int x = 0; x < inv.totalCardTypes; x++)
        {
            if(deckObjs.ContainsKey(x))
            {
                for(int y = 0; y < deckObjs.GetValueOrDefault(x).Count; y++)
                {
                    GameObject c = deckObjs.GetValueOrDefault(x)[y];

                    if (count <= 10)
                        c.transform.localPosition = new Vector3((count * 60) - 350, -150, 0);
                    else
                        c.transform.localPosition = new Vector3(((count - 10) * 60) - 350, -250, 0);

                    c.transform.SetAsLastSibling();

                    count++;
                }
            }
        }
    }

    public bool AddToTempDeck()
    {
        if(tempDeck.Count <= 19)
        {
            tempDeck.Add(selectedCardID);
            GameObject c = Instantiate(cardDisplay, DeckPos);
            c.transform.localScale *= 0.75f;
            c.transform.Find("Icon").GetComponent<Image>().sprite = cards.icons[selectedCardID];

            deckCount.text = tempDeck.Count + "/20";

            SortDeck();

            return true;
        }

        return false;
    }

    public void OnDisable()
    {
        deck.StartDeck(tempDeck);
    }
}
