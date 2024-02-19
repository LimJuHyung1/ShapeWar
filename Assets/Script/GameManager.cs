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
        // 1�ʸ��� UpdateTimer �޼ҵ� ȣ��
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

        // Ÿ�̸Ӱ� 0 ���Ϸ� �������� Ÿ�̸� ����
        if (timer <= 0f)
        {
            screen.gameObject.SetActive(false); // ��ũ�� ����

            CancelInvoke("UpdateTimer");
        }
    }

    void UpdateCountdownText()
    {
        // UI Text ������Ʈ
        countdownText.text = "���۱��� " + timer.ToString("F0") + "��"; // "F0"�� �Ҽ��� ���ϸ� ǥ������ ����
    }
}
