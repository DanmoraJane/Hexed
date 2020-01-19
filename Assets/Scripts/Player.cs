using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    //movement variables
    public float moveSpeedPlayer;
    private bool facingRight;

    //jumping variables
    public float jumpY = 5f;
    public bool groundCheck;
    public bool fallCheck;
    public bool jumpCheck;
    public bool canDoubleJump;

    //references
    private Rigidbody2D body;
    private GameMaster gm;
    public Animator animator;

    [SerializeField] private Stats hp;
    [SerializeField] private Stats mp;

    // Start is called before the first frame update
    void Start()
    {
        facingRight = true;
        body = gameObject.GetComponent<Rigidbody2D>();
        hp.Initialize(100, 100);
        mp.Initialize(100, 100);
        gm = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameMaster>();
    }

    // Update is called once per frame
    void Update()
    {

        animator.SetFloat("Speed", Mathf.Abs(Input.GetAxis("Horizontal")));
        animator.SetBool("GroundCheck", groundCheck);
        animator.SetBool("FallCheck", fallCheck);
        animator.SetBool("JumpCheck", jumpCheck);

        SetAnimationState();

        if (transform.position.y < -6)
            hp.MyCurVal = 0;
        
        if (hp.MyCurVal <= 0)
            Die();
    }

    void FixedUpdate()
    {

        Move(Input.GetAxis("Horizontal"));

        Jump();

        Flip(Input.GetAxis("Horizontal"));
    }

    void Move(float horizontal)
    {
        Vector2 moveVelocity = body.velocity;
        moveVelocity.x = horizontal * moveSpeedPlayer;
        body.velocity = moveVelocity;
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (groundCheck == true)
            {
                //adds a force and the type of force
                body.AddForce(new Vector2(0.0f, jumpY), ForceMode2D.Impulse);
                canDoubleJump = true;
            } else {
                if (canDoubleJump)
                {
                    canDoubleJump = false;
                    body.velocity = new Vector2(body.velocity.x, 0.0f);
                    body.AddForce(new Vector2(0.0f, jumpY), ForceMode2D.Impulse);
                }
            }
            
        }
    }


    private void Flip(float horizontal)
    {
        if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
        {
            facingRight = !facingRight;

            Vector3 theScale = transform.localScale;

            theScale.x *= -1;

            transform.localScale = theScale;
        }
    }

    void Die()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void Damage(int damage)
    {
        hp.MyCurVal -= damage;
        gameObject.GetComponent<Animation>().Play("Player_Damage");
    }

    public IEnumerator Knockback(float kbDuration, float kbPower, Vector3 kbDirection)
    {
        float timer = 0;

        while (kbDuration > timer)
        {
            timer += Time.deltaTime;
            body.AddForce(new Vector3(kbDirection.x * -70, kbDirection.y + kbPower, transform.position.z));
        }
            
        yield return 0;

    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("coin") && collider.gameObject.GetComponent<BoxCollider2D>().enabled)
        {
            collider.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            Destroy(collider.gameObject);
            gm.coins += 1;
        }
    }

    void SetAnimationState()
    {
        
        if (body.velocity.y == 0)
            animator.SetBool("FallCheck", false);
        if (body.velocity.y > 0)
            animator.SetBool("JumpCheck", true);
        if (body.velocity.y < 0)
        {
            animator.SetBool("JumpCheck", false);
            animator.SetBool("FallCheck", true);
        }
            
    }
        
}
