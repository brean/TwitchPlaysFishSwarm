using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class Biotope
{
    public Biotope(GameObject phValueGO)
    {
        this.phValueGO = phValueGO;
    }

    public GameObject phValueGO;
    float phValue = 0.0f;
    // boost phValue (e.g. fish farm)
    float phValueBoost = 0.0f;

    public float PhValue
    {
        get
        {
            return phValue;
        }
    }

    public void calculateNewPhValue()
    {
        phValue = Random.Range(-2.0f, 2.0f);
        phValue = (PhValue < 0) ? PhValue - phValueBoost : PhValue + phValueBoost;
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
        foreach (KeyValuePair<Direction, Biotope> bio in biotopes)
        {
            bio.Value.calculateNewPhValue();
        }
    }

    void TimeIsUp()
    {
        pauseTimer = true;
        arrowLeft.SetActive(false);
        arrowForward.SetActive(false);
        arrowRight.SetActive(false);

        Animation ani = camRails.GetComponent<Animation>();
        ani.Play("animate_"+direction.ToString().ToLower());

    }

    public void MoveAnimationDone()
    {
        // update based on current state - kill/spawn fish based on ph-level
        float newFish = -Mathf.Sin(biotopes[direction].PhValue * Mathf.PI)*10;
        Debug.Log(newFish);
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

    void UpdateArrowAnimation()
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

	// Update is called once per frame
	void Update () {
        if (pauseTimer)
        {
            return;
        }

        if (offlineMode)
        {
            if (Input.GetAxis("Horizontal") > 0)
            {
                direction = Direction.Right;
                UpdateArrowAnimation();
            }
            else if (Input.GetAxis("Horizontal") < 0)
            {
                direction = Direction.Left;
                UpdateArrowAnimation();
            }
            else if (Input.GetAxis("Vertical") > 0)
            {
                direction = Direction.Forward;
                UpdateArrowAnimation();
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