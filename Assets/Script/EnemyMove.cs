// EnemyMove.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public int nextMove;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capCollider;
    Animator animator;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        Invoke("Think", 3);
    }

    void FixedUpdate()
    {
        //�̵�
        rigid.velocity = new Vector2(nextMove * 2, rigid.velocity.y);

        //�������� ����(�ʷϻ�)
        Vector2 front = new Vector2(rigid.position.x + nextMove * 0.5f, rigid.position.y - 0.5f);
        Debug.DrawRay(front, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(front, Vector3.down, 0.2f, LayerMask.GetMask("Platform"));

        if (rayHit.collider == null)
        {
            if (spriteRenderer.flipY != true) //���Ͱ� �װ� �������� ������� �ʱ� ���� �ڵ�
            {
                Turn();
            }
        }

        // ���濡 ��ֹ� ����(������)
        
        Vector2 forward = new Vector2(rigid.position.x + nextMove, rigid.position.y);
        Debug.DrawRay(forward, Vector2.right * nextMove, Color.red); // ���������� ǥ��
        RaycastHit2D hit = Physics2D.Raycast(forward, Vector2.right * nextMove, 0.2f, LayerMask.GetMask("Platform"));

        if ((hit.collider != null))
        {
            Turn();
        }

        // �и� �� ������������ ������ ����(�����)
        Vector2 fall = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y - 0.5f);
        Debug.DrawRay(fall, Vector3.down, Color.yellow);
        RaycastHit2D fallHit = Physics2D.Raycast(fall, Vector3.down, 0.2f, LayerMask.GetMask("Platform"));

        if (fallHit.collider == null)
        {
            rigid.constraints = RigidbodyConstraints2D.FreezePositionX;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Turn();
        }
    }

    public void Think()
    {
        nextMove = Random.Range(-1, 2); //�̵����� ����, 2�� ���Ե��� ����

        animator.SetInteger("WalkSpeed", nextMove); //�ȴ¼ӵ��� ���� �ȱ� �ִϸ��̼� ��ȯ 

        if (nextMove != 0) //�ٶ󺸴� ���� ��ȯ
        {
            spriteRenderer.flipX = nextMove == 1;
        }

        Invoke("Think", 3);
    }


    void Turn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;
        CancelInvoke();
        Invoke("Think", 2);
    }

    public void OnDamaged()
    {
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //flipY
        spriteRenderer.flipY = true;
        //Color Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        //polygonCollider Enabled
        capCollider.enabled = false;
        //DeActive Delay
        Invoke("DeActive", 3);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }

}