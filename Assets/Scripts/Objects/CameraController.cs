using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject Player;
    public GameObject foreground;
    public GameObject midground;
    public GameObject background;
    public Transform parralax;

    public float fgMulti;
    public float mgMulti;
    public float bgMulti;

    private GameObject[] fg = new GameObject[3];
    private GameObject[] mg = new GameObject[3];
    private GameObject[] bg = new GameObject[3];

    public float speed;
    public Vector2 bounds;

    private Vector3 newPos;
    private Vector3 lastPos;
    private float dist;
    private float moveDist;

    public bool sub_area;

    // Start is called before the first frame update
    void Start()
    {
        for(int x = 0; x < 3; x++)
        {
            fg[x] = Instantiate(foreground, parralax); //70
            mg[x] = Instantiate(midground, parralax);  //27
            bg[x] = Instantiate(background, parralax); //27

            fg[x].transform.position = new Vector3(fg[x].transform.position.x + (70 * (x - 1)), fg[x].transform.position.y, fg[x].transform.position.z);
            mg[x].transform.position = new Vector3(mg[x].transform.position.x + (27 * (x - 1)), mg[x].transform.position.y, mg[x].transform.position.z);
            bg[x].transform.position = new Vector3(bg[x].transform.position.x + (27 * (x - 1)), bg[x].transform.position.y, bg[x].transform.position.z);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!sub_area)
        {
            dist = Player.transform.position.x - transform.position.x;
            if (dist < 0)
            {
                dist = Mathf.Abs(dist);
            }

            newPos = Vector3.MoveTowards(transform.position, Player.transform.position, speed * Time.deltaTime * dist);
            newPos.y = 0;
            newPos.z = -10;

            if (newPos.x < bounds.x)
            {
                newPos.x = bounds.x;
            }
            else if (newPos.x > bounds.y)
            {
                newPos.x = bounds.y;
            }

            /*
             * Parralaxing
             * Take change in distance and apply it based on the movement direction as the camera
             * Then see if elemenets need to be replaced
             */
            for(int x = 0; x < 3; x++)
            {
                fg[x].transform.Translate(new Vector3(moveDist * fgMulti * -1, 0, 0));
                mg[x].transform.Translate(new Vector3(moveDist * mgMulti * -0.33f, 0, 0));
                bg[x].transform.Translate(new Vector3(moveDist * bgMulti * -0.33f, 0, 0));
            }

            lastPos = transform.position;
            transform.position = newPos;
            moveDist = transform.position.x - lastPos.x;
        }   
    }
}
