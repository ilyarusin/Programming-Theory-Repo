using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hero : Entity
{
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private int health;
    [SerializeField] private float jumpForce = 15.0f;
    [SerializeField] private Image[] hearts;

    [SerializeField] private Sprite aliveHeart;
    [SerializeField] private Sprite deadHeart;
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource damageSound;
    [SerializeField] private AudioSource missAttack;
    [SerializeField] private AudioSource attackMob;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject winPanel;

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
    public Joystick joystick;
    bool isDead = false;

    public static Hero Instance { get; set; }


    private States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    private void Awake()
    {
        lives = 5;
        health = lives;
        Instance = this;
        playerRb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerSprite = GetComponentInChildren<SpriteRenderer>();
        isRecharged = true;
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    private void Update()
    {
        if (isGround && !isAttacking && health >= 0) State = States.idle;

        if (!isAttacking && (joystick.Horizontal != 0 || Input.GetButton("Horizontal")) && health >= 0)
        {
            Run();
        }

        if (!isAttacking && isGround && (joystick.Vertical > 0.5f || Input.GetButtonDown("Jump")))
        {
            Jump();
        }

        if (Input.GetButtonDown("Fire1") && !isDead)
        {
            Attack();
        }
        

        if (transform.position.y < -18)
        {
            Die();
        }

        if (health > lives)
            health = lives;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
                hearts[i].sprite = aliveHeart;
            else
                hearts[i].sprite = deadHeart;

            if (i < lives)
                hearts[i].enabled = true;
            else
                hearts[i].enabled = false;
        }
    }
    void Run()
    {
        if (isGround) State = States.run;

        // Vector3 direction = transform.right * joystick.Horizontal;
        Vector3 direction = transform.right * Input.GetAxis("Horizontal");
        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);
        playerSprite.flipX = direction.x < leftBoundary;

    }

    void Jump()
    {
        playerRb.velocity = Vector2.up * jumpForce;
        jumpSound.Play();
    }

    public void Attack()
    {
        if (isGround && isRecharged)
        {
            State = States.attack;
            isAttacking = true;
            isRecharged = false;

            StartCoroutine(AttackAnimation());
            StartCoroutine(AttackCoolDown());
        }
    }

    void OnAttack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPos.position, attackRange, enemy);

        if (colliders.Length == 0)
            missAttack.Play();
        else
            attackMob.Play();

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].GetComponent<Entity>().GetDamage();
            // StartCoroutine(EnemyOnAttack(colliders[i]));
            if (!colliders[i].GetComponent<Entity>().GetIsDie())
                StartCoroutine(EnemyOnAttack(colliders[i]));
        }
    }

    IEnumerator AttackAnimation()
    {
        yield return new WaitForSeconds(0.6f);
        isAttacking = false;
    }

    IEnumerator AttackCoolDown()
    {
        yield return new WaitForSeconds(0.7f);
        isRecharged = true;
    }

    IEnumerator EnemyOnAttack(Collider2D enemy)
    {
        SpriteRenderer enemyColor = enemy.GetComponent<SpriteRenderer>();
        enemyColor.color = new Color(1f, 0.4375f, 0.4375f);
        yield return new WaitForSeconds(0.2f);
        enemyColor.color = new Color(1, 1, 1);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }

    // ABSTRACTION
    void CheckGround()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, circleRadius);
        isGround = collider.Length > colliderLenght;

        if (!isGround && health >= 0) State = States.jump;
    }

    public override void GetDamage()
    {
        lives -= 1;
        damageSound.Play();
        if (health == 0)
        {
            foreach (var h in hearts)
                h.sprite = deadHeart;
            Die();
        }
    }


    public override void Die()
    {
        State = States.death;
        isDead = true;
        LevelController.Instance.EnemiesCount();
        Invoke("SetLosePanel", 1.1f);
    }

    void SetLosePanel()
    {
        losePanel.SetActive(true);
        Time.timeScale = 0;
    }

    void SetWinPanel()
    {
        winPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public enum States
    {
        idle,
        run,
        jump,
        attack,
        death
    }
}
