using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    private Vector2 targetPos;
    private Transform player;
    private Rigidbody2D rigidbody;

    public float smoothing = 3;
    public int lostFood = 10;

    private BoxCollider2D collider;
    private Animator animator;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rigidbody = GetComponent<Rigidbody2D>();
        targetPos = transform.position;
        collider = GetComponent<BoxCollider2D>();
        GameManager.Instance.enemyList.Add(this);
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        rigidbody.MovePosition(Vector2.Lerp(transform.position, targetPos, smoothing * Time.deltaTime));

    }

    public void Move()
    {
        Vector2 offset = player.position-transform.position;
        if (offset.magnitude < 1.1f) //攻击
        {
            animator.SetTrigger("Attack");
            player.SendMessage("TakeDamage",lostFood);
        }
        else  //移动
        {
            float x = 0, y = 0;
            if (Mathf.Abs(offset.y) > Mathf.Abs(offset.x)) //按y轴移动
            {
                if (offset.y < 0)
                {
                    y = -1;
                }
                else
                {
                    y = 1;
                }
            }
            else //按x轴移动
            {
                if (offset.x < 0)
                {
                    x = -1;
                }
                else
                {
                    x = 1;
                }
            }

            collider.enabled = false;
            RaycastHit2D hit = Physics2D.Linecast(targetPos, targetPos + new Vector2(x, y));
            collider.enabled = true;
            if (hit.transform == null)
            {
                targetPos += new Vector2(x, y);
            }
            else
            {
                if (hit.collider.tag == "Food" || hit.collider.tag == "Soda")
                {
                    targetPos += new Vector2(x, y);
                }
            }         
        }
    }
	
}
