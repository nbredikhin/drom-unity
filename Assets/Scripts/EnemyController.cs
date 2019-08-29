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
    private Rigidbody2D rb;

    private TilePathFinder pathFinder;

    private GameObject target;
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

    public Vector2 lookDirection;
    public Vector2 forceLookDirection = Vector2.zero;
    private bool isDying;

    public bool isFollowingEnabled = true;
    public bool isAttackEnabled = true;

    public bool isWalking = false;

    // Start is called before the first frame update
    void Start()
    {
        if (rb == null)
        {
            rb = gameObject.GetComponent<Rigidbody2D>();
        }
        if (pathFinder == null)
        {
            pathFinder = GameObject.Find("Grid").GetComponent<TilePathFinder>();
        }

        target = GameObject.Find("Player");
        RecalculatePath();
    }

    void RecalculatePath()
    {
        // If path was recalculated too recently
        if (pathUpdateTime > 0 && Time.time - pathUpdateTime < PATH_RECALCULATE_TIME)
            return;
        if (target == null)
        {
            currentPath = null;
            return;
        }

        pathUpdateTime = Time.time;
        pathTargetPosition = target.transform.position;
        if (isFollowingEnabled)
        {
            currentPath = pathFinder.FindPath(transform.position, pathTargetPosition);
        }
        if (currentPath != null)
            currentPathNode = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDying) return;

        if (target == null) return;

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

        var targetPosition = target.transform.position;
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
            if ((pathTargetPosition - target.transform.position).magnitude > PATH_RECALCULATE_DISTANCE)
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

        lookDirection = direction.normalized;
        if (forceLookDirection != Vector2.zero)
            lookDirection = forceLookDirection;

        if (attacking && isAttackEnabled)
        {
            if (Time.time - attackStart >= AttackTime)
            {
                attacking = false;
            }
            else
            {
                if (isAttackEnabled)
                {
                    if (Mathf.Abs(Time.time - attackStart - AttackTime / 2) <= 2 * Time.deltaTime)
                    {
                        AttackUnit();
                    }
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
            if (isFollowingEnabled)
            {
                isWalking = true;
                rb.velocity = lookDirection * Time.deltaTime;
            }
            else
            {
                isWalking = false;
            }
        }
        else
        {
            isWalking = false;
            lookDirection = Vector2.zero;
            rb.velocity = lookDirection;
            if (currentPath != null && currentPathNode < currentPath.Count - 1)
            {
                currentPathNode = currentPathNode + 1;
            }
        }
    }

    void PrepareForAttack()
    {
        if (isDying) return;
        if (attacking)
        {
            return;
        }
        rb.velocity = Vector2.zero;
        attacking = true;
        attackStart = Time.time;
    }

    public void AttackUnit()
    {
        if (isDying) return;
        SendMessage("Attack", lookDirection);
    }

    void KnockBack(Vector2 direction)
    {
        if (isDying) return;
        knockedBack = true;
        knockBackStart = Time.time;

        rb.AddForce(direction);
    }

    void Die()
    {
        isDying = true;
        Debug.Log("I'm " + gameObject.name + " dying!");
        var collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;
        Destroy(gameObject, 3.0f);
    }
}
