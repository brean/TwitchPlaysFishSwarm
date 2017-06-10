using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TwitchIRC))]
public class TwitchBot : MonoBehaviour {
    private TwitchIRC IRC;

    void OnChatMsgRecieved(string msg)
    {
        Debug.Log(msg);
    }

    // Use this for initialization
    void Start () {
        IRC = this.GetComponent<TwitchIRC>();
        //IRC.SendCommand("CAP REQ :twitch.tv/tags"); //register for additional data such as emote-ids, name color etc.
        IRC.messageRecievedEvent.AddListener(OnChatMsgRecieved);
        Debug.Log("Login Done!");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
