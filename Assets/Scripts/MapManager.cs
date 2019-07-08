using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Node: IComparable<Node>
{
    #region 变量
    public int x;
    public int y;
    public bool canWalk;

    public Node preNode;

    public int fValue;
    public int gValue;
    public int hValue;
    #endregion
    public Node(int x, int y, Node preNode, bool canWalk = true, int endX = 2, int endY = 2)
    {
        this.x = x;
        this.y = y;
        this.canWalk = canWalk;
        this.preNode = preNode;
        if (preNode != null)
            gValue = preNode.gValue + 1;
        else
            gValue = 0;
        hValue = Mathf.Abs(x - endX) + Mathf.Abs(y - endY);
        fValue = gValue + hValue;
    }
    public void ChangeParent(Node parent)
    {
        preNode = parent;
        gValue = parent.gValue + 1;
        fValue = gValue + hValue;
    }
    public Node(int x, int y, bool canWalk, Node preNode, int endX = 2, int endY = 2)
    {
        this.x = x;
        this.y = y;
        this.canWalk = canWalk;
        this.preNode = preNode;
        if (preNode != null)
            gValue = preNode.gValue + 1;
        else
            gValue = 0;
        hValue = Mathf.Abs(x - endX) + Mathf.Abs(y - endY);
        fValue = gValue + hValue;
    }

    public int CompareTo(Node other)
    {
        return this.fValue.CompareTo(other.fValue);
    }
}

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    public List<Button> btnList = new List<Button>();
    private GameObject[] contents;
    public Text tips;

    public int[,] room = new int[6, 11]
    {
        {0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,-1,0,0,0,0,0,0,0},
        {0,0,1,0,0,0,0,0,0,0,0},
        {0,0,-1,-1,0,0,0,2,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0},
    };
    [SerializeField]
    public GameObject[,] floors = new GameObject[6,11]; 
    void RefreshMap()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var t = transform.GetChild(i);
            for (int j = 0; j < t.childCount; j++)
            {
                floors[i, j] = t.GetChild(j).gameObject;
                floors[i, j].transform.Find("G").GetComponent<Text>().text = "";
                floors[i, j].transform.Find("H").GetComponent<Text>().text = "";
                floors[i, j].transform.Find("F").GetComponent<Text>().text = "";
                floors[i, j].transform.Find("Direct").gameObject.SetActive(false);
                if (room[i, j] == -1)
                {
                    t.GetChild(j).GetComponent<Image>().color = Color.gray;
                }else if (room[i, j] == 1)
                {
                    t.GetChild(j).transform.Find("Text").GetComponent<Text>().text = "终点";
                    t.GetChild(j).GetComponent<Image>().color = new Color(0,1,1);
                    t.GetChild(j).gameObject.GetComponent<Button>().enabled = false;
                }else if(room[i,j] == 0)
                {
                    t.GetChild(j).GetComponent<Image>().color = Color.white;
                }
                else if(room[i,j] == 2)
                {
                    t.GetChild(j).transform.Find("Text").GetComponent<Text>().text = "起点";
                    t.GetChild(j).GetComponent<Image>().color = Color.red;
                    t.GetChild(j).gameObject.GetComponent<Button>().enabled = false;
                }
                var btnS = t.GetChild(j).GetComponent<Button>();
                if (btnS != null)
                {
                    btnS.gameObject.name = i+","+j;
                    btnList.Add(btnS);
                }
            }
        }
    }
    void Start()
    {
        instance = this;
        RefreshMap();
        foreach (var btn in btnList)
        {
            btn.onClick.AddListener(() => { OnClick(btn);});
        }
    }
    public void ChangeNodeVisited(Node node)
    {
        if (node.preNode == null) return;
        int x, y, gValue, hValue, fValue;
        x = node.x;
        y = node.y;
        gValue = node.gValue;
        hValue = node.hValue;
        fValue = node.fValue;
        var gb = floors[x, y];
        gb.transform.Find("G").GetComponent<Text>().text = gValue + "";
        gb.transform.Find("H").GetComponent<Text>().text = hValue + "";
        gb.transform.Find("F").GetComponent<Text>().text = fValue + "";
        var dir = gb.transform.Find("Direct");
        if(room[x,y] == 0)  dir.gameObject.SetActive(true);
        if (node.preNode.x == node.x)
        {
            dir.transform.rotation = Quaternion.Euler(0, 0, (node.preNode.y - node.y) * 90);
        }
        else if (node.preNode.y == node.y)
        {
            if (node.preNode.x < node.x) dir.transform.rotation = Quaternion.Euler(0, 0, 180);
            else dir.transform.rotation = Quaternion.identity;
        }
        else
        {
            if (node.preNode.x > node.x && node.preNode.y > node.y) { }
            else if (node.preNode.x < node.x && node.preNode.y < node.y) { }
            else if (node.preNode.x < node.x && node.preNode.y > node.y) { }
            else { }

        }
        if (room[x, y] == 0) gb.GetComponent<Image>().color = new Color(1, 1, 132f / 255);
    }
    public void ChangeItem(Node node)
    {
        if (node.preNode == null) return;
        int x,  y,  gValue,  hValue,  fValue;
        x = node.x;
        y = node.y;
        gValue = node.gValue;
        hValue = node.hValue;
        fValue = node.fValue;
        var gb = floors[x, y];
        gb.transform.Find("G").GetComponent<Text>().text = gValue + "";
        gb.transform.Find("H").GetComponent<Text>().text = hValue + "";
        gb.transform.Find("F").GetComponent<Text>().text = fValue + "";
        var dir = gb.transform.Find("Direct");
        if (room[x, y] == 0) dir.gameObject.SetActive(true);
        if (node.preNode.x == node.x)
        {
            dir.transform.rotation = Quaternion.Euler(0, 0, (node.preNode.y - node.y)* 90);
        }else if(node.preNode.y == node.y)
        {
            if (node.preNode.x < node.x) dir.transform.rotation = Quaternion.Euler(0, 0, 180);
            else dir.transform.rotation = Quaternion.identity;
        }
        else
        {
            if (node.preNode.x > node.x && node.preNode.y > node.y) { }
            else if (node.preNode.x < node.x && node.preNode.y < node.y) { }
            else if (node.preNode.x < node.x && node.preNode.y > node.y) { }
            else { }
            
        }
        if (room[x, y] == 0) gb.GetComponent<Image>().color = new Color(184f / 255, 1, 141f / 255);
    }

    void OnClick(Button button)
    {
        var info = button.name.Split(',');
        int x = int.Parse(info[0]);
        int y = int.Parse(info[1]);
        if (room[x, y] == -1) room[x, y] = 0;
        else if (room[x, y] == 0) room[x, y] = -1;
        RefreshMap();
        return;
        //FindPath.path.BeginDepthFirstSearch(int.Parse(info[0]), int.Parse(info[1]));
        
    }
    bool flag = false;
    public Dropdown dropdown;
    public void BeginSearch(Text text)
    {
         
        flag = !flag;
        if (flag)
        {
            switch (dropdown.value)
            {
                case 0:
                    DepthFirstSearch.instance.BeginSerch(3, 7, 2, 2);
                    break;
                case 1:
                    DepthSerachHigh.instance.BeginSerch(3, 7, 2, 2);
                    break;
                case 2:
                    BreadthFirstSearch.instance.BeginSerch(3, 7, 2, 2);
                    break;
                case 3:
                    AstarSearch.instance.BeginAstarSearch(3, 7, 2, 2);
                    break;
            }
            foreach (var btn in btnList)
            {
                btn.enabled = false;
            }
            text.text = "刷新";
        }
        else
        {
            text.text = "开始寻路";
            tips.text = "";
            foreach (var btn in btnList)
            {
                btn.enabled = true;
            }
            RefreshMap();
        }
    }
}
