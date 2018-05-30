using UnityEngine;
using System.Collections;
using Spine.Unity;

public class AI_FSM2 : MonoBehaviour {

	public GameObject hero;
	public GameObject birthPoint;


	enum State {
		State_Idle, //待机
		State_Follow, //跟随
		State_Back, //返回出生点
		State_Attack, //攻击状态
	}

	enum IdleState {
		IdleState_stand, //站立不动
		IdleState_play_sword, //舞剑
		IdleState_play_sheild, //举盾
	}

	State state;
	IdleState idleState;

	string animation_idle1 = "Idle";
	string animation_idle2 = "Idle2";
	string animation_idle3 = "Idle3";
	string animation_attack = "Attack";
	string animation_move = "Move";

	SkeletonAnimation skeletonAnimation;
	string curAnimationName;
	Spine.AnimationState spineAnimationState;

	float curTime;

	// Use this for initialization
	void Start () {
		Debug.Log ("AI 启动");
		state = State.State_Idle;
		idleState = IdleState.IdleState_stand;
		curTime = Time.time;

		skeletonAnimation = GetComponent<SkeletonAnimation>();
		spineAnimationState = skeletonAnimation.state;

		spineAnimationState.SetAnimation (0, animation_idle1, true);
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log ("当前状态为：" + state);

		switch (state) {

		case State.State_Idle:
			float newTime = Time.time;
			Debug.Log ("当前子状态为：" + idleState);

			switch (idleState) {
			case IdleState.IdleState_stand:
				if (newTime - curTime > 4) {
					curTime = newTime;
					idleState = IdleState.IdleState_play_sword;
					spineAnimationState.SetAnimation (0, animation_idle2, true);
				}
				break;
			case IdleState.IdleState_play_sword:
				if (newTime - curTime > 4) {
					curTime = newTime;
					idleState = IdleState.IdleState_play_sheild;
					spineAnimationState.SetAnimation (0, animation_idle3, true);
				}
				break;
			case IdleState.IdleState_play_sheild:
				if (newTime - curTime > 4) {
					curTime = newTime;
					idleState = IdleState.IdleState_stand;
					spineAnimationState.SetAnimation (0, animation_idle1, true);
				}
				break;
			}

			if (getDistanceAbs (hero, this.gameObject) < 5.0f) {
				state = State.State_Follow;

				spineAnimationState.SetAnimation (0, animation_move, true);
			}
			break;

		case State.State_Follow:
			gotoObject (hero);

			if (getDistanceAbs (hero, this.gameObject) > 5.0f) {
				state = State.State_Back;
			}
			if (getDistanceAbs (hero, this.gameObject) < 1.0f) {
				state = State.State_Attack;
				spineAnimationState.SetAnimation (0, animation_attack, true);
			}
			break;

		case State.State_Back:
			gotoObject (birthPoint);

			if (getDistanceAbs (this.gameObject, birthPoint) < 0.1f) {
				state = State.State_Idle;
				spineAnimationState.SetAnimation (0, animation_idle1, true);				
			}
			break;

		case State.State_Attack:
			if (getDistanceAbs (this.gameObject, hero) > 1.5f) {
				state = State.State_Follow;
				spineAnimationState.SetAnimation (0, animation_move, true);
			}
			break;
		}
	}

	void gotoObject(GameObject target)
	{
		float direction = getDistance (this.gameObject, target) > 0 ? 1 : -1;
		this.gameObject.transform.localScale = new Vector3 (-direction,
			this.gameObject.transform.localScale.y, 
			this.gameObject.transform.localScale.z);

		this.gameObject.transform.Translate (new Vector3 (0.02f * -direction, 0, 0));
	}

	float getDistance(GameObject a, GameObject b)
	{
		return a.transform.localPosition.x - b.transform.localPosition.x;
	}

	float getDistanceAbs(GameObject a, GameObject b)
	{
		return Mathf.Abs(a.transform.localPosition.x - b.transform.localPosition.x);
	}
}
