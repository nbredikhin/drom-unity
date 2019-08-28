using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("General stuff")]
    [Range(1, 500)] public float MOVEMENT_BASE_SPEED;
    public LayerMask PathFindObstaclesMask;
    public bool DrawPath;
    public Color PathColor = Color.green;
    [Space(10)]

    [Header("References")]
    public Rigidbody2D rb;

    [SerializeField] private TilePathFinder _pathFinder;
    public TilePathFinder PathFinder => _pathFinder;

    private GameObject _target;
    private Vector3 pathTargetPosition;
    private List<Vector3> currentPath;
    private float PATH_RECALCULATE_TIME = 2.0f;
    private float PATH_RECALCULATE_DISTANCE = 0.16f;
    private int currentPathNode;
    private float pathUpdateTime = 0;

    bool knockedBack;
    float knockBackStart;
    public float KnockBackTime = 0.3f;

    bool attacking;
    float attackStart;
    public float AttackTime = 1.0f;
    public float MaxAttackDistance = 0.2f;

    public Vector2 MovementVelocity;

    // Start is called before the first frame update
    void Start()
    {
        if (rb == null)
        {
            rb = gameObject.GetComponent<Rigidbody2D>();
        }
        if (_pathFinder == null)
        {
            _pathFinder = GameObject.Find("Grid").GetComponent<TilePathFinder>();
        }

        _target = GameObject.Find("Player");
        RecalculatePath();
    }

    void RecalculatePath()
    {
        // If path was recalculated too recently
        if (pathUpdateTime > 0 && Time.time - pathUpdateTime < PATH_RECALCULATE_TIME)
            return;
        if (_target == null)
        {
            currentPath = null;
            return;
        }

        pathUpdateTime = Time.time;
        pathTargetPosition = _target.transform.position;
        currentPath = _pathFinder.FindPath(transform.position, pathTargetPosition);
        if (currentPath != null)
            currentPathNode = 0;
    }

    // Update is called once per frame
    void Update()
    {
        MovementVelocity = Vector2.zero;

        if (_target == null) return;

        if (knockedBack)
        {
            if (Time.time - knockBackStart >= KnockBackTime)
            {
                knockedBack = false;
            }
            else
            {
                return;
            }
        }

        var targetPosition = _target.transform.position;
        var currentPosition = transform.position;
        var raycastDirection = targetPosition - currentPosition;
        raycastDirection.z = 0;
        var raycastHit = Physics2D.Raycast(currentPosition,
                                           raycastDirection.normalized,
                                           raycastDirection.magnitude - 0.05f,
                                           PathFindObstaclesMask);

        Debug.DrawLine(currentPosition, currentPosition + raycastDirection, Color.red);
        var isPathClear = raycastHit.collider == null || raycastHit.collider.gameObject.Equals(gameObject);

        if (isPathClear)
        {
            if (DrawPath)
                Debug.DrawLine(currentPosition, currentPosition + raycastDirection, Color.red);
        }
        else
        {
            if (currentPath == null)
            {
                // If it's been too long since last path update, recalculate path
                if (Time.time - pathUpdateTime > PATH_RECALCULATE_TIME) RecalculatePath();
                return;
            }

            // If target moved far enough to recalculate path
            if ((pathTargetPosition - _target.transform.position).magnitude > PATH_RECALCULATE_DISTANCE)
            {
                RecalculatePath();
            }

            if (currentPath != null)
            {
                targetPosition = currentPath[currentPathNode];

                if (DrawPath && currentPath.Count > 1)
                {
                    for (int i = 1; i < currentPath.Count; i++)
                    {
                        Debug.DrawLine(currentPath[i - 1], currentPath[i], PathColor);
                    }
                }
            }
        }

        var direction = targetPosition - currentPosition;
        direction.z = 0;

        if (attacking)
        {
            if (Time.time - attackStart >= AttackTime)
            {
                attacking = false;
            }
            else
            {
                if (Mathf.Abs(Time.time - attackStart - AttackTime / 2) <= 2 * Time.deltaTime)
                {
                    AttackUnit(new Vector2(direction.x, direction.y));
                }
                return;
            }
        }

        // Move enemy
        if (direction.magnitude < MaxAttackDistance && isPathClear)
        {
            PrepareForAttack();
            return;
        }
        if (direction.magnitude > 0.05f)
        {
            MovementVelocity = direction.normalized * MOVEMENT_BASE_SPEED;
            rb.velocity = MovementVelocity * Time.deltaTime;
        }
        else
        {
            MovementVelocity = Vector2.zero;
            rb.velocity = MovementVelocity;
            if (currentPath != null && currentPathNode < currentPath.Count - 1)
            {
                currentPathNode = currentPathNode + 1;
            }
        }
    }

    void PrepareForAttack()
    {
        if (attacking)
        {
            return;
        }
        rb.velocity = Vector2.zero;
        attacking = true;
        attackStart = Time.time;
    }

    void AttackUnit(Vector2 targetDirection)
    {
        SendMessage("Attack", targetDirection);
    }

    void KnockBack(Vector2 direction)
    {
        knockedBack = true;
        knockBackStart = Time.time;

        rb.AddForce(direction);
    }

    void Die()
    {
        Debug.Log("I'm " + gameObject.name + " dead!");
        Destroy(gameObject);
    }
}
