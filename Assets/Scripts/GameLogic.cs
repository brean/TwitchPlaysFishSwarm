using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour {
    NextDirection direction = NextDirection.Forward;

    [Tooltip("reset timer every X seconds (default: 30)")]
    public float resetAfter = 120;

    [Tooltip("text that will be set from current time (e.g. 1:23 if you have 83 seconds left)")]
    public Text text;

    private int lastTime;

    private float lastReset;

    public bool pauseTimer = false;

    public GameObject arrowLeft;
    public GameObject arrowForward;
    public GameObject arrowRight;

    public GameObject camRails;

    // Use this for initialization
    void Start () {
        lastReset = Time.realtimeSinceStartup + resetAfter;
        arrowLeft.SetActive(false);
        arrowRight.SetActive(false);
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
        Animation ani = camRails.GetComponent<Animation>();
        ani.Rewind();
        ani.Stop();
        
        camRails.transform.position = Vector3.zero;
        UpdateArrowAnimation();
        pauseTimer = false;
        lastReset = Time.realtimeSinceStartup + resetAfter;
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
            case NextDirection.Left:
                RestartArrowAnimation(arrowLeft);
                break;
            case NextDirection.Forward:
                RestartArrowAnimation(arrowForward);
                break;
            case NextDirection.Right:
                RestartArrowAnimation(arrowRight);
                break;
        }
    }

    void updateText()
    {
        if (text != null)
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

        if (Input.GetAxis("Horizontal") > 0)
        {
            direction = NextDirection.Right;
            UpdateArrowAnimation();
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            direction = NextDirection.Left;
            UpdateArrowAnimation();
        }
        else if (Input.GetAxis("Vertical") > 0)
        {
            direction = NextDirection.Forward;
            UpdateArrowAnimation();
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

enum NextDirection
{
    Left,
    Forward,
    Right
}