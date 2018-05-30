using UnityEngine;
using System.Collections;
using Spine.Unity;

public class AI_FSM : MonoBehaviour {
	
	public GameObject hero;
	public GameObject patrolA;
	public GameObject patrolB;
	public GameObject birthPoint;

	public float range = 10;
	public float attackRange = 1;

	enum State {
		State_idle = 0,
		State_follow = 1,
		State_attack = 2,
		State_back = 4,
		State_patrol = 5,
	};

	enum IdleState {
		IdleState_idle1 = 0,
		IdleState_idle2 = 1,
		IdleState_idle3 = 2,
	};

	string idleAnimationName = "Idle";
	string idleAnimationName2 = "Idle2";
	string idleAnimationName3 = "Idle3";
	string moveAnimation = "Move";
	string attackAnimationName = "Attack";

	float speed = 2.0f;

	State state;
	IdleState idleState;

	SkeletonAnimation skeletonAnimation;
	string curAnimationName;

	Spine.AnimationState spineAnimationState;

	float cutTime;

	// Use this for initialization
	void Start () {
		state = State.State_idle;
		idleState = IdleState.IdleState_idle1;
		cutTime = Time.time;
		skeletonAnimation = GetComponent<SkeletonAnimation>();

		curAnimationName = "";
		spineAnimationState = skeletonAnimation.state;
		playAnimation (idleAnimationName);
	}

	// Update is called once per frame
	void Update () {
		switch (state) {
		case State.State_idle:
			float newTime = Time.time;
			switch (idleState) {
			case IdleState.IdleState_idle1:
				playAnimation (idleAnimationName);
				if (Time.time - cutTime > 4) {
					idleState = IdleState.IdleState_idle2;
					cutTime = newTime;
				}
				break;
			case IdleState.IdleState_idle2:
				playAnimation (idleAnimationName2);
				if (Time.time - cutTime > 4) {
					idleState = IdleState.IdleState_idle3;
					cutTime = newTime;
				}
				break;
			case IdleState.IdleState_idle3:
				playAnimation (idleAnimationName3);
				if (Time.time - cutTime > 4) {
					idleState = IdleState.IdleState_idle1;
					cutTime = newTime;
				}
				break;
			}
			if (getDistance (this.gameObject, hero) < range) {
				setState (State.State_follow);
			}
			break;
		case State.State_follow:
			playAnimation (moveAnimation);
			moveTo (hero);
			if (getDistance (this.gameObject, hero) > range) {
				setState (State.State_back);
			}
			if (getDistance (this.gameObject, hero) < attackRange) {
				setState (State.State_attack);
			}
			break;
		case State.State_attack:
			playAnimation (attackAnimationName);
			if (getDistance (this.gameObject, hero) > attackRange) {
				setState (State.State_follow);
			}
			break;
		case State.State_back:
			playAnimation (moveAnimation);
			moveTo (birthPoint);
			if (getDistance (this.gameObject, birthPoint) < 0.2) {
				setState (State.State_idle);
			}
			break;
		case State.State_patrol:
			break;
		}
	}

	//计算2个对象之间距离
	float getDistance(GameObject a, GameObject b)
	{
		return Mathf.Abs(b.transform.localPosition.x - a.transform.localPosition.x);
	}

	//移动至对象B
	void moveTo(GameObject b)
	{
		float direction = b.transform.localPosition.x - gameObject.transform.localPosition.x;
		transform.localScale = new Vector3(direction > 0 ? 1.0f : -1.0f, transform.localScale.y, transform.localScale.z);
		transform.Translate (speed * (direction > 0 ? 1.0f : -1.0f) / 60.0f, 0, 0);
	}

	//播放动画
	void playAnimation(string animation)
	{
		if (curAnimationName != animation) {
			Debug.Log ("player animation" + animation);
			spineAnimationState.SetAnimation (0, animation, true);
			curAnimationName = animation;
		}
	}

	void setState(State state)
	{
		this.state = state;
		Debug.Log ("new state" + state);
	}
}
