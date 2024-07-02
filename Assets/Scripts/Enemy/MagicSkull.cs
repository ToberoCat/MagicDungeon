using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Visuals.MessageSystem;
using Random = UnityEngine.Random;

namespace Enemy
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MagicSkull : MonoBehaviour, IEnemy
    {
        [SerializeField] private int damage = 5;
        [SerializeField] private float maxLifetime = 5;

        [Header("Movement")] [SerializeField] private int speed = 5;
        [SerializeField] private float rotateSpeed = 5;

        [Header("Visuals")] [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Light2D light2D;
        [SerializeField] private Gradient lifetimeColorGradient;
        [SerializeField] private ParticleSystem explosionParticles;
        [SerializeField] private Color damageColor;

        private float _lifetime;
        private float _stunDuration;

        public Rigidbody2D rigidbody2D { get; private set; }
        public void Stun()
        {
            _stunDuration = 1;
        }

        private void Awake()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            FacePlayerInstantly();
            _lifetime = maxLifetime;
            light2D.color = spriteRenderer.color;
        }

        private void Update()
        {
            _stunDuration -= Time.deltaTime;
            _lifetime -= Time.deltaTime;
            if (_lifetime <= 0)
            {
                Die();
                return;
            }
            if (_stunDuration > 0)
                return;

            transform.rotation = Quaternion.Lerp(transform.rotation, FacingPlayerRotation(),
                Time.deltaTime * rotateSpeed);
            rigidbody2D.MovePosition(rigidbody2D.position + (Vector2)(speed * Time.deltaTime * transform.right));
            UpdateVisuals();
        }

        public void FacePlayerInstantly()
        {
            transform.rotation = FacingPlayerRotation();
        }

        private void UpdateVisuals()
        {
            var normalizedLifetime = 1 - _lifetime / maxLifetime;
            spriteRenderer.color = lifetimeColorGradient.Evaluate(normalizedLifetime);
            light2D.color = spriteRenderer.color;
        }


        private Quaternion FacingPlayerRotation()
        {
            var direction = (Vector2)Player.Instance.transform.position - rigidbody2D.position;
            return Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;

            Player.Instance.TakeDamage(damage);
            Die();
        }

        float IDamagable.Health { get; set; }

        public void Die()
        {
            explosionParticles.transform.parent = null;
            explosionParticles.Play();
            Destroy(gameObject);
        }

        public void TakeDamage(float damage)
        {
            NotificationManager.Instance.ShowNotification(transform.position, $"{damage:F0}", damageColor);
            Die();
        }
    }
}