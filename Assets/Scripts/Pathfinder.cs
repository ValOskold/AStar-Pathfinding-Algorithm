using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour {
    Grid grid;
    public Transform seeker;
    public Transform target;

    void Awake(){
        //fetching a script
        grid = GetComponent<Grid>();
    }

    void FindPath(Vector3 startPos, Vector3 targetPos) {
        //find where the seeker is and where the object being seeked is
        Node startNode = grid.GetNodeFromWorldPoint(startPos);
        Node targetNode = grid.GetNodeFromWorldPoint(targetPos);

        //create open and closed set that will hold the nodes 
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        //give the open set the starting node
        openSet.Add(startNode);

        //loop through all the items in the open set
        while (openSet.Count > 0){
            //set refrence for loop
            Node currentNode = openSet[0];
            //iterate through the openset
            for (int i = 1; i < openSet.Count; i++){
                //find a better path then set that
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hcost < currentNode.hcost){
                    currentNode = openSet[i];
                }//end of if
            }//end of for
            //after we have the lowest Fcost we remove the node from the open list and add it to the closed list
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            //check to see if we have found our path
            if (currentNode == targetNode) {
                //if so retrace a new path then return
                RetracePath(startNode, targetNode);
                return;
            }
            //check to see what neighbours are walkable or considered closed, when we find an unwalkable path we then disregard it
            foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                //check to see if there is a path to the neighbour that is shoerter, or if the neighbour is closed
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                    //to set the f cost update g cost and h cost
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hcost = GetDistance(neighbour, targetNode);
                    //set the neighbours parent
                    neighbour.parent = currentNode;

                    //if this node is not already updated, update it
                    if (!openSet.Contains(neighbour)) {
                        openSet.Add(neighbour);

                    }//end of if
                } //end of if
            }//end of foreach
        }//end of while
    }

    int GetDistance(Node nodeA, Node nodeB){
        //find distance values
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        //if x is greater than y then 14*y and 10*x-y else 14x and 10*y-x
        if  (dstX > dstY) {
            return 14 * dstY + 10 * (dstX - dstY);
        }//end of if
        return 14 * dstX + 10 * (dstY - dstX);
    }

    void RetracePath(Node __startNode, Node __endNode) {
        //create a path to retrace
        List<Node> path = new List<Node>();
        //find the end of the path and search backwards
        Node currentNode = __endNode;
        //traverse backwards
        while (currentNode != __startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }//end of while
        //because our path is backwards, reverse to get a forward path
        path.Reverse();

        grid.path = path;
    }

    void Update() {
        //make sure that we updated the path for any objects that may have moved last frame
        FindPath(seeker.position, target.position);
    }

}//end of class
