using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMeshTile : MonoBehaviour
{
    Mesh mesh;
    public Vector3[] TileVertices;
    public int[] TileTriangles;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        DrawFilled();

        MeshCollider mc = GetComponent<MeshCollider>();
        mc.sharedMesh = mesh;
        mc.convex = true;
    }

    void Update()
    {
    }

    private void DrawFilled()
    {
        TileVertices = GetTileVertices().ToArray();
        TileTriangles = DrawTriangles(TileVertices);

        mesh.Clear();

        mesh.vertices = TileVertices;
        mesh.triangles = TileTriangles;
    }

    private int[] DrawTriangles(Vector3[] tileVertices)
    {
        int triangleAmount = tileVertices.Length - 2;
        List<int> triangles = new List<int>();

        for (int i = 0; i < triangleAmount; i++)
        {
            triangles.Add(0);
            triangles.Add(i+2);
            triangles.Add(i+1);
        }
        
        return triangles.ToArray();
    }

    private List<Vector3> GetTileVertices()
    {
        List<Vector3> tileVertices = new List<Vector3>();

        tileVertices.Add(new Vector3(1.0f, 0, 0.0f)); // Top Right
        tileVertices.Add(new Vector3(0.0f, 0, 0.0f)); // Top Left
        tileVertices.Add(new Vector3(0.0f, 0, 1.0f)); // Bottom Left
        tileVertices.Add(new Vector3(1.0f, 0, 1.0f)); // Bottom Right

        return tileVertices;
    }
}
