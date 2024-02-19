using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CameraScript : MonoBehaviour
{
    [SerializeField] GameObject player; // ���� ��� ������Ʈ�� Transform
    Camera cam;
    Vector3 dis;

    int otherPlayerLayer = 8;   // Enemy ���̾�

    void Start()
    {
        cam = GetComponent<Camera>();

        dis = new Vector3(0, 0, -15);
    }

    void Update()
    {
        if (player != null)    // �ڽ��� ĳ���Ͱ� ������ ���
        {
            // ��� �÷��̾ �߰�
            if (player.GetComponent<Player>().isDiscovering)
            {
                ActiveOtherPlayerLayer();
            }

            // ��� �÷��̾� �þ߿��� ���
            if (!player.GetComponent<Player>().isDiscovering)
            {
                UnactiveOtherPlayerLayer();
            }

            // ī�޶� ��ġ ����
            this.transform.position = player.transform.position + dis;

            // ī�޶� �׻� ��� ������Ʈ�� �ٶ󺸰Բ� ȸ�� ����
            transform.LookAt(player.transform);
        }
    }
    
    void ActiveOtherPlayerLayer()   // ��� �÷��̾� ���̰� �ϱ�
    {
        // ���� culling mask ��������
        int currentCullingMask = cam.cullingMask;

        // ���̾ Ȱ��ȭ�ϱ� ���� �ش� ���̾��� ��Ʈ�� OR �������� �߰�
        currentCullingMask |= 1 << otherPlayerLayer;

        // ������Ʈ�� culling mask ����
        cam.cullingMask = currentCullingMask;
    }

    void UnactiveOtherPlayerLayer() // ��� �÷��̾� ������ �ʰ� �ϱ�
    {
        // ���� culling mask ��������
        int currentCullingMask = cam.cullingMask;

        // ���� ���̾�� OtherPlayer ���̾� ����
        currentCullingMask &= ~(1 << otherPlayerLayer);

        cam.cullingMask = currentCullingMask;
    }

    public void FindPlayer(GameObject refPlayer)
    {
        /*
        if (PhotonNetwork.IsMasterClient)
            player = GameObject.Find("Square(Clone)");
        else
            player = GameObject.Find("Circle(Clone)");
        */

        // player = GameObject.Find("Circle");                
        // player = GameObject.Find(refPlayer);
        // player = GameObject.Find("Triangle");

        player = refPlayer;
    }
}