using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardText : MonoBehaviour
{
    public TMP_Text title;
    public TMP_Text description;
    public TMP_Text spice;

    public InventoryManager inv;

    public string[] cardTitles;
    public string[] cardDescriptions;
    public string[] cardSpices;

    // Start is called before the first frame update
    void Start()
    {
        cardTitles = new string[inv.totalCardTypes];
        cardDescriptions = new string[inv.totalCardTypes];
        cardSpices = new string[inv.totalCardTypes];

        cardTitles[0] = "Flame Pillar";
        cardDescriptions[0] = "Create a burning spire to repeatedly damage enemies";
        cardSpices[0] = "\"Into The Fray\"";

        cardTitles[1] = "Fire Swipe";
        cardDescriptions[1] = "Heavy, Slash with a blade of flame to stun enemies";
        cardSpices[1] = "\"Sear And Tear\"";

        cardTitles[2] = "Star Boomerang";
        cardDescriptions[2] = "Shoot a returning projectile, hold for greater effect";
        cardSpices[2] = "\"What Do You Wish For?\"";

        cardTitles[3] = "Bottled Spite";
        cardDescriptions[3] = "Cursed, release a slow moving projectile that chases enemies";
        cardSpices[3] = "\"Feeling Lonely?\"";

        cardTitles[4] = "Meteor Shot";
        cardDescriptions[4] = "Heavy, lob a large projectile that explodes on impact";
        cardSpices[4] = "\"This One Doesn't Bounce\"";

        cardTitles[5] = "Four Seasons";
        cardDescriptions[5] = "Create four flames on each corner to follow enemies";
        cardSpices[5] = "\"Rain Or Shine\"";

        cardTitles[6] = "Brighter Burn";
        cardDescriptions[6] = "Slightly boost basic attacks for the rest of combat";
        cardSpices[6] = "\"Fan The Flames\"";

        cardTitles[7] = "Douse";
        cardDescriptions[7] = "Reduce damage dealt by enemies for a limited time";
        cardSpices[7] = "\"Hush Now...\"";

        cardTitles[8] = "Meditate";
        cardDescriptions[8] = "Limited, heal yourself, reshuffle and draw an extra card for the rest of combat";
        cardSpices[8] = "\"Do You See It?\"";
    }

    public void SetText(int i)
    {
        title.text = cardTitles[i];
        description.text = cardDescriptions[i];
        spice.text = cardSpices[i];
    }
}
