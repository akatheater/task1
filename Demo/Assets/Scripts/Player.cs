using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float smoothing = 1;
    public float restTime = 1;
    public AudioClip chop1Audio;
    public AudioClip chop2Audio;

    private float restTimer = 0;
    public Vector2 targetPos = new Vector2(1, 1);
    
    private Rigidbody2D rigidbody;
    private BoxCollider2D collider;
    private Animator animator;

	// Use this for initialization
	void Start () {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

        rigidbody.MovePosition(Vector2.Lerp(transform.position, targetPos, smoothing * Time.deltaTime));
        if (GameManager.Instance.food <= 0||GameManager.Instance.isEnd==true) return;

        restTimer += Time.deltaTime;
        if (restTimer < restTime) return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        if (h > 0)
        {
            v = 0;
        }

        if (h != 0 || v != 0)
        {
            GameManager.Instance.ReduceFood(1);

            collider.enabled = false;
            RaycastHit2D hit= Physics2D.Linecast(targetPos,targetPos+new Vector2(h,v));
            collider.enabled=true;
            if(hit.transform==null)
            {                 
                targetPos += new Vector2(h, v);    
            }
            else{
                switch(hit.collider.tag){
                    case "OutWall":
                        break;

                    case  "Wall":
                        animator.SetTrigger("Attack");
                        AudioManager.Instance.RandomPlay(chop1Audio, chop2Audio);
                        hit.collider.SendMessage("TakeDamage");
                        break;
                    case "Food":
                        GameManager.Instance.AddFood(10);
                        targetPos += new Vector2(h, v);
                        Destroy(hit.transform.gameObject);
                        break;
                    case"Soda":
                        GameManager.Instance.AddFood(20);
                        targetPos += new Vector2(h, v);
                        Destroy(hit.transform.gameObject);
                        break;
                    case "Enemy":
                        break;
                }
            }

            GameManager.Instance.OnPlayerMove();
            restTimer = 0;         
        }
       

	}
    public void TakeDamage(int lostFood)
    {
        GameManager.Instance.ReduceFood(lostFood);
        animator.SetTrigger("Damage");
    }
}
