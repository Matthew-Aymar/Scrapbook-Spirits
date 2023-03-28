using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFireball : MonoBehaviour
{
    public PlayerCombat player;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= player.ground)
        {
            anim.Play("Spawn_Effect");
        }
        else
        {
            transform.Translate(Vector2.down * Time.deltaTime * 20);
        }
    }

    public void ShowPlayer()
    {
        player.combatPlayer.SetActive(true);
    }
}
