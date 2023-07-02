using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Entity
{
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private int lives = 5;
    [SerializeField] private float jumpForce = 15.0f;

    private Rigidbody2D playerRb;
    private SpriteRenderer playerSprite;
    private Animator anim;

    private float leftBoundary = 0.0f;
    private bool isGround = false;
    private int colliderLenght = 1;
    private float circleRadius = 0.3f;

    public static Hero Instance { get; set; }
    private States State
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
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    private void Update()
    {
        if (isGround) State = States.idle;

        if (Input.GetButton("Horizontal"))
        {
            Run();
        }

        if (Input.GetButtonDown("Jump") && isGround)
        {
            Jump();
        }

         
    }
    void Run()
    {
        Vector3 direction = transform.right * Input.GetAxis("Horizontal");
        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);
        playerSprite.flipX = direction.x < leftBoundary;
        if (isGround) State = States.run;
    }

    void Jump()
    {
        playerRb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    void CheckGround()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, circleRadius);
        isGround = collider.Length > colliderLenght;

        if (!isGround) State = States.jump;
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
        jump
    }
}
