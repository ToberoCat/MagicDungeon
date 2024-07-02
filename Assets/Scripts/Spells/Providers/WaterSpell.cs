using Spells.Casts;
using UnityEngine;

namespace Spells.Providers
{
    [CreateAssetMenu(fileName = "spell", menuName = "Spells/WaterSpell")]
    public class WaterSpell : Spell
    {
        [SerializeField] private GameObject waterPrefab;

        public override void CastSpell(Player player, float power)
        {
            var waterCast = Instantiate(waterPrefab, player.transform.position, Quaternion.identity)
                .GetComponent<WaterCast>();
            waterCast.SpawnWithPower(player, power);
        }
    }
}