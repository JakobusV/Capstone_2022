using Assets.Scripts.PlayerControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BSPR : Algorithm
{
    private float Deviation { get; set; }
    private int SplitIterations { get; set; }
    public Tile[,] Tiles { get; set; }

    public BSPR()
    {
        SplitIterations = 4; // 3
        Deviation = 0.1f;
    }

    public string GetFileType()
    {
        return ".bsp";
    }

    public Tile[,] Run()
    {
        int[] Origin = { 0, 0 };

        BinarySpacePartitioningNode bspNode = new BinarySpacePartitioningNode(Tiles, SplitIterations, Origin);
        bspNode.Perform();

        return bspNode.Data;
    }

    public Tile[,] Read(string file)
    {
        // Get relative file path
        StringBuilder filePath = new StringBuilder(PlayerStatus.GetSavePath());
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
        StringBuilder filePath = new StringBuilder(PlayerStatus.GetSavePath());
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