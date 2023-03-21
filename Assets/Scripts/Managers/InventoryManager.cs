using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public DeckBuilder deck;
    public int[] CardCollection;
    public int totalCardTypes = 9;
    // Start is called before the first frame update
    void Start()
    {
        for(int x = 0; x < CardCollection.Length; x++)
        {
            CardCollection[x] = Random.Range(1, 6);
        }

        deck.CreateMenu();
    }
}
