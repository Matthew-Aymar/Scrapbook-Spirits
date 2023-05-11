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
        ents[1] = "test-editor-01";

        dialouge[1, 0] = "|wWhat's going on?|w";
        dialouge[1, 1] = "Who are we?";
        dialouge[1, 2] = "This is going to take a |swhile|s |wisn't it?|w";
        dialouge[1, 3] = "|sACHOOOOO|s";
        dialouge[1, 4] = "I wonder |wwhen this will end|w and when we can take a nap, I'm |ssleepy!|s";
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

        return dialouge[entIndex, page];
    }
}
