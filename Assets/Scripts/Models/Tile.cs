using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Value { get; set; }
    public bool Visitied { get; set; }
    public float Chance { get; set; }

    public void Draw(GameObject Parent, Material material)
    {
        // Tile name
        string Name = $"x{X}y{Y}";

        // Create game object
        GameObject Game_Object = new GameObject(Name);

        // Create mesh, filter, and renderer
        Mesh mesh = new Mesh();
        MeshFilter Mesh_Filter = Game_Object.AddComponent(typeof(MeshFilter)) as MeshFilter;
        MeshRenderer Renderer = Game_Object.AddComponent<MeshRenderer>();
        MeshCollider Collider = Game_Object.AddComponent<MeshCollider>();

        // Set game object details
        Game_Object.transform.parent = Parent.transform;
        Game_Object.transform.position = new Vector3(X, 0, -Y);

        // Set filter's mesh
        Mesh_Filter.mesh = mesh;

        // Set renderer's material
        Renderer.material = material;

        // Funciton variables
        Vector3[] TileVertices;
        int[] TileTriangles;

        TileVertices = GetTileVertices().ToArray();
        TileTriangles = DrawTriangles(TileVertices);

        mesh.Clear();

        mesh.vertices = TileVertices;
        mesh.triangles = TileTriangles;

        Collider.sharedMesh = mesh;
        Collider.convex = true;
    }

    private List<Vector3> GetTileVertices()
    {
        List<Vector3> tileVertices = new List<Vector3>();

        // The Y is swapped to negative to correct the placement of the tiles to reflect the layout of the original generation.
        float yNegative = Y * -1;
        float xOffset = X + 2;
        float yOffset = yNegative - 2;

        tileVertices.Add(new Vector3(xOffset, 0, yNegative));   // Top Right
        tileVertices.Add(new Vector3(X, 0, yNegative));         // Top Left
        tileVertices.Add(new Vector3(X, 0, yOffset));           // Bottom Left
        tileVertices.Add(new Vector3(xOffset, 0, yOffset));     // Bottom Right

        return tileVertices;
    }

    private int[] DrawTriangles(Vector3[] tileVertices)
    {
        int triangleAmount = tileVertices.Length - 2;
        List<int> triangles = new List<int>();

        for (int i = 0; i < triangleAmount; i++)
        {
            triangles.Add(0);
            triangles.Add(i + 2);
            triangles.Add(i + 1);
        }

        return triangles.ToArray();
    }
}
