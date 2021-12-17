using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject Player;
    public GameObject Foreground;
    public GameObject Treeline;
    public GameObject Sky;

    public float speed;
    public Vector2 bounds;

    private Vector3 temp;
    private float dist;
    private bool left;
    private float percent_travelled;

    public bool sub_area;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void hide_parralax()
    {
        Foreground.SetActive(!Foreground.activeSelf);
        Treeline.SetActive(!Treeline.activeSelf);
        Sky.SetActive(!Sky.activeSelf);
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
                left = false;
            }
            else
            {
                left = true;
            }

            temp = Vector3.MoveTowards(transform.position, Player.transform.position, speed * Time.deltaTime * dist);
            temp.y = 0;
            temp.z = -10;

            if (temp.x < bounds.x)
            {
                temp.x = bounds.x;
            }
            else if (temp.x > bounds.y)
            {
                temp.x = bounds.y;
            }
            else
            {
                if (left)
                {
                    Foreground.transform.Translate(Vector3.left * dist * Time.deltaTime * 3);
                    Treeline.transform.Translate(Vector3.left * dist * Time.deltaTime * 0.5f);
                    Sky.transform.Translate(Vector3.left * dist * Time.deltaTime * 0.25f);
                }
                else
                {
                    Foreground.transform.Translate(Vector3.right * dist * Time.deltaTime * 3);
                    Treeline.transform.Translate(Vector3.right * dist * Time.deltaTime * 0.5f);
                    Sky.transform.Translate(Vector3.right * dist * Time.deltaTime * 0.25f);
                }
            }

            transform.position = temp;
        }   
    }
}
