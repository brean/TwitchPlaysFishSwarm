using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class Biotope
{
    // TODO: refactor - merge with Biotope instances/BiotopeSettings
    public Biotope(GameObject phValueGO)
    {
        this.phValueGO = phValueGO;
    }

    public GameObject biotopeGO;
    public GameObject phValueGO;
    float phValue = 0.0f;

    public float PhValue
    {
        get
        {
            return phValue;
        }
    }

    public void calculateNewPhValue()
    {
        BiotopeSettings settings = biotopeGO.GetComponent<BiotopeSettings>();

        phValue = Random.Range(-2.0f, 2.0f);
        phValue = (PhValue < 0) ? PhValue - settings.phBoost : PhValue + settings.phBoost;
        phValue = Mathf.Min(Mathf.Max(PhValue, -2.5f), 2.5f);
        
        foreach (Transform child in phValueGO.transform)
        {
            if (child.gameObject.name == "Slider")
            {
                Vector3 pos = child.gameObject.transform.localPosition;
                pos.x = PhValue;
                child.gameObject.transform.localPosition = pos;
                break;
            }
        }
        
    }
}

public class GameLogic : MonoBehaviour {

    public Direction direction = Direction.Forward;

    public BiotopeManager biotopeManager;

    [Tooltip("Reset timer every X seconds (default: 30)")]
    public float resetAfter = 120;

    [Tooltip("Text that will be set from current time (e.g. 1:23 if you have 83 seconds left)")]
    public Text text;

    [Tooltip("Offline Mode - Use Keyboard input instead of twitch")]
    public bool offlineMode = false;

    private int lastTime;

    private float lastReset;

    public bool pauseTimer = false;

    public GameObject arrowLeft;
    public GameObject arrowForward;
    public GameObject arrowRight;

    public GameObject phValueLeft;
    public GameObject phValueForward;
    public GameObject phValueRight;

    public GameObject camRails;

    public SwarmLogic swarmLogic;

    public TwitchBot bot;

    Dictionary<Direction, Biotope> biotopes;

    // Use this for initialization
    void Start () {
        lastReset = Time.realtimeSinceStartup + resetAfter;
        arrowLeft.SetActive(false);
        arrowRight.SetActive(false);

        biotopes = new Dictionary<Direction, Biotope>();
        biotopes.Add(Direction.Left, new Biotope(phValueLeft));
        biotopes.Add(Direction.Right, new Biotope(phValueRight));
        biotopes.Add(Direction.Forward, new Biotope(phValueForward));

        ResetGL();
    }

    private void ResetGL()
    {
        List<GameObject> biotopes = new List<GameObject>(biotopeManager.biotopes);
        
        foreach (GameObject bioGO in biotopeManager.biotopes)
        {
            if (bioGO != null)
            {
                bioGO.SetActive(false);
            }
        }

        GameObject biotopeGO = getNextBiotopeGO(biotopes, new Vector3(-6, 0, 4), 0);
        this.biotopes[Direction.Left].biotopeGO = biotopeGO;

        biotopeGO = getNextBiotopeGO(biotopes, new Vector3(0, 0, 8), 60);
        this.biotopes[Direction.Forward].biotopeGO = biotopeGO;

        biotopeGO = getNextBiotopeGO(biotopes, new Vector3(6, 0, 4), 120);
        this.biotopes[Direction.Right].biotopeGO = biotopeGO;

        // show biotopes for the new starting round
        foreach (Biotope bio in this.biotopes.Values)
        {
            if (bio.biotopeGO != null)
            {
                bio.biotopeGO.SetActive(true);
            }
        }

        // create new biotope ph-values

        foreach (Biotope bio in this.biotopes.Values)
        {
            bio.calculateNewPhValue();
        }
    }

    GameObject getNextBiotopeGO(List<GameObject> biotopes, Vector3 pos, float angle)
    {
        GameObject biotopeGO = biotopeManager.getRandomBiotope(biotopes);
        biotopeGO.transform.parent = gameObject.transform.parent;
        biotopeManager.RotateBiotope(biotopeGO, angle);
        biotopeGO.transform.position = pos;
        return biotopeGO;
    }

    void TimeIsUp()
    {
        //TODO: show and fade out last

        pauseTimer = true;
        arrowLeft.SetActive(false);
        arrowForward.SetActive(false);
        arrowRight.SetActive(false);

        Animation ani = camRails.GetComponent<Animation>();
        ani.Play("animate_"+direction.ToString().ToLower());

    }

    public void MoveAnimationDone()
    {
        bot.ResetRound();
        // update based on current state - kill/spawn fish based on ph-level
        Debug.Log(biotopes[direction].PhValue);
        float newFish = Mathf.Cos((biotopes[direction].PhValue / 2.5f) * Mathf.PI);

        BiotopeSettings settings = biotopes[direction].biotopeGO.GetComponent<BiotopeSettings>();

        newFish -= settings.killFish;

        newFish = Mathf.Max(newFish, -0.9f);
        newFish = Mathf.Min(newFish, 0.9f);
        
        
        newFish *= swarmLogic.allFish.Count;
        newFish = Mathf.Min(newFish, 100f);
        
        // Debug.Log((biotopes[direction].PhValue) + " --> " + (biotopes[direction].PhValue / 2.5f) + " --> " + (int)newFish);
        if (newFish > 0)
        {
            for (int i = 0; i < newFish; i++)
            {
                swarmLogic.addFish();
            }
        } else
        {
            for (int i = 0; i < -newFish; i++)
            {
                swarmLogic.removeFish();
            }
        }

        // reset animation for next choice
        Animation ani = camRails.GetComponent<Animation>();
        ani.Rewind();
        ani.Stop();

        camRails.transform.position = Vector3.zero;
        UpdateArrowAnimation();
        pauseTimer = false;
        lastReset = Time.realtimeSinceStartup + resetAfter;

        ResetGL();
    }

    void RestartArrowAnimation(GameObject arrow)
    {
        arrow.SetActive(true);
        Animation ani = arrow.GetComponent<Animation>();
        ani.Rewind();
        ani.Play();
    }

    public void UpdateArrowAnimation()
    {
        arrowLeft.SetActive(false);
        arrowForward.SetActive(false);
        arrowRight.SetActive(false);
        switch (direction)
        {
            case Direction.Left:
                RestartArrowAnimation(arrowLeft);
                break;
            case Direction.Forward:
                RestartArrowAnimation(arrowForward);
                break;
            case Direction.Right:
                RestartArrowAnimation(arrowRight);
                break;
        }
    }

    void updateText()
    {
        if (text == null)
        {
            return;
        }

        if (lastTime < 0)
        {
            text.text = "0:00";
            return;
        }
        else 
        {
            int secs = (lastTime % 60);
            string seconds = (secs < 10) ? "0" + secs : secs.ToString();
            text.text = (lastTime / 60) + ":" + seconds;
        }
    }

    int userid = 0;

	// Update is called once per frame
	void Update () {
        if (pauseTimer)
        {
            return;
        }

        
        if (offlineMode)
        {
            userid++;
            string option = ":user"+userid+ "!user" + userid + "@user" + userid + ".tmi.twitch.tv PRIVMSG #breandev :";
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                option += Direction.Right.ToString().ToLower();
                bot.OnChatMsgRecieved(option);
            }
            else if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                option += Direction.Left.ToString().ToLower();
                bot.OnChatMsgRecieved(option);
            }
            else if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                option += Direction.Forward.ToString().ToLower();
                bot.OnChatMsgRecieved(option);
            }
        }

        float countDown = lastReset - Time.realtimeSinceStartup;
        if ((int)countDown != lastTime)
        {
            lastTime = (int)countDown;
            updateText();
        }
        if (countDown <= 0)
        {
            TimeIsUp();
        }
    }
}

public enum Direction
{
    Left,
    Forward,
    Right
}