using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{

    //movement variables
    public float moveSpeed;
    public bool facingRight;

    //jumping variables
    public float jumpY = 5f;
    public bool groundCheck;
    public bool fallCheck;
    public bool jumpCheck;
    public bool canDoubleJump;


    //health
    public int maxHealth = 3;
    public int health;

    //references
    private Rigidbody2D body;
    private GameMaster gm;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {

        facingRight = true;

        body = gameObject.GetComponent<Rigidbody2D>();

        health = maxHealth;

        gm = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameMaster>();

      
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0.0f, 0.0f);
        float horizontal = Input.GetAxis("Horizontal");

        animator.SetFloat("Speed", Mathf.Abs(horizontal));
        animator.SetBool("GroundCheck", groundCheck);
        animator.SetBool("FallCheck", fallCheck);
        animator.SetBool("JumpCheck", jumpCheck);

        //move the character
        transform.position += movement * Time.deltaTime * moveSpeed;

        Jump();

        SetAnimationState();
        
        Flip(horizontal);

        if (transform.position.y < -6)
            health = 0;

        if (health > maxHealth)
            health = maxHealth;

        if (health < 0)
            health = 0;
        
        if (health <= 0)
            Die();
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
        Application.LoadLevel(Application.loadedLevel);
    }

    public void Damage(int damage)
    {
        health -= damage;
        gameObject.GetComponent<Animation>().Play("Player_Damage");
    }

    public IEnumerator Knockback(float kbDuration, float kbPower, Vector3 kbDirection)
    {
        float timer = 0;

        while (kbDuration > timer)
        {
            timer += Time.deltaTime;
            body.AddForce(new Vector3(kbDirection.x * -30, kbDirection.y + kbPower, transform.position.z));
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
