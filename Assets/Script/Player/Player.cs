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
    protected Vector2 moveVec;    // rigid.MovePosition을 통해 움직이도록 설정

    protected float x;
    protected float y;

    protected float fadeDuration = 1.0f; // 페이드 지속 시간 설정    
    public PhotonView PV;

    void Start()
    {
        isAttacking = false;

        // "Canvas"라는 이름을 가진 부모 오브젝트를 찾음
        canvasObject = GameObject.Find("Canvas");

        if (canvasObject != null)
        {
            // 부모 오브젝트 내에서 "joystick"이라는 이름을 가진 자식 오브젝트를 찾음
            joystickTransform = canvasObject.transform.Find("Joystick");

            if (joystickTransform != null)
            {
                // 찾은 자식 오브젝트에 대한 작업 수행
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

                // 이동 방향 설정
                Vector3 moveDirection = new Vector3(x, y, 0f).normalized;

                // 플레이어를 이동 방향으로 이동
                transform.position += moveDirection * moveSpeed * Time.deltaTime;

                if (moveVec.sqrMagnitude == 0)
                    return; // no input = no rotation

                Quaternion dirQuat = Quaternion.LookRotation(moveVec);  // 회전하려는 방향

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

    void InitStatus()   // 스탯 초기화
    {
        this.hp = 10;
        this.hpMax = 10;
        this.atkDamage = 3;
        this.atkDelay = 2;
    }

    //    [PunRPC]
    public void Damaged(int yourAtkDamage)  // 피해 받음
    {
        this.hp -= yourAtkDamage;
    }

    void InvokeAtkDelay()
    {
        isAttacking = false;
    }

    void ShootBullet()  // 탄환 발사
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
        finalColor.a = 1f; // 완전히 페이드 인되었을 때 알파 값 1로 설정
        spriteRenderer.color = finalColor;
    }

    // 페이드 아웃 함수
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
        finalColor.a = 0f; // 완전히 페이드 아웃되었을 때 알파 값 0으로 설정
        spriteRenderer.color = finalColor;
    }    
}
