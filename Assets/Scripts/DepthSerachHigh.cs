using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthSerachHigh : MonoBehaviour
{
    int[,] room;
    public int[,] move = new int[4, 2]
    {
        {-1, 0}, {1, 0}, {0, -1}, {0, 1},//{-1,-1},{-1,1},{1,-1},{1,1},
    };
    int[,] state = new int[6, 11];
    public static DepthSerachHigh instance;

    void Start()
    {
        instance = this;
        room = MapManager.instance.room;
        for (int i = 0; i < state.GetLength(0); i++)
        {
            for (int j = 0; j < state.GetLength(1); j++)
            {
                state[i, j] = 100;
            }
        }
    }

    Stack st = new Stack();
    private void Reset()
    {
        st.Clear();
        for (int i = 0; i < state.GetLength(0); i++)
        {
            for (int j = 0; j < state.GetLength(1); j++)
            {
                state[i, j] = 100;
            }
        }
    }
    public void BeginSerch(int x, int y, int endX, int endY)
    {
        Reset();
        Node beginNode = new Node(x, y, null);
        FindNextNode(beginNode, endX, endY);
        state[x, y] = 0;
        var result = sNode;
        if (result == null || result.x != endX || result.y != endY)
        {
            MapManager.instance.tips.text = "当前无解！";
        }
        else
        {
            var node = sNode;
            while (node.preNode != null)
            {
                MapManager.instance.ChangeItem(node);
                node = node.preNode;
            }
        }
    }
    Node sNode;
    void FindNextNode(Node beginNode, int endX, int endY)
    {
        int x = beginNode.x;
        int y = beginNode.y;
        st.Push(beginNode);
        if (x == endX && y == endY) sNode = beginNode;
        for (int i = 0; i < move.GetLength(0); i++)
        {
            #region 下一个移动坐标
            int moveX = x + move[i, 0];
            int moveY = y + move[i, 1];
            if (moveX < 0 || moveX >= room.GetLength(0) || moveY < 0 || moveY >= room.GetLength(1)) continue;
            #endregion

            //可以走，且gvalue会减少 gValue当前gValue的大小
            int gValue = state[moveX, moveY];
            int compareValue = beginNode.gValue + 1;
            if (room[moveX, moveY] != -1 && gValue > compareValue)
            {
                state[moveX, moveY] = compareValue;
                Node node = new Node(moveX, moveY, beginNode);
                MapManager.instance.ChangeNodeVisited(node);
                FindNextNode(node, endX, endY);
            }
        }
        //该位置无法移动，死胡同
        st.Pop();
    }
}
