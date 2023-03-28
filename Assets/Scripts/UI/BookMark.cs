using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookMark : MonoBehaviour
{
    public GameObject HP;
    public GameObject Draw;
    public GameObject segment;
    public float speed;

    private Animator anim;

    private int segmentCount;
    private float movementToSpawn;

    private bool inMovement;
    // Start is called before the first frame update
    void OnEnable()
    {
        anim = gameObject.GetComponent<Animator>();

        inMovement = true;
        segmentCount = 0;
        transform.localPosition = new Vector3(-1010, 490, 0);
        MakeSegment();
        movementToSpawn = 150;
    }

    // Update is called once per frame
    void Update()
    {
        if(inMovement)
        {
            float move = Time.deltaTime * speed;
            transform.localPosition = new Vector3(transform.localPosition.x + move, transform.localPosition.y, 0);
            HP.transform.localPosition = new Vector3(HP.transform.localPosition.x + move, HP.transform.localPosition.y, 0);
            Draw.transform.localPosition = new Vector3(Draw.transform.localPosition.x + move, Draw.transform.localPosition.y, 0);
            movementToSpawn -= move;

            if(movementToSpawn <= 0)
            {
                MakeSegment();
                movementToSpawn = 150;
            }    

            if(transform.localPosition.x >= 880)
            {
                inMovement = false;
                MakeStatic();
            }
        }
    }

    private void MakeSegment()
    {
        GameObject seg = Instantiate(segment, this.transform);
        seg.transform.localPosition = new Vector3(-200 * segmentCount - 150, 0, 0);
        segmentCount++;
    }

    public void MakeStatic()
    {
        anim.Play("BookMarkEndStatic");

        foreach(Transform t in transform)
        {
            Animator a = t.gameObject.GetComponent<Animator>();
            a.Play("BookMarkStatic");
        }
    }

    public void OnDisable()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
    }
}
