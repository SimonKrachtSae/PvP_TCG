using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CustomImage : MonoBehaviour
{

    [SerializeField] private Vector2Int m_NumberOfTexturesPerRow;

    [SerializeField, Tooltip("front, top, back, left, right,bottom")] public List<int> m_NumberOfTexture;

    private List<Vector2> m_UVs;

    public int X_Index;
    public int Y_Index;
    public int Z_Index;

    public Vector3 Position;

    private Vector3 FrontTopLeft;
    private Vector3 FrontTopRight;
    private Vector3 FrontBottomLeft;
    private Vector3 FrontBottomRight;

    private List<Vector3> Vertices = new List<Vector3>();
    private List<int> Indices = new List<int>();

    [SerializeField] private bool generateMesh;
    [SerializeField] private MeshFilter meshFiter;
    private void Awake()
    {
    }
    private void OnValidate()
    {
        FrontTopLeft = Vector3.up * 10f + Vector3.left * 7f;
        FrontTopRight = Vector3.up * 10f + Vector3.right * 7f;
        FrontBottomLeft = Vector3.down * 10f + Vector3.left * 7f;
        FrontBottomRight = Vector3.down * 10f + Vector3.right * 7f;
        if (generateMesh)
        {
            generateMesh = false;
            GenerateMesh();
        }
    }
    public void GenerateMesh()
    {
        Position = new Vector3(X_Index, Y_Index, Z_Index);

        Vertices = new List<Vector3>();
        Indices = new List<int>();
        m_UVs = new List<Vector2>();

        //Front
        Vertices.Add(FrontBottomRight);
        Vertices.Add(FrontTopRight);
        Vertices.Add(FrontTopLeft);
        Vertices.Add(FrontBottomLeft);
        CalculateIndices();
        CalculateUVs(m_NumberOfTexture[0]);

        Mesh mesh = new Mesh();

        mesh.vertices = Vertices.ToArray();
        mesh.triangles = Indices.ToArray();

        mesh.uv = m_UVs.ToArray();

        mesh.RecalculateNormals();

        meshFiter.mesh = mesh;
        
    }
    private void CalculateUVs(int textureNumber)
    {
        float uSize = 1.0f / m_NumberOfTexturesPerRow.x;
        float vSize = 1.0f / m_NumberOfTexturesPerRow.y;

        float vStart = (textureNumber / m_NumberOfTexturesPerRow.x) * uSize;
        float uStart = (textureNumber % m_NumberOfTexturesPerRow.x) * vSize;

        m_UVs.Add(new Vector2(uStart, vStart));
        m_UVs.Add(new Vector2(uStart, vStart + vSize));
        m_UVs.Add(new Vector2(uStart + uSize, vStart + vSize));
        m_UVs.Add(new Vector2(uStart + uSize, vStart));
    }
    private void CalculateIndices()
    {
        int count = Vertices.Count;

        Indices.Add(count - 4);
        Indices.Add(count - 3);
        Indices.Add(count - 2);

        Indices.Add(count - 2);
        Indices.Add(count - 1);
        Indices.Add(count - 4);
    }
}
