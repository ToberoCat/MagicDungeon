using Spells.Casts;
using UnityEngine;

namespace Spells.Providers
{
    [CreateAssetMenu(fileName = "spell", menuName = "Spells/EarthSpell")]
    public class EarthSpell : Spell
    {
        [SerializeField] private GameObject earthPrefab;

        public override void CastSpell(Player player, float power)
        {
            var earthCast = SpawnCast(player);
            earthCast.SpawnWithPower(player, power);
            if (!(power >= 0.8))
                return;
            earthCast = SpawnCast(player);
            earthCast.SpawnWithPower(player, EarthCast.GetNearestEnemy(player.transform.position, skip: 1), power);
        }

        private EarthCast SpawnCast(Player player)
        {
            return Instantiate(earthPrefab, player.transform.position, Quaternion.identity).GetComponent<EarthCast>();
        }
    }
}