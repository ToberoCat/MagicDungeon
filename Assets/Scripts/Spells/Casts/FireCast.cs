using Enemy;
using UnityEngine;

namespace Spells.Casts
{
    public class FireCast : CollisionCast
    {
        [Header("Curves")] 
        [SerializeField] private AnimationCurve speedByPower;
        [SerializeField] private AnimationCurve sizeByPower;
        [SerializeField] private AnimationCurve durationByPower;
        
        private float _speed;
        private float _duration;

        public override void SpawnWithPower(Player player, float power)
        {
            _speed = speedByPower.Evaluate(power);
            _duration = durationByPower.Evaluate(power);
            var size = sizeByPower.Evaluate(power);
            transform.localScale = new Vector3(size, size, 1);
            base.SpawnWithPower(player, power);
        }

        private void Update()
        {
            _duration -= Time.deltaTime;
            if (_duration <= 0)
            {
                Die();
                return;
            }

            Rigidbody2D.velocity = transform.right * _speed;
        }
    }
}