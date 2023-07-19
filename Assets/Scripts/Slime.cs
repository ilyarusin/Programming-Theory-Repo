using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Entity // INHERITANCE
{
    void Start()
    {
        lives = 3;
        isDie = false;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject)
        {
            Hero.Instance.GetDamage();
        }
    }

    // POLYMORPHISM
    public override void Die() 
    {
        Destroy(this.gameObject);
        gameObject.tag = "Enemy_dead";
        LevelController.Instance.EnemiesCount();
    }
}
