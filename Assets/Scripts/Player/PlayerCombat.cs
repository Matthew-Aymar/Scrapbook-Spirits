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

    public SpriteRenderer darkBot;
    public SpriteRenderer darkTop;
    public SpriteRenderer lightBot;
    public SpriteRenderer lightTop;

    public GameObject spawnObject;

    public PlayerMovement pm;
    public AttackSelector attacker;
    public CardSelector cards;
    public EnemyManager enemy;

    private Rigidbody2D combatRb;
    private Animator anim;
    private SpriteRenderer sr;

    public int dir;         //Direction of movement
    public float speed;
    public float jump;
    public float platform;  //y value of the second level during combat
    public float ground;    //y value of the first level

    public bool inTransition;

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
    public bool onUpper;

    public bool movementLocked;
    public bool inAttackAnim;

    private bool justLanded = false;
    private bool justAttacked = false;

    public int attackDir;       //Direction to enemy
    public bool canCancel;

    // Start is called before the first frame update
    void Start()
    {
        anim = combatPlayer.GetComponentInChildren<Animator>();
        sr = combatPlayer.GetComponentInChildren<SpriteRenderer>();
        combatRb = combatPlayer.GetComponent<Rigidbody2D>();
        SwapWalls(onUpper);

        canCancel = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(inCombat)
        {
            if (enemy.currentEnemy && Vector3.Distance(new Vector3(combatPlayer.transform.localPosition.x,0,0), new Vector3(enemy.currentEnemy.transform.localPosition.x,0,0)) > 0.25f)
            {
                if(!cards.usingCard)
                {
                    if (combatPlayer.transform.localPosition.x < enemy.currentEnemy.transform.localPosition.x)
                    {
                        attackDir = 1;
                        sr.flipX = false;
                    }
                    else
                    {
                        attackDir = -1;
                        sr.flipX = true;
                    }
                }
            }

            if (combatPlayer.activeSelf && inTransition)
            {
                enemy.NewEnemy();
                cards.ShowCards();
                pm.enabled = false;
                this.gameObject.GetComponent<Rigidbody2D>().simulated = false;
                canJump = true;
                combatPlayer.transform.localPosition = new Vector3(0, -2, 10);

                inTransition = false;
            }
            else if(combatPlayer.activeSelf)
            {
                if (Input.GetButtonDown("Special") && !inAttackAnim && !movementLocked)
                {
                    cards.HoldCards();
                    attacker.BreakHold();
                    dir = 0;
                    shouldMove = false;
                }
                else if (Input.GetButtonUp("Special") && !inAttackAnim && !attacker.inCharge)
                {
                    cards.DropCards();
                    attacker.BreakHold();
                    attacker.StopUsing();
                }

                if(cards.holding && !inAttackAnim && !Input.GetButton("Special"))
                {
                    cards.DropCards();
                }

                if (!inFall && !inJump && canCancel)
                {
                    if (canJump && (Input.GetButton("Jump") || Input.GetButton("Interact")))
                    {
                        if (Input.GetButton("Right"))
                            jumpDir = 1;

                        if (Input.GetButton("Left"))
                            jumpDir = -1;

                        combatRb.AddForce(new Vector2(jumpDir * speed * 0.5f, jump), ForceMode2D.Impulse);
                        inJump = true;
                        canJump = false;

                        onUpper = true;
                        SwapWalls(onUpper);
                        attacker.JumpCancel();
                    }
                    else if (canFall && Input.GetButton("Down"))
                    {
                        if (Input.GetButton("Right"))
                            jumpDir = 1;

                        if (Input.GetButton("Left"))
                            jumpDir = -1;

                        if ((combatPlayer.transform.localPosition.x < -6.235f && jumpDir != 1) || (combatPlayer.transform.localPosition.x > 6.235f && jumpDir != -1))
                        {
                            combatRb.AddForce(new Vector2((-1 * combatPlayer.transform.localPosition.x) * 2, -jump), ForceMode2D.Impulse);
                        }
                        else if ((combatPlayer.transform.localPosition.x < -3.75f && jumpDir == -1) || (combatPlayer.transform.localPosition.x > 3.75f && jumpDir == 1))
                        {
                            combatRb.AddForce(new Vector2(0, -jump), ForceMode2D.Impulse);
                        }
                        else
                            combatRb.AddForce(new Vector2(jumpDir * speed * 0.5f, -jump), ForceMode2D.Impulse);
                        inFall = true;
                        canFall = false;
                        attacker.JumpCancel();
                    }
                }
                else
                {
                    shouldMove = false;
                }

                if (!inJump && !inFall)
                {
                    if (!cards.holding && !movementLocked) //normal attacks and inputs
                    {
                        shouldMove = false;

                        if (Input.GetButton("Attack"))
                        {
                            if((Input.GetButtonDown("Attack") || justLanded) && !attacker.isHeld)
                            {
                                if (!justAttacked)
                                {
                                    anim.Play("Wisp_Charge");
                                    anim.SetInteger("State", 3);
                                }

                                combatRb.velocity = combatRb.velocity * 0.15f;
                                movementLocked = true;
                            }

                            attacker.NewAttack(false);
                        }
                        else if (Input.GetButton("Right"))
                        {
                            if (attackDir == 1 && !justAttacked)
                                anim.SetInteger("State", 1);
                            else if(!justAttacked)
                                anim.SetInteger("State", 2);
                            shouldMove = true;
                            dir = 1;
                            attacker.BreakHold();
                            attacker.StopUsing();
                        }
                        else if (Input.GetButton("Left"))
                        {
                            if (attackDir == -1 && !justAttacked)
                                anim.SetInteger("State", 1);
                            else if(!justAttacked)
                                anim.SetInteger("State", 2);
                            shouldMove = true;
                            dir = -1;
                            attacker.BreakHold();
                            attacker.StopUsing();
                        }
                        else
                        {
                            if (!justAttacked)
                                anim.SetInteger("State", 0);
                            dir = 0;
                            attacker.BreakHold();
                            attacker.StopUsing();
                        }
                    }
                    else if (cards.holding) //Card-based attacks and inputs
                    {
                        if(Input.GetButton("Attack"))
                        {
                            if ((Input.GetButtonDown("Attack") || justLanded) && !attacker.isHeld && !movementLocked)
                            {
                                if (!justAttacked)
                                {
                                    anim.Play("Wisp_Charge");
                                    anim.SetInteger("State", 3);
                                }

                                cards.UseCard();

                                combatRb.velocity = combatRb.velocity * 0.15f;
                                movementLocked = true;
                            }

                            if (attacker.nextCheck < Time.time)
                            {
                                attacker.NewAttack(true);
                            }
                        }
                        else if(Input.GetButtonUp("Attack"))
                        {
                            cards.DropCards();
                            attacker.BreakHold();
                        }
                        else if(Input.GetButtonDown("Right"))
                        {
                            cards.SelectRight();
                        }
                        else if(Input.GetButton("Right"))
                        {
                            dir = 1;
                            if(!attacker.inCharge)
                            {
                                if (!justAttacked)
                                    anim.SetInteger("State", 0);
                            }
                        }
                        else if(Input.GetButtonDown("Left"))
                        {
                            cards.SelectLeft();
                        }
                        else if(Input.GetButton("Left"))
                        {
                            dir = -1;
                            if (!attacker.inCharge)
                            {
                                if (!justAttacked)
                                    anim.SetInteger("State", 0);
                            }
                        }
                        else if(!movementLocked)
                        {
                            if (!justAttacked)
                                anim.SetInteger("State", 0);
                            dir = 0;
                            attacker.BreakHold();
                            attacker.StopUsing();
                        }
                    }
                }
            }
            else
            {
                lightBot.color = new Color(1, 1, 1, lightBot.color.a * (1 + Time.deltaTime * 1.6f));
                lightTop.color = new Color(1, 1, 1, lightTop.color.a * (1 + Time.deltaTime * 1.6f));
                darkBot.color = new Color(1, 1, 1, darkBot.color.a * (1 + Time.deltaTime * 1.6f));
                darkTop.color = new Color(1, 1, 1, darkTop.color.a * (1 + Time.deltaTime * 1.6f));
            }
        }

        if(onUpper)
        {
            lightBot.enabled = false;
            if(!inJump)
                lightTop.enabled = true;
        }
        else
        {
            lightTop.enabled = false;
            lightBot.enabled = true;
        }

        if(Input.GetButtonDown("Draw"))
        {
            cards.DrawCard();
        }

        justLanded = false;

        if(justAttacked)
        {
            anim.SetInteger("State", 4);
            justAttacked = false;
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
                        justLanded = true;
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
                    justLanded = true;
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
        inTransition = true;

        lightBot.color = new Color(1, 1, 1, 0.1f);
        lightTop.color = new Color(1, 1, 1, 0.1f);
        darkBot.color = new Color(1, 1, 1, 0.1f);
        darkTop.color = new Color(1, 1, 1, 0.1f);

        GameObject temp = Instantiate(spawnObject, cam.transform);
        temp.transform.Translate(Vector2.up * 20);
        temp.GetComponent<SpawnFireball>().player = this;

        inCombat = true;
    }

    public void SwapWalls(bool upper)
    {
        wallLeft.SetActive(!upper);
        wallRight.SetActive(!upper);
        wallLeftUpper.SetActive(upper);
        wallRightUpper.SetActive(upper);
    }

    public void EndCharge()
    {
        anim.SetInteger("State", 4);
        inAttackAnim = true;
        justAttacked = true;
    }

    public void SetIdle()
    {
        if (!justAttacked)
            anim.SetInteger("State", 0);
    }
}
