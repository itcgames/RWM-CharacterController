using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopdownRangedAttack : MonoBehaviour
{
	public GameObject projectilePrefab;

	[SerializeField]
	private float _cooldown = 0.5f;
	public float Cooldown { get { return _cooldown; }
							set { SetCooldown(value); } }

	public bool LimitedAmmo = false;
	public int Ammo = 0;

	private const string RANGED_SPEED_MULTIPLIER = "RangedSpeedMultiplier";
	private const string RANGED_ATTACK = "RangedAttack";

	private float _lastAttackTime;
	private TopdownCharacterController controller;

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

			// If handling animation events, trigger the ranged attack animation.
			if (controller && controller.Animator && controller.HandleAnimationEvents)
				controller.Animator.SetTrigger(RANGED_ATTACK);

			return projectileObj;
		}

		return null;
	}

	private void Start()
	{
		// Ensures no cooldown needs to be waited on when the scene begins.
		SetCooldown(_cooldown);

		controller = GetComponent<TopdownCharacterController>();
	}

	private void OnValidate()
	{
		// Ensures the properties' set functions are called when one changes.
		Cooldown = _cooldown;
	}

	private void SetCooldown(float value)
	{
		_cooldown = value;

		// Sets the last attack time so no cooldown needs to be waited on.
		_lastAttackTime = Time.time - _cooldown * 2.0f;

		// If handling animation events, set the ranged animation speed multiplier.
		if (controller && controller.Animator && controller.HandleAnimationEvents)
			controller.Animator.SetFloat(RANGED_SPEED_MULTIPLIER, 1.0f / _cooldown);
	}
}