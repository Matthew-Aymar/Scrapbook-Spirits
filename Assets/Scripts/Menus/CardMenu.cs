using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardMenu : MonoBehaviour
{
    private List<int> library = new List<int>();
    private List<int> deck = new List<int>();
    public List<GameObject> cards;
    public GameObject txt;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        for (int x = 0; x < 8; x++)
        {
            if (x < 2)
                library.Add(1);
            else if (x < 3)
                library.Add(2);
            else if (x < 4)
                library.Add(3);
            else if (x < 5)
                library.Add(4);
            else if (x < 6)
                library.Add(5);
            else if (x < 7)
                library.Add(6);
            else if (x < 8)
                library.Add(7);
        }

        MakeCards();
    }

    void MakeCards()
    {
        GameObject temp = new GameObject();
        Destroy(temp);
        Vector3 location = new Vector3();
        bool hascards = false;

        GameObject[] oldcards = GameObject.FindGameObjectsWithTag("Card");
        if(oldcards.Length > 0)
        {
            for(int x = 0; x < oldcards.Length; x++)
            {
                Destroy(oldcards[x]);
            }
        }

        for(int x = 0; x < library.Count; x++)
        {
            if(x < 4)
                location.x = this.gameObject.transform.position.x + (3 * x);
            else
            {
                location.x = this.gameObject.transform.position.x + (3 * (x - 4));
                location.y = this.gameObject.transform.position.y - 4;
            }
            location.z = 100;

            switch (library[x])
            {
                case 0:
                    temp = Instantiate(cards[library[x]], this.gameObject.transform);
                    temp.GetComponent<Card>().SetCard(0, x);
                    hascards = true;
                    break;
                case 1:
                    temp = Instantiate(cards[library[x]], this.gameObject.transform);
                    temp.GetComponent<Card>().SetCard(0, x);
                    hascards = true;
                    break;
                case 2:
                    temp = Instantiate(cards[library[x]], this.gameObject.transform);
                    temp.GetComponent<Card>().SetCard(0, x);
                    hascards = true;
                    break;
                case 3:
                    temp = Instantiate(cards[library[x]], this.gameObject.transform);
                    temp.GetComponent<Card>().SetCard(1, x);
                    hascards = true;
                    break;
                case 4:
                    temp = Instantiate(cards[library[x]], this.gameObject.transform);
                    temp.GetComponent<Card>().SetCard(1, x);
                    hascards = true;
                    break;
                case 5:
                    temp = Instantiate(cards[library[x]], this.gameObject.transform);
                    temp.GetComponent<Card>().SetCard(2, x);
                    hascards = true;
                    break;
                case 6:
                    temp = Instantiate(cards[library[x]], this.gameObject.transform);
                    temp.GetComponent<Card>().SetCard(2, x);
                    hascards = true;
                    break;
            }

            if(hascards)
            {
                temp.GetComponent<Card>().cm = this;
                temp.transform.position = location;
            }
        }

        MakeDeck();
    }

    void MakeDeck()
    {
        GameObject temp = new GameObject();
        Destroy(temp);

        GameObject[] olddeck = GameObject.FindGameObjectsWithTag("Deck");
        if (olddeck.Length > 0)
        {
            for (int x = 0; x < olddeck.Length; x++)
            {
                Destroy(olddeck[x]);
            }
        }

        for (int x = 0; x < deck.Count; x++)
        {
            switch (deck[x])
            {
                case 0:
                    temp = Instantiate(txt, this.transform);
                    temp.GetComponent<Text>().text = "Fire Wheel";
                    break;
                case 1:
                    temp = Instantiate(txt, this.transform);
                    temp.GetComponent<Text>().text = "Fire Wheel";
                    break;
                case 2:
                    temp = Instantiate(txt, this.transform);
                    temp.GetComponent<Text>().text = "Fire Wheel";
                    break;
                case 3:
                    temp = Instantiate(txt, this.transform);
                    temp.GetComponent<Text>().text = "Star Beam";
                    break;
                case 4:
                    temp = Instantiate(txt, this.transform);
                    temp.GetComponent<Text>().text = "Star Beam";
                    break;
                case 5:
                    temp = Instantiate(txt, this.transform);
                    temp.GetComponent<Text>().text = "Spirit Shell";
                    break;
                case 6:
                    temp = Instantiate(txt, this.transform);
                    temp.GetComponent<Text>().text = "Spirit Shell";
                    break;
            }

            temp.transform.position = new Vector3(temp.transform.position.x, temp.transform.position.y - (0.5f * x), temp.transform.position.z);
        }
    }

    public void AddToDeck(int card)
    {
        deck.Add(library[card]);
        library.RemoveAt(card);
        MakeCards();
    }
}
