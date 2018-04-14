﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	enum PlayerState {
		STAND,
		WALK,
		JUMP,
		STAND_HOLDING,
		WALK_HOLDING,
		JUMP_HOLDING,
		GRAB}

	;

	[Range (1, 2)]
	[SerializeField]private int playerNumber = 1;
	[SerializeField]private float walkSpeed, jumpSpeed, minJumpForce, maxFallSpeed, gravityForce;

	private Vector2 velocity;
	private Vector3 playerScale;
	private bool onGround, pushingWallLeft, pushingWallRight, againstCeiling, isWalkingLeft, touchingCeiling;
	public bool isHolding;
	private PlayerState currentState;
	private Animator anim;

	// Use this for initializationF
	void Start () {
		velocity = Vector2.zero;
		currentState = PlayerState.STAND;
		onGround = false;
		pushingWallLeft = false;
		pushingWallRight = false;
		againstCeiling = false;
		isWalkingLeft = false;
		touchingCeiling = false;
		isHolding = false;
		anim = gameObject.GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float horizontalDir = Input.GetAxis ("Horizontal" + playerNumber);
		VelocityUpdate (horizontalDir);
		setFacing (horizontalDir);
		transform.Translate (velocity * Time.deltaTime);
		Debug.Log (onGround);
	}

	void VelocityUpdate (float horizontalDir) {
		string animState;
		switch (currentState) {
		case PlayerState.STAND:
			velocity = Vector2.zero;
			animState = isHolding ? "Idle_Holding" : "Idle";
			anim.Play (animState);
			if (!onGround) {
				currentState = PlayerState.JUMP;
				break;
			}
			if (horizontalDir != 0) {
				currentState = PlayerState.WALK;
				break;
			} else if (Input.GetButton ("Jump" + playerNumber)) {
				velocity.y = jumpSpeed;
				currentState = PlayerState.JUMP;
				break;
			}
			break;

		case PlayerState.WALK:
			animState = isHolding ? "Walk_Holding" : "Walk";
			anim.Play (animState);
			if (horizontalDir == 0) {
				currentState = PlayerState.STAND;
				velocity = Vector2.zero;
				break;
			} else {
				velocity.x = SetVelocityX (horizontalDir);
			}

			if (Input.GetButton ("Jump" + playerNumber)) {
				velocity.y = jumpSpeed;
				//TODO Add audio(?)
				currentState = PlayerState.JUMP;
				break;
			} else if (!onGround) {
				currentState = PlayerState.JUMP;
				break;
			}
			break;
		case PlayerState.JUMP:
			//TODO Insert animations
			velocity.y -= gravityForce * Time.deltaTime;
			velocity.y = Mathf.Max (velocity.y, -maxFallSpeed);
			if (horizontalDir == 0) {
				velocity.x = 0;
			} else {
				velocity.x = SetVelocityX (horizontalDir);
			}
			if (!Input.GetButton ("Jump" + playerNumber) && velocity.y > 0.0f)
				velocity.y = Mathf.Min (velocity.y, minJumpForce);
			if (onGround) {
				if (horizontalDir == 0) {
					currentState = PlayerState.STAND;
					velocity = Vector2.zero;
				} else {
					currentState = PlayerState.WALK;
					velocity.y = 0;
				}
				//TODO Insert audio(?)
				break;
			}
			break;
		}
	}

	void setFacing (float horizontalDir) {
		Vector3 vScale = Vector3.one;
		vScale.x = isWalkingLeft ? -1 : 1;
		transform.localScale = vScale;
	}

	float SetVelocityX (float horizontalDir) {
		float targetSpeed = horizontalDir * walkSpeed;
		if (horizontalDir < 0f) {
			isWalkingLeft = true;
			return pushingWallLeft ? 0f : targetSpeed;
		} else if (horizontalDir > 0) {
			isWalkingLeft = false;
			return pushingWallRight ? 0f : targetSpeed;
		} else {
			return 0f;
		}
	}

	void OnTriggerEnter2D (Collider2D col) {
		if (col.gameObject.CompareTag ("Floor")) {
			onGround = true;
		}
	}

	void OnTriggerExit2D (Collider2D col) {
		if (col.gameObject.CompareTag ("Floor")) {
			onGround = false;
		}
	}

	void OnCollisionEnter2D (Collision2D col) {
		foreach (ContactPoint2D contact in col.contacts) {
			if (contact.point.x > transform.position.x) {
				pushingWallRight = true;
			} else if (contact.point.x < transform.position.x) {
				pushingWallLeft = true;
			}
		}

	}

	void OnCollisionExit2D (Collision2D col) {
		if (pushingWallRight) {
			pushingWallRight = false;
		}
		if (pushingWallLeft) {
			pushingWallLeft = false;
		}
	}

	public bool IsWalkingLeft () {
		return isWalkingLeft;
	}

	public void SetIsHolding (bool value) {
		isHolding = value;
	}
}
