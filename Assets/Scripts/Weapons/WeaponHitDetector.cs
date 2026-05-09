using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using OffTheClock.Combat;

namespace OffTheClock.Weapons
{
    // Attach to a child GameObject on the weapon. Sweeps a sphere along the weapon's
    // motion each physics step to catch fast swings that would otherwise tunnel.
    [RequireComponent(typeof(Collider))]
    public class WeaponHitDetector : MonoBehaviour
    {
        [Tooltip("Minimum speed (m/s) the weapon must be moving to register a hit.")]
        public float minVelocityToHit = 0.5f;

        [Tooltip("Radius of the sweep test. Make it roughly the blade thickness.")]
        public float sweepRadius = 0.08f;

        [Tooltip("Cooldown per enemy so one swing doesn't multi-hit the same target.")]
        public float perEnemyCooldown = 0.25f;

        [Tooltip("Particle prefab spawned at hit point.")]
        public GameObject hitEffect;

        [Tooltip("Seconds before spawned effect auto-destroys. 0 = leave alone.")]
        public float hitEffectLifetime = 2f;

        [Tooltip("Sound played at hit point.")]
        public AudioClip hitSound;

        [Range(0f, 1f)] public float hitSoundVolume = 1f;

        [Header("Haptics")]
        [Range(0f, 1f)] public float hapticAmplitude = 0.8f;
        public float hapticDuration = 0.15f;

        private WeaponBase _weapon;
        private XRGrabInteractable _grab;
        private Vector3 _prevPos;
        private bool _initialized;
        private readonly Dictionary<EnemyHealth, float> _recentHits = new();

        void Awake()
        {
            _weapon = GetComponentInParent<WeaponBase>();
            _grab = GetComponentInParent<XRGrabInteractable>();
            GetComponent<Collider>().isTrigger = true;
        }

        void SendHaptic()
        {
            if (_grab == null) return;
            foreach (var interactor in _grab.interactorsSelecting)
            {
                if (interactor is XRBaseInputInteractor input)
                    input.SendHapticImpulse(hapticAmplitude, hapticDuration);
            }
        }

        void OnEnable()
        {
            _prevPos = transform.position;
            _initialized = false;
            _recentHits.Clear();
        }

        void FixedUpdate()
        {
            Vector3 currentPos = transform.position;
            Vector3 delta = currentPos - _prevPos;
            float dist = delta.magnitude;
            float velocity = dist / Time.fixedDeltaTime;

            if (_initialized && velocity >= minVelocityToHit && dist > 0.0001f)
            {
                var hits = Physics.SphereCastAll(_prevPos, sweepRadius, delta.normalized, dist, ~0, QueryTriggerInteraction.Collide);
                foreach (var hit in hits)
                {
                    var enemyHealth = hit.collider.GetComponentInParent<EnemyHealth>();
                    if (enemyHealth == null) continue;

                    if (_recentHits.TryGetValue(enemyHealth, out float lastTime)
                        && Time.time - lastTime < perEnemyCooldown) continue;

                    _recentHits[enemyHealth] = Time.time;

                    float dmg = (_weapon != null ? _weapon.damage : 25f)
                                * (PlayerStats.Instance?.DamageMultiplier ?? 1f);
                    enemyHealth.OnHit(dmg, _weapon);

                    Vector3 spawnPos = hit.point != Vector3.zero ? hit.point : currentPos;

                    if (hitEffect != null)
                    {
                        var fx = Instantiate(hitEffect, spawnPos, Quaternion.identity);
                        if (hitEffectLifetime > 0f) Destroy(fx, hitEffectLifetime);
                    }

                    if (hitSound != null)
                        AudioSource.PlayClipAtPoint(hitSound, spawnPos, hitSoundVolume);

                    SendHaptic();
                }
            }

            _prevPos = currentPos;
            _initialized = true;
        }
    }
}
