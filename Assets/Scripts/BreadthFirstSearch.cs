using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadthFirstSearch : MonoBehaviour
{
    public Queue queue = new Queue();
    public int[,] room;
    public int[,] move = new int[4, 2]
    {
        {-1, 0}, {1, 0}, {0, -1}, {0, 1},//{-1,-1},{-1,1},{1,-1},{1,1},
    };
    public int[,] state = new int[6, 11];
    public Node endNode;
    public static BreadthFirstSearch instance;
    private void Start()
    {
        instance = this;
        room = MapManager.instance.room;
    }
    private void Reset()
    {
        queue.Clear();
        endNode = null;
        for (int i = 0; i < state.GetLength(0); i++)
        {
            for (int j = 0; j < state.GetLength(1); j++)
            {
                state[i, j] = 0;
            }
        }
    }
    public void BeginSerch(int x, int y, int endX, int endY)
    {
        Reset();
        Node beginNode = new Node(x, y, null);
        state[x, y] = -1;
        queue.Enqueue(beginNode);
        FindNextNode(endX, endY);
        if (endNode == null || endNode.x != endX || endNode.y != endY)
        {
            MapManager.instance.tips.text = "当前无解！";
        }
        else
        {
            while (endNode.preNode != null)
            {
                MapManager.instance.ChangeItem(endNode);
                endNode = endNode.preNode;
            }
        }
    }
    void FindNextNode(int endX, int endY)
    {
        if (queue.Count == 0) return;
        var beginNode = (Node)queue.Dequeue();
        int x = beginNode.x;
        int y = beginNode.y;
        for (int i = 0; i < move.GetLength(0); i++)
        {
            #region 下一个移动坐标
            int moveX = x + move[i, 0];
            int moveY = y + move[i, 1];
            if (moveX < 0 || moveX >= room.GetLength(0) || moveY < 0 || moveY >= room.GetLength(1)) continue;
            #endregion
            //如果可以加入队列
            if(room[moveX,moveY]!=-1 && state[moveX,moveY] == 0)
            {
                Node node = new Node(moveX, moveY, beginNode);
                state[moveX, moveY] = -1;
                queue.Enqueue(node);
                MapManager.instance.ChangeNodeVisited(node);
                if (moveX==endX&&moveY == endY)
                {
                    endNode = node;
                    return;
                }
            }
            if (endNode != null)
            {
                return;
            }
        }
        FindNextNode(endX,endY);
    }
}