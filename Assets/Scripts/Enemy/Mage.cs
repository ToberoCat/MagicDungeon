using System;
using System.Collections;
using UnityEngine;
using Visuals.MessageSystem;
using Random = UnityEngine.Random;

namespace Enemy
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Mage : MonoBehaviour, IEnemy
    {
        [SerializeField] private float minSpawnInterval = 5f;
        [SerializeField] private float maxSpawnInterval = 6f;
        [SerializeField] private float health = 10f;
        [SerializeField] private GameObject magicSkullPrefab;
        [SerializeField] private float spawnDistanceThreshold = 10;
        [SerializeField] private GameObject dungeonExitPrefab;
        [Header("Visuals")] [SerializeField] private Color damageColor;

        public Rigidbody2D rigidbody2D { get; set; }

        public void Stun()
        {
        }

        private void Awake()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            StartCoroutine(SpawnSkull());
        }

        private void FixedUpdate()
        {
            FlipToFacePlayer();
        }

        private void FlipToFacePlayer()
        {
            var playerX = Player.Instance.transform.position.x;
            var mageX = transform.position.x;
            transform.rotation = Quaternion.Euler(0, playerX < mageX ? 180 : 0, 0);
        }

        private IEnumerator SpawnSkull()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));
                if (Vector3.Distance(Player.Instance.transform.position, transform.position) >= spawnDistanceThreshold)
                    continue;

                var magicSkull = Instantiate(magicSkullPrefab, transform.position, Quaternion.identity)
                    .GetComponent<MagicSkull>();
                magicSkull.FacePlayerInstantly();
            }
        }

        float IDamagable.Health { get; set; }

        public void Die()
        {
            if (FindObjectsByType<Mage>(FindObjectsSortMode.None).Length == 1)
            {
                NotificationManager.Instance.ShowNotification(Player.Instance.transform.position, "Dungeon Cleared",
                    Color.green);
                Instantiate(dungeonExitPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }

        public void TakeDamage(float damage)
        {
            health -= damage;
            NotificationManager.Instance.ShowNotification(transform.position, $"{damage:F0}", damageColor);
            if (health <= 0)
                Die();
        }
    }
}