using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public States current_state;
    public enum States
    {
        Idle = 0b_0000_0000,  // 0
        Walking = 0b_0000_0001,  // 1
        Jumping = 0b_0000_0010,  // 2
        Falling = 0b_0000_0100,  // 4
        Attacking = 0b_0000_1000,  // 8
    }
    public bool direction; //right = false, left = true
    public bool doublejumped;
    public float speed = 1;
    public float jumpforce = 1;

    //onslope checks if it is moving along the slope
    private bool onslope = false;

    public bool nomovement = false;

    private bool interacting = false;

    public Rigidbody2D rb;
    public BoxCollider2D bc;
    public SpriteRenderer sr;
    public Animator anim;

    public GameObject raytest;

    private float feetoffset = 0.2f; //value to make sure the player's feet are on the ground for semi solid platforms
    private int animstate = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Initialize as falling in case they are not directly placed on ground
        current_state = States.Idle | States.Falling;
        rb.gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Get basic movement
        nomovement = true;
        if (Input.GetButton("Right"))
        {
            nomovement = false;
            direction = false;

            current_state |= States.Walking;
            sr.flipX = false;
        }

        if (Input.GetButton("Left"))
        {
            nomovement = false;
            direction = true;

            current_state |= States.Walking;
            sr.flipX = true;
        }

        //Check if the player is trying to interact with a object
        if (Input.GetButton("Interact"))
        {
            interacting = true;
        }
        else if (interacting)
        {
            interacting = false;
        }

        //Update based on movement
        if ((current_state & States.Walking) == States.Walking)
        {
            Walk();
        }

        //Update falling, can only jump while not falling
        if((current_state & States.Falling) == States.Falling)
        {
            Fall();
        }
        else
        {
            if(Input.GetButtonDown("Jump"))
            {
                Jump();
            }
        }

        //Check if the player has hit the apex of thier jump and will switch to falling state
        if((current_state & States.Jumping) == States.Jumping)
        {
            Jump();
        }

        //Animation controller
        animstate = 0;
        if ((current_state & States.Falling) == States.Falling && (current_state & States.Jumping) != States.Jumping && !onslope)
        {
            animstate = 2;
        }
        else if((current_state & States.Walking) == States.Walking)
        {
            animstate = 1;
        }

        if(animstate == 0 && anim.GetInteger("State") != 0)
        {
            anim.SetInteger("State", 0);
        }
        else if(animstate == 1 && anim.GetInteger("State") != 1)
        {
            anim.SetInteger("State", 1);
        }
        else if(animstate == 2 && anim.GetInteger("State") != 2)
        {
            anim.SetInteger("State", 2);
        }
    }

    private void FixedUpdate()
    {
        //Snap to the slope
        if (onslope)
        {
            Vector2 temp = CastToSlope(transform.position);
            transform.position = new Vector2(temp.x, temp.y);
        }

        //Need to simulate friction even when gravity is set to 0 for the purpose of one-way and sloped platforms
        if (rb.gravityScale == 0)
        {
            float newvelx = rb.velocity.x;
            //0.4f is the default friction coefficient, 6 is the proper gravity scale, 5 is the mass of our object
            //Although this isnt the proper method it gives a feel similar to the default on normal ground
            if (newvelx < 0)
            {
                newvelx += 0.4f * Time.deltaTime * 6 * 5;
                if(newvelx > -0.01f)
                {
                    newvelx = 0;
                }
            }
            else
            {
                newvelx -= 0.4f * Time.deltaTime * 6 * 5;
                if(newvelx < 0.01f)
                {
                    newvelx = 0;
                }
            }
            
            //Set our x vel to the new value after friction
            rb.velocity = new Vector2(newvelx, rb.velocity.y);
        }
    }

    public void Walk()
    {
        if (direction == false)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);

            //Check if they should not be moving anymore
            if (Input.GetButtonUp("Right"))
            {
                current_state -= States.Walking;
            }
        }
        else if (direction == true)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);

            //Check if they should not be moving anymore
            if (Input.GetButtonUp("Left"))
            {
                current_state -= States.Walking;
            }
        }
    }

    public Vector2 CastToSlope(Vector2 current)
    {
        //Create a raycast from a little above the current player's bottom position, aim down towards the assumed slope
        Vector2 newpos = new Vector2(current.x, current.y);

        Vector2 pretendposition = new Vector2(newpos.x, newpos.y - (transform.localScale.y * 0.3f));
        raytest.transform.position = pretendposition;
        RaycastHit2D hit = Physics2D.Raycast(pretendposition, -Vector2.up, 2.0f, 1 << LayerMask.NameToLayer("Sloped"));

        if (hit.collider != null && hit.point.y < pretendposition.y)
        {
            //on hit snap to the new position and set gravity/y velocity to 0 sso we do not fall through
            rb.gravityScale = 0;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            newpos = new Vector2(hit.point.x, hit.point.y + (transform.localScale.y * 0.5f) + feetoffset);
        }
        else
        {
            /*
             * messy
             * on fail check further on the side we are moving towards in case we clipped it
             * if the player is not pressing a movement button then we assume they are trying to land and check both sides
             */
            if (direction == false || nomovement)
            {
                hit = Physics2D.Raycast(new Vector2(pretendposition.x + (transform.localScale.x * 0.25f), pretendposition.y), -Vector2.up, 2.0f, 1 << LayerMask.NameToLayer("Sloped"));
                if (hit.collider != null && hit.point.y < pretendposition.y)
                {
                    rb.gravityScale = 0;
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    newpos = new Vector2(hit.point.x - (transform.localScale.x * 0.25f), hit.point.y + (transform.localScale.y * 0.5f) + feetoffset);

                    return newpos;
                }
            }

            if (direction == true || nomovement)
            {
                hit = Physics2D.Raycast(new Vector2(pretendposition.x - (transform.localScale.x * 0.25f), pretendposition.y), -Vector2.up, 2.0f, 1 << LayerMask.NameToLayer("Sloped"));
                if (hit.collider != null && hit.point.y < pretendposition.y)
                {
                    rb.gravityScale = 0;
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    newpos = new Vector2(hit.point.x + (transform.localScale.x * 0.25f), hit.point.y + (transform.localScale.y * 0.5f) + feetoffset);

                    return newpos;
                }
            }

            //if it misses then we are no longer attached to the slope and proceed to move normally
            onslope = false;
            rb.gravityScale = 6;
        }

        return newpos;
    }

    public void Fall()
    {
        if (rb.velocity.y > -0.001 && rb.velocity.y <= 0.001)
        {
            current_state -= States.Falling;
        }
    }

    public void Jump()
    {
        //Just started the jump...
        rb.gravityScale = 6;
        onslope = false;
        if((current_state & States.Jumping) != States.Jumping)
        {
            current_state |= States.Jumping;
            rb.velocity = new Vector2(rb.velocity.x, jumpforce);
        }
        else
        {
            if (rb.velocity.y <= 0)
            {
                doublejumped = false;
                current_state -= States.Jumping;
                current_state |= States.Falling;
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {


    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        current_state |= States.Falling;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("SemiSolid"))
        {
            //Ensure the player is landing onto the platform to check for collision
            float playerbottom = gameObject.transform.position.y - gameObject.transform.localScale.y * 0.25f;
            float platformtop = collision.gameObject.transform.position.y + (collision.transform.localScale.y * 0.5f);

            if (playerbottom > platformtop)
            {
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, 
                                                            platformtop + (gameObject.transform.localScale.y * 0.5f) + feetoffset, 
                                                            gameObject.transform.position.z);
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.gravityScale = 0;
            }
        }
        else if(collision.gameObject.layer == LayerMask.NameToLayer("Sloped"))
        {
            onslope = true;
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if(interacting)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Door"))
            {
                InteractableDoor d = collision.gameObject.GetComponent<InteractableDoor>();
                gameObject.transform.position = d.UseDoor();
                Camera.main.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, Camera.main.transform.position.z);
                interacting = false;
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        rb.gravityScale = 6;
        if((current_state & States.Jumping) != States.Jumping && (current_state & States.Falling) != States.Falling)
        {
            rb.velocity = new Vector2(rb.velocity.x, -0.002f);
        }
        current_state |= States.Falling;
    }
}
