using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSystem : MonoBehaviour
{
    public static TileSystem instance;
    //Load from Map Info
    int max_x = 36;
    int max_y = 20;
    //Node Grid for navigation
    public Node[,] grid;
    List<Vector3> path;
    //Highlights for UI
    public GameObject Highlight;
    List<GameObject> Highlighted;
    //LayerMask for Walls
    public LayerMask WallLayerMask;
    //Layermask for Marking occupied tiles
    public LayerMask UnitLayerMask;

    private void Awake()
    {
        instance = this;
        grid = new Node[max_x, max_y];
        //Generate Grid
        for (int i = 0; i < max_x; i++)
        {
            for (int j = 0; j < max_y; j++)
            {
                grid[i, j] = new Node(new Vector3(i - (max_x / 2), (j + 1) - (max_y / 2)), i, j);
                UpdateNode(grid[i, j], CheckNodeWall(grid[i, j].pos), NodeOccupied(grid[i,j].pos));
            }
        }
        Highlighted = new List<GameObject>();
    }

    private void Start()
    {

    }
    //Grid Node Class
    public class Node
    {
        public bool hasUnit;
        public bool isWall;
        public bool hasItem;
        //Navigation Values
        public float h_cost; //heuristic cost, idealized straight line distance value
        public float c_cost; //cumulative cost, cost to go from start to this node
        public float f_cost; //final cost, sum of h and g cost, the lower the better
        //World Space Position
        public Vector3 pos;
        public Node parent;
        //position in grid
        public int x;
        public int y;
        //Placeholder var
        public int value;
        public List<Node> Neighbors { get; set; }
        public Node(Vector3 position, int x_index, int y_index)
        {
            isWall = false;
            hasUnit = false;
            value = 0;
            h_cost = int.MaxValue;
            c_cost = 0;
            f_cost = 0;
            pos = position;
            x = x_index;
            y = y_index;
        }
    }

    //Node Search for Navigation
    public List<Node> CalculatePath(Node startNode, Node destNode)
    {
        List<Node> to_search = new List<Node>();
        List<Node> searched = new List<Node>();
        List<Node> node_path = new List<Node>();
        to_search.Add(startNode);

        //While loop search, Break condition
        while (to_search.Count > 0)
        {
            if (to_search.Count > 1000)
            {
                print("Search Depth too Large");
                break;
            }

            Node current = to_search[0];
            //check our searchable nodes
            //If final cost is less than our current, swap to other node
            //Or if final cost is the same but heuristic cost is less than current, swap to other node
            foreach (Node n in to_search)
            {
                if (n.f_cost < current.f_cost || (n.f_cost == current.f_cost && n.h_cost < current.h_cost))
                {
                    current = n;
                }
            }
            //once we've found the best current node, remove that node from our search
            searched.Add(current);
            to_search.Remove(current);

            //if the node we're on is the destination, add the parent nodes, clear changes and return our path
            if (current == destNode)
            {
                while (current.parent != null)
                {
                    node_path.Add(current);
                    current = current.parent;
                }

                ClearCosts();

                return node_path;
            }

            //Get our neighbors
            current.Neighbors = GetNeighbors(current);

            //For each Neighbor Node
            foreach (Node neighbor in current.Neighbors)
            {
                //Check if we've already searched it
                if (!searched.Contains(neighbor))
                {
                    //Check if we are planning to search it
                    bool searching = to_search.Contains(neighbor);

                    //Get the cost to get there from our current position
                    float neighbor_cost = current.c_cost + GetTileMovement(neighbor);

                    //make sure it's not an occupied node
                    if (neighbor.hasUnit)
                    {
                        neighbor_cost += 100;
                    }

                    //If we're not planning to search it or if the cost to get their is less than its previously assessed c cost
                    if (!searching || neighbor_cost < neighbor.c_cost)
                    {
                        //Update its c cost then update its f cost. Set its parent to our current node
                        neighbor.c_cost = neighbor_cost;
                        neighbor.f_cost = neighbor.c_cost + neighbor.h_cost;
                        neighbor.parent = current;

                        //then if we weren't searching through it before
                        if (!searching)
                        {
                            //recalc its cost based on its distance to destination and add it to our list to search through
                            neighbor.h_cost = Vector3.Distance(neighbor.pos, destNode.pos);
                            neighbor.f_cost = neighbor.c_cost + neighbor.h_cost;
                            to_search.Add(neighbor);
                        }
                    }
                }
            }
        }
        //if we don't return a path, log out a message
        print("No Suitable Path Found");
        return null;
    }

    //Create a vector3 list from positions given and Nodes returned from CalculatePath
    public List<Vector3> GetPath(Vector3 startpos, Vector3 destpos)
    {
        Node startNode = GetNode(startpos);
        Node destNode = GetNode(destpos);
        List<Node> nodes = CalculatePath(startNode, destNode);
        List<Vector3> path = new List<Vector3>();
        foreach (Node n in nodes)
        {
            path.Add(n.pos);
        }
        path.Reverse();
        foreach (Node n in grid)
        {
            n.parent = null;
        }
        return path;
    }

    //Get all existing neighbors left, right, up and down from current node
    List<Node> GetNeighbors(Node node)
    {
        List<Node> Neighbors = new List<Node>();
        if (node.x > 0)
        {
            Neighbors.Add(grid[node.x - 1, node.y]);
        }
        if (node.x + 1 < max_x)
        {
            Neighbors.Add(grid[node.x + 1, node.y]);
        }
        if (node.y > 0)
        {
            Neighbors.Add(grid[node.x, node.y - 1]);
        }
        if (node.y + 1 < max_y)
        {
            Neighbors.Add(grid[node.x, node.y + 1]);
        }
        return Neighbors;
    }

    //get node from worldspace position
    public Node GetNode(Vector3 pos)
    {
        int x = Mathf.RoundToInt(pos.x);
        int y = Mathf.RoundToInt(pos.y);
        if (x > (max_x / 2) || x < (-1 - max_x / 2) || y < (-max_y / 2) || y > (1 + max_y / 2))
        {
            return null;
        }
        return grid[x + (max_x / 2), (y - 1) + (max_y / 2)];
    }

    public void UpdateNode(Node n, bool isWall, bool isOccupied)
    {
        n.isWall = isWall;
        n.hasUnit = isOccupied;
    }

    public void UpdateAllNodes()
    {
        for (int i = 0; i < max_x; i++)
        {
            for (int j = 0; j < max_y; j++)
            {
                UpdateNode(grid[i, j], CheckNodeWall(grid[i, j].pos), NodeOccupied(grid[i, j].pos));
            }
        }
    }

    public bool CheckNodeWall(Vector3 pos)
    {
        Collider2D col = Physics2D.OverlapCircle(pos, .25f, WallLayerMask);
        if (col != null)
        {
            return true;
        }
        return false;
    }

    //If there's a unit on the tile, it's occupied
    public bool NodeOccupied(Vector3 pos)
    {
        Collider2D col = Physics2D.OverlapCircle(pos, .25f, UnitLayerMask);
        if (col != null)
        {
            return true;
        }
        return false;
    }

    //Move through the neighbors of our current tile until we're out of movement speed
    public List<Node> CalculateMovement(Vector3 start_pos, int distance)
    {
        //Where we start
        Node start = GetNode(start_pos);
        //Track which nodes we can move to
        List<Node> MovementNodes = new List<Node>();

        //List to iterate through
        List<Node> to_check = new List<Node>();
        to_check.Add(start);
        //Node to check for neighbors
        Node current = start;

        while (to_check.Count > 0)
        {
            //Get neighbors
            current.Neighbors = GetNeighbors(current);
            foreach (Node n in current.Neighbors)
            {
                //if it's not in our accessible nodes list and it's not the starting position
                if (!MovementNodes.Contains(n) && n != start)
                {
                    //get a movement cost based on the cost of our current plus the movement cost to the neighbor
                    n.f_cost = current.f_cost + GetTileMovement(n);
                    if (n.hasUnit)
                    {
                        n.f_cost += 100; //if occupied, inaccessible
                    }
                    //if the movement cost is lower or equal to our move speed, add it to accessible nodes and check its neighbors
                    if (n.f_cost <= distance)
                    {
                        MovementNodes.Add(n);
                        to_check.Add(n);
                    }
                }
            }
            to_check.Remove(current);

            if (to_check.Count > 0)
                current = to_check[0];
        }

        ClearCosts();

        return MovementNodes;
    }

    public List<Node> CalculateAttack(Vector3 start_pos, int distance)
    {
        List<Node> AttackNodes = new List<Node>();

        Node node = GetNode(start_pos);

        for (int i = 0; i < distance; i++)
        {
            //Left
            if (node.x - i > 0)
            {
                if (!grid[node.x - 1 - i, node.y].isWall)
                {
                    AttackNodes.Add(grid[node.x - 1 - i, node.y]);
                }

                //Left Down
                if(node.y - i > 0)
                {
                    if (!grid[node.x - 1 - i, node.y - 1 - i].isWall)
                    {
                        AttackNodes.Add(grid[node.x - 1 - i, node.y - 1 - i]);
                    }
                }

                //Left Up
                if (node.y + 1 + i < max_y)
                {
                    if (!grid[node.x - 1 - i, node.y + 1 + i].isWall)
                    {
                        AttackNodes.Add(grid[node.x - 1 - i, node.y + 1 + i]);
                    }
                }
            }

            //Right
            if (node.x + 1 + i < max_x)
            {
                if (!grid[node.x + 1 + i, node.y].isWall)
                {
                    AttackNodes.Add(grid[node.x + 1 + i, node.y]);
                }

                //Right Down
                if (node.y - i > 0)
                {
                    if (!grid[node.x + 1 + i, node.y - 1 - i].isWall)
                    {
                        AttackNodes.Add(grid[node.x + 1 + i, node.y - 1 - i]);
                    }
                }

                //Right Up
                if (node.y + 1 + i < max_y)
                {
                    if (!grid[node.x + 1 + i, node.y + 1 + i].isWall)
                    {
                        AttackNodes.Add(grid[node.x + 1 + i, node.y + 1 + i]);
                    }
                }
            }

            //Up
            if (node.y - i > 0)
            {
                if (!grid[node.x, node.y - 1 - i].isWall)
                {
                    AttackNodes.Add(grid[node.x, node.y - 1 - i]);
                }
            }

            //Down
            if (node.y + 1 + i < max_y)
            {
                if (!grid[node.x, node.y + 1 + i].isWall)
                {
                    AttackNodes.Add(grid[node.x, node.y + 1 + i]);
                }
            }
        }

        return AttackNodes;
    }

    //Create highlights for accessible movement nodes
    public void GenerateMovementHighlights(List<Node> nodes)
    {
        ClearHighlights();
        foreach (Node n in nodes)
        {
            GameObject obj = Instantiate(Highlight, n.pos, Quaternion.identity, transform);
            AttackSquare attack = obj.GetComponent<AttackSquare>();
            attack.enabled = false;
            Highlighted.Add(obj);
        }
    }

    public void GenerateAttackHighlights(List<Node> nodes)
    {
        ClearHighlights();
        foreach (Node n in nodes)
        {
            GameObject obj = Instantiate(Highlight, n.pos, Quaternion.identity, transform);
            MovementSquare movement = obj.GetComponent<MovementSquare>();
            movement.enabled = false;
            Highlighted.Add(obj);
        }
    }

    //Remove generated highlights
    public void ClearHighlights()
    {
        foreach (GameObject obj in Highlighted)
        {
            Destroy(obj);
        }
    }

    //remove all cost alterations from navigation
    void ClearCosts()
    {
        foreach (Node n in grid)
        {
            n.f_cost = 0;
            n.h_cost = 0;
            n.c_cost = 0;
        }
    }

    public int GetTileMovement(Node n)
    {
        if (n.isWall)
        {
            return 999;
        }
        else
        {
            return 1;
        }
    }

    public Unit GetUnit(Vector3 pos)
    {
        Node n = GetNode(pos);
        Collider2D col = Physics2D.OverlapCircle(n.pos, .25f, UnitLayerMask);
        if (col != null)
        {
            GameObject obj = col.gameObject;
            return obj.GetComponent<Unit>();
        }
        return null;
    }
}
