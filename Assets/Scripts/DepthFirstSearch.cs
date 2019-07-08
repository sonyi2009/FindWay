using System.Collections;
using UnityEngine;


public class DepthFirstSearch : MonoBehaviour
{
    int[,] room;
    public int[,] move = new int[4, 2]
    {
        {-1, 0}, {1, 0}, {0, -1}, {0, 1}
    };
    int[,] state = new int[6, 11];
    public static DepthFirstSearch instance;

    void Start()
    {
        instance = this;
        room = MapManager.instance.room;
    }

    Stack st = new Stack();
    private void Reset()
    {
        st.Clear();
        for(int i = 0; i < state.GetLength(0); i++)
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
        FindNextNode(beginNode, endX, endY);
        Node result = null;
        if(st.Count!=0) result = (Node)st.Peek();
        if (result == null || result.x != endX || result.y != endY)
        {
            MapManager.instance.tips.text = "当前无解！";
        }
        else
        {
            while (st.Count != 1)
            {
                var node = (Node)st.Pop();
                MapManager.instance.ChangeItem(node);
            }
        }
    }
    void FindNextNode(Node beginNode, int endX, int endY)
    {
        int x = beginNode.x;
        int y = beginNode.y;
        state[x, y] = -1;
        st.Push(beginNode);
        for (int i = 0; i < move.GetLength(0); i++)
        {
            #region 下一个移动坐标
            int moveX = x + move[i, 0];
            int moveY = y + move[i, 1];
            if (moveX < 0 || moveX >= room.GetLength(0) || moveY < 0 || moveY >= room.GetLength(1)) continue;
            #endregion
           
            //没到终点则进行判断是不是可以走，走过的也不用走
            if (room[moveX, moveY] != -1 && state[moveX, moveY] == 0)
            {
                Node node = new Node(moveX, moveY, beginNode);
                MapManager.instance.ChangeNodeVisited(node);
                FindNextNode(node, endX, endY);
            }

            //必须放在行走之后，假设放在行走之前，此时之前递归到最后一层for，即为i==move.GetLength-1,会跳出for循环剔除不该删除的路径节点
            Node firstNode = (Node)st.Peek();
            if (firstNode.x == endX && firstNode.y == endY) return;

        }
        //该位置无法移动，死胡同
        st.Pop();
    }
}
