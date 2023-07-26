using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireManager : MonoBehaviour
{
    private int currentIndex;
    private int lastIndex;
    public float speed = 1;
    public GameObject flame;
    public Vector2[] flamePositions;
    public Vector2 target = new Vector2();
    private bool lastFlip = false;

    // Start is called before the first frame update
    void Start()
    {
        flame.transform.localPosition = flamePositions[0];
    }

    // Update is called once per frame
    void Update()
    {
        bool currentFlip = this.gameObject.GetComponent<SpriteRenderer>().flipX;
        if (currentFlip != lastFlip)
        {
            flame.gameObject.GetComponent<SpriteRenderer>().flipX = currentFlip;
            flame.transform.localPosition = new Vector2(-1 * flame.transform.localPosition.x, flame.transform.localPosition.y);
            target = new Vector2(-1 * target.x, target.y);
            lastFlip = currentFlip;
        }

        float dist = Vector2.Distance(target, flame.transform.localPosition);
        if (dist > 0.01f && lastIndex == currentIndex)
        {
            Vector2 translation = new Vector2(target.x - flame.transform.localPosition.x, target.y - flame.transform.localPosition.y);
            translation.Normalize();
            flame.transform.Translate(translation * (speed * Time.deltaTime + dist * Time.deltaTime));
        }

        lastIndex = currentIndex;
    }

    public void setFlamePos(int index)
    {
        target = flamePositions[index];
        if(this.gameObject.GetComponent<SpriteRenderer>().flipX)
            target = new Vector2(-1 * target.x, target.y);

        currentIndex = index;
    }
}
