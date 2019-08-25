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

    public Vector2 MovementDirection => _movementDirection;

    [SerializeField] private Vector2 _mouseDirection;

    public Vector2 MouseDirection => _mouseDirection;

    [Space(10)]

    [Header("References")]

    public Rigidbody2D rb;

    private bool _isDashAvailable = true;
    private bool _isDashActive;
    private Vector2 _dashDirection;

    private void Awake()
    {
        if (rb == null)
        {
            gameObject.GetComponent<Rigidbody2D>();
        }
    }

    void Update()
    {
        ProcessInput();
        Move();
    }

    IEnumerator Dash(Vector2 direction)
    {
        _isDashAvailable = false;
        _isDashActive = true;
        _dashDirection = direction.normalized;
        yield return new WaitForSeconds(DASH_DURATION);
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
}
