using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace Visuals
{
    [RequireComponent(typeof(Light2D))]
    public class FireLight : MonoBehaviour
    {
        [SerializeField] private float minIntensity = 0.5f;
        [SerializeField] private float maxIntensity = 1f;

        [SerializeField] private float minOuterRadius = 1f;
        [SerializeField] private float maxOuterRadius = 2f;

        [SerializeField] private AnimationCurve intensityCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve outerRadiusCurve = AnimationCurve.Linear(0, 0, 1, 1);

        private Light2D _light2D;
        private float _valueOffset;

        private void Awake()
        {
            _light2D = GetComponent<Light2D>();
            _valueOffset = Random.value;
        }

        private void Update()
        {
            var normalizedTime = 2 * Mathf.Abs(-0.5f + (Time.time + _valueOffset) % 1);
            var noise = Mathf.PerlinNoise(Time.time + _valueOffset, 0);
            var offsetTime = normalizedTime + noise * 0.1f;
            _light2D.intensity = Mathf.Lerp(minIntensity, maxIntensity, intensityCurve.Evaluate(offsetTime));
            _light2D.pointLightOuterRadius = Mathf.Lerp(minOuterRadius, maxOuterRadius, outerRadiusCurve.Evaluate(offsetTime));
        }
    }
}