using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarSearch : MonoBehaviour
{
    #region 地图与移动相关，节点保存
    public List<Node> openNodes = new List<Node>();
    public int[,] move = new int[4, 2]
    {
        {-1, 0}, {1, 0}, {0, -1}, {0, 1}
    };
    private int[,] room;

    #endregion

    #region 获取附近节点

    public bool CanMove(Node parent, Node child)
    {
        if (child.gValue > parent.gValue + 1)
        {
            child.gValue = parent.gValue + 1;
            child.preNode = parent;
            MapManager.instance.ChangeItem(child);
            return true;
        }
        return false;
    }

    public void JudgeNearNodes(Node node)
    {
        for (int i = 0; i < move.GetLength(0); i++)
        {
            int moveX = node.x + move[i, 0];
            int moveY = node.y + move[i, 1];

            if (moveX < 0 || moveX >= room.GetLength(0) || moveY < 0 || moveY >= room.GetLength(1)) continue;

            if (room[moveX, moveY] == -1) continue;

            Node temp = null;

            foreach (var openNode in openNodes)
            {
                if (openNode.x == moveX && openNode.y == moveY)
                {
                    temp = openNode;
                    break;
                }
            }
            if (temp == null)
            {
                var newNode = new Node(moveX, moveY, true, node);
                MapManager.instance.ChangeNodeVisited(newNode);
                openNodes.Add(newNode);
            }
            else
            {
                int nowG = node.gValue + 1;
                int preG = temp.gValue;
                if(nowG<preG)
                {
                    temp.ChangeParent(node);
                }
            }
        }
    }
    #endregion
    public void FindResult(int endX,int endY)
    {
        openNodes.Sort();
        Node node = openNodes[0];
        if (node.x == 2 && node.y == 2)
        {
            endNode = node;
            return;
        }
        node.canWalk = false;

        openNodes.Remove(node);

        JudgeNearNodes(node);
        FindResult(endX,endY);
    }
    private void Reset()
    {
        endNode = null;
        openNodes.Clear();
    }
    Node endNode = null;
    public void BeginAstarSearch(int x, int y,int endX,int endY)
    {
        Reset();
        Node begiNode = new Node(x, y, false, null);
        openNodes.Add(begiNode);
        FindResult(endX, endY);

        if(endNode == null)
        {
            MapManager.instance.tips.text = "当前无解!";
        }
        else 
        {
            while (endNode.preNode != null)
            {
                endNode = endNode.preNode;
                MapManager.instance.ChangeItem(endNode);
            }
        }
    }
    public static AstarSearch instance;
    void Start()
    {
        instance = this;
        room = MapManager.instance.room;
    }
}