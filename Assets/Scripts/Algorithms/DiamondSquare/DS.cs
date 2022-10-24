using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class DS : Algorithm
{
    public System.Random Random { get; set; } = new System.Random();
    public int[] Difference { get; set; }
    public int[] Height { get; set; }
    public int FocusSize { get; set; }
    public int HalfSize { get; set; }
    public Tile[,] Tiles { get; set; }

    public DS()
    {
        SetRandomRange(8);

        Height = new int[16];

        for (int i = 0; i < Height.Length; i++)
        {
            Height[i] = i + 1;
        }
    }

    public Tile[,] Run()
    {
        // Set corners
        SetCorners();

        // Track Size
        FocusSize = Tiles.GetLength(1) - 1;

        // While Size > 1
        while (FocusSize > 1)
        {
            // Set Half
            HalfSize = FocusSize / 2;

            // Diamond step
            DiamondStep();

            // Square Step
            SquareStep();

            // Update Size
            FocusSize /= 2;

            // Update Random
            SetRandomRange(Difference[Difference.Length - 1] / 2);
        }

        // DEBUG
        CellularAutomataCleanUp();

        return Tiles;
    }

    private void DiamondStep()
    {
        for (int y = 0; y < Tiles.GetLength(1) - 1; y += FocusSize)
        {
            for (int x = 0; x < Tiles.GetLength(1) - 1; x += FocusSize)
            {
                int Average = Tiles[y, x].Value +
                    Tiles[y, x + FocusSize].Value +
                    Tiles[y + FocusSize, x].Value +
                    Tiles[y + FocusSize, x + FocusSize].Value;

                Average /= 4;

                Average += Difference[Random.Next(Difference.Length)];

                Average = FloorValue(Average);

                Tiles[y + HalfSize, x + HalfSize].Value = Average;
            }
        }
    }

    private void SquareStep()
    {
        for (int y = 0; y < Tiles.GetLength(1); y += HalfSize)
        {
            for (int x = (y + HalfSize) % FocusSize; x < Tiles.GetLength(1); x += FocusSize)
            {
                int Average = GetAverageValues(y, x);

                Average += Difference[Random.Next(Difference.Length)];

                Average = FloorValue(Average);

                Tiles[y, x].Value = Average;
            }
        }
    }

    private void SetCorners()
    {
        Tiles[0, 0].Value = Random.Next(Height.Length) + 1;
        Tiles[0, Tiles.GetLength(1) - 1].Value = Random.Next(Height.Length) + 1;
        Tiles[Tiles.GetLength(1) - 1, 0].Value = Random.Next(Height.Length) + 1;
        Tiles[Tiles.GetLength(1) - 1, Tiles.GetLength(1) - 1].Value = Random.Next(Height.Length) + 1;
    }

    private void SetRandomRange(int difference)
    {
        Difference = new int[difference * 2 + 1];
        int differenceIndex = 0;
        for (int i = difference * -1; i < difference + 1; i++)
        {
            Difference[differenceIndex] = i;
            differenceIndex++;
        }
    }

    private int GetAverageValues(int y, int x)
    {
        int Count = 0;
        int Sum = 0;
        if (y - HalfSize > -1)
        {
            Sum += Tiles[y - HalfSize, x].Value;
            Count++;
        }
        if (x - HalfSize > -1)
        {
            Sum += Tiles[y, x - HalfSize].Value;
            Count++;
        }
        if (x < Tiles.GetLength(1) - HalfSize) // Right
        {
            Sum += Tiles[y, x + HalfSize].Value;
            Count++;
        }
        if (y < Tiles.GetLength(1) - HalfSize) // Below
        {
            Sum += Tiles[y + HalfSize, x].Value;
            Count++;
        }

        Sum /= Count;

        return Sum;
    }

    private int FloorValue(int Value)
    {
        if (Value < Height[0])
        {
            Value = Height[0];
        }
        else if (Value > Height[Height.Length - 1])
        {
            Value = Height[Height.Length - 1];
        }

        return Value;
    }

    private void CellularAutomataCleanUp()
    {
        CellularAutomata_Simple cas = new CellularAutomata_Simple();
        cas.Tiles = Tiles;

        for (int Y = 0; Y < Tiles.GetLength(1); Y++)
        {
            for (int X = 0; X < Tiles.GetLength(0); X++)
            {
                // Adjacent 8 tiles
                List<Tile> MooreN = cas.MooreNeighborhood(Tiles[Y, X]);

                // Collection for tile values and amount of
                IDictionary<int, int> dict = new Dictionary<int, int>();

                foreach (Tile tile in MooreN)
                {
                    // Check for key
                    if (dict.ContainsKey(tile.Value))
                    {
                        // Increment value
                        dict[tile.Value]++;
                    } else
                    {
                        // Create key in dict
                        dict.Add(tile.Value, 1);
                    }
                }

                // Sort by amount then by key (aka tile value) then apply that key to the current tiles value
                Tiles[Y, X].Value = dict.OrderByDescending(kv => kv.Value).ThenByDescending(kv => kv.Key).First().Key;
            }
        }
    }

    public string GetFileType()
    {
        return ".dsq";
    }

    public Tile[,] Read(string file)
    {
        // Get relative file path
        StringBuilder filePath = new StringBuilder("Saves/");
        filePath.Append(file);
        filePath.Append(GetFileType());

        // Collection of all lines on file
        string[] Rows = File.ReadAllLines(filePath.ToString());

        // Track Y for tiles setup
        for (int Y = 0; Y < Rows.Length; Y++)
        {
            // Row in Rows
            string Row = Rows[Y];

            // Collection of all values in row
            string[] RowValues = Row.Split(",");

            // Track X for tiles setup
            for (int X = 0; X < RowValues.Length; X++)
            {
                if (X == 0 && Y == 0)
                {
                    Tiles = new Tile[Rows.Length, RowValues.Length];
                }

                // StrValue in RowValues
                string StrValue = RowValues[X];

                // Convert Value from string to int
                int Value = int.Parse(StrValue);

                Tiles[Y, X] = new Tile()
                {
                    X = X,
                    Y = Y,
                    Value = Value,
                };
            }
        }

        return Tiles;
    }

    public void Write(string file)
    {
        StringBuilder filePath = new StringBuilder("Saves/");
        filePath.Append(file);
        filePath.Append(GetFileType());

        StringBuilder stringBuilder = new StringBuilder();

        for (int Y = 0; Y < Tiles.GetLength(1); Y++)
        {
            if (Y != 0)
            {
                stringBuilder.Append("\n");
            }

            for (int X = 0; X < Tiles.GetLength(0); X++)
            {
                if (X != 0)
                {
                    stringBuilder.Append(",");
                }

                stringBuilder.Append(Tiles[Y, X].Value.ToString());
            }
        }

        File.WriteAllText(filePath.ToString(), stringBuilder.ToString());
    }
}
