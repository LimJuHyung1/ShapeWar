using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject joystick;
    [SerializeField] GameObject canvasObject;
    [SerializeField] Transform joystickTransform;
    [SerializeField] GameObject squareBullet;

    [SerializeField] protected bool isAttacking = false;
    [SerializeField] public int hp;
    [SerializeField] public int hpMax;
    [SerializeField] public int atkDamage;
    [SerializeField] public float atkDelay;
    float moveSpeed = 5f;

    protected SpriteRenderer spriteRenderer;

    public bool isDiscovering = false;
    protected Vector2 moveVec;    // rigid.MovePosition�� ���� �����̵��� ����

    protected float x;
    protected float y;

    protected float fadeDuration = 1.0f; // ���̵� ���� �ð� ����    
    public PhotonView PV;

    void Start()
    {
        isAttacking = false;

        // "Canvas"��� �̸��� ���� �θ� ������Ʈ�� ã��
        canvasObject = GameObject.Find("Canvas");

        if (canvasObject != null)
        {
            // �θ� ������Ʈ ������ "joystick"�̶�� �̸��� ���� �ڽ� ������Ʈ�� ã��
            joystickTransform = canvasObject.transform.Find("Joystick");

            if (joystickTransform != null)
            {
                // ã�� �ڽ� ������Ʈ�� ���� �۾� ����
                joystick = joystickTransform.gameObject;
                Debug.Log("Joystick found!");
            }
            else
            {
                Debug.LogError("Joystick not found!");
            }
        }
        else
        {
            Debug.LogError("Canvas not found!");
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        squareBullet = Resources.Load<GameObject>("SquareBullet");
    }

    void Update()
    {
        if (isDiscovering)
        {
            ShootBullet();
        }
    }


    void FixedUpdate()
    {
        if (PV.IsMine)
        {
            if (joystick != null)
            {
                x = joystick.GetComponent<VariableJoystick>().Horizontal;
                y = joystick.GetComponent<VariableJoystick>().Vertical;

                // Move
                moveVec = new Vector2(x, y) * moveSpeed * Time.deltaTime;

                // �̵� ���� ����
                Vector3 moveDirection = new Vector3(x, y, 0f).normalized;

                // �÷��̾ �̵� �������� �̵�
                transform.position += moveDirection * moveSpeed * Time.deltaTime;

                if (moveVec.sqrMagnitude == 0)
                    return; // no input = no rotation

                Quaternion dirQuat = Quaternion.LookRotation(moveVec);  // ȸ���Ϸ��� ����

                float angle = Mathf.Atan2(moveVec.y, moveVec.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.3f);
            }

        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("OtherPlayer"))
        {
            isDiscovering = true;
            //collision.GetComponent<Player>().FadeIn();

            //PV.RPC("ShootBullet", RpcTarget.All);
        }
    }
    /*
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("OtherPlayer"))
        {            
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= this.atkDelay)
            {
                ShootBullet();
                elapsedTime = 0;
            }
        }
    }
    */
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("OtherPlayer"))
        {
            isDiscovering = false;
            //collision.GetComponent<Player>().FadeOut();
        }
    }

    void InitStatus()   // ���� �ʱ�ȭ
    {
        this.hp = 10;
        this.hpMax = 10;
        this.atkDamage = 3;
        this.atkDelay = 2;
    }

    //    [PunRPC]
    public void Damaged(int yourAtkDamage)  // ���� ����
    {
        this.hp -= yourAtkDamage;
    }

    void InvokeAtkDelay()
    {
        isAttacking = false;
    }

    void ShootBullet()  // źȯ �߻�
    {
        if (squareBullet != null)
        {
            GameObject bullet =
                PhotonNetwork.Instantiate
                ("SquareBullet", this.transform.position, Quaternion.identity);

            bullet.GetComponent<PhotonView>().RPC("ShootRPC", RpcTarget.All, this.transform.right, PV.ViewID);
        }
    }

    IEnumerator FadeIn()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            Color spriteColor = spriteRenderer.color;
            spriteColor.a = alpha;
            spriteRenderer.color = spriteColor;
            timer += Time.deltaTime;
            yield return null;
        }

        Color finalColor = spriteRenderer.color;
        finalColor.a = 1f; // ������ ���̵� �εǾ��� �� ���� �� 1�� ����
        spriteRenderer.color = finalColor;
    }

    // ���̵� �ƿ� �Լ�
    IEnumerator FadeOut()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            Color spriteColor = spriteRenderer.color;
            spriteColor.a = alpha;
            spriteRenderer.color = spriteColor;
            timer += Time.deltaTime;
            yield return null;
        }

        Color finalColor = spriteRenderer.color;
        finalColor.a = 0f; // ������ ���̵� �ƿ��Ǿ��� �� ���� �� 0���� ����
        spriteRenderer.color = finalColor;
    }    
}
