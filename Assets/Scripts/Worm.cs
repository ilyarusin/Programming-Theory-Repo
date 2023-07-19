using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : Entity
{
    private Animator anim;
    private Collider2D col;

    private void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        lives = 3;
        isDie = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject && lives > 0)
        {
            Hero.Instance.GetDamage();
        }
    }

    public override void Die()
    {
        col.isTrigger = true;
        anim.SetTrigger("death");
        gameObject.tag = "Enemy_dead";
        LevelController.Instance.EnemiesCount();
    }
}
