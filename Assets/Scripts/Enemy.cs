using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeedEnemy;
    public float range;
    public float groundRange;

    private float attackTimer;  //timed attack animation
    private float nowSpeed;     //current speed to pass to animator
    private bool facingRight = true;
    private bool playerNear = false;

    private Player player;
    private Rigidbody2D body;

    public Transform enemyGroundCheck;     //checks ground for collision
    public Transform playerTarget;         //retrieves player position
    public Animator enemyAnimator;         //retrives enemy Animator
    
    void Start()
    {
        Physics2D.queriesStartInColliders = false;

        playerTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();   

        body = gameObject.GetComponent<Rigidbody2D>();

    }

    void Update()
    {

        //ray from enemy to player to see if he sees the player
        Vector3 direction = (playerTarget.position - transform.position).normalized;
        RaycastHit2D playerInfo = Physics2D.Raycast(transform.position, direction, range);
        Debug.DrawRay(transform.position, direction, Color.red);

        //check if there's ground
        RaycastHit2D groundInfo = Physics2D.Raycast(enemyGroundCheck.position, Vector2.down, groundRange);
        //check if there's an obstacle
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.right, 0.5f);

        //wait before attacking
        if(attackTimer <= 0f) attackTimer = 1f;

        //animator variables
        enemyAnimator.SetFloat("Speed", nowSpeed);
        enemyAnimator.SetBool("GroundInfo", groundInfo);
        enemyAnimator.SetBool("PlayerNear", playerNear);


        //behaviour when no player in sight
        Patrol(hitInfo, groundInfo);

        Debug.DrawRay(enemyGroundCheck.position, Vector2.down, Color.blue);
        
        //behaviour with player in sight
        DetectPlayer(range, playerInfo, groundInfo, hitInfo);

        Attack(playerNear);

        //Debug.Log(playerNear);
        //if (playerInfo.collider)
        //    Debug.Log(playerInfo.collider.gameObject.name);
        // if (hitInfo.collider)
        //    Debug.Log(hitInfo.collider.gameObject.name);
            


    }

    private bool IsPlayerInRange(float range)
    {
        return Vector3.Distance(transform.position, playerTarget.position) <= range;
    }


    public void DetectPlayer(float range, RaycastHit2D playerInfo, RaycastHit2D groundInfo, RaycastHit2D hitInfo){
            if (IsPlayerInRange(range)){
                if (playerInfo.collider){
                    //follows if in range and sees player and there's no obstacle
                    if (playerInfo.collider.CompareTag("PlayerAttack") && groundInfo.collider == true && hitInfo.collider == false){
                        nowSpeed = (moveSpeedEnemy + 1) * Time.deltaTime;
                        transform.position = Vector2.MoveTowards(transform.position, playerTarget.position, nowSpeed);
                        FlipFollow();
                        } //if sees player but can't get access to him (obstacle/dump)
                        else if (playerInfo.collider.CompareTag("PlayerAttack") && groundInfo.collider == false || playerInfo.collider.CompareTag("PlayerAttack") && hitInfo.collider.CompareTag("ground")){
                        nowSpeed = 0 * Time.deltaTime;
                        transform.Translate(Vector2.right * nowSpeed);
                        FlipFollow();
                        }
                }
            }
    }

    private void Patrol(RaycastHit2D hitInfo, RaycastHit2D groundInfo){
            if (groundInfo.collider == true && groundInfo.collider.CompareTag("ground")){
                nowSpeed = moveSpeedEnemy * Time.deltaTime;
                transform.Translate(Vector2.right * nowSpeed);
            }
            else if (groundInfo.collider == false || groundInfo.collider.CompareTag("spikes") || hitInfo.collider == true && hitInfo.collider.CompareTag("ground")){
                Flip();
            }

    }

    private void Flip(){
            if (facingRight == true){
                transform.eulerAngles = new Vector3 (0, -180, 0);
                facingRight = false;
                } else {
                transform.eulerAngles = new Vector3 (0, 0, 0);
                facingRight = true;
            }
            
    }

    private void FlipFollow(){
        if(playerTarget.position.x > transform.position.x){
            transform.eulerAngles = new Vector3 (0, 0, 0);
        } else if (playerTarget.position.x < transform.position.x) {
            transform.eulerAngles = new Vector3 (0, -180, 0);
        }
    }

    void OnTriggerEnter2D(Collider2D collider){
        if (collider.gameObject.CompareTag("PlayerAttack"))
        {
            playerNear = true;
        }
    }

    void OnTriggerExit2D(Collider2D collider){
        if (collider.gameObject.tag != "PlayerAttack")
        {
            playerNear = false;
        }
    }

    void Attack(bool playerNear){
        if (playerNear){
            if(attackTimer > 0f){
                attackTimer -= Time.deltaTime;
                    if(attackTimer <= 0f){
                        player.Damage(10);
                        attackTimer = 0f;
                    }
            }
        }
    }



    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }

}

