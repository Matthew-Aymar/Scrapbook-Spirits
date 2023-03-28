using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public float health;

    public bool stunned;
    public float stunEnd;

    public abstract void Stun();
}
