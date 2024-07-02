using System.Collections;
using Enemy;
using UnityEngine;

namespace Spells.Providers
{
    [CreateAssetMenu(fileName = "spell", menuName = "Spells/WindSpell")]
    public class WindSpell : Spell
    {
        [SerializeField] private float basePushForce = 100;
        private static readonly int Wind = Animator.StringToHash("wind");

        public override void CastSpell(Player player, float power)
        {
            player.windShieldAnimator.SetTrigger(Wind);
            foreach (var enemyGm in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                var facingAwayFromPLayer = enemyGm.transform.position - player.transform.position;
                var enemy = enemyGm.GetComponent<IEnemy>();
                enemy.rigidbody2D.AddForce(power * basePushForce * facingAwayFromPLayer.normalized);
                enemy.Stun();
            }
        }
    }
}