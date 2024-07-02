using Enemy;
using UnityEngine;

namespace Spells.Casts
{
    public class WaterCast : CollisionCast
    {
        [SerializeField] private AnimationCurve lifeTime = AnimationCurve.Linear(0, 0, 1, 30);

        private float _lifeTime;

        public override void SpawnWithPower(Player player, float power)
        {
            _lifeTime = lifeTime.Evaluate(power);
            base.SpawnWithPower(player, power);
        }

        private void Update()
        {
            _lifeTime -= Time.deltaTime;
            if (_lifeTime <= 0)
            {
                Die();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Enemy"))
                return;

            other.GetComponent<IDamagable>().TakeDamage(Damage);
            transform.position = (Vector3) Random.insideUnitCircle + other.transform.position;
        }
    }
}