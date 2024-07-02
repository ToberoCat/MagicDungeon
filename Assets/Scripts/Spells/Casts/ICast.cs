namespace Spells.Casts
{
    public interface ICast
    {
        void SpawnWithPower(Player player, float power);
        void Die();
    }
}