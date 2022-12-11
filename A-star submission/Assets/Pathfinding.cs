using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Pathfinding : MonoBehaviour
{
   public Transform seeker, target;
   private Grid grid;
   private List<Node> path;
   private Node targetNode;
   private void Awake()
   {
      grid = GetComponent<Grid>();
      
   }

   private void Update()
   {
      FindPath(seeker.position, target.position);

      if (Input.GetMouseButtonDown(0))
      {
         
         Vector3 mouse = Input.mousePosition;
         Ray castPoint = Camera.main.ScreenPointToRay(mouse);
         RaycastHit hit;
         if (Physics.Raycast(castPoint, out hit, Mathf.Infinity))
         {
            if(grid.NodeFromWorldPoint(hit.point).walkable)
               target.position = hit.point;
         }
      }
      
      if (path[0] != targetNode)
      {
         seeker.position = Vector3.Lerp(seeker.position, path[1].WorldPostion, Time.deltaTime * 10);
      }
   }

   private void FindPath(Vector3 startPos, Vector3 targetPos)
   {
      Node startNode = grid.NodeFromWorldPoint(startPos);
      targetNode = grid.NodeFromWorldPoint(targetPos);
      
      List<Node> openSet = new List<Node>();
      HashSet<Node> closeSet = new HashSet<Node>();
      openSet.Add(startNode);

      while (openSet.Count > 0)
      {
         Node currentNode = openSet[0];
         for (int i = 1; i < openSet.Count; i++)
            if (openSet[i].fCost < currentNode.fCost ||
                openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
               currentNode = openSet[i];

         openSet.Remove(currentNode);
         closeSet.Add(currentNode);

         if (currentNode == targetNode)
         {
            RetracePath(startNode, targetNode);
            return;
         }

         foreach (Node neighbour in grid.GetNeigbours(currentNode))
         {
            if (!neighbour.walkable || closeSet.Contains(neighbour))
            {
               continue;
            }

            int newMovementCostToNeighbour = currentNode.gCost + Getdistance(currentNode, neighbour);
            if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
            {
               neighbour.gCost = newMovementCostToNeighbour;
               neighbour.hCost = Getdistance(neighbour, targetNode);
               neighbour.parent = currentNode;

               if (!openSet.Contains(neighbour))
                  openSet.Add(neighbour);

            }
         }
         
      }
   }

   private void RetracePath(Node startNode, Node endNode)
   {
      path = new List<Node>();
      Node currentnode = endNode;

      while (currentnode != startNode)
      {
         path.Add(currentnode);
         currentnode = currentnode.parent;
      }
      path.Add(currentnode);
      path.Reverse();

      grid.path = path;
   }
   int Getdistance(Node nodeA, Node nodeB)
   {
      int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
      int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

      if (dstX > dstY)
         return 14 * dstY + 10 * (dstX - dstY);
      return 14 * dstX + 10 * (dstY - dstX);
   }
   
}