using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopdownCharacterController
{
	public class Health : MonoBehaviour
	{
		// ==== Properties ====

		public float HP = 5.0f;

		[SerializeField]
		private float _damageGracePeriod = 0.8f;
		public float DamageGracePeriod
		{
			get { return _damageGracePeriod; }
			set { SetDamageGracePeriod(value); }
		}

		public int GracePeriodFlashes = 4;
		public List<string> DamageWhitelistTags = new List<string>();

		public delegate void DeathCallback(Dictionary<string, string> damageInfo);
		public List<DeathCallback> DeathCallbacks = new List<DeathCallback>();

		public delegate void HealthChangedCallback (Dictionary<string, string> changeInfo);
		public List<HealthChangedCallback> HealthChangedCallbacks = new List<HealthChangedCallback>();

		// ==== Private Variables ====

		private const string ALL_TAG = "All";

		private Renderer _renderer;
		private float _lastHitTaken = 0.0f;

		// ==== Public Custom Methods ====

		public bool TakeDamage(float damage, string attackersTag = null, Dictionary<string, string> damageInfo = null)
		{
			// Checks the grace period has elapsed.
			if (Time.time >= _lastHitTaken + _damageGracePeriod)
			{
				// Checks if the attackers tag is whitelisted to damage this object,
				//		or if the whitelist contains "All".
				if (attackersTag == null
					|| DamageWhitelistTags.Contains(attackersTag)
					|| DamageWhitelistTags.Contains(ALL_TAG))
				{
					// Sets the new last hit taken time and takes the damage.
					_lastHitTaken = Time.time;
					HP -= damage;

					// Checks if health has reached zero and destroys if so.
					if (HP <= 0.0f)
					{
						foreach (var callback in DeathCallbacks)
							callback(damageInfo);

						Destroy(gameObject);
					}
					// Starts the flash coroutine if not dead.
					else StartCoroutine(FlashForGracePeriod());

					return true;
				}
			}

			return false;
		}

		// ==== Unity Method Overrides ====

		private void OnValidate()
		{
			// Ensures the property's set function is called when it changes.
			SetDamageGracePeriod(_damageGracePeriod);
		}

		void Start()
		{
			_renderer = GetComponent<Renderer>();
			SetDamageGracePeriod(_damageGracePeriod);
		}

		// ==== Private Custom Methods ====

		private IEnumerator FlashForGracePeriod()
		{
			yield return null;

			// Checks there's a renderer as the character can't flash without it.
			if (_renderer)
			{
				// Works out the number of iterations to flash for.
				int iterations = GracePeriodFlashes * 2;
				for (int i = 0; i < iterations; ++i)
				{
					// Toggles the visibility and waits.
					_renderer.enabled = !_renderer.enabled;
					yield return new WaitForSeconds(_damageGracePeriod / iterations);
				}
			}
		}

		private void SetDamageGracePeriod(float value)
		{
			_damageGracePeriod = value;
			_lastHitTaken = Time.time - _damageGracePeriod * 2.0f;
		}
	}
}