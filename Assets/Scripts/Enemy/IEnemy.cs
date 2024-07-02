using UnityEngine;

namespace Enemy
{
    public interface IEnemy : IDamagable
    {
        public Rigidbody2D rigidbody2D { get; }
        public void Stun();
    }
}