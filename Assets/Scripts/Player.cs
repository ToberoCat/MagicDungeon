using Enemy;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Visuals.MessageSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour, IDamagable
{
    public static Player Instance { get; private set; }
    private Rigidbody2D _rigidbody2D;

    [SerializeField] private float health = 100f;

    [Header("Movement")] [SerializeField] private float moveSpeed;
    [SerializeField] private float diagonalMoveSpeed;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color damageColor;

    public Animator windShieldAnimator;


    float IDamagable.Health { get; set; }

    public void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        NotificationManager.Instance.ShowNotification(transform.position, $"{damage:F0}", damageColor);
        if (health <= 0)
        {
            Die();
        }
    }

    public bool FlipX
    {
        get => spriteRenderer.flipX;
        set => spriteRenderer.flipX = value;
    }

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        Instance = this;
    }

    private void Update()
    {
        var movement = GetMovement();
        Move(movement);
    }

    private Vector2 GetMovement()
    {
        var vertical = Input.GetAxis("Vertical");
        var horizontal = Input.GetAxis("Horizontal");

        spriteRenderer.flipX = horizontal switch
        {
            > 0 => false,
            < 0 => true,
            _ => spriteRenderer.flipX
        };
        return new Vector2(horizontal, vertical) * Time.deltaTime;
    }

    private void Move(Vector2 movement)
    {
        _rigidbody2D.velocity =
            movement.x != 0 && movement.y != 0 ? movement * diagonalMoveSpeed : movement * moveSpeed;
    }
}