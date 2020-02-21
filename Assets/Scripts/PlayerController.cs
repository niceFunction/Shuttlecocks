﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {

	[System.Serializable]
	public class PlayerProperties {
		[Range(0f, 1f)]
		public float groundAcceleration;
		[Range(0f, 1f)]
		public float airAcceleration;
		[Range(0f, 10f)]
		public float walkingSpeed;
		[Range(0f, 10f)]
		public float jumpForce;
		[Min(0)]
		public float coyoteTime;
		[Min(0)]
		public int numberOfJumps;
	}

	private CharacterController controller;
	private Vector3 motion;

	private int jumpsLeft;
	private float timeAtFall;
	private bool canFall;

	public PlayerProperties properties;

	void Start() {
		controller = GetComponent<CharacterController>();
		motion = Vector3.zero;
		jumpsLeft = 0;
		timeAtFall = Time.realtimeSinceStartup;
		// Prevent OutOfCoyote from reducing jumpsLeft more than once per fall.
		canFall = false;
	}

	private bool OutOfCoyote(float time) {
		return !(Time.realtimeSinceStartup < time + properties.coyoteTime);
	}

	private void Update() {
		float a = properties.groundAcceleration;
		if(controller.isGrounded) {
			canFall = true;
			jumpsLeft = properties.numberOfJumps;
			timeAtFall = Time.realtimeSinceStartup;
		} else if(OutOfCoyote(timeAtFall) && canFall) {
			if(jumpsLeft > 0) { --jumpsLeft; }
			canFall = false;
		} else {
			a = properties.airAcceleration;
		}

		float targetSpeed = Input.GetAxis("Horizontal") * properties.walkingSpeed;
		motion.x = a * targetSpeed + (1f - a) * controller.velocity.x;
		motion.y = controller.velocity.y + (Physics.gravity.y * Time.deltaTime);

		if(Input.GetButtonDown("Jump") && jumpsLeft > 0) {
			float impulse = properties.jumpForce * (jumpsLeft / (float)properties.numberOfJumps);
			motion.y =
				motion.y > impulse ? motion.y :
				motion.y < 0 ? impulse :
				motion.y + impulse
			;
			canFall = false;
			--jumpsLeft;
		}

		controller.Move(motion * Time.deltaTime);
	}
}
