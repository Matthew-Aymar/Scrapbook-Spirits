using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : MonoBehaviour
{
    public float rotatespeed;
    public float movespeed;

    public RenderTexture rt;
    public MeshRenderer mr;
    public Camera cam;

    private bool transitioning = false;
    private bool rendering;

    // Start is called before the first frame update
    void Start()
    {
        rt.width = Screen.width;
        rt.height = Screen.height;
    }

    // Update is called once per frame
    void Update()
    {
        if(transitioning)
        {
            if(!rendering)
            {
                mr.enabled = true;
                rendering = true;
            }

            if(transform.rotation.eulerAngles.y < 90)
            {
                transform.Rotate(new Vector3(0, 1, 0), rotatespeed * Time.deltaTime);
            }
            else
            {
                transitioning = false;
            }

            if(transform.position.x > cam.ScreenToWorldPoint(new Vector3(0, 0)).x)
            {
                transform.Translate(Vector3.left * movespeed * Time.deltaTime);
            }
            else
            {
                transitioning = false;
            }
        }
        else if(rendering)
        {
            mr.enabled = false;
            rendering = false;
        }
    }

    public void StartTransition(Vector3 startpos, Vector3 newpos)
    {
        gameObject.transform.position = new Vector3(newpos.x, newpos.y, -5);
        cam.transform.position = new Vector3(startpos.x, startpos.y, startpos.z);
        transitioning = true;
    }
}
