using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Stats")]

    [Range(1, 500)] public float MOVEMENT_BASE_SPEED;

    [Range(1, 500)] public float DASH_SPEED;
    [SerializeField] public float DASH_DURATION;
    [SerializeField] public float DASH_COOLDOWN;

    [Space(10)]

    [Header("Movement State")]

    [SerializeField] private float _movementSpeed;

    public float MovementSpeed => _movementSpeed;

    [SerializeField] private Vector2 _movementDirection;
    [SerializeField] private Vector2 _previousMovementDirection;

    public Vector2 MovementDirection => _movementDirection;

    [SerializeField] private Vector2 _mouseDirection;

    public Vector2 MouseDirection => _mouseDirection;

    [Space(10)]

    [Header("References")]

    public Rigidbody2D rb;

    private bool _isDashAvailable = true;
    private bool _isDashActive;
    public bool IsDashing => _isDashActive;
    private Vector2 _dashDirection;

    public string[] dashIgnoredLayerNames;
    private int[] dashIgnoredLayers;
    private int playerLayer;

    public ParticleSystem _particleSystem;

    public bool enemiesAttackOnClick = false;
    private bool isDead = false;

    public Vector2 respawnPosition;

    private void Awake()
    {
        if (rb == null)
        {
            rb = gameObject.GetComponent<Rigidbody2D>();
        }

        dashIgnoredLayers = new int[dashIgnoredLayerNames.Length];
        for (int i = 0; i < dashIgnoredLayerNames.Length; i++)
        {
            dashIgnoredLayers[i] = LayerMask.NameToLayer(dashIgnoredLayerNames[i]);
        }
        playerLayer = LayerMask.NameToLayer("Player");

        var emission = _particleSystem.emission;
        emission.enabled = false;

        respawnPosition = transform.position;
    }

    void Update()
    {
        if (isDead) return;
        ProcessInput();
        Move();
    }

    IEnumerator Dash(Vector2 direction)
    {
        var emission = _particleSystem.emission;
        foreach (int layer in dashIgnoredLayers)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, layer, true);
        }
        emission.enabled = true;
        _isDashAvailable = false;
        _isDashActive = true;
        _dashDirection = direction.normalized;
        yield return new WaitForSeconds(DASH_DURATION);
        foreach (int layer in dashIgnoredLayers)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, layer, false);
        }
        emission.enabled = false;
        _isDashActive = false;
        yield return new WaitForSeconds(DASH_COOLDOWN);
        _isDashAvailable = true;
    }

    void ProcessInput()
    {
        //movement direction processing
        _movementDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        _movementSpeed = Mathf.Clamp(_movementDirection.magnitude, 0.0f, 1.0f);
        _movementDirection.Normalize();

        //mouse direction processing
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _mouseDirection = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);
        _mouseDirection.Normalize();

        if (Input.GetButtonDown("Dash") && _isDashAvailable)
        {
            StartCoroutine(Dash(_movementDirection));
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (enemiesAttackOnClick)
            {
                foreach (var enemy in GameObject.FindObjectsOfType<EnemyController>())
                {
                    enemy.AttackUnit();
                    Debug.Log("Attack " + enemy.gameObject.name);
                };
            }
            else
            {
                SendMessage("Attack", _mouseDirection);
            }
        }

        if (_movementDirection.magnitude != 0)
        {
            _previousMovementDirection = _movementDirection;
        }
    }

    void Move()
    {
        if (_isDashActive)
        {
            rb.velocity = _dashDirection * DASH_SPEED * Time.deltaTime;
        }
        else
        {
            rb.velocity = _movementDirection * _movementSpeed * MOVEMENT_BASE_SPEED * Time.deltaTime;
        }
    }

    void OnArrowFlightStart()
    {
        Physics2D.IgnoreLayerCollision(playerLayer, LayerMask.NameToLayer("Level"), true);
    }

    void OnArrowFlightEnd()
    {
        Physics2D.IgnoreLayerCollision(playerLayer, LayerMask.NameToLayer("Level"), false);
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        GameObject.Find("UI").SendMessage("OnPlayerDead");
    }

    public void Respawn()
    {
        var player = Instantiate(gameObject);
        player.transform.position = respawnPosition;
        player.GetComponent<PlayerController>().respawnPosition = respawnPosition;
        Destroy(gameObject);
    }
}
