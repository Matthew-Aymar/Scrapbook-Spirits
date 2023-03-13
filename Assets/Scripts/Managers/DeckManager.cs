using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    private List<int> deck;                             //The list of cards set through the deckbuilding UI
    public List<int> activeDeck;                        //Cards to be shuffled mid combat
    private List<int> discard;                          //Cards to be readded to the deck when it runs out

    public void StartDeck(List<int> uiDeck)
    {
        deck = new List<int>(uiDeck);
        activeDeck = new List<int>(deck);
        discard = new List<int>();

        Shuffle();
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
        if(activeDeck.Count <= 0)
        {
            ReAdd();
        }

        int val = activeDeck[0];
        activeDeck.RemoveAt(0);
        return val;
    }

    public void DiscardCard(int i)
    {
        discard.Add(i);
    }
}
