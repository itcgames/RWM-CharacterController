using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopdownCharacterController
{
	public class RangedAttack : MonoBehaviour
	{
		// ==== Properties ====

		public bool HandleAnimationEvents = true;
		public GameObject projectilePrefab;

		[SerializeField]
		private float _cooldown = 0.5f;
		public float Cooldown
		{
			get { return _cooldown; }
			set { SetCooldown(value); }
		}

		public bool LimitedAmmo = false;
		public int Ammo = 0;

		// ==== Private Variables ====

		private const string RANGED_SPEED_MULTIPLIER = "RangedSpeedMultiplier";
		private const string RANGED_ATTACK = "RangedAttack";

		private Animator _animator;
		private Collider2D _collider;

		private float _lastAttackTime;

		// ==== Public Custom Methods ====

		public GameObject Fire(Vector2 direction)
		{
			// Ensures the cooldown has expired before firing.
			if (Time.time >= _lastAttackTime + _cooldown
				&& (!LimitedAmmo || Ammo > 0))
			{
				// Updates the last attack time.
				_lastAttackTime = Time.time;

				// Decreases ammo if limited ammo is enabled.
				if (LimitedAmmo) Ammo--;

				// Gets the direction as a quaternion.
				Quaternion rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f,
					Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));

				// Instantiates the projectile.
				GameObject projectileObj =
					Instantiate(projectilePrefab, transform.position, rotation);

				// Gets the projectile component and assigns the shooters tag.
				Projectile projectile = projectileObj.GetComponent<Projectile>();
				projectile.ShootersTag = tag;

				// Ensures the projectile doesn't collide with the shooter.
				Collider2D projectCollider = projectile.GetComponent<Collider2D>();
				if (_collider && projectCollider)
					Physics2D.IgnoreCollision(projectCollider, _collider, true);

				// If handling animation events, trigger the ranged attack animation.
				if (_animator && HandleAnimationEvents)
					_animator.SetTrigger(RANGED_ATTACK);

				return projectileObj;
			}

			return null;
		}

		// ==== Unity Method Overrides ====

		private void Start()
		{
			// Ensures no cooldown needs to be waited on when the scene begins.
			SetCooldown(_cooldown);

			_animator = GetComponent<Animator>();
			_collider = GetComponent<Collider2D>();
		}

		private void OnValidate()
		{
			// Ensures the properties' set functions are called when one changes.
			Cooldown = _cooldown;
		}

		// ==== Private Custom Methods ====

		private void SetCooldown(float value)
		{
			_cooldown = value;

			// Sets the last attack time so no cooldown needs to be waited on.
			_lastAttackTime = Time.time - _cooldown * 2.0f;

			// If handling animation events, set the ranged animation speed multiplier.
			if (_animator && HandleAnimationEvents)
				_animator.SetFloat(RANGED_SPEED_MULTIPLIER, 1.0f / _cooldown);
		}
	}
}