using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public int[] CardCollection = new int[9];
    // Start is called before the first frame update
    void Start()
    {
        for(int x = 0; x < CardCollection.Length; x++)
        {
            CardCollection[x] = 5;
        }
    }
}
