using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.UI;

class Player {
    string name;
    Direction direction;
}

[RequireComponent(typeof(TwitchIRC))]
public class TwitchBot : MonoBehaviour {
    private string cmdPattern = @"^\:(?<name1>[\w\d]*)\!(?<name2>[\w\d]*)\@(?<name3>[\w\d\.]*)\ PRIVMSG\ \#(?<chat>[\w\d]*)\ \:(?<cmd>[\w\d]*)$";
    public Text chatActions;
    private TwitchIRC IRC;
    public GameLogic gameLogic;

    public Dictionary<string, Direction> user;
    public Dictionary<Direction, int> count;

    public void ResetRound()
    {
        chatActions.text = "";
        user = new Dictionary<string, Direction>();
        count = new Dictionary<Direction, int>();
        count.Add(Direction.Forward, 0);
        count.Add(Direction.Left, 0);
        count.Add(Direction.Right, 0);
    }

    public void OnChatMsgRecieved(string msg)
    {
        if (gameLogic.pauseTimer)
        {
            return;
        }

        Match match = new Regex(cmdPattern).Match(msg);
        if (!match.Success)
        {
            return;
        }
        string cmd = match.Groups["cmd"].Value;
        string username = match.Groups["name2"].Value;
        Direction dir;

        if (cmd == "left")
        {
            dir = Direction.Left;
        }
        else if (cmd == "right")
        {
            dir = Direction.Right;
        }
        else if (cmd == "forward")
        {
            dir = Direction.Forward;
        }
        else
        {
            return;
        }

        if (user.ContainsKey(username))
        {
            // user already voted this round, reset!
            count[user[username]]--;
            user[username] = dir;
        }
        else
        {
            user.Add(username, dir);
        }

        count[dir]++;
        int max = 0;

        foreach (KeyValuePair<Direction, int> pair in count) {
            if (pair.Value > max)
            {
                gameLogic.direction = pair.Key;
                max = pair.Value;
            }
        }

        chatActions.text = username + " voted " + dir.ToString() + "\n" + chatActions.text;
        gameLogic.UpdateArrowAnimation();
    }

    // Use this for initialization
    void Start () {
        ResetRound();

        if (!gameLogic.offlineMode)
        {
            GetComponent<TwitchIRC>().enabled = true;
            IRC = GetComponent<TwitchIRC>();
            //IRC.SendCommand("CAP REQ :twitch.tv/tags"); //register for additional data such as emote-ids, name color etc.
            IRC.messageRecievedEvent.AddListener(OnChatMsgRecieved);
        }
        else
        {
            GetComponent<TwitchIRC>().enabled = false;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
