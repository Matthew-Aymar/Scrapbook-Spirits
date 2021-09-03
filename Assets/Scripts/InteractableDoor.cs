using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDoor : MonoBehaviour
{
    public Transform location;
    public Transition trans;

    public void Start()
    {
        trans = GameObject.Find("Transition").GetComponent<Transition>();
    }

    public Vector2 UseDoor()
    {
        trans.StartTransition(Camera.main.transform.position, location.position);
        return location.position;
    }
}
