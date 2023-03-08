using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public bool stunned;
    public float stunEnd;

    public abstract void Stun();
}
