using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public Vector2 dir;
    public float timeout;
    public float speed;

    private float currentTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(currentTime >= timeout)
        {
            Destroy(this.gameObject);
        }

        transform.Translate(dir * speed * Time.deltaTime);

        currentTime += Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(this.gameObject);
    }
}
