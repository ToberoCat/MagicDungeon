using Spells.Casts;
using UnityEngine;

namespace Spells.Providers
{
    [CreateAssetMenu(fileName = "spell", menuName = "Spells/FireSpell")]
    public class FireSpell : Spell
    {
        [SerializeField] private GameObject firePrefab;
        [SerializeField] private AnimationCurve fireCount = AnimationCurve.Linear(0, 0, 1, 30);
        [SerializeField] private AnimationCurve minAngleCurve = AnimationCurve.Linear(0, 0, 1, -45);
        [SerializeField] private AnimationCurve maxAngleCurve = AnimationCurve.Linear(0, 0, 1, 45);
        [SerializeField] private float powerNoise = 0.04f;

        public override void CastSpell(Player player, float power)
        {
            var minAngle = minAngleCurve.Evaluate(power);
            var maxAngle = maxAngleCurve.Evaluate(power);
            for (var i = 0; i < fireCount.Evaluate(power); i++)
            {
                var angle = Random.Range(minAngle, maxAngle);
                var fire = Instantiate(firePrefab, player.transform.position,
                    Quaternion.Euler(0, player.FlipX ? 180 : 0, angle));
                fire.GetComponent<FireCast>().SpawnWithPower(player, power + Random.Range(-powerNoise, powerNoise));
            }
        }
    }
}