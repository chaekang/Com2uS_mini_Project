using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;
    public float maxSpeed;
    public float jumpPower;
    bool gotKey = false;
    public float maxFallSpeed = 0f;
    bool passTunnel = false;  // 터널
    public bool tram = false;  // 트램벌린
    public bool isclear;

    Rigidbody2D rigid;
    Animator animator;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsuleCollider;

    private void Awake()
    {
        isclear = false;
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    private void FixedUpdate()
    {
        gameManager.SetText();

        // 움직임 구현
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if (rigid.velocity.x > maxSpeed) //오른쪽 최대속도 설정
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);

        else if (rigid.velocity.x < maxSpeed * (-1)) //왼쪽 최대속도 설정
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

        //점프 착지 확인용 레이캐스트 구현
        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayhit = Physics2D.Raycast(rigid.position, Vector3.down, 5, LayerMask.GetMask("Platform"));
            if (rayhit.collider != null)
            {
                if (rayhit.distance < 1)
                    animator.SetBool("isJumping", false);
            }
        }
    }

    private void Update()
    {
        // 점프
        if (Input.GetKeyDown(KeyCode.UpArrow) && !animator.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            animator.SetBool("isJumping", true);
        }

        //걷는 애니메이션 작동
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
        {
            animator.SetBool("isWalking", false);
        }
        else
        {
            animator.SetBool("isWalking", true);
        }

        //미끄러짐 방지
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.7f, rigid.velocity.y);
        }

        // 방향 전환
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            spriteRenderer.flipX = false;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            spriteRenderer.flipX = true;
        }

        // 열쇠블럭 활성화
        if (gotKey)
        {
            GameObject KeyBlock = GameObject.FindGameObjectWithTag("KeyBlock");

            if (KeyBlock != null)
            {
                Collider2D collider = KeyBlock.GetComponent<BoxCollider2D>();

                if (collider != null)
                {
                    collider.enabled = true;
                }
            }
        }

        //낙하데미지 구현
        float currentFallSpeed = rigid.velocity.y;

        // 현재 속도가 최고 속도보다 높다면 갱신
        if (!tram)
        {
            if (currentFallSpeed < maxFallSpeed)
            {
                maxFallSpeed = currentFallSpeed;
            }
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 트램벌린
        if (collision.transform.tag == "Tram")
        {
            tram = true;
            maxFallSpeed = 0;
        }
        if (collision.transform.tag == "Platform")
        {
            tram = false;
        }

        // 열쇠 획득
        if (collision.gameObject.tag == "Key")
        {
            collision.gameObject.SetActive(false);
            gotKey = true;
        }

        // 몬스터
        if (collision.gameObject.tag == "Enemy")
        {
            //Attack
            if ((collision.transform.position.y < transform.position.y))
            {
                //Enemy Die
                OnAttack(collision.transform);
            }
            else
            {
                //몬스터의 머리를 밟은 게 아니면 데미지 받음
                gameManager.EnemyHealthDown();
                OnDamaged(collision.transform.position);
            }
        }

        // 폭탄 블럭
        if (collision.gameObject.tag == "Bomb")
        {
            StartCoroutine(DeactivateBombAfterDelay(collision.gameObject));
        }

        // 낙하데미지
        if (passTunnel)
        {
            passTunnel = false;
            maxFallSpeed = 0f;
            return;
        }

        if (maxFallSpeed < -12f && collision.gameObject.tag == "Platform")
        {
            gameManager.DropHealthDown();
            OnDamaged(rigid.transform.position);
            Debug.Log(maxFallSpeed);
            maxFallSpeed = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tunnel"))
        {
            passTunnel = true;
        }

        if (collision.transform.tag == "Coin")
        {
            gameManager.stagePoint += 50;

            collision.gameObject.SetActive(false);
        }

        if (collision.transform.tag == "Finish")
        {
            isclear = true;
        }
    }

    void OnDamaged(Vector2 targetPos)
    {
        // 체력 관리
        gameManager.HeartControl();

        //무적상태 레이어로 변경
        gameObject.layer = 8;

        //무적상태 변환시 투명도 조절
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // 피격 애니메이션
        animator.SetTrigger("DoDamaged");

        //튕겨나가는 방향 조절
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);

        //무적상태는 3초만 유지
        Invoke("OffDamaged", 3);
    }

    void OffDamaged() //무적상태 해제
    {
        gameObject.layer = 7;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    void OnAttack(Transform enemy)
    {
        //EnemyAttack
        gameManager.stagePoint += 100;
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
        rigid.AddForce(Vector2.up * 3, ForceMode2D.Impulse);
        //audioManager.PlaySound("Attack");
    }

    public void OnDie()
    {
        //Heart Control
        for (int i = 0; i < gameManager.HP; i++)
        {
            gameManager.Hearts[i].gameObject.SetActive(false);
        }
        gameManager.HP = 0;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //flipY
        spriteRenderer.flipY = true;
        //Color Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        //capsuleCollider Enabled
        capsuleCollider.enabled = false;
        
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }

    IEnumerator DeactivateBombAfterDelay(GameObject bombObject)
    {
        // 일정 시간(1초) 대기 후에 폭탄 비활성화
        yield return new WaitForSeconds(0.5f);

        // 비활성화 코드
        bombObject.SetActive(false);
    }
}
