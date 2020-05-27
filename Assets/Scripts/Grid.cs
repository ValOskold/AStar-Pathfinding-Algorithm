using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {
    public Transform seeker;
    public Transform target;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    void Start() {
        //find the diameter for the nodes
        nodeDiameter = nodeRadius * 2;
        //find the grid size
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    void CreateGrid(){
        //create the grid object
        grid = new Node[gridSizeX, gridSizeY];
        //find a coord on the grid we that we can use to map other grid cells to the grid
        Vector3 worldBotLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        //iterate through all the positions on the X then Y axis of the grid also keep in mind Y is Z
        for (int x = 0; x < gridSizeX; x++) {
            for (int y = 0; y < gridSizeY; y++) {
                //create a point to test collision with, if the collision is true there is an obsticle in the path
                Vector3 worldPoint = worldBotLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                //set nodes either walkable or not depending on the bool test above
                grid[x, y] = new Node(walkable, worldPoint,x,y);
            }//end of sizeY for
        }//end of sizeX for
    }

    public List<Node> GetNeighbours(Node __node) {
        //create a list of possible cells that are adjacent to a target cell.
        List<Node> neighbours = new List<Node>();
        //iterate through the x and y positions of the grid (searching a 3 by 3 grid relative to a target cells positions)
        for(int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++)
            {
                //if we are in the center of the 3 by 3 then disregard it because that is our __node
                if (x == 0 && y == 0){
                    continue;
                }//end of if

                //if we are not at the center of the 3 by 3 grid but still within the 3x3 grid then add the neighbour to the datastructure. 
                    int checkX = __node.gridX + x;
                    int checkY = __node.gridY + y;

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
                        neighbours.Add(grid[checkX, checkY]);
                }//end of if
            }//end of for
        }//end of for

        //after we iterate through the grid and assign the correct neighbours to the __node then we return what we just did.
        return neighbours;
    }

    public Node GetNodeFromWorldPoint(Vector3 __WorldPosition){
        //this method finds nodes that belong to the grid in world space, this is used for drawing a debugger or for the acutal path finding method
        float percentX = (__WorldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (__WorldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    //create a path variable we set in the pathfinder script
    public List<Node> path;
    void OnDrawGizmos2(){
        //Draw the cells of the grid within the world space
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if (grid != null){
            ////iterate through all the cells of the grid
            foreach (Node n in grid){
            //for each walkable cell in the grid color them white, else they are red
            Gizmos.color = (n.walkable) ? Color.white : Color.red;
            if (path != null)
                {
                    if (path.Contains(n)) {
                        Gizmos.color = Color.magenta;
                    }
                }//end of if
            //after the color is assigned to each cell they can be drawn in their positions then spaced a tiny bit away from one another
            Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .05f));
            }//end of foreach
        }//end of if
    }

    void OnDrawGizmos() {
        //Draw the cells of the grid within the world space
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if (grid != null)
        {
            ////find the grid spot where the player is at
            Node seekerNode = GetNodeFromWorldPoint(seeker.position);
            Node targetNode = GetNodeFromWorldPoint(target.position);
            ////iterate through all the cells of the grid
            foreach (Node n in grid)
            {
                //for each walkable cell in the grid color them white, else they are red
                Gizmos.color = (n.walkable) ? Color.gray : Color.red;

                //color the a* path
                if (path != null)
                {
                    if (path.Contains(n))
                    {
                        Gizmos.color = Color.magenta;
                    }//end of if
                }//end of if

                //test to see where the seeker / target are then color their grid spot black or white
                if (seekerNode == n)
                {
                    Gizmos.color = Color.black;
                }
                if (targetNode == n)
                {
                    Gizmos.color = Color.white;
                }
                //after the color is assigned to each cell they can be drawn in their positions then spaced a tiny bit away from one another
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .05f));
            }//end of foreach
        }//end of if
    }

}//end of class
