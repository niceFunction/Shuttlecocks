using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerController : MonoBehaviour {

	[System.Serializable]
	public class PlayerProperties {
		[Range(0f, 100f)]
		public float walkingSpeed;
		[Range(0f, 100f)]
		public float runningSpeed;
		[Range(0f, 100f)]
		public float acceleration;
		[Range(0f, 100f)]
		public float jumpForce;
	}

	[System.Flags]
	private enum PlayerInputs {
		MOVE_LEFT,
		MOVE_RIGHT,
		JUMP
	}

	private Rigidbody2D body;
	private new CapsuleCollider2D collider;
	public PlayerProperties properties;
	private int inputs;

	void Start() {
		body = GetComponent<Rigidbody2D>();
		collider = GetComponent<CapsuleCollider2D>();
	}

	private void OnCollisionStay2D(Collision2D collision) {
		
	}
	
	void Update() {
	
	}

	private void FixedUpdate() {
		
	}
}
