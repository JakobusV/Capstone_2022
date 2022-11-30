using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class Grid
{
    // Height = Tiles.GetLength(1);
    // Width = Tiles.GetLength(0);
    public Tile[,] Tiles { get; set; }
    public GameObject gameObject { get; set; }

    public Grid()
    {
        Tiles = new Tile[16, 16];

        GenerateGameObject();
    }
    public Grid(int Width, int Height)
    {
        // Create Grid
        Tiles = new Tile[Width, Height];

        // Populate Grid
        for (int i = 0; i < Tiles.Length; i++)
        {
            int X = i % Width;
            int Y = i / Height;
            Tile tile = new Tile()
            {
                X = X,
                Y = Y
            };
            Tiles[Y, X] = tile;
        }

        GenerateGameObject();
    }
    public Grid(Tile[,] Tiles)
    {
        this.Tiles = Tiles;

        GenerateGameObject();
    }

    public void Build(Texture2D Texture, Algorithm algorithm)
    {
        // Get size of grid
        int LengthX = Tiles.GetLength(1);
        int LengthY = Tiles.GetLength(0);

        // Variables for tracking progress
        int VerticesIndex = -1;
        int NormalIndex = 0;
        int TriangleCount = 0;

        // Variables for building the mesh
        List<Vector3> TileVertices = new List<Vector3>();
        List<int> TileTriangles = new List<int>();
        Vector2[] UVs = new Vector2[LengthX * LengthY];
        Vector3[] TileNormals;
        int HeightMultiplier = -15;

        // Set HeightMultiplier
        if (algorithm.GetType() == typeof(DS))
        {
             HeightMultiplier = -10;
        }

        // Get Verticies
        for (int X = 0; X < LengthX; X++)
        {
            for (int Y = 0; Y < LengthY; Y++)
            {
                // Reached a vertex, add to index
                VerticesIndex++;

                if (X > 0 && Y > 0)
                {
                    // Upper left triangle
                    TileTriangles.Add(VerticesIndex - LengthX);
                    TileTriangles.Add(VerticesIndex - LengthX - 1);
                    TileTriangles.Add(VerticesIndex - 1);

                    // Lower right triangle
                    TileTriangles.Add(VerticesIndex - LengthX);
                    TileTriangles.Add(VerticesIndex - 1);
                    TileTriangles.Add(VerticesIndex);
                }

                // Add Vertex
                TileVertices.Add(new Vector3(Y, Tiles[Y, X].Value * HeightMultiplier, X));

                // Add UV
                UVs[VerticesIndex] = new Vector2(Y / (float)LengthY, X / (float)LengthX);
            }
        }

        // Get Normals
        TriangleCount = TileTriangles.Count / 3;
        TileNormals = new Vector3[TileVertices.Count];
        for (int i = 0; i < TriangleCount; i++)
        {
            // Get Triangle index from current normal index
            int VertexIndexA = TileTriangles[NormalIndex];
            NormalIndex =+ 1;
            int VertexIndexB = TileTriangles[NormalIndex];
            NormalIndex =+ 1;
            int VertexIndexC = TileTriangles[NormalIndex];
            NormalIndex =+ 1;

            // Get Vertices
            Vector3 PointA = TileVertices[VertexIndexA];
            Vector3 PointB = TileVertices[VertexIndexB];
            Vector3 PointC = TileVertices[VertexIndexC];

            // Get the two sides of the triangle
            Vector3 SideAB = PointB - PointA;
            Vector3 SideAC = PointC - PointA;

            // Create Cross product of the two Vectors
            Vector3 Normal = Vector3.Cross(SideAB, SideAC).normalized;

            // Save Normals
            TileNormals[VertexIndexA] += Normal;
            TileNormals[VertexIndexB] += Normal;
            TileNormals[VertexIndexC] += Normal;
        }

        // Create new mesh
        Mesh mesh = new Mesh();

        // Create components for the mesh and attach them
        MeshFilter Filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        MeshRenderer Renderer = gameObject.AddComponent<MeshRenderer>();
        MeshCollider Collider = gameObject.AddComponent<MeshCollider>();

        // Attach mesh to filter
        Filter.mesh = mesh;

        // Create texture for the mesh
        //Texture2D CustomTexture = TextureGeneration.ValueMapTexture(Tiles);
        Texture2D CustomTexture = Texture;

        // Apply variables to mesh
        mesh.vertices = TileVertices.ToArray();
        mesh.triangles = TileTriangles.ToArray();
        mesh.uv = UVs;
        mesh.normals = TileNormals;

        mesh.RecalculateNormals();

        // Generate Collider
        Collider.sharedMesh = mesh;

        // Render mesh
        Renderer.material.mainTexture = CustomTexture;
        Renderer.transform.localScale = new Vector3(1, 1, 1);
        // Renderer.transform.localScale = new Vector3(CustomTexture.width, (CustomTexture.width + CustomTexture.height) / 2, CustomTexture.height);

        // Set layer to Ground
        gameObject.layer = 6;
    }

    public void Display_Tiles(GameObject gameObject, Material material)
    {
        for (int X = 0; X < Tiles.GetLength(1); X++)
        {
            for (int Y = 0; Y < Tiles.GetLength(0); Y++)
            {
                if (Tiles[Y, X].Value > 0)
                {
                    Tiles[Y, X].Draw(gameObject, material);
                }
            }
        }
    }
    private void GenerateGameObject()
    {
        gameObject = new GameObject();
        gameObject.name = "Grid";
        gameObject.transform.position = new Vector3(0, 0, 0);
    }
    public void GenerateGameObject(Vector3 position)
    {
        gameObject = new GameObject();
        gameObject.name = "Grid";
        gameObject.transform.position = position;
    }
    public void GenerateRandomGrid()
    {
        System.Random Random = new System.Random();

        for (int i = 0; i < Tiles.Length; i++)
        {
            int X = i % Tiles.GetLength(1);
            int Y = i / Tiles.GetLength(0);
            Tile tile = new Tile()
            {
                X = X,
                Y = Y,
                Value = Random.Next(2)
            };
            Tiles[Y, X] = tile;
        }
    }

    /// <summary>
    /// 2^N grid generation. Default is 2^7, add an int for override.
    /// </summary>
    public void GenerateTwoNSquareGrid()
    {
        int N = 7;

        int Size = (int)Math.Pow(2, N);

        Size++;

        Tiles = new Tile[Size, Size];

        // Populate Grid
        for (int i = 0; i < Tiles.Length; i++)
        {
            int X = i % Size;
            int Y = i / Size;
            Tile tile = new Tile()
            {
                X = X,
                Y = Y
            };
            Tiles[Y, X] = tile;
        }
    }

    /// <summary>
    /// 2^N grid generation. Default is 2^7, leave out the int for default.
    /// </summary>
    /// <param name="N">Grid will be 2^N</param>
    public void GenerateTwoNSquareGrid(int N)
    {
        int Size = (int)Math.Pow(2, N);

        Size++;

        Tiles = new Tile[Size, Size];

        // Populate Grid
        for (int i = 0; i < Tiles.Length; i++)
        {
            int X = i % Size;
            int Y = i / Size;
            Tile tile = new Tile()
            {
                X = X,
                Y = Y
            };
            Tiles[Y, X] = tile;
        }
    }
}
