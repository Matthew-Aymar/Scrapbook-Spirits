using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlerpTest : MonoBehaviour
{
    public GameObject other;
    public float time;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 center = transform.localPosition - other.transform.localPosition;
        center.x += 5;
        center.y += 5;
        Vector3 pos1 = transform.localPosition - center;
        Vector3 pos2 = other.transform.localPosition - center;
        transform.localPosition = Vector3.Slerp(pos1, pos2, time) + center;
        if (time < 1.0f)
            time += Time.deltaTime * 0.01f;
    }
}
