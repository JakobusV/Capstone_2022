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
}