using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
public class ProfileData
{
    public string username;
    public ProfileData()
    {
        this.username = "DEFAULT USERNAME";
    }

    public ProfileData(string u)
    {
        this.username = u;
    }

   
    
}

[System.Serializable]

public class MapData
{
    public string name;
    public int scene;

}

public class Launcher : MonoBehaviourPunCallbacks
{
    public InputField usernameField;
    public InputField roomnameField;
    public Text mapValue;
    public Slider maxPLayersSlider;
    public Text maxPlayersValue;
    public static ProfileData myProfile = new ProfileData();

    public GameObject tabMain;
    public GameObject tabRooms;
    public GameObject tabCreate;

    public GameObject buttonRoom;

    public MapData[] maps;
    private int currentmap = 0;

    private List<RoomInfo> roomList;

    private void Awake()
    {
        
        PhotonNetwork.AutomaticallySyncScene = true;
        Connect();
    }

    public override void OnConnectedToMaster()
    {
        print("Connected");
        PhotonNetwork.JoinLobby();
        base.OnConnectedToMaster();
    }

    public void Connect()
    {
        Debug.Log("Trying to connect....");
        PhotonNetwork.GameVersion = "0.0.0";
        PhotonNetwork.ConnectUsingSettings();

    }

    public override void OnJoinedRoom()
    {
        StartGame();
        base.OnJoinedRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Create();
        base.OnJoinRandomFailed(returnCode, message);
    }

    public void Join()
    { 
        PhotonNetwork.JoinRandomRoom();  
    }

    public void Create()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte) maxPLayersSlider.value;

        options.CustomRoomPropertiesForLobby = new string[] { "map"};

        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
        properties.Add("map", currentmap);
        options.CustomRoomProperties = properties;
        PhotonNetwork.CreateRoom(roomnameField.text,options);
    }

    public void ChangeMap()
    {
        currentmap++;
        if (currentmap >= maps.Length) currentmap = 0;
        mapValue.text = "MAP " + maps[currentmap].name.ToUpper();
    }

    public void ChangeMaxPlayerSlider(float t_value)
    {
        maxPlayersValue.text = Mathf.RoundToInt(t_value).ToString();
    }

    public void TabCloseAll()
    {
        tabMain.SetActive(false);
        tabRooms.SetActive(false);
        tabCreate.SetActive(false);
    }

    public void TabOpenMain()
    {
        TabCloseAll();
        tabMain.SetActive(true);
    }


    public void TabOpenRooms()
    {
        TabCloseAll();
        tabRooms.SetActive(true);
    }

    public void TabOpenCreate()
    {
        TabCloseAll();
        tabCreate.SetActive(true);

        roomnameField.text = "";

        currentmap = 0;
        mapValue.text = "Map : " + maps[currentmap].name.ToUpper();

        maxPLayersSlider.value = maxPLayersSlider.maxValue;
        maxPlayersValue.text = Mathf.RoundToInt(maxPLayersSlider.value).ToString();
    }

    private void ClearRoomList()
    {
        Transform content = tabRooms.transform.Find("Scroll View/Viewport/Content");
        foreach (Transform a in content) Destroy(a.gameObject);
    }

    private void VerifyUsername ()
    {
        if(string.IsNullOrEmpty(usernameField.text))
        {
            myProfile.username = "RANDOM_USER_" + Random.Range(100, 1000);
        }
        else
        {
            myProfile.username = usernameField.text;
        }
    }


    public override void OnRoomListUpdate(List<RoomInfo> p_list)
    {
        roomList = p_list;
        ClearRoomList();

        
        Transform content = tabRooms.transform.Find("Scroll View/Viewport/Content");

        foreach(RoomInfo a in roomList)
        {
            GameObject newRoomButton = Instantiate(buttonRoom, content) as GameObject;

            newRoomButton.transform.Find("Name").GetComponent<Text>().text = a.Name;
            newRoomButton.transform.Find("Players").GetComponent<Text>().text = a.PlayerCount + " / " + a.MaxPlayers;

            if (a.CustomProperties.ContainsKey("map"))
                newRoomButton.transform.Find("Map/Name").GetComponent<Text>().text = maps[(int)a.CustomProperties["map"]].name;
            else
                newRoomButton.transform.Find("Map/Name").GetComponent<Text>().text = " ------ ";

            newRoomButton.GetComponent<Button>().onClick.AddListener(delegate { JoinRoom(newRoomButton.transform); });
        }
        base.OnRoomListUpdate(roomList);
    }

    public void JoinRoom(Transform p_button)
    {
       
        string t_roomName = p_button.Find("Name").GetComponent<Text>().text;
        VerifyUsername();
        PhotonNetwork.JoinRoom(t_roomName);
        
    }
    public void StartGame()
    {
        VerifyUsername();

        if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.LoadLevel(maps[currentmap].scene);
        }
    }


}
