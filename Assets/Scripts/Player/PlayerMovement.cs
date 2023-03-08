using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public States current_state;
    public enum States
    {
        Idle = 0b_0000_0000,        // 0
        Walking = 0b_0000_0001,     // 1
        Jumping = 0b_0000_0010,     // 2
        Falling = 0b_0000_0100,     // 4
    }

    public bool direction; //right = false, left = true
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
    public PlayerCombat pc;

    //public GameObject raytest;
    public GameObject Wall = null;
    private int animstate = 0;

    public bool passthrough = false;

    public bool fadeout = false;
    public GameObject fade;

    private bool paused;
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
        if (Input.GetButtonDown("Pause") && !paused)
        {
            paused = true;
            rb.gravityScale = 0;
            rb.velocity = Vector3.zero;
            anim.enabled = false;
        }
        else if(Input.GetButtonDown("Pause") && paused)
        {
            paused = false;
            rb.gravityScale = 6;
            anim.enabled = true;
        }

        if (paused)
            return;

        //Get basic movement
        nomovement = true;
        if (Input.GetButton("Right"))
        {
            nomovement = false;
            direction = false;

            if(Wall)
            {
                if(WalkingToWall())
                {
                    current_state |= States.Walking;
                    sr.flipX = true;
                }
            }
            else
            {
                current_state |= States.Walking;
                sr.flipX = false;
            }
        }

        if (Input.GetButton("Left"))
        {
            nomovement = false;
            direction = true;

            if (Wall)
            {
                if (WalkingToWall())
                {
                    current_state |= States.Walking;
                    sr.flipX = false;
                }
            }
            else
            {
                current_state |= States.Walking;
                sr.flipX = true;
            }
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
            if(rb.velocity.y <= -3.1f) //smooth the transition by making it happen later in the jump arc
            {
                animstate = 2;
            }
        }
        else if((current_state & States.Falling) != States.Falling && (current_state & States.Jumping) == States.Jumping)
        {
            animstate = 0;
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
        else if (animstate == 3 && anim.GetInteger("State") != 3)
        {
            anim.SetInteger("State", 3);
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

    public bool WalkingToWall()
    {
        float xdiff = Wall.transform.position.x - transform.position.x;
        if(xdiff < 0 && direction == true)
        {
            return true;
        }
        else if(xdiff > 0 && direction == false)
        {
            return true;
        }

        return false;
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
        //raytest.transform.position = pretendposition;
        RaycastHit2D hit = Physics2D.Raycast(pretendposition, -Vector2.up, 2.0f, 1 << LayerMask.NameToLayer("Sloped"));

        if (hit.collider != null && hit.point.y < pretendposition.y)
        {
            //on hit snap to the new position and set gravity/y velocity to 0 sso we do not fall through
            rb.gravityScale = 0;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            newpos = new Vector2(hit.point.x, hit.point.y + (transform.localScale.y * 0.5f));
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
                    newpos = new Vector2(hit.point.x - (transform.localScale.x * 0.25f), hit.point.y + (transform.localScale.y * 0.5f));

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
                    newpos = new Vector2(hit.point.x + (transform.localScale.x * 0.25f), hit.point.y + (transform.localScale.y * 0.5f));

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
        rb.gravityScale = 6;
        onslope = false;
        if((current_state & States.Jumping) != States.Jumping)
        {
            //Just started the jump...
            current_state |= States.Jumping;
            rb.velocity = new Vector2(rb.velocity.x, jumpforce);
        }
        else
        {
            if (rb.velocity.y <= 0)
            {
                current_state -= States.Jumping;
                current_state |= States.Falling;
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            if((current_state & States.Jumping) == States.Jumping || (current_state & States.Falling) != States.Falling)
            {
                Wall = collision.gameObject;
            }
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if(Wall)
        {
            Wall = null;
        }
        current_state |= States.Falling;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("SemiSolid"))
        {
            if (passthrough)
            {
                rb.gravityScale = 6;
                current_state |= States.Falling;
                return;
            }
            //Ensure the player is landing onto the platform to check for collision
            float playerbottom = gameObject.transform.position.y - gameObject.transform.localScale.y * 0.25f;
            float platformtop = collision.gameObject.transform.position.y + (collision.transform.localScale.y * 0.5f);

            if (playerbottom > platformtop)
            {
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, 
                                                            platformtop + (gameObject.transform.localScale.y * 0.5f), 
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
        if (collision.gameObject.layer == LayerMask.NameToLayer("SemiSolid") && passthrough)
        {
            rb.gravityScale = 6;
            current_state |= States.Falling;
            return;
        }

        if (interacting)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Door") && !pc.GetComponent<PlayerCombat>().inTransition)
            {
                pc.StartTransition();
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
