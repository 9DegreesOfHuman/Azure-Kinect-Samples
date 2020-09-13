﻿using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;

// public class PhotonConnect : MonoBehaviour
public class PhotonConnect : MonoBehaviourPunCallbacks
{   // e9fe0af3-b001-4ef6-82d3-c102fafd5a63
    RoomOptions roomOptions;
    private byte maxPlayersPerRoom = 4;
    public static PhotonConnect Lobby;

    [SerializeField]
    private string defaultRoomName = "defaultExerciseRoom";
    private int userIdCount;
    private int roomNumber = 1;
    private const byte COLOR_CHANGE_EVENT = 0;
    private const byte BODY_TRACKING_EVENT = 1;
    private bool connectionAttempted = false;

    public void onClick_test()
    {
        _print(true, "onclick test");
        SendMessage();
    }

    private void Awake()
    {
        _print(true, "Awake");
        roomOptions = new RoomOptions { IsVisible = true, IsOpen = true, MaxPlayers = maxPlayersPerRoom };
        if (Lobby == null)
        {
            Lobby = this;
        }

        if (Lobby != this)
        {
            Destroy(Lobby.gameObject);
            Lobby = this;
        }

        DontDestroyOnLoad(gameObject);
        _print(true, "Awoke");
    }

    void Start()
    {
        _print(true, "Start start");
        PhotonNetwork.ConnectUsingSettings();
        _print(true, "Start started");
    }

    void Update()
    {
        if (!connectionAttempted && SceneManager.GetActiveScene().name == "SampleScene")
        {
            _print(true, "SampleScene active");
            connectionAttempted = true;
            PhotonNetwork.ConnectUsingSettings();
        }
        else if (!connectionAttempted)
        {
            _print(true, "!connectionAttempted && SampleScene inactive: " + SceneManager.GetActiveScene().name);
        }
    }

    public override void OnConnectedToMaster()
    {
        _print(true, "OnConnectedToMaster begin");
        var randomUserId = UnityEngine.Random.Range(0, 999999);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.AuthValues = new AuthenticationValues();
        PhotonNetwork.AuthValues.UserId = randomUserId.ToString();
        userIdCount++;
        PhotonNetwork.NickName = PhotonNetwork.AuthValues.UserId;
        _print(true, "OnConnectedToMaster finish, joining random");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        _print(true, "\nPhotonLobby.OnJoinRandomFailed()");
        JoinOrCreateRoom_defaultRoomName();
    }

    public void JoinOrCreateRoom_defaultRoomName()
    {
        _print(true, "JoinOrCreateRoom_" + defaultRoomName);
        PhotonNetwork.JoinOrCreateRoom(defaultRoomName, roomOptions, TypedLobby.Default);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        _print(true, "\nPhotonLobby.OnCreateRoomFailed()");
        JoinOrCreateRoom_defaultRoomName();
    }

    public override void OnCreatedRoom()
    {
        _print(true, "\nPhotonLobby.OnCreatedRoom()");
        base.OnCreatedRoom();
        roomNumber++;
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        _print(true, "\nPhotonLobby.OnJoinedRoom(): " + PhotonNetwork.CurrentRoom.Name);

        if (PhotonNetwork.CurrentRoom.Name != defaultRoomName)
        {
            _print(true, "PhotonNetwork.CurrentRoom.Name != defaultRoomName");
            _print(true, PhotonNetwork.CurrentRoom.Name + " != " + defaultRoomName);
            JoinOrCreateRoom_defaultRoomName();
        }

        networkEventsEnable();
        _print(true, "Other/Total players in room: " + PhotonNetwork.CountOfPlayersInRooms + " / " + (PhotonNetwork.CountOfPlayersInRooms + 1));
    }

#region Networking -------------------------------------------------------------------------------
    public override void OnDisconnected(DisconnectCause cause)
    {
        _print(true, $"PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {cause}");
        networkEventsDisable();
    }

    private void networkEventsEnable()
    {
        _print(true, "adding event callback");
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    }

    private void networkEventsDisable()
    {
        _print(true, "removing event callback");
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
    }

    public void SendMessage()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.All,
        };
        object[] datas = new object[] { "r, g, b" }; // base.photonView.ViewID,
        PhotonNetwork.RaiseEvent(COLOR_CHANGE_EVENT, datas, raiseEventOptions, SendOptions.SendUnreliable);
    }

    public void SendBodyTrackingEventData(string data)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.All,
        };        
        object[] datas = new object[]{data as object};
        PhotonNetwork.RaiseEvent(BODY_TRACKING_EVENT,
                                 datas,
                                 raiseEventOptions,
                                 SendOptions.SendUnreliable);
    }

    private void NetworkingClient_EventReceived(EventData obj)
    {
        if (obj == null || obj.Code == null)
        {
            _print(true, "invalid EventData obj recieved");
            return;
        }

        object[] datas = (object[])obj.CustomData;
        switch (obj.Code)
        {
            case COLOR_CHANGE_EVENT:
                _print(true, "received COLOR_CHANGE_EVENT");
                break;
            case BODY_TRACKING_EVENT:
                _print(true, "received BODY_TRACKING_EVENT");

                string coordinateString = (string)datas[0];
                _print(true, coordinateString);
                break;
            default:
                _print(true, "default unhandled obj.Code: " + obj.Code);
                break;
        }
    }
    #endregion

    private void _print(bool shouldPrint, string msg)
    {
        if (shouldPrint) Debug.Log(msg);
    }
}
