using UnityEngine;
using System.Collections.Generic;

public class GoblinController : MonoBehaviour
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

    private void Awake()
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

    private void RecalculatePath()
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

    private void Update()
    {
        if (_target == null) return;

        var targetPosition = _target.transform.position;
        var currentPosition = transform.position;
        var raycastDirection = targetPosition - currentPosition;
        var raycastHit = Physics2D.Raycast(
            currentPosition,
            raycastDirection.normalized,
            raycastDirection.magnitude,
            PathFindObstaclesMask
        );
        var isPathClear = raycastHit.collider == null;

        if (isPathClear)
        {
            if (DrawPath)
                Debug.DrawLine(currentPosition, currentPosition + raycastDirection, PathColor);
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
                RecalculatePath();

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

        // Move goblin
        var direction = targetPosition - currentPosition;
        direction.z = 0;
        if (direction.magnitude > 0.05f)
        {
            rb.velocity = direction.normalized * MOVEMENT_BASE_SPEED * Time.deltaTime;
        }
        else
        {
            rb.velocity = Vector2.zero;
            if (currentPath != null && currentPathNode < currentPath.Count - 1)
            {
                currentPathNode = currentPathNode + 1;
            }
        }
    }
}
