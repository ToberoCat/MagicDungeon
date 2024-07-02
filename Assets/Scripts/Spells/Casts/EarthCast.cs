using System.Linq;
using UnityEngine;

namespace Spells.Casts
{
    public class EarthCast : CollisionCast
    {
        [SerializeField] private float speed;

        public override void SpawnWithPower(Player player, float power)
        {
           SpawnWithPower(player, GetNearestEnemy(player.transform.position), power);
            base.SpawnWithPower(player, power);
        }

        public static Vector3 GetNearestEnemy(Vector3 reference, int skip = 0)
        {
            return GameObject.FindGameObjectsWithTag("Enemy")
                .Select(obj => obj.transform.position)
                .OrderBy(obj => Vector2.Distance(reference, obj))
                .Skip(skip)
                .FirstOrDefault();
        }

        public void SpawnWithPower(Player player, Vector3 target, float power)
        {
            Vector2 direction = target - player.transform.position;
            transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            base.SpawnWithPower(player, power);
        }

        private void Update()
        {
            Rigidbody2D.velocity = transform.right * speed;
        }
    }
}