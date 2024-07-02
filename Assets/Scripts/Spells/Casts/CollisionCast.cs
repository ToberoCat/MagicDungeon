using Enemy;
using UnityEngine;

namespace Spells.Casts
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class CollisionCast : MonoBehaviour, ICast
    {
        [SerializeField] private AnimationCurve damageByPower;

        protected Rigidbody2D Rigidbody2D;

        protected float Damage;

        public virtual void SpawnWithPower(Player player, float power)
        {
            Damage = damageByPower.Evaluate(power);
        }

        private void Awake()
        {
            Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                other.GetComponent<IDamagable>().TakeDamage(Damage);
                Die();
            }
            else if (other.CompareTag("Walls"))
            {
                Die();
            }
        }


        public void Die()
        {
            Destroy(gameObject);
        }
    }
}