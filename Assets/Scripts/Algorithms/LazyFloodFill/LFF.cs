using Assets.Scripts.PlayerControls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class LFF : Algorithm
{
    public System.Random Random { get; set; } = new System.Random();
    public Tile[,] Tiles { get; set; }
    public float Chance { get; set; } = 100f;
    public float Decay { get; set; } = .998f;
    public Stack<Tile> Deque { get; set; } = new Stack<Tile>();
    private int[] Start { get; set; }

    public Tile[,] Run(int X, int Y)
    {
        Start = new int[2] { Y, X };

        return Run();
    }
    public Tile[,] Run(int Both_X_Y)
    {
        Start = new int[2] { Both_X_Y, Both_X_Y };

        return Run();
    }
    public Tile[,] Run()
    {
        if (Start == null)
        {
            Start = new int[2] { Tiles.GetLength(0) / 2, Tiles.GetLength(1) / 2 };
        }

        Tile startTile = Tiles[Start[0], Start[1]];

        startTile.Visitied = true;

        startTile.Chance = Chance;

        Deque.Push(startTile);

        LazyFloodFill();

        return Tiles;
    }

    private void LazyFloodFill()
    {
        while (Deque.Count > 0)
        {
            // Pop and Fill Tile from Deque
            Tile focusTile = Deque.Pop();
            focusTile.Value = 1;

            // If Chance >= Random
            if (focusTile.Chance >= Random.Next(0, 101))
            {
                // Get neighbors not visited
                List<Tile> neighbors = GetNeighbors(focusTile);

                // Select all that are not visited
                neighbors = neighbors.Where(t => !t.Visitied).ToList();

                // Decrease Chance by Decay factor
                Chance = focusTile.Chance * Decay;

                // For each neighbor
                foreach (Tile tile in neighbors)
                {
                    // Add neighbors to Deque
                    Deque.Push(tile);

                    // Set neighbors as "Visited"
                    tile.Visitied = true;

                    // Set Chance as current chance
                    tile.Chance = Chance;
                }
            }
        }
    }

    private List<Tile> GetNeighbors(Tile tile)
    {
        List<Tile> neightbors = new List<Tile>();

        if (tile.Y > 0)
        {
            neightbors.Add(Tiles[tile.Y - 1, tile.X]);
        }
        if (tile.X > 0)
        {
            neightbors.Add(Tiles[tile.Y, tile.X - 1]);
        }
        if (tile.Y < Tiles.GetLength(0) - 1)
        {
            neightbors.Add(Tiles[tile.Y + 1, tile.X]);
        }
        if (tile.X < Tiles.GetLength(1) - 1)
        {
            neightbors.Add(Tiles[tile.Y, tile.X + 1]);
        }

        return neightbors;
    }

    public Tile[,] Read(string file)
    {
        // SB to generate file path
        StringBuilder filePath = new StringBuilder(PlayerStatus.GetSavePath());
        filePath.Append(file);
        filePath.Append(GetFileType());

        // SB to generate grid
        StringBuilder gridBuilder = new StringBuilder();

        // collection of all lines on file
        string[] Rows = File.ReadAllLines(filePath.ToString());

        // Loop through lines
        foreach (string Row in Rows)
        {
            // Split on .
            string[] RowBytes = Row.Split('.');

            // Loop through each section
            foreach (string RowByte in RowBytes)
            {
                // Convert string to int
                int Int = int.Parse(RowByte) + 256;

                // Convert int to binary
                string Binary = Convert.ToString(Int, 2);

                // Convert binary to string and append gb
                gridBuilder.Append(Binary.Substring(1));
            }

            // Append newline delimiter
            gridBuilder.Append("\n");
        }

        // Remove final newline
        gridBuilder.Remove(gridBuilder.Length - 1, 1);

        // New set of now converted rows
        Rows = gridBuilder.ToString().Split('\n');

        // Setup for LFF object
        Tiles = new Tile[Rows[0].Length, Rows.Length]; // Could be backwards? 

        // Loop through Rows
        for (int Y = 0; Y < Rows.Length; Y++)
        {
            // Loop through columns
            for (int X = 0; X < Rows[Y].Length; X++)
            {
                // Get value from char --> double --cast-> int
                int Value = (int)char.GetNumericValue(Rows[X][Y]);

                // Setup tile
                Tiles[Y, X] = new Tile()
                {
                    X = X,
                    Y = Y,
                    Value = Value
                };
            }
        }

        return Tiles;
    }

    public void Write(string file)
    {
        StringBuilder filePath = new StringBuilder(PlayerStatus.GetSavePath());
        filePath.Append(file);
        filePath.Append(GetFileType());

        // SB to construct file
        StringBuilder fileBuilder = new StringBuilder();
        // SB to construct bytes
        StringBuilder byteBuilder = new StringBuilder();
        // Keep track of byte length
        int eightCount = 0;

        for (int Y = 0; Y < Tiles.GetLength(0); Y++)
        {
            // Reset eightCount
            eightCount = 0;

            // Check to see if it's the first line
            if (Y != 0)
            {
                // Append line delimiter
                fileBuilder.Append("\n");
            }

            for (int X = 0; X < Tiles.GetLength(1); X++)
            {
                // If amount of bits is too small, keep going
                if (eightCount < 8)
                {
                    // Increment and add value
                    eightCount++;
                    byteBuilder.Append(Tiles[X, Y].Value);

                    // Else if byte length is fulfilled, write it
                }
                else
                {
                    // Convert and append
                    fileBuilder.Append(Convert.ToInt32(byteBuilder.ToString(), 2).ToString());

                    // Reset byteBuilder
                    byteBuilder.Clear();

                    // Append current value
                    byteBuilder.Append(Tiles[X, Y].Value);

                    // Reset count
                    eightCount = 1;

                    // Check for more data
                    if (X != Tiles.GetLength(1) - 1)
                    {
                        // Apply delimiter
                        fileBuilder.Append(".");
                    }
                }

            }

            // If the count is not 0, create leftovers as 0's to finish last byte
            if (eightCount > 0)
            {
                // Loop amount of leftovers
                for (int leftover = eightCount; leftover < 8; leftover++)
                {
                    // Append 0
                    byteBuilder.Append("0");
                }

                // Append final byte
                fileBuilder.Append(Convert.ToInt32(byteBuilder.ToString(), 2).ToString());

                // Reset byteBuilder
                byteBuilder.Clear();
            }
        }

        // Write out file
        File.WriteAllText(filePath.ToString(), fileBuilder.ToString());
    }

    public string GetFileType()
    {
        return ".lff";
    }
}
