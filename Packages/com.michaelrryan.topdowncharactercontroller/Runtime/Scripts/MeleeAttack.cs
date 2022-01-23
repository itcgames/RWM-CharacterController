using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopdownCharacterController
{
	public class MeleeAttack : MonoBehaviour
	{
		// ==== Properties ====

		public bool HandleAnimationEvents = true;
		public float AttackDamage = 1.0f;
		public float AttackRadius = 0.5f;

		[SerializeField]
		private float _attackCooldown = 0.5f;
		public float AttackCooldown
		{
			get { return _attackCooldown; }
			set { SetAttackCooldown(value); }
		}

		public float ThornsDamage = 0.0f;
		public bool FreezeOnAttack = false;

		// ==== Private Variables ====

		private const string MELEE_SPEED_MULTIPLIER = "MeleeSpeedMultiplier";
		private const string MELEE_ATTACK = "MeleeAttack";

		private Rigidbody2D _rb;
		private Animator _animator;

		private float _lastAttackTime = 0.0f;

		// ==== Public Custom Methods ====

		public void Attack(Vector2 direction)
		{
			// Checks the attack cooldown has expired before attacking.
			if (CanAttack())
			{
				_lastAttackTime = Time.time;

				// Finds all colliders within a radius in front of the character.
				Vector2 attackPosition = (Vector2)transform.position + direction * AttackRadius;
				Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPosition, AttackRadius);

				// Check through colliders and damages any character's with a health component.
				foreach (Collider2D collider in colliders)
				{
					Health characterHealth = collider.GetComponent<Health>();
					if (characterHealth) characterHealth.TakeDamage(AttackDamage, tag);
				}

				if (FreezeOnAttack && _rb)
					_rb.velocity = Vector2.zero;

				// If handling animation events, trigger the melee attack animation.
				if (_animator && HandleAnimationEvents)
					_animator.SetTrigger(MELEE_ATTACK);
			}
		}

		public bool CanAttack()
		{
			return Time.time >= _lastAttackTime + _attackCooldown;
		}

		// ==== Unity Method Overrides ====

		private void OnValidate()
		{
			SetAttackCooldown(_attackCooldown);
		}

		void Start()
		{
			_rb = GetComponent<Rigidbody2D>();
			_animator = GetComponent<Animator>();

			SetAttackCooldown(_attackCooldown);
		}

		private void OnCollisionStay2D(Collision2D collision)
		{
			if (ThornsDamage != 0.0f)
			{
				Health characterHealth =
					collision.collider.GetComponent<Health>();

				// If a character health component was retrieved, damages it.
				if (characterHealth)
					characterHealth.TakeDamage(ThornsDamage, tag);
			}
		}

		// ==== Private Custom Methods ====

		private void SetAttackCooldown(float value)
		{
			_attackCooldown = value;
			_lastAttackTime = Time.time - _attackCooldown * 2.0f;

			// If handling animation events, set the melee animation speed multiplier.
			if (_animator && HandleAnimationEvents)
				_animator.SetFloat(MELEE_SPEED_MULTIPLIER, 1.0f / _attackCooldown);
		}
	}
}