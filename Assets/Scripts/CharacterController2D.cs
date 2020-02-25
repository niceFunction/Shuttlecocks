using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using GameConstants;

namespace Shuttlecocks {
	[RequireComponent(typeof(BoxCollider2D))]
	public class CharacterController2D : MonoBehaviour {
		public enum CharacterAnimationState {
			IDLE,
			WALKING,
			RUNNING,
			JUMPING,
			FALLING,
			LANDING
		}

		private int _jumpsLeft;
		private float _timeAtFall;
		private float _input;
		// Prevents OutOfCoyote from reducing jumpsLeft more than once per fall
		private bool _canCoyote;
		private Vector2 _motion;
		private int _collisionLayerMask;

		[Serializable]
		public class CharacterProperties {
			[Tooltip(Strings.GROUND_ACCEL)]
			[Min(0f)] public float groundAcceleration;
			[Tooltip(Strings.GROUND_DECEL)]
			[Min(0f)] public float groundDeceleration;
			[Tooltip(Strings.WALK_SPEED)]
			[Min(0f)] public float walkingSpeed;
			[Tooltip(Strings.RUN_SPEED)]
			[Min(0f)] public float runningSpeed;
			[Tooltip(Strings.RUN_THRESHOLD)]
			[Range(0f, 1f)] public float runThreshold;
			[Tooltip(Strings.AIR_ACCEL)]
			[Min(0f)] public float airAcceleration;
			[Tooltip(Strings.AIR_DECEL)]
			[Min(0f)] public float airDeceleration;
			[Tooltip(Strings.AIR_SPEED)]
			[Min(0f)] public float airSpeed;
			[Tooltip(Strings.JUMP_HEIGHT)]
			[Min(0f)] public float jumpHeight;
			[Tooltip(Strings.NUMBER_OF_JUMPS)]
			[Min(0)] public int numberOfJumps;
			[Tooltip(Strings.JUMP_CONTROL)]
			[Range(0f, 1f)] public float jumpControl;
			[Tooltip(Strings.COYOTE_TIME)]
			[Min(0f)] public float coyoteTime;
			[Tooltip(Strings.GRAVITY)]
			[Min(-1f)] public float gravity = -1f;
			[Tooltip(Strings.TERMINAL_VELOCITY)]
			[Min(0f)] public float terminalVelocity;
		}

		public CharacterProperties properties;

		public new BoxCollider2D collider { get; private set; }
		public Vector2 velocity { get; private set; }
		public bool isGrounded { get; private set; }
		public CharacterAnimationState animationState { get; private set; }
		public float gravity {
			get { return properties.gravity < 0f ? Mathf.Abs(Physics2D.gravity.y) : properties.gravity; }
		}

		private void Awake() {
			_motion = Vector2.zero;
			_jumpsLeft = 0;
			_timeAtFall = Time.realtimeSinceStartup;
			_input = 0f;
			_canCoyote = false;
			_collisionLayerMask = Physics2D.GetLayerCollisionMask(gameObject.layer);

			collider = GetComponent<BoxCollider2D>();
			velocity = Vector2.zero;
			isGrounded = false;
			animationState = CharacterAnimationState.IDLE;
		}

		private void ResolveCollisions(Vector2 position) {
			Collider2D[] hits =
				Physics2D.OverlapBoxAll(
				position,
				collider.size,
				0f,
				_collisionLayerMask
			);
			isGrounded = false;
			foreach(Collider2D hit in hits) {
				// no self collision possible
				if(hit == collider) { continue; }
				ColliderDistance2D colliderDistance = hit.Distance(collider);
				if(colliderDistance.isOverlapped) {
					// inelastic collision
					var separation = colliderDistance.pointA - colliderDistance.pointB;

					transform.Translate(separation);

					float angle = Vector2.SignedAngle(colliderDistance.normal, Vector2.up);
					// touching a left wall, going left
					// or a right wall, going right
					if(
						(Mathf.Approximately(angle, 90f) && _motion.x < 0f) ||
						(Mathf.Approximately(angle, -90f) && _motion.x > 0f)
					) {
						_motion.x = 0f;
					// touching a ceiling, going up
					} else if(Mathf.Approximately(angle, 180f) && _motion.y > 0f) {
						_motion.y = 0f;
					// touching a ground, going down
					} else if(Mathf.Approximately(angle, 0f) && _motion.y < 0f) {
						_motion.y = 0f;
						isGrounded = true;
					}
				}
			}
		}

		private bool OutOfCoyote(float time) {
 			return !(Time.realtimeSinceStartup < time + properties.coyoteTime);
		}

		private void UpdateAnimationState() {
			// Implement
		}

		private void Update() {
			// First up, get the input from the stick
			_input = Input.GetAxis("Horizontal");

			float targetSpeed;
			float accel;
			// set target speeds and acceleration or deceleration based on speed and grounded state
			if(isGrounded) {
				targetSpeed =
					Mathf.Abs(_input) < properties.runThreshold ?
					_input * properties.walkingSpeed :
					_input * properties.runningSpeed
				;
				accel =
					Mathf.Abs(targetSpeed) < Mathf.Abs(velocity.x) ?
					properties.groundDeceleration :
					properties.groundAcceleration
				;
				// Reset jumps and coyote time
				_jumpsLeft = properties.numberOfJumps;
				_timeAtFall = Time.realtimeSinceStartup;
				_canCoyote = true;
			} else {
				targetSpeed = _input * properties.airSpeed;
				accel =
					Mathf.Abs(targetSpeed) < Mathf.Abs(velocity.x) ?
					properties.airDeceleration:
					properties.airAcceleration
				;
				// Reduce jumpsLeft, if out of coyote time
				if(OutOfCoyote(_timeAtFall) && _canCoyote) {
					if(_jumpsLeft > 0) { --_jumpsLeft; }
					_canCoyote = false;
				}
			}
			// Calculate new motion
			_motion.x = Mathf.MoveTowards(velocity.x, targetSpeed, accel * Time.deltaTime);
			_motion.y = Mathf.MoveTowards(velocity.y, -properties.terminalVelocity, gravity * Time.deltaTime);
			// Jump!
			if(Input.GetButtonDown("Jump") && _jumpsLeft > 0) {
				float jumpImpulse = Mathf.Sqrt(2f * properties.jumpHeight * gravity);
				// Implement jump height scaling
				_motion.y = jumpImpulse;
				--_jumpsLeft;
				// Prevent OutOfCoyote from reducing jumpsLeft if we have already jumped
				_canCoyote = false;
			// Control jump height by button release
			} else if(Input.GetButtonUp("Jump")) {
				if(_motion.y > 0f) { _motion.y *= (1f - properties.jumpControl); }
			}

			transform.Translate(_motion * Time.deltaTime);
			ResolveCollisions((Vector2)transform.position + collider.offset);

			velocity = _motion;

			UpdateAnimationState();
		}
	}
}
