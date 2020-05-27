using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

    public bool walkable;
    public Vector3 worldPosition;
    public int gCost, hcost, gridX, gridY;
    public Node parent;

    public Node(bool __Walkable, Vector3 __WorldPosition, int __gridX, int __gridY){
        walkable = __Walkable;
        worldPosition = __WorldPosition;
        gridX = __gridX;
        gridY = __gridY;
    }

    //fcost is the sum of the g+h costs
    public int fCost {
        get{
            return gCost + hcost;
        }
    }

    void Start () {
		
	}
	
}
