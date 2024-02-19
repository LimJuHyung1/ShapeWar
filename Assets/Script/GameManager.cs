using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public Camera camera;
    private PhotonView PV;

    public Image screen;
    public Text countdownText;

    GameObject player;

    private float timer = 10f;

    void Awake()
    {
        // PhotonNetwork.ConnectUsingSettings();       
        Application.targetFrameRate = 90;
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        // 1초마다 UpdateTimer 메소드 호출
        InvokeRepeating("UpdateTimer", 1f, 1f);

        if (PhotonNetwork.IsMasterClient)
            player = PhotonNetwork.Instantiate("Square", Vector2.zero + new Vector2(3, 0), Quaternion.identity);
        else
            player = PhotonNetwork.Instantiate("Square", Vector2.zero - new Vector2(3, 0), Quaternion.identity);

        if (player != null) camera.GetComponent<CameraScript>().FindPlayer(player);
    }

    void Update()
    {

    }



    void UpdateTimer()
    {
        timer -= 1f;
        UpdateCountdownText();

        // 타이머가 0 이하로 떨어지면 타이머 중지
        if (timer <= 0f)
        {
            screen.gameObject.SetActive(false); // 스크린 제거

            CancelInvoke("UpdateTimer");
        }
    }

    void UpdateCountdownText()
    {
        // UI Text 업데이트
        countdownText.text = "시작까지 " + timer.ToString("F0") + "초"; // "F0"는 소수점 이하를 표시하지 않음
    }
}
