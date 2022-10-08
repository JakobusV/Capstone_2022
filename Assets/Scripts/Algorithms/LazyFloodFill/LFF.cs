using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class LFF
{
    public System.Random Random { get; set; } = new System.Random();
    public int Height { get; set; }
    public int Width { get; set; }
    public float Chance { get; set; }
    public float Decay { get; set; }
    public Tile[,] Grid { get; set; } = new Tile[0, 0];
    public Stack<Tile> Deque { get; set; } = new Stack<Tile>();
    public bool Debug { get; set; }

    public void Run(int X, int Y)
    {
        Tile startTile = Grid[Y, X];

        startTile.Visitied = true;

        startTile.Chance = Chance;

        Deque.Push(startTile);

        LazyFloodFill();
    }

    private void LazyFloodFill()
    {
        //int DEBUG_COUNTROUNDS = 0;

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

            /*if (DEBUG_COUNTROUNDS % 10 == 0 && Debug)
            {
                ConsoleIO.PrintGrid(Grid);
            }

            DEBUG_COUNTROUNDS++;*/
        }
    }

    private List<Tile> GetNeighbors(Tile tile)
    {
        List<Tile> neightbors = new List<Tile>();

        if (tile.Y > 0)
        {
            neightbors.Add(Grid[tile.Y - 1, tile.X]);
        }
        if (tile.X > 0)
        {
            neightbors.Add(Grid[tile.Y, tile.X - 1]);
        }
        if (tile.Y < Height - 1)
        {
            neightbors.Add(Grid[tile.Y + 1, tile.X]);
        }
        if (tile.X < Width - 1)
        {
            neightbors.Add(Grid[tile.Y, tile.X + 1]);
        }

        return neightbors;
    }

    public void Display(GameObject gameObject, Material material)
    {
        for (int X = 0; X < Grid.GetLength(1); X++)
        {
            for (int Y = 0; Y < Grid.GetLength(0); Y++)
            {
                if (Grid[Y, X].Value > 0)
                {
                    Grid[Y, X].Draw(gameObject, material);
                }
            }
        }
    }

    public void Write(string file)
    {
        StringBuilder filePath = new StringBuilder("Saves/");
        filePath.Append(file);
        filePath.Append(".txt");

        // SB to construct file
        StringBuilder fileBuilder = new StringBuilder();
        // SB to construct bytes
        StringBuilder byteBuilder = new StringBuilder();
        // Keep track of byte length
        int eightCount = 0;

        for (int Y = 0; Y < Grid.GetLength(0); Y++)
        {
            // Reset eightCount
            eightCount = 0;

            // Check to see if it's the first line
            if (Y != 0)
            {
                // Append line delimiter
                fileBuilder.Append("\n");
            }

            for (int X = 0; X < Grid.GetLength(1); X++)
            {
                // If amount of bits is too small, keep going
                if (eightCount < 8)
                {
                    // Increment and add value
                    eightCount++;
                    byteBuilder.Append(Grid[X, Y].Value);

                // Else if byte length is fulfilled, write it
                } else
                {
                    // Convert and append
                    fileBuilder.Append(Convert.ToInt32(byteBuilder.ToString(), 2).ToString());

                    // Reset byteBuilder
                    byteBuilder.Clear();

                    // Append current value
                    byteBuilder.Append(Grid[X, Y].Value);

                    // Reset count
                    eightCount = 1;

                    // Check for more data
                    if (X != Grid.GetLength(1) - 1)
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
            }
        }

        // Write out file
        File.WriteAllText(filePath.ToString(), fileBuilder.ToString());


        /*StringBuilder stringBuilder = new StringBuilder();
        for (int X = 0; X < Grid.GetLength(1); X++)
        {
            for (int Y = 0; Y < Grid.GetLength(0); Y++)
            {
                stringBuilder.Append(Grid[Y, X].Value.ToString());
            }
            stringBuilder.Append("\n");
        }
        File.WriteAllText(file, stringBuilder.ToString());*/
    }

    public void Read(string file)
    {
        // SB to generate file path
        StringBuilder filePath = new StringBuilder("Saves/");
        filePath.Append(file);
        filePath.Append(".txt");

        // Generate encoding to convert int to binary
        Encoding encoding = new UTF8Encoding();
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

        // New set of now converted rows
        Rows = gridBuilder.ToString().Split('\n');

        // Setup for LFF object
        Height = Rows.Length;
        Width = Rows[0].Length;
        Grid = new Tile[Rows[0].Length, Rows.Length]; // Could be backwards? 

        // Loop through Rows
        for (int Y = 0; Y < Rows.Length; Y++)
        {
            // Loop through columns
            for (int X = 0; X < Rows[Y].Length; X++)
            {
                // Get value from string
                int Value = Convert.ToInt32(Rows[Y][X]);

                // Setup tile
                Grid[Y, X] = new Tile() 
                { 
                    X = X,
                    Y = Y,
                    Value = Value
                };
            }
        }
    }
}
