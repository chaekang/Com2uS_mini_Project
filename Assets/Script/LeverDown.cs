using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverDown : MonoBehaviour
{
    Animator anim;
    public GameObject rope;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            anim.SetBool("isLever", true);
            rope.SetActive(false);
        }
    }
}
