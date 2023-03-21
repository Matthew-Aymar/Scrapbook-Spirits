using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack : MonoBehaviour
{
    public int _id;

    public AttackSelector attacker;

    public int direction;
    public float chargeAmount; //Minimum 0.3f
    public float chargeTime;
    public float speed;
    public float timeout;
    public float timeoutTime;
    public bool heavy;
    public float stunDuration;

    public abstract bool Init(bool held);

    public abstract void Spawn(int dir, bool upper);

    public abstract void Move();

    public abstract bool CheckCharge();

    public abstract bool CheckTimeout();

    public abstract Collider2D CheckCollision();
}
