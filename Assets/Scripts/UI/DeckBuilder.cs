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
    public float rowOffset = -5; //xOffset each row
    public float colOffset = 5;

    public List<GameObject> cardObjs;
    private int selected;
    // Start is called before the first frame update
    void Start()
    {
        CreateMenu();
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
                    selected += 2;
            }

            if (Input.GetButtonDown("Right"))
            {
                if (col < 2)
                    selected++;
                else
                    selected -= 2;
            }

            if (Input.GetButtonDown("Interact"))
            {
                if(row > 0)
                    selected -= 3;
            }

            if (Input.GetButtonDown("Down"))
            {
                if(row < (cardObjs.Count - 1) / 3)
                    selected += 3;
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

                card.transform.Find("Icon").GetComponent<Image>().sprite = cards.icons[x];

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
                cardPos.x += rowOffset * rowCount;

                cardPos.y = startPos.y + (-225 * rowCount);
                cardPos.y += colOffset * colCount;

                cardObjs.Add(card);
            }
        }

        cardCount.text = "Total Cards: " + totalCards;
    }

    public void RemoveFromMenu(int i)
    {
        Debug.Log(i + ":" + inv.CardCollection[i]);

        inv.CardCollection[i]--;
        if (inv.CardCollection[i] >= 1)
        {
            if(inv.CardCollection[i] == 1)
                cardObjs[i].GetComponentInChildren<TMP_Text>().text = "";
            else
                cardObjs[i].GetComponentInChildren<TMP_Text>().text = "x" + inv.CardCollection[i];
        }
        else if(inv.CardCollection[i] == 0)
        {
            for(int x = i; x < cardObjs.Count - 1; x++)
            {
                inv.CardCollection[i] = inv.CardCollection[i + 1];
            }

            GameObject g = cardObjs[i];
            cardObjs.RemoveAt(i);
            Destroy(g);
            int rowCount;
            int colCount;
            Vector3 newPos = new Vector3();

            for(int x = i; x < cardObjs.Count; x++)
            {
                rowCount = x / 3;
                colCount = x % 3;
                newPos.x = startPos.x + (225 * colCount);
                newPos.x += rowOffset * rowCount;
                newPos.y = startPos.y + (-225 * rowCount);
                newPos.y += colOffset * colCount;

                cardObjs[x].transform.localPosition = newPos;
            }
        }
    }
}
