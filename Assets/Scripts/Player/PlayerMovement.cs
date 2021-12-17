using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public States current_state;
    public enum States
    {
        Idle = 0b_0000_0000,        // 0
        Walking = 0b_0000_0001,     // 1
        Jumping = 0b_0000_0010,     // 2
        Falling = 0b_0000_0100,     // 4
        Attacking = 0b_0000_1000,   // 8
        Wall = 0b_0001_0000,        // 16
        Dash = 0b_0010_0000,        // 32
    }

    //Stats
    public float maxhp;
    public float currenthp;
    public GameObject hpbar;

    public bool direction; //right = false, left = true
    public float speed = 1;
    public float jumpforce = 1;
    public float dashforce = 1;
    //onslope checks if it is moving along the slope
    private bool onslope = false;

    public bool nomovement = false;

    private bool interacting = false;

    public Rigidbody2D rb;
    public BoxCollider2D bc;
    public SpriteRenderer sr;
    public Animator anim;

    //public GameObject raytest;
    public GameObject Wall = null;
    private int animstate = 0;

    public bool passthrough = false;

    public FireScript f;

    private bool candash;
    private float dashtimer = 0;
    private float endofdash = 0;
    private Vector2 dashdir;
    public GameObject dashui;

    public bool inCombat = false;
    private bool isDead = false;
    public bool fadeout = false;
    public GameObject fade;
    private SpriteRenderer fsr;

    private bool paused;
    // Start is called before the first frame update
    void Start()
    {
        //Initialize as falling in case they are not directly placed on ground
        current_state = States.Idle | States.Falling;
        rb.gameObject.GetComponent<Rigidbody2D>();
        if (inCombat)
        {
            fsr = fade.GetComponent<SpriteRenderer>();
            dashui.GetComponent<SpriteRenderer>().enabled = false;
            currenthp = maxhp;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inCombat)
        {
            if(currenthp/maxhp >= 0)
                hpbar.transform.localScale = new Vector3((currenthp/maxhp) * 10, 1, 1);
            else
                hpbar.transform.localScale = new Vector3(0, 1, 1);


            if (currenthp <= 0 || fadeout)
            {
                isDead = true;
                fsr.color = new Color(0, 0, 0, fsr.color.a + 0.01f);

                if(fsr.color.a > 1.0f)
                {
                    SceneManager.LoadScene("SampleScene");
                }
            }
        }
        else
        {
            if(Input.GetButtonDown("Pause") && !paused)
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

            if(Input.GetButtonDown("Start") && paused)
            {
                paused = false;
                rb.gravityScale = 6;
                anim.enabled = true;
            }
        }

        if (paused)
            return;

        if (isDead)
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
                    current_state |= States.Wall;
                    current_state |= States.Walking;
                    sr.flipX = true;
                }
            }
            else
            {
                if((current_state & States.Wall) == States.Wall)
                {
                    current_state -= States.Wall;
                }
                current_state |= States.Walking;
                sr.flipX = false;
                if(inCombat)
                {
                    f.SetPos(1);
                }
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
                    current_state |= States.Wall;
                    current_state |= States.Walking;
                    sr.flipX = false;
                }
            }
            else
            {
                if ((current_state & States.Wall) == States.Wall)
                {
                    current_state -= States.Wall;
                }
                current_state |= States.Walking;
                sr.flipX = true;
                if (inCombat)
                {
                    f.SetPos(2);
                }
            }
        }

        if (inCombat)
        {
            if (Input.GetButton("Down") && passthrough == false)
            {
                passthrough = true;
            }
            else if (!Input.GetButton("Down") && passthrough == true)
            {
                passthrough = false;
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

        if (inCombat)
        {
            if (Input.GetButtonDown("Attack"))
            {
                f.FireAttack();
            }


            if (Input.GetButtonDown("Dash") && (current_state & States.Dash) != States.Dash)
            {
                if (candash)
                {
                    dashui.GetComponent<SpriteRenderer>().enabled = true;
                    candash = false;
                    dashtimer = 0;
                    current_state |= States.Dash;
                }
            }
        }

        //Check if the player has hit the apex of thier jump and will switch to falling state
        if((current_state & States.Jumping) == States.Jumping)
        {
            Jump();
        }

        if (inCombat)
        {
            //Done after jump to pause in mid air
            if ((current_state & States.Dash) == States.Dash)
            {
                Dash();
            }
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

        if (inCombat)
        {
            if (sr.flipX)
            {
                if ((current_state & States.Walking) == States.Walking)
                {
                    f.SetPos(4);
                }
                else
                {
                    f.SetPos(2);
                }
            }
            else
            {
                if ((current_state & States.Walking) == States.Walking)
                {
                    f.SetPos(3);
                }
                else
                {
                    f.SetPos(1);
                }
            }
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
        if ((current_state & States.Dash) == States.Dash)
        {
            return;
        }

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
        if ((current_state & States.Dash) == States.Dash)
        {
            return;
        }

        if (rb.velocity.y > -0.001 && rb.velocity.y <= 0.001)
        {
            candash = false;
            current_state -= States.Falling;
        }
    }

    public void Jump()
    {
        if ((current_state & States.Dash) == States.Dash)
        {
            return;
        }

        rb.gravityScale = 6;
        onslope = false;
        if((current_state & States.Jumping) != States.Jumping)
        {
            //Just started the jump...
            candash = true;
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

    public void Dash()
    {
        rb.gravityScale = 0;

        if(endofdash != 0)
        {
            if(dashtimer > endofdash)
            {
                dashui.GetComponent<SpriteRenderer>().enabled = false;
                current_state -= States.Dash;
                endofdash = 0;
                rb.gravityScale = 6;
            }
            else
            {
                dashtimer += Time.deltaTime;
                rb.velocity = new Vector2(dashdir.x * dashforce, dashdir.y * dashforce);
            }
        }
        else if(dashtimer > 0.25f)
        {
            dashdir = new Vector2();

            if(Input.GetButton("Left"))
            {
                dashdir.x -= 1;
            }

            if (Input.GetButton("Right"))
            {
                dashdir.x += 1;
            }

            if (Input.GetButton("Interact"))
            {
                dashdir.y += 1;
            }

            if (!onslope && Input.GetButton("Down"))
            {
                dashdir.y -= 1;
            }

            dashdir.Normalize();
            endofdash = dashtimer + 0.1f;
        }
        else
        {
            Vector2 tempdir = new Vector2();

            if (Input.GetButton("Left"))
            {
                tempdir.x -= 1;
            }

            if (Input.GetButton("Right"))
            {
                tempdir.x += 1;
            }

            if (Input.GetButton("Interact"))
            {
                tempdir.y += 1;
            }

            if (!onslope && Input.GetButton("Down"))
            {
                tempdir.y -= 1;
            }
            tempdir.Normalize();
            Vector2 newpos = this.transform.position;
            newpos += tempdir * 2;

            dashui.transform.up = tempdir;

            dashui.transform.position = newpos;

            dashtimer += Time.deltaTime;
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
            if (collision.gameObject.layer == LayerMask.NameToLayer("Door"))
            {
                SceneManager.LoadScene("combatTest");
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        rb.gravityScale = 6;
        if((current_state & States.Jumping) != States.Jumping && (current_state & States.Falling) != States.Falling)
        {
            candash = true;
            rb.velocity = new Vector2(rb.velocity.x, -0.002f);
        }
        current_state |= States.Falling;
    }
}
