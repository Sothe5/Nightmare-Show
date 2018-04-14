﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileThrower : MonoBehaviour {

	public GameObject projectile;
	[SerializeField] public GameObject projectileHolder;
	[SerializeField] private GameObject scope;
	[SerializeField] private float angleSpeed;
	[SerializeField] private float force;
	[Range (1, 2)]
	public int playerNumber;
	private PlayerMovement playerMovement;
	private Rigidbody2D projectileRigidBody;
	private bool isGoingUp = true;
	public bool isGrabbing = false;
	public bool hasReleaseProjectileCatcherButton = false;

	void Start () {
		playerMovement = transform.parent.gameObject.GetComponent<PlayerMovement> ();
		scope.SetActive (false);
	}

	void Update () { //TODO Assign self player fire button
		if (projectile || isGrabbing) {
			if (hasReleaseProjectileCatcherButton) {
				if (Input.GetButton ("Fire" + playerNumber)) {
					AimShot ();
				} else if (Input.GetButtonUp ("Fire" + playerNumber)) {
					scope.SetActive (false);
					playerMovement.SetCurrentState (PlayerMovement.PlayerState.THROW);
				}
			} else if (Input.GetButtonUp ("Fire" + playerNumber)) {
				hasReleaseProjectileCatcherButton = true;
			}
		}
	}

	void AimShot () {
		scope.SetActive (true);
		if (isGoingUp) {
			if (scope.transform.localRotation.eulerAngles.z < 90 || scope.transform.localRotation.eulerAngles.z > 355) {
				scope.transform.localRotation = Quaternion.Euler (0, 0, scope.transform.localRotation.eulerAngles.z + angleSpeed);
			} else {
				isGoingUp = false;
			}
		} else {
			if (scope.transform.localRotation.eulerAngles.z < 93) {
				scope.transform.localRotation = Quaternion.Euler (0, 0, scope.transform.localRotation.eulerAngles.z - angleSpeed);
			} else {
				isGoingUp = true;
			}
		}
	}

	public void setProjectile (GameObject proj) {
		if (!projectile) {
			projectile = proj;
			projectileRigidBody = projectile.GetComponent<Rigidbody2D> ();
			projectile.transform.SetParent (projectileHolder.transform);
			projectile.transform.localPosition = Vector3.zero;
			projectile.transform.localScale = Vector3.one;
			projectileRigidBody.velocity = Vector2.zero;
			projectileRigidBody.isKinematic = true;
			PhobiaAI phobiaAI = projectile.GetComponent<PhobiaAI> ();
			phobiaAI.canMove = false;
			phobiaAI.PlayWalkSound ();
			playerMovement.isHolding = true;
		}
	}

	public void ThrowProjectile () {
		int direction = playerMovement.IsWalkingLeft () ? -1 : 1;
		float ang = Mathf.Deg2Rad * scope.transform.rotation.eulerAngles.z;
		PhobiaAI phobiaAI = projectile.GetComponent<PhobiaAI> ();
		phobiaAI.canMove = true;
		phobiaAI.PlayThrowSound ();
		projectileRigidBody.isKinematic = false;
		projectileRigidBody.AddForce (new Vector2 (direction * force * Mathf.Cos (ang), direction * force * Mathf.Sin (ang)));
		projectile.transform.SetParent (null);
		hasReleaseProjectileCatcherButton = false;
		playerMovement.isHolding = false;

		projectile = null;
	}

	public void DropProjectile () {
		if (projectile) {
			projectileRigidBody.isKinematic = false;
			projectile.transform.SetParent (null);
			projectile = null;
		}
	}
}
