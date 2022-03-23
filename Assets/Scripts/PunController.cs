using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

public class PunController : MonoBehaviourPunCallbacks
{
    public InputField roomName;
    private bool isConnected;
    private bool joined;
    public string PlayerPrefabName;
    public CinemachineFreeLook thirdPersonCam;

    public void ConnectedToMaster()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = "Alpha";
    }

    public void CreateRoom()
    {
        if (!isConnected || joined || roomName.text == "") return;

        PhotonNetwork.CreateRoom(roomName.text,new RoomOptions(){ MaxPlayers = 16},TypedLobby.Default);
    }


    public void JoinRoom()
    {
        if (!isConnected || joined) return;

        PhotonNetwork.JoinRoom(roomName.text);
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        isConnected = true;
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("房间 \""+roomName.text + "\" 创建成功");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        joined = true;
        Debug.Log("已成功加入房间");
        GameObject player = PhotonNetwork.Instantiate(PlayerPrefabName,Vector3.zero,Quaternion.identity);
        thirdPersonCam.Follow = player.transform;
        thirdPersonCam.LookAt = player.transform;
        gameObject.SetActive(false);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log("房间名为：\"" + roomName.text + "\" 不存在");
        roomName.text = "";
        roomName.placeholder.GetComponent<Text>().text = "请输入正确的房间名";
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("房间名：\"" + roomName.text + "\" 不合格，请重新输入");
        roomName.text = "";
    }
}
