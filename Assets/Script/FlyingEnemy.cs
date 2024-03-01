using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public Transform startPos;  // ���� ��ġ
    public Transform endPos;    // �� ��ġ
    public Transform desPos;    // �������� Transform
    float speed = 1.5f;         // �ӵ� ����

    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        transform.position = startPos.position;  // ������ startPos
        desPos = endPos;  // �������� endPos
    }

    void FixedUpdate()
    {
        // ���� ����Ʈ�� �̵�, MoveTowards(������ġ, ��ǥ��ġ, �ӵ�)
        transform.position = Vector2.MoveTowards(transform.position, desPos.position, Time.deltaTime * speed);

        // �������� �����ϸ� ������� �������� ����
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
