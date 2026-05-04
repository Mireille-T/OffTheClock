using System.Collections.Generic;
using UnityEngine;

namespace OffTheClock.Combat
{
    // Grid-based A* pathfinder for the mecha arena.
    // Place one of these GameObjects at the arena center; configure worldSize to cover the floor.
    // EnemyAI calls Pathfinder.Instance.FindPath(start, end) to get a list of waypoints.
    [DefaultExecutionOrder(-100)]
    public class Pathfinder : MonoBehaviour
    {
        public static Pathfinder Instance { get; private set; }

        [Header("Grid")]
        public Vector2 worldSize = new Vector2(60f, 60f);
        public float nodeRadius = 0.5f;
        public LayerMask obstacleMask;
        public bool drawGizmos = false;

        private Node[,] _grid;
        private int _gridX, _gridY;
        private float _nodeDiameter;
        private Vector3 _origin;

        void Awake()
        {
            Instance = this;
            _nodeDiameter = nodeRadius * 2f;
            _gridX = Mathf.Max(1, Mathf.RoundToInt(worldSize.x / _nodeDiameter));
            _gridY = Mathf.Max(1, Mathf.RoundToInt(worldSize.y / _nodeDiameter));
            BuildGrid();
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        void BuildGrid()
        {
            _origin = transform.position - new Vector3(worldSize.x * 0.5f, 0f, worldSize.y * 0.5f);
            _grid = new Node[_gridX, _gridY];
            for (int x = 0; x < _gridX; x++)
            {
                for (int y = 0; y < _gridY; y++)
                {
                    Vector3 wp = _origin + new Vector3(x * _nodeDiameter + nodeRadius, 0f, y * _nodeDiameter + nodeRadius);
                    bool walkable = !Physics.CheckSphere(wp, nodeRadius * 0.9f, obstacleMask);
                    _grid[x, y] = new Node(walkable, wp, x, y);
                }
            }
        }

        Node NodeFromWorld(Vector3 wp)
        {
            Vector3 rel = wp - _origin;
            int x = Mathf.Clamp(Mathf.FloorToInt(rel.x / _nodeDiameter), 0, _gridX - 1);
            int y = Mathf.Clamp(Mathf.FloorToInt(rel.z / _nodeDiameter), 0, _gridY - 1);
            return _grid[x, y];
        }

        IEnumerable<Node> Neighbors(Node n)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    int nx = n.gridX + dx;
                    int ny = n.gridY + dy;
                    if (nx < 0 || nx >= _gridX || ny < 0 || ny >= _gridY) continue;
                    yield return _grid[nx, ny];
                }
            }
        }

        public List<Vector3> FindPath(Vector3 from, Vector3 to)
        {
            if (_grid == null) return null;

            Node start = NodeFromWorld(from);
            Node target = NodeFromWorld(to);
            if (!start.walkable || !target.walkable) return null;

            for (int x = 0; x < _gridX; x++)
                for (int y = 0; y < _gridY; y++)
                {
                    _grid[x, y].gCost = int.MaxValue;
                    _grid[x, y].hCost = 0;
                    _grid[x, y].parent = null;
                }

            var open = new List<Node> { start };
            var closed = new HashSet<Node>();
            start.gCost = 0;
            start.hCost = Heuristic(start, target);

            while (open.Count > 0)
            {
                int bestIdx = 0;
                int bestF = open[0].FCost;
                for (int i = 1; i < open.Count; i++)
                {
                    int f = open[i].FCost;
                    if (f < bestF || (f == bestF && open[i].hCost < open[bestIdx].hCost))
                    {
                        bestIdx = i;
                        bestF = f;
                    }
                }
                Node current = open[bestIdx];
                open.RemoveAt(bestIdx);
                closed.Add(current);

                if (current == target) return Retrace(start, target);

                foreach (var n in Neighbors(current))
                {
                    if (!n.walkable || closed.Contains(n)) continue;
                    int tentativeG = current.gCost + Heuristic(current, n);
                    if (tentativeG < n.gCost)
                    {
                        n.gCost = tentativeG;
                        n.hCost = Heuristic(n, target);
                        n.parent = current;
                        if (!open.Contains(n)) open.Add(n);
                    }
                }
            }
            return null;
        }

        static int Heuristic(Node a, Node b)
        {
            int dx = Mathf.Abs(a.gridX - b.gridX);
            int dy = Mathf.Abs(a.gridY - b.gridY);
            // Octile distance: 14 diagonal, 10 cardinal
            return dx > dy ? 14 * dy + 10 * (dx - dy) : 14 * dx + 10 * (dy - dx);
        }

        static List<Vector3> Retrace(Node start, Node end)
        {
            var path = new List<Vector3>();
            var cur = end;
            while (cur != start)
            {
                path.Add(cur.worldPos);
                cur = cur.parent;
            }
            path.Reverse();
            return path;
        }

        void OnDrawGizmosSelected()
        {
            if (!drawGizmos) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, new Vector3(worldSize.x, 0.1f, worldSize.y));
            if (_grid == null) return;
            foreach (var n in _grid)
            {
                Gizmos.color = n.walkable ? new Color(0, 1, 0, 0.2f) : new Color(1, 0, 0, 0.4f);
                Gizmos.DrawCube(n.worldPos, Vector3.one * (_nodeDiameter * 0.9f));
            }
        }

        class Node
        {
            public readonly bool walkable;
            public readonly Vector3 worldPos;
            public readonly int gridX, gridY;
            public int gCost, hCost;
            public Node parent;
            public int FCost => gCost + hCost;
            public Node(bool walkable, Vector3 worldPos, int gx, int gy)
            {
                this.walkable = walkable;
                this.worldPos = worldPos;
                this.gridX = gx;
                this.gridY = gy;
            }
        }
    }
}
