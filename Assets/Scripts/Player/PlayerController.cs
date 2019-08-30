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

    public bool isDashEnabled = true;
    public bool isBlockEnabled = true;

    private bool isDashAvaiable = true;
    private bool isDashActive;
    public bool IsDashing => isDashActive;
    private Vector2 dashDirection;

    public string[] dashIgnoredLayerNames;
    private int[] dashIgnoredLayers;
    private int playerLayer;

    public ParticleSystem particles;

    public bool enemiesAttackOnClick = false;
    private bool isDead = false;

    public Vector2 respawnPosition;

    public float blockDuration = 0.5f;
    public float blockCooldown = 1.0f;
    private bool isBlockAvailable = true;
    public bool isBlockActive = false;
    private float blockUsedTime = 0.0f;

    public List<AudioClip> StepSounds;

    public AudioClip DashSound;
    public AudioClip BlockSound;

    private void Start()
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

        var emission = particles.emission;
        emission.enabled = false;

        respawnPosition = transform.position;
        transform.Find("Shield").gameObject.SetActive(false);
    }

    void Update()
    {
        if (isDead) return;
        ProcessInput();
        Move();

        if (_movementSpeed > 0 && !GetComponent<AudioSource>().isPlaying)
        {
            DigitalRuby.SoundManagerNamespace.SoundManager.PlayOneShotSound(GetComponent<AudioSource>(), StepSounds[UnityEngine.Random.Range(0, StepSounds.Count)]);
        }
    }

    IEnumerator BlockCoroutine()
    {
        if (!isBlockEnabled) yield break;
        if (!isBlockAvailable) yield break;
        if (isDashActive) yield break;
        var shield = transform.Find("Shield").gameObject;
        var healthController = GetComponent<HealthController>();
        shield.SetActive(true);
        // shield.GetComponent<SpriteRenderer>().sortingOrder = MovementDirection.y > 0 ? -1 : 1;
        healthController.damageBlocked = true;
        isBlockAvailable = false;
        isBlockActive = true;
        yield return new WaitForSeconds(blockDuration);
        isBlockActive = false;
        shield.gameObject.SetActive(false);
        healthController.damageBlocked = false;
        yield return new WaitForSeconds(blockCooldown);
        isBlockAvailable = true;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (isBlockActive && ((1 << col.gameObject.layer) & LayerMask.GetMask("GoblinWeapons", "PlayerWeapons")) != 0)
        {
            DigitalRuby.SoundManagerNamespace.SoundManager.PlayOneShotSound(GetComponent<AudioSource>(), BlockSound);
        }
    }

    IEnumerator DashCoroutine(Vector2 direction)
    {
        if (!isDashEnabled) yield break;
        if (!isDashAvaiable) yield break;
        if (isBlockActive) yield break;

        var emission = particles.emission;
        foreach (int layer in dashIgnoredLayers)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, layer, true);
        }
        emission.enabled = true;
        isDashAvaiable = false;
        isDashActive = true;
        dashDirection = direction.normalized;
        yield return new WaitForSeconds(DASH_DURATION);
        foreach (int layer in dashIgnoredLayers)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, layer, false);
        }
        emission.enabled = false;
        isDashActive = false;
        yield return new WaitForSeconds(DASH_COOLDOWN);
        isDashAvaiable = true;
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

        if (Input.GetButtonDown("Dash") && isDashAvaiable)
        {
            StartCoroutine(DashCoroutine(_movementDirection));
            DigitalRuby.SoundManagerNamespace.SoundManager.PlayOneShotSound(GetComponent<AudioSource>(), DashSound);
        }

        if (Input.GetButtonDown("Block") && isBlockAvailable)
        {
            Debug.Log("Block");
            StartCoroutine(BlockCoroutine());
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
        if (isDashActive)
        {
            rb.velocity = dashDirection * DASH_SPEED * Time.deltaTime;
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

    void OnDamage()
    {
        var healthController = GetComponent<HealthController>();
        GameObject.Find("UI").SendMessage("UpdateHealth", healthController.health / healthController.MaxHealth);
    }
}
