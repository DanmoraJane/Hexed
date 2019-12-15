using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed;
    public float range;
    public float groundRange;

    private float nowSpeed;
    private bool facingRight = true;
    private bool playerNear = false;


    public Transform enemyGroundCheck;     //checks ground for collision
    public Transform playerTarget;         //retrieves player position
    public Animator enemyAnimator;         //retrives enemy Animator

    public Player player;                  //accesses player script

    void Start()
    {
        Physics2D.queriesStartInColliders = false;

        playerTarget = GameObject.FindGameObjectWithTag("player").GetComponent<Transform>();       

    }

    void Update()
    {
        //enemy detection range
        Collider2D visionInfo = Physics2D.OverlapCircle(transform.position, range);

        //ray from enemy to player to see if he sees the player
        Vector3 direction = (playerTarget.position - transform.position).normalized;
        RaycastHit2D playerInfo = Physics2D.Raycast(transform.position, direction, range);
        Debug.DrawRay(transform.position, direction, Color.red);

        //check if there's ground
        RaycastHit2D groundInfo = Physics2D.Raycast(enemyGroundCheck.position, Vector2.down, groundRange);
        //check fi there's an obstacle
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.right, 0.5f);

        //animator variables
        enemyAnimator.SetFloat("Speed", nowSpeed);
        enemyAnimator.SetBool("GroundInfo", groundInfo);
        enemyAnimator.SetBool("PlayerNear", playerNear);

        //behaviour when no player in sight
        Patrol(hitInfo, groundInfo);

        Debug.DrawRay(enemyGroundCheck.position, Vector2.down, Color.blue);
        
        //behaviour with player in sight
        DetectPlayer(visionInfo, playerInfo, groundInfo, hitInfo);

    }

    public void DetectPlayer(Collider2D visionInfo, RaycastHit2D playerInfo, RaycastHit2D groundInfo, RaycastHit2D hitInfo){
        if (visionInfo.CompareTag("player")){
            Debug.Log("In range");
            if (playerInfo.collider){
                if (playerInfo.collider.CompareTag("player") && groundInfo.collider == true && !hitInfo.collider){
                Debug.Log("In range and follows");
                nowSpeed = (moveSpeed + 1) * Time.deltaTime;
                transform.position = Vector2.MoveTowards(transform.position, playerTarget.position, nowSpeed);
                
            } else if (playerInfo.collider.CompareTag("player") && groundInfo.collider == false || playerInfo.collider.CompareTag("player") && hitInfo.collider.CompareTag("ground")){
                Debug.Log("In range and sees");
                nowSpeed = 0 * Time.deltaTime;
                transform.Translate(Vector2.right * nowSpeed);
            }

            if (playerInfo.collider.CompareTag("player") && groundInfo.collider == true || playerInfo.collider.CompareTag("player") && groundInfo.collider == false){
                if(playerTarget.position.x > transform.position.x){
                    transform.eulerAngles = new Vector3 (0, 0, 0);
                } else if (playerTarget.position.x < transform.position.x) {
                    transform.eulerAngles = new Vector3 (0, -180, 0);
                }
            }
            }
            
        }
    }

    private void Patrol(RaycastHit2D hitInfo, RaycastHit2D groundInfo){
        if (hitInfo.collider != null){
            if (groundInfo.collider == true && hitInfo.collider.CompareTag("ground") )
            {
                Flip();
            }
        } else {
            if (groundInfo.collider == false || groundInfo.collider.CompareTag("spikes")) { 
                Flip();
            } else {
                nowSpeed = moveSpeed * Time.deltaTime;
                transform.Translate(Vector2.right * nowSpeed);
            }  
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

    void OnTriggerEnter2D(Collider2D collider){
        if (collider.CompareTag("player")){
            playerNear = true;
        }         
    }

    void OnTriggerExit2D(Collider2D collider){
        if (collider.CompareTag("player"))
        {
            playerNear = false;
        }
            
    }


    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }

}

