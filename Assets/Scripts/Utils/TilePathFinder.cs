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

    private void Awake()
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
        var closedSet = new Collection<PathNode>();
        var openSet = new Collection<PathNode>();

        var startPosition = grid.WorldToCell(from);
        var finishPosition = grid.WorldToCell(to);
        PathNode startNode = new PathNode()
        {
            position = startPosition,
            previousNode = null,
            pathLengthFromStart = 0,
            heuristicEstimatePathLength = GetHeuristicPathLength(startPosition, finishPosition)
        };

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            var currentNode = openSet.OrderBy(node => node.EstimateFullPathLength).First();
            if (currentNode.position == finishPosition)
            {
                return GetPathFromNode(currentNode);
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            foreach (var neighbourNode in GetNodeNeighbours(currentNode, finishPosition))
            {
                if (closedSet.Count(node => node.position == neighbourNode.position) > 0)
                    continue;
                var openNode = openSet.FirstOrDefault(node => node.position == neighbourNode.position);
                if (openNode == null)
                {
                    openSet.Add(neighbourNode);
                }
                else if (openNode.pathLengthFromStart > neighbourNode.pathLengthFromStart)
                {
                    openNode.previousNode = currentNode;
                    openNode.pathLengthFromStart = neighbourNode.pathLengthFromStart;
                }
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

    private bool GetTilePassable(Vector3Int position)
    {
        bool isPassable = false;
        collisionTilemaps.ForEach(tilemap =>
            isPassable = isPassable || tilemap.GetTile(position));
        return !isPassable;
    }

    private List<PathNode> GetNodeNeighbours(PathNode node, Vector3Int finishPosition)
    {
        var neighbourPositions = new Vector3Int[4];
        neighbourPositions[0] = node.position + new Vector3Int(1, 0, 0);
        neighbourPositions[1] = node.position + new Vector3Int(-1, 0, 0);
        neighbourPositions[2] = node.position + new Vector3Int(0, 1, 0);
        neighbourPositions[3] = node.position + new Vector3Int(0, -1, 0);

        var result = new List<PathNode>();
        foreach (var pos in neighbourPositions)
        {
            if (GetTilePassable(pos))
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
