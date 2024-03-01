using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    Animator anim;
    public PlayerMove playerMove;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            
            anim.SetBool("onTram", true);

            JumpOtherObject(collision.gameObject);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            anim.SetBool("onTram", false);
        }
    }

    void JumpOtherObject(GameObject otherObject)
    {
        float jumpPower = playerMove.jumpPower * 1.5f;
        Rigidbody2D rigid = otherObject.GetComponent<Rigidbody2D>();
        Animator animator = otherObject.GetComponent<Animator>();
        if (otherObject != null)
        {
            animator.SetBool("isJumping", true);
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
    }
}
