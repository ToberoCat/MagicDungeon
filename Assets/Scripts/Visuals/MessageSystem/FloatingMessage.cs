using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Visuals.MessageSystem
{
    public class FloatingMessage : MonoBehaviour
    {
        [SerializeField] private float initialYVelocity = 7f;
        [SerializeField] private float xVelocity = 3f;
        [SerializeField] private float lifeTime = 0.8f;

        private Rigidbody2D _rigidbody2D;
        private TMP_Text _text;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _text = GetComponentInChildren<TMP_Text>();
        }

        private void Start()
        {
            _rigidbody2D.velocity = new Vector2(Random.Range(-xVelocity, xVelocity), initialYVelocity);
            Destroy(gameObject, lifeTime);
        }

        public void SetText(string text)
        {
            _text.text = text;
        }

        public void SetColor(Color color)
        {
            _text.color = color;
        }
    }
}