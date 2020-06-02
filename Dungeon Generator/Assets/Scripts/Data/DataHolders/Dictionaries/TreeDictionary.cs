using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TreeGenerator
{
    public Leaf[] leaves;
    List<TreeData> data = new List<TreeData>
    {
        new TreeData(TreeData.TreeType.Apple, new Vector2Int(3,4), new Vector2Int(5, 8), new Vector2Int(8,8), 4),
        new TreeData(TreeData.TreeType.Oak, new Vector2Int(4,6), new Vector2Int(4,6),new Vector2Int(8,8), 4),
        new TreeData(TreeData.TreeType.Maple, new Vector2Int(5,6), new Vector2Int(4,6), new Vector2Int(8,8), 4),
        new TreeData(TreeData.TreeType.Spruce, new Vector2Int(10, 15), new Vector2Int(3, 4), new Vector2Int(8,8), 1)
    };
}