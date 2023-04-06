using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    private List<int> deck;                             //The list of cards set through the deckbuilding UI
    public List<int> activeDeck;                        //Cards to be shuffled mid combat
    public List<int> discard;                          //Cards to be readded to the deck when it runs out
    public CardSelector cards;

    private bool wasStarted;

    public void StartDeck(List<int> uiDeck)
    {
        List<int> temp = new List<int>(uiDeck);
        for (int x = 0; x < temp.Count; x++)
            temp[x]++;

        deck = new List<int>(temp);
        activeDeck = new List<int>(deck);
        discard = new List<int>();

        Shuffle();
        cards.CheckHandSize();

        wasStarted = true;
    }

    public void Shuffle()
    {
        for(int x = 0; x < activeDeck.Count; x++)
        {
            int temp = activeDeck[x];
            int randomIndex = Random.Range(x, activeDeck.Count);
            activeDeck[x] = activeDeck[randomIndex];
            activeDeck[randomIndex] = temp;
        }
    }

    public void ReAdd()
    {
        for(int x = 0; x < discard.Count; x++)
        {
            activeDeck.Add(discard[x]);
        }

        discard.Clear();
        Shuffle();
    }

    public int DrawCard()
    {
        if(wasStarted)
        {
            if (activeDeck.Count <= 0 && discard.Count > 0)
            {
                ReAdd();
            }

            if (activeDeck.Count > 0)
            {
                int val = activeDeck[0];
                activeDeck.RemoveAt(0);
                return val;
            }
            else return -1;
        }

        return -1;
    }

    public void DiscardCard(int i)
    {
        discard.Add(i);
    }

    public void RemoveLastDiscard()
    {
        discard.RemoveAt(discard.Count - 1);
    }
}
