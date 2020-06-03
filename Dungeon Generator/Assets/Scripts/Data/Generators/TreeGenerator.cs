using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TreeGenerator : MonoBehaviour
{
    public List<GameObject> Segments; //The first element is the top, and then it gets thicker then higher the index is
    //0 = 4, 1 = 6tr, 2 = 6, 3 = 8tr, 4 = 8, 5 = 10tr, 6 = 10
    public List<GameObject> Bases; //Same as above, but it contains only bases
    public List<GameObject> Branches; //Also increases in size with index

    public class TreePart
    {
        public List<GameObject> Segments = new List<GameObject>(){};
    }
    public struct TreeData
    {
        public TreeData(TreeType tt, Vector2Int th, Vector2Int bl, Vector2Int wdth, float brtend)
        {
            myType = tt;
            minmaxTrunkHeight = th;
            minmaxBranchLength = bl;
            branchThreshold = minmaxTrunkHeight.x;
            minmaxWidth = wdth;
            tendencyToBranch = brtend;
        }
        public enum TreeType
        {
            Apple = 0,//Drops apples, of which can be made juice, pies, sauces, apple butter, salads
            Oak = 1, //Good for house building, boat building and furniture, also for wine barrels and such
            Maple = 2, //Good for making maple syrup, violins to cellos, and their leaves can be fried
            Spruce = 3, //Good for building houses, its fresh shoots can be eaten and used as ingredient, can be used for violins to cellos, guitars, pianos and harps
        }
        public Vector2Int minmaxTrunkHeight;
        public Vector2Int minmaxBranchLength; //Apple grows a short trunk, and then splits into multiple branches that reach upwards. 
        //Oak does the same, but its trunk is longer. 
        //Maple too, but is even longer, but much thinner, so the trunk height is high but branch length is short
        public Vector2Int minmaxWidth;

        public int branchThreshold;

        public float tendencyToBranch;
        TreeType myType;
    }
    [SerializeField]TreeData.TreeType treeToGenerate;

    private void Start() 
    {
        GenerateTree(treeToGenerate);
    }
    void GenerateTree(TreeData.TreeType type)
    {
        GameObject Tree = new GameObject(type + " tree");
        TreeData tree = data[(int)type];
        TreePart trunk = CreateTrunk((int)Random.Range(tree.minmaxTrunkHeight.x, tree.minmaxTrunkHeight.y), (int)Random.Range(tree.minmaxWidth.x, tree.minmaxWidth.y), 10, Tree.transform);
        Branch(trunk.Segments[Random.Range(tree.branchThreshold, trunk.Segments.Count)]); //Obviously not going to be the final code
        //Because this implies the branches could start at the base of the trunk
        //There should be some kind of loop that uses the branchtendency... but im not sure what yet
        //CreateLeaves(type);
    }
    TreePart CreateTrunk(int trunkHeigth, int width, int totalHeight, Transform treeTrans)
    {
        //width corresponds to index[width - 3]
        float trunkDivision = (float)width/(float)totalHeight;
        float currentTrunkIndex = trunkDivision;
        TreePart trunk = new TreePart();
        trunk.Segments.Add(Instantiate(Bases[0], Vector3.zero, Quaternion.identity, treeTrans));
        for(int i = 1; i <= trunkHeigth; i++)
        {
            Debug.Log(currentTrunkIndex);
            currentTrunkIndex+=trunkDivision;
            Debug.Log(width-3-(int)Mathf.Floor(currentTrunkIndex));
            trunk.Segments.Add(Instantiate(Segments[width-3-(int)Mathf.Floor(currentTrunkIndex)], 
            new Vector3(trunk.Segments[0].transform.position.x, trunk.Segments[0].transform.position.y+(trunk.Segments.Count*1.6f), trunk.Segments[0].transform.position.z),
            Quaternion.identity, treeTrans));
        }
        return trunk;
    }
    TreePart Branch(GameObject origin)
    {
        //The origin is one of the segments inside the origin treepart
        //Branch from the given segment in another direction
        //Return the entire branch
        return null;
    }

    void CreateLeaves(TreeData.TreeType type)
    {
        Leaf prefab = leaves[(int) type];
    }
}
