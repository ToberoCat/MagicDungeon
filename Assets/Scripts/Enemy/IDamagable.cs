namespace Enemy
{
    public interface IDamagable
    {
        public float Health { get; protected set; }
        void Die();
        void TakeDamage(float damage);
    }
}