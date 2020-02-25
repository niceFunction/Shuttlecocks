using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {

	[System.Serializable]
	public class PlayerProperties {
		[Tooltip(
			"Horizontal acceleration and decceleration used when the player character is on the ground.\n" +
			"Measured in units/seconds²"
		)]
		[Range(0f, 100f)]
		public float groundAcceleration;
		[Tooltip(
			"Horizontal acceleration and decceleration used when the player character is int the air.\n" +
			"Measured in units/seconds²"
		)]
		[Range(0f, 100f)]
		public float airAcceleration;
		[Tooltip(
			"Maximum walking speed, in units per second. Actual walking speed will be scaled linearly by\n" +
			"controller input."
		)]
		[Range(0f, 10f)]
		public float walkingSpeed;
		[Tooltip(
			"This impulse is used to propell the player character into the air.\n" +
			"If the maximum number of jumps is higher than one, the actual velocity used is scaled by the\n" +
			"current velocity, allowing for a sweet spot where players can jump slightly higher if they time\n" +
			"their jumps perfectly."
		)]
		[Range(0f, 10f)]
		public float jumpImpulse;
		[Tooltip(
			"Time measured in seconds after the player runs off an edge, before which they can still jump\n" +
			"as if they were tanding on the ground."
		)]
		[Min(0)]
		public float coyoteTime;
		[Tooltip(
			"Maximum number of times the player can jump before having to land.\n" +
			"Actual jumping velocity is a fraction of the number of jumps, i.e. The second jump of two\n" +
			"would be half as powerful as the first. The third jump of three would be a third as powerful\n" +
			"as the first, etc."
		)]
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

	private void OnControllerColliderHit(ControllerColliderHit hit) {
		if(Mathf.Abs(controller.velocity.x) < Time.deltaTime && controller.velocity.y <= 0f) {
			var vectorDistanceToGround = Vector3.Distance(hit.normal, Vector3.up);
			if(vectorDistanceToGround > 0.05 && vectorDistanceToGround < 1.4) {
				transform.Translate(hit.normal.x * Time.deltaTime, 0f, 0f);
			}
		}
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
		motion.x = Mathf.MoveTowards(controller.velocity.x, targetSpeed, a * Time.deltaTime);
		motion.y = controller.velocity.y + (Physics.gravity.y * Time.deltaTime);

		if(Input.GetButtonDown("Jump") && jumpsLeft > 0) {
			float impulse = properties.jumpImpulse * (jumpsLeft / (float)properties.numberOfJumps);
			motion.y =
				motion.y > impulse ? motion.y :
				motion.y < 0 ? impulse :
				motion.y + impulse
			;
			canFall = false;
			--jumpsLeft;
		}

		controller.Move(motion * Time.deltaTime);
		/*
		Collider2D[] hits = Physics2D.OverlapCapsuleAll(transform.position, new Vector2(controller.radius, controller.height), CapsuleDirection2D.Vertical, 0);
		foreach(Collider2D hit in hits) {
			ColliderDistance2D cd = hit.Distance(controller.collider);

			if(cd.isOverlapped) {
				transform.Translate(cd.pointA - cd.pointB);
			}
		}
		*/
		// InitialVelocity = sqrt(2 * JumpHeight * Gravity)
	}
}
