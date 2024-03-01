using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public Transform startPos;  // 시작 위치
    public Transform endPos;    // 끝 위치
    public Transform desPos;    // 도착지의 Transform
    float speed = 1.5f;         // 속도 조절

    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        transform.position = startPos.position;  // 발판은 startPos
        desPos = endPos;  // 목적지는 endPos
    }

    void FixedUpdate()
    {
        // 일정 포인트로 이동, MoveTowards(현재위치, 목표위치, 속도)
        transform.position = Vector2.MoveTowards(transform.position, desPos.position, Time.deltaTime * speed);

        // 목적지에 도착하면 출발지를 도착지로 변경
        if (Vector2.Distance(transform.position, desPos.position) <= 0.05f)
        {
            if (desPos == endPos)
            {
                desPos = startPos;
                spriteRenderer.flipX = false;
            }
            else
            {
                desPos = endPos;
                spriteRenderer.flipX = true;
            }
        }
    }
}
