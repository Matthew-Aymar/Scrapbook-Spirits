using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Camera cam;
    public GameObject combatTransition;
    public GameObject combatBackground;
    public GameObject combatPlayer;

    public GameObject wallLeft;
    public GameObject wallRight;
    public GameObject wallLeftUpper;
    public GameObject wallRightUpper;

    public PlayerMovement pm;
    public AttackSelector attacker;
    public CardSelector cards;

    private Rigidbody2D combatRb;
    private Animator anim;
    
    public int dir;
    public float speed;
    public float jump;
    public float platform;  //y value of the second level during combat
    public float ground;    //y value of the first level

    public float playerSpawnBuffer;
    private float transitionTime;

    private bool inJump;
    private bool inFall;
    private bool canJump;
    private bool canFall;

    private bool shouldMove;
    private bool inCombat;

    private int jumpDir;
    private bool snapBack;
    private float snapTime;
    private Vector3 snapStart;
    private Vector3 jumpTarget;
    private bool onUpper;

    public bool movementLocked;

    // Start is called before the first frame update
    void Start()
    {
        anim = combatPlayer.GetComponentInChildren<Animator>();
        combatRb = combatPlayer.GetComponent<Rigidbody2D>();
        SwapWalls(onUpper);
    }

    // Update is called once per frame
    void Update()
    {
        if(inCombat)
        {
            if (!combatPlayer.activeSelf && Time.time > transitionTime)
            {
                combatPlayer.SetActive(true);
                cards.ShowCards();
                pm.enabled = false;
                this.gameObject.GetComponent<Rigidbody2D>().simulated = false;
                canJump = true;
            }
            else
            {
                if (Input.GetButtonDown("Special"))
                {
                    cards.HoldCards();
                    attacker.BreakHold();
                    anim.SetInteger("State", 0);
                    dir = 0;
                    shouldMove = false;
                }
                else if (Input.GetButtonUp("Special"))
                {
                    cards.DropCards();
                    attacker.BreakHold();
                }

                if (!inFall && !inJump && !movementLocked)
                {
                    if (canJump && (Input.GetButtonDown("Jump") || Input.GetButtonDown("Interact")))
                    {
                        combatRb.AddForce(new Vector2(dir * speed * 0.5f, jump), ForceMode2D.Impulse);
                        inJump = true;
                        canJump = false;
                        jumpDir = dir;

                        onUpper = true;
                        SwapWalls(onUpper);
                    }
                    else if (canFall && Input.GetButtonDown("Down"))
                    {
                        if ((combatPlayer.transform.localPosition.x < -6.235f && dir != 1) || (combatPlayer.transform.localPosition.x > 6.235f && dir != -1))
                        {
                            combatRb.AddForce(new Vector2((-1 * combatPlayer.transform.localPosition.x) * 2, -jump), ForceMode2D.Impulse);
                        }
                        else if ((combatPlayer.transform.localPosition.x < -3.75f && dir == -1) || (combatPlayer.transform.localPosition.x > 3.75f && dir == 1))
                        {
                            combatRb.AddForce(new Vector2(0, -jump), ForceMode2D.Impulse);
                        }
                        else
                            combatRb.AddForce(new Vector2(dir * speed * 0.5f, -jump), ForceMode2D.Impulse);
                        inFall = true;
                        canFall = false;
                    }
                }
                else
                {
                    shouldMove = false;
                }

                if (combatPlayer.activeSelf && !inJump && !inFall)
                {
                    if (!cards.holding && !movementLocked)
                    {
                        shouldMove = false;

                        if (Input.GetButton("Attack"))
                        {
                            combatRb.velocity = combatRb.velocity * 0.15f;
                            attacker.NewAttack(false);
                            anim.SetInteger("State", 3);
                            movementLocked = true;
                        }
                        else if (Input.GetButton("Right"))
                        {
                            anim.SetInteger("State", 1);
                            shouldMove = true;
                            dir = 1;
                            attacker.BreakHold();
                        }
                        else if (Input.GetButton("Left"))
                        {
                            anim.SetInteger("State", 2);
                            shouldMove = true;
                            dir = -1;
                            attacker.BreakHold();
                        }
                        else
                        {
                            anim.SetInteger("State", 0);
                            dir = 0;
                            attacker.BreakHold();
                        }
                    }
                    else if (cards.holding) 
                    {
                        if(Input.GetButton("Attack"))
                        {
                            if(attacker.nextCheck < Time.time)
                            {
                                attacker.NewAttack(true);
                            }

                            combatRb.velocity = combatRb.velocity * 0.15f;
                            anim.SetInteger("State", 3);
                            movementLocked = true;
                        }
                        else if(Input.GetButtonDown("Right"))
                        {
                            cards.SelectRight();
                        }
                        else if(Input.GetButton("Right"))
                        {
                            dir = 1;
                            if(!attacker.inCharge)
                                anim.SetInteger("State", 0);
                        }
                        else if(Input.GetButtonDown("Left"))
                        {
                            cards.SelectLeft();
                        }
                        else if(Input.GetButton("Left"))
                        {
                            dir = -1;
                            if (!attacker.inCharge)
                                anim.SetInteger("State", 0);
                        }
                        else if(!movementLocked)
                        {
                            anim.SetInteger("State", 0);
                            dir = 0;
                            attacker.BreakHold();
                        }
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        MovementUpdate();
    }

    public void MovementUpdate()
    {
        if (inCombat && combatPlayer.activeSelf)
        {
            if (inJump)
            {
                if (combatPlayer.transform.localPosition.y >= platform * 1.1f)
                {
                    if (!snapBack)
                    {
                        if (jumpDir == 0)
                        {
                            if (combatPlayer.transform.localPosition.x > 0)
                            {
                                jumpTarget = new Vector3(combatPlayer.transform.localPosition.x + 0.75f, platform, 10);
                            }
                            else
                            {
                                jumpTarget = new Vector3(combatPlayer.transform.localPosition.x - 0.75f, platform, 10);
                            }
                        }
                        else
                        {
                            jumpTarget = new Vector3(combatPlayer.transform.localPosition.x + (jumpDir * 1.5f), platform, 10);
                        }

                        snapStart = combatPlayer.transform.localPosition;

                        snapBack = true;
                    }
                }

                if (snapBack)
                {
                    combatPlayer.transform.localPosition = Vector3.Slerp(snapStart, jumpTarget, snapTime);
                    if (snapTime >= 1.0f)
                    {
                        snapBack = false;
                        inJump = false;
                        combatPlayer.transform.localPosition = jumpTarget;
                        combatRb.velocity = new Vector2(combatRb.velocity.x, 0);
                        snapTime = 0;
                        canFall = true;
                    }
                    else if (snapTime < 1.0f)
                    {
                        snapTime += Time.deltaTime * (speed * 0.2f);
                        combatPlayer.transform.localScale = new Vector3(1 + 0.25f * snapTime, 1 + 0.25f * snapTime, 1 + 0.25f * snapTime);
                    }
                }
            }
            else if (shouldMove)
            {
                if (onUpper)
                {
                    if (combatRb.velocity.x <= speed * 1.27f || combatRb.velocity.x >= speed * -1.27f)
                    {
                        combatRb.AddForce(new Vector2(dir, 0) * speed * 1.27f);
                    }
                }
                else
                {
                    if (combatRb.velocity.x <= speed * 1 || combatRb.velocity.x >= speed * -1)
                    {
                        combatRb.AddForce(new Vector2(dir, 0) * speed);
                    }
                }
            }

            if (inFall)
            {
                if (combatPlayer.transform.localPosition.y <= ground)
                {
                    combatRb.velocity = Vector3.zero;
                    combatPlayer.transform.localPosition = new Vector3(combatPlayer.transform.localPosition.x, ground, 20);
                    inFall = false;
                    canJump = true;
                    combatPlayer.transform.localScale = new Vector3(1, 1, 1);

                    combatRb.AddForce(new Vector2(dir * 5, 0), ForceMode2D.Impulse);

                    onUpper = false;
                    SwapWalls(onUpper);
                }
                else
                {
                    float distPercent = (combatPlayer.transform.localPosition.y - ground) / (platform - ground);
                    combatPlayer.transform.localScale = Vector3.Lerp(new Vector3(1.25f, 1.25f, 1.25f), new Vector3(1, 1, 1), 1 - distPercent);
                }
            }
        }
    }

    public void StartTransition()
    {
        combatTransition.SetActive(true);
        combatBackground.SetActive(true);
        transitionTime = 1.05f + playerSpawnBuffer;
        transitionTime += Time.time;
        inCombat = true;
    }

    public void SwapWalls(bool upper)
    {
        wallLeft.SetActive(!upper);
        wallRight.SetActive(!upper);
        wallLeftUpper.SetActive(upper);
        wallRightUpper.SetActive(upper);
    }
}
