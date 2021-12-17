using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public int ID;
    public string card_name;
    public string description;
    public CardMenu cm;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCard(int type, int id)
    {
        ID = id;
        if (type == 0)
        {
            card_name = "Fire Wheel";
            description = "Attack nearby enemies with a ring of fire";
        }
        else if (type == 1)
        {
            card_name = "Star Beam";
            description = "Attack everything in front with a beam of light";
        }
        else if (type == 2)
        {
            card_name = "Spirit Shell";
            description = "Become immune to close ranged attacks, and deal damage on contact";
        }
    }

    private void OnMouseOver()
    {
        if(Input.GetButtonDown("Attack"))
        {
            cm.AddToDeck(ID);
        }
    }
}
