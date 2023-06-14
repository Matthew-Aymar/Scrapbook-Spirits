using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllText", menuName = "ScriptableObjects/DialogueTexts", order = 1)]
public class DialogueTexts : ScriptableObject
{
    //Entity ID (NPC will have multiple entries depending on appearance), Text Page
    public string[,] dialouge = new string[100, 5];
    public string[] ents = new string[100];

    public void Init()
    {
        ents[0] = "name-area-instance";
        ents[1] = "test-object-01";
        ents[2] = "test-object-02";
        ents[3] = "test-object-03";
        ents[4] = "test-object-04";
        ents[5] = "test-enter-combat";

        dialouge[1, 0] = "This is a thing";
        dialouge[1, 1] = "Really im not sure";
        dialouge[1, 2] = "Definitely something";
        dialouge[1, 3] = "";

        dialouge[2, 0] = "|sBOO|s";
        dialouge[2, 1] = "Jk its just another thing";
        dialouge[2, 2] = "";

        dialouge[3, 0] = "|wTreeeeeeeeeeeeeeeee|w";
        dialouge[3, 1] = "";

        dialouge[4, 0] = "Door?";
        dialouge[4, 1] = "";

        dialouge[5, 0] = "|wWhat have we here?|w";
        dialouge[5, 1] = "|c";
    }

    public string GetText(string ent, int page)
    {
        int entIndex = -1;

        for(int e = 0; e < ents.Length; e++)
        {
            if(ents[e].Equals(ent))
            {
                entIndex = e;
                break;
            }
        }

        if (entIndex == -1)
        {
            Debug.Log("no ent found");
            return "";
        }

        return dialouge[entIndex, page];
    }
}
