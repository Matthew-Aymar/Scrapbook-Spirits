using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public Camera cam;

    public class Attack
    {
        private int _ID;
        public int posX;
        public int posY;
        public GameObject ent;

        public Attack(int id, int x, int y)
        {
            _ID = id;
            posX = x;
            posY = y;
        }

        public int GetId()
        {
            return _ID;
        }
    }

    public List<Action<Attack>> attackEffects = new List<Action<Attack>>();
    public List<GameObject> attacks = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}
