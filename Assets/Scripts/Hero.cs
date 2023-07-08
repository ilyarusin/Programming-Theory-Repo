using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Entity
{
    [SerializeField] private float speed = 3.0f;
    // [SerializeField] private int lives = 5;
    [SerializeField] private float jumpForce = 15.0f;

    private Rigidbody2D playerRb;
    private SpriteRenderer playerSprite;
    private Animator anim;

    private float leftBoundary = 0.0f;
    private bool isGround = false;
    private int colliderLenght = 1;
    private float circleRadius = 0.3f;

    public bool isAttacking = false;
    public bool isRecharged = true;
    public Transform attackPos;
    public float attackRange;
    public LayerMask enemy;

    public static Hero Instance { get; set; }


    private States state
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();
        playerSprite = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponent<Animator>();
        Instance = this;
        isRecharged = true;
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    private void Update()
    {
        if (isGround && !isAttacking) state = States.idle;

        if (Input.GetButton("Horizontal") && !isAttacking)
        {
            Run();
        }

        if (!isAttacking && Input.GetButtonDown("Jump") && isGround)
        {
            Jump();
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }

        if (transform.position.y < -12)
        {
            Die();
        }
    }
    void Run()
    {
        Vector3 direction = transform.right * Input.GetAxis("Horizontal");
        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);
        playerSprite.flipX = direction.x < leftBoundary;
        if (isGround) state = States.run;
    }

    void Jump()
    {
        playerRb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    void Attack()
    {
        if (isGround && isRecharged)
        {
            state = States.attack;
            isAttacking = true;
            isRecharged = false;

            StartCoroutine(AttackAnimation());
            StartCoroutine(AttackCoolDown());
        }
    }

    void OnAttack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPos.position, attackRange, enemy);

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].GetComponent<Entity>().GetDamage();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }

    IEnumerator AttackAnimation()
    {
        yield return new WaitForSeconds(0.4f);
        isAttacking = false;
    }

    IEnumerator AttackCoolDown()
    {
        yield return new WaitForSeconds(0.5f);
        isRecharged = true;
    }

    void CheckGround()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, circleRadius);
        isGround = collider.Length > colliderLenght;


        if (!isGround) state = States.jump;
    }

    public override void GetDamage()
    {
        lives -= 1;
        Debug.Log(lives);

        /* if (lives < 1)
             Die();
        */
    }

    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (lives < 1)
            Die();
    }

    */

    public enum States
    {
        idle,
        run,
        jump,
        attack
    }
}
