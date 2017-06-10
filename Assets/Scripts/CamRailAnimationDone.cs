using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRailAnimationDone : MonoBehaviour {
public GameLogic logic;

	public void AnimationDone () {
        logic.MoveAnimationDone();
	}
}
