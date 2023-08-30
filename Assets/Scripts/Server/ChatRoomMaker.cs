using OpenAI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatRoomMaker : MonoBehaviour
{
    public static string RoomName;
    public int roomCount = 8;
    public LobbyScreenUI lobbyScreen;

    private TMP_Dropdown dropdown;

    private void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();

        int half = roomCount / 2;
        for (int i = 0; i < roomCount; i++)
        {
            var dt = System.DateTime.Now.ToString("HHmm");
            int roomNumber;
            if (int.TryParse(dt, out roomNumber))
            {
                var channelName = "Room" + (roomNumber - half + i);
                dropdown.options.Add(new TMP_Dropdown.OptionData(channelName));
            }
        }
        dropdown.onValueChanged.AddListener(a => { Debug.Log(dropdown.options[a].text); lobbyScreen.LobbyChannelName = dropdown.options[a].text; });
    }
}
