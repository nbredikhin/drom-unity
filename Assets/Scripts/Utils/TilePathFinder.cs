using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class PathNode
{
    public Vector3Int position;
    public int pathLengthFromStart;
    public PathNode previousNode;
    public int heuristicEstimatePathLength;

    public int EstimateFullPathLength
    {
        get
        {
            return this.pathLengthFromStart + this.heuristicEstimatePathLength;
        }
    }
}

public class TilePathFinder : MonoBehaviour
{
    public Grid grid;
    private List<Tilemap> collisionTilemaps = new List<Tilemap>();

    private void Start()
    {
        if (grid == null)
        {
            grid = this.GetComponent<Grid>();
        }

        var tilemaps = grid.GetComponentsInChildren<Tilemap>();
        foreach (var tilemap in tilemaps)
        {
            if (tilemap.GetComponent<TilemapCollider2D>())
            {
                collisionTilemaps.Add(tilemap);
            }
        }
    }

    public List<Vector3> FindPath(Vector2 from, Vector2 to)
    {
        var visitedNodes = new Collection<PathNode>();
        var nodesToVisit = new Collection<PathNode>();

        var startPosition = grid.WorldToCell(from);
        var finishPosition = grid.WorldToCell(to);

        PathNode startNode = new PathNode()
        {
            position = startPosition,
            previousNode = null,
            pathLengthFromStart = 0,
            heuristicEstimatePathLength = GetHeuristicPathLength(startPosition, finishPosition)
        };

        nodesToVisit.Add(startNode);

        int iterationsCount = 0;

        while (nodesToVisit.Count > 0)
        {
            var currentNode = nodesToVisit.OrderBy(node => node.EstimateFullPathLength).First();
            if (currentNode.position == finishPosition)
            {
                return GetPathFromNode(currentNode);
            }
            nodesToVisit.Remove(currentNode);
            visitedNodes.Add(currentNode);

            foreach (var neighbourNode in GetNodeNeighbours(currentNode, startPosition, finishPosition))
            {
                if (visitedNodes.Count(node => node.position == neighbourNode.position) > 0)
                    continue;
                var openNode = nodesToVisit.FirstOrDefault(node => node.position == neighbourNode.position);
                if (openNode == null)
                {
                    nodesToVisit.Add(neighbourNode);
                }
                else if (openNode.pathLengthFromStart > neighbourNode.pathLengthFromStart)
                {
                    openNode.previousNode = currentNode;
                    openNode.pathLengthFromStart = neighbourNode.pathLengthFromStart;
                }
            }

            iterationsCount++;

            if (iterationsCount > 500)
            {
                Debug.LogError("PathFinder iterations limit reached");
                return null;
            }
        }

        return null;
    }

    private List<Vector3> GetPathFromNode(PathNode node)
    {
        var result = new List<Vector3>();
        var currentNode = node;
        while (currentNode != null)
        {
            result.Add(grid.CellToWorld(currentNode.position) + Vector3.one * 0.08f);
            currentNode = currentNode.previousNode;
        }
        result.Reverse();
        return result;
    }

    private bool GetTilePassable(Vector3Int position, Vector3Int startPosition)
    {
        if ((position - startPosition).magnitude > 1.6) return false;
        foreach (var tilemap in collisionTilemaps)
        {
            if (tilemap.GetTile(position)) return false;
        }
        return true;
    }

    private List<PathNode> GetNodeNeighbours(PathNode node, Vector3Int startPosition, Vector3Int finishPosition)
    {
        var neighbourPositions = new Vector3Int[4];
        neighbourPositions[0] = node.position + new Vector3Int(1, 0, 0);
        neighbourPositions[1] = node.position + new Vector3Int(-1, 0, 0);
        neighbourPositions[2] = node.position + new Vector3Int(0, 1, 0);
        neighbourPositions[3] = node.position + new Vector3Int(0, -1, 0);

        var result = new List<PathNode>();
        foreach (var pos in neighbourPositions)
        {
            if (GetTilePassable(pos, startPosition))
            {
                var neighbourNode = new PathNode()
                {
                    position = pos,
                    previousNode = node,
                    pathLengthFromStart = node.pathLengthFromStart + 1,
                    heuristicEstimatePathLength = GetHeuristicPathLength(pos, finishPosition)
                };
                result.Add(neighbourNode);
            }
        }
        return result;
    }

    private int GetHeuristicPathLength(Vector3Int from, Vector3Int to)
    {
        return Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);
    }
}
