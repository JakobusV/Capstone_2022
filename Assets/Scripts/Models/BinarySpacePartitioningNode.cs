using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BinarySpacePartitioningNode
{
    public Tile[,] Data { get; set; }
    public BinarySpacePartitioningNode Left { get; set; } // A
    public BinarySpacePartitioningNode Right { get; set; }// B
    public int[] Center { get; set; }
    private int SplitTracking { get; set; } = 0;
    private int[] Origin { get; set; } = {0, 0};
    private Random Random { get; set; }

    public BinarySpacePartitioningNode(Tile[,] data, int splitTracking, int[] Origin)
    {
        Data = data;
        SplitTracking = splitTracking;
        this.Origin = Origin;
        Random = new Random();
    }

    public void Perform()
    {
        if (SplitTracking > 0 && canSplit())
        {
            SplitCurrentGrid2(Random.Next(2) == 0);

            Left.Perform();
                
            Right.Perform();

            ApplyToSelf();

            AttachCenters();
        } 
        else
        {
            CreateRoom();
        }
    }

    private void AttachCenters()
    {
        // Get Left Centers
        List<int[]> LeftCenters = Left.GetCenters();

        // Get Right Centers
        List<int[]> RightCenters = new List<int[]>();
        foreach (int[] center in Right.GetCenters())
        {
            RightCenters.Add(new int[] { center[0] + Right.Origin[0], center[1] + Right.Origin[1] });
        }

        // Tracking how short the shortest path is
        int ShortestPath = int.MaxValue;
        // Tracking which centers it came from
        int SP_LC = 0;
        int SP_RC = 0;
        // Tracking the index of each list
        int LC_Index = 0;
        int RC_Index = 0;

        // Start comparing paths
        foreach (int[] L_Center in LeftCenters)
        {
            RC_Index = 0;
            foreach (int[] R_Center in RightCenters)
            {
                // Get the distance between x's and y's
                int Y_distance = Math.Abs(L_Center[0] - R_Center[0]);
                int X_distance = Math.Abs(L_Center[1] - R_Center[1]);
                // Get how long the path is
                int PathValue = Y_distance + X_distance;

                // Compare it to the current shortest path
                if (PathValue < ShortestPath)
                {
                    // Set path and indexes
                    ShortestPath = PathValue;
                    SP_LC = LC_Index;
                    SP_RC = RC_Index;
                }

                RC_Index++;
            }
            LC_Index++;
        }

        // Draw Path from Y to Y (Left to Right)
        int X_Still = RightCenters[SP_RC][0];
        int Y_Start = Math.Min(LeftCenters[SP_LC][1], RightCenters[SP_RC][1]);
        int Y_End = Math.Max(LeftCenters[SP_LC][1], RightCenters[SP_RC][1]);
        for (int Y = Y_Start; Y < Y_End; Y++)
        {
            Data[Y, X_Still].Value = 1;
            try
            {
                Data[Y, X_Still + 1].Value = 1;
            }
            catch
            {
                Console.WriteLine("south_wall_hit");
            }
            try
            {
                Data[Y, X_Still - 1].Value = 1;
            }
            catch
            {
                Console.WriteLine("north_wall_hit");
            }
            if (Y + 1 == Y_End)
            {
                try
                {
                    Data[Y + 1, X_Still].Value = 1;
                    Data[Y + 1, X_Still + 1].Value = 1;
                    Data[Y + 1, X_Still - 1].Value = 1;
                }
                catch
                {
                    Console.WriteLine("last_push_wall_hit");
                }
            }
        }

        // Draw Path from X to X (Bottom to Top)
        int Y_Still = LeftCenters[SP_LC][1];
        int X_Start = Math.Min(LeftCenters[SP_LC][0], RightCenters[SP_RC][0]);
        int X_End = Math.Max(LeftCenters[SP_LC][0], RightCenters[SP_RC][0]);
        for (int X = X_Start; X < X_End; X++)
        {
            Data[Y_Still, X].Value = 1;
            try
            {
                Data[Y_Still + 1, X].Value = 1;
            }
            catch 
            {
                Console.WriteLine("east_wall_hit");
            }
            try
            {
                Data[Y_Start - 1, X].Value = 1;
            }
            catch 
            {
                Console.WriteLine("west_wall_hit");
            }
            if (X + 1 == X_End)
            {
                try
                {
                    Data[Y_Still, X + 1].Value = 1;
                    Data[Y_Still + 1, X + 1].Value = 1;
                    Data[Y_Still - 1, X + 1].Value = 1;
                }
                catch
                {
                    Console.WriteLine("last_push_wall_hit");
                }
            }
        }
    }

    /// <summary>
    /// When a node calls for it's centers, it will grab either it's center if there are no children.
    /// Or it will grab all centers within it's children's grid's.
    /// When getting grids from it's children, it will compensate for offset of their grid using their origins.
    /// </summary>
    /// <returns>All leaf node centers.</returns>
    private List<int[]> GetCenters()
    {
        List<int[]> Centers = new List<int[]>();

        if (Left == null || Right == null)
        {
            Centers.Add(Center);

            /*int[] TrueCenter = new int[] { Origin[0] + Center[0], Origin[1] + Center[1] };

            Centers.Add(TrueCenter);*/
        }
        else
        {
            // Left Centers don't need to be added to the origin
            // because the origin will always be { 0, 0 }
            Centers.AddRange(Left.GetCenters());

            // However Right has been displaced and will have a varring origin
            foreach (int[] R_Centers in Right.GetCenters())
            {
                int[] TrueCenter = new int[] { Right.Origin[0] + R_Centers[0], Origin[1] + R_Centers[1] };
                Centers.Add(TrueCenter);
            }
        }

        return Centers;
    }

    private void ApplyToSelf()
    {
        for (int A_Y = 0; A_Y < Left.Data.GetLength(0); A_Y++)
        {
            for (int A_X = 0; A_X < Left.Data.GetLength(1); A_X++)
            {
                Data[A_Y, A_X].Value = Left.Data[A_Y, A_X].Value;
            }
        }
        for (int B_Y = 0; B_Y < Right.Data.GetLength(0); B_Y++)
        {
            for (int B_X = 0; B_X < Right.Data.GetLength(1); B_X++)
            {
                Data[B_Y + Right.Origin[1], B_X + Right.Origin[0]].Value = Right.Data[B_Y, B_X].Value;
            }
        }
    }

    private void CreateRoom()
    {
        int[] Buffer_Array = CreateBuffer();

        int Y_StartBuffer = Buffer_Array[0];  // LEFT
        int X_StartBuffer = Buffer_Array[1];  // TOP
        int Y_EndBuffer = Buffer_Array[2];    // RIGHT
        int X_EndBuffer = Buffer_Array[3];    // BOTTOM
        for (int Y = 0 + Y_StartBuffer; Y < Data.GetLength(0) - Y_EndBuffer; Y++)
        {
            for (int X = 0 + X_StartBuffer; X < Data.GetLength(1) - X_EndBuffer; X++)
            {
                Data[Y, X].Value = 1;
            }
        }

        Center = new int[] { Data.GetLength(1) / 2, Data.GetLength(0) / 2 };
    }

    private int[] CreateBuffer()
    {
        int Y_Len = Data.GetLength(0);
        int X_Len = Data.GetLength(1);

        if (Y_Len * 3 < X_Len)
        {
            X_Len = Y_Len * 3;
        }
        if (X_Len * 3 < Y_Len)
        {
            Y_Len = X_Len * 3;
        }

        int Difference_Y = Data.GetLength(0) - Y_Len;
        int Difference_X = Data.GetLength(1) - X_Len;

        int Left = Difference_Y/2;
        int Top = Difference_X/2;
        int Right = Difference_Y/2;
        int Bottom = Difference_X/2;

        Left += GetRoomDeviation(Y_Len);
        Top += GetRoomDeviation(X_Len);
        Right += GetRoomDeviation(Y_Len);
        Bottom += GetRoomDeviation(X_Len);

        // Left, Top, Right, Bottom
        return new int[] { Left, Top, Right, Bottom };
    }

    private int GetRoomDeviation(int Len)
    {
        int DeviationMax = Len / 6;
        return Random.Next(DeviationMax + 1);
    }

    private void SplitCurrentGrid()
    {
        int ChildSplitTracking = SplitTracking - 1;

        // NOTE: THIS IS A SPLIT ACROSS THE Y
        // MEANING THAT THE ROOMS WILL COME OUT AS HORIZONTAL STRIPS

        // A Size is also the index of the split where no rooms are allowed
        int A_Size = Data.GetLength(1) / 2;
        // B Size should match A size. Minus 1 for a spot taken by the split
        // And plus 1 for a spot missed by division
        int B_Size = A_Size - 1 + (Data.GetLength(1) % 2);

        //if even length then minus one instead because split takes one 

        /// NOTE: A_Size is a length of the A's Grid however,
        /// A_Size also marks the INDEX in the grids array of
        /// where the split is, there should be no boxes that
        /// go over this area unless it is to connect the two

        // A Origin sits right where the current grid sits so nothing changes
        int[] A_Origin = Origin;
        // However the B Origin needs to start,
        // AFTER the split
        int[] B_Origin = new int[] { A_Size + 1, Origin[1] };

        /// NOTE: As stated before, A_Size is the length of 
        /// A's grid AND the INDEX of the split. Since this
        /// is true, we can simply add one to A_Size to get
        /// the INDEX of B's origin.

        // Side A
        Tile[,] A_Grid = CreateGrid(A_Size, Data.GetLength(0));
        Left = new BinarySpacePartitioningNode(A_Grid, ChildSplitTracking, A_Origin);

        // Side B
        Tile[,] B_Grid = CreateGrid(B_Size, Data.GetLength(0));
        Right = new BinarySpacePartitioningNode(B_Grid, ChildSplitTracking, B_Origin);
    }
    private void SplitCurrentGrid(bool SplitX)
    {
        // Setup
        int ChildSplitTracking = SplitTracking - 1;
        int LengthBeingSplit = SplitX ? Data.GetLength(0) : Data.GetLength(1);

        // NOTE: THIS IS A SPLIT ACROSS THE Y
        // MEANING THAT THE ROOMS WILL COME OUT AS HORIZONTAL STRIPS

        // A Size is also the index of the split where no rooms are allowed
        int A_Size = LengthBeingSplit / 2;
        // B Size should match A size. Minus 1 for a spot taken by the split
        // And plus 1 for a spot missed by division
        int B_Size = A_Size - 1 + (LengthBeingSplit % 2);

        //if even length then minus one instead because split takes one 

        /// NOTE: A_Size is a length of the A's Grid however,
        /// A_Size also marks the INDEX in the grids array of
        /// where the split is, there should be no boxes that
        /// go over this area unless it is to connect the two

        // A Origin sits right where the current grid sits so nothing changes
        int[] A_Origin = new int[] { 0, 0 };
        // However the B Origin needs to start,
        // AFTER the split
        int[] B_Origin = SplitX ? new int[] { 0, A_Size + 1 } : new int[] { A_Size + 1, 0 };

        /// NOTE: As stated before, A_Size is the length of 
        /// A's grid AND the INDEX of the split. Since this
        /// is true, we can simply add one to A_Size to get
        /// the INDEX of B's origin.

        // Side A
        Tile[,] A_Grid = SplitX ? CreateGrid(Data.GetLength(1), A_Size) : CreateGrid(A_Size, Data.GetLength(0));
        Left = new BinarySpacePartitioningNode(A_Grid, ChildSplitTracking, A_Origin);

        // Side B
        Tile[,] B_Grid = SplitX ? CreateGrid(Data.GetLength(1), B_Size) : CreateGrid(B_Size, Data.GetLength(0));
        Right = new BinarySpacePartitioningNode(B_Grid, ChildSplitTracking, B_Origin);
    }
    /// <summary>
    /// SplitX is used to decide which way the Grid will be split.
    /// True = Split Vertical
    /// False = Split Horizontal
    /// (I think? Could be backwards...)
    /// </summary>
    private void SplitCurrentGrid2(bool SplitX)
    {
        // Setup
        int ChildSplitTracking = SplitTracking - 1;
        int LengthBeingSplit = SplitX ? Data.GetLength(0) : Data.GetLength(1);
        int SplitDeviation = GetSplitDeviation(LengthBeingSplit);

        // A Size is also the index of the split where no rooms are allowed
        int A_Size = LengthBeingSplit / 2;
        // B Size should match A size. Minus 1 for a spot taken by the split
        // And plus 1 for a spot missed by division
        int B_Size = A_Size - 1 + (LengthBeingSplit % 2);
            
        // Add deviation
        A_Size += SplitDeviation;
        // Add deviations negative
        B_Size += SplitDeviation * -1;

        //if even length then minus one instead because split takes one 

        /// NOTE: A_Size is a length of the A's Grid however,
        /// A_Size also marks the INDEX in the grids array of
        /// where the split is, there should be no boxes that
        /// go over this area unless it is to connect the two

        // A Origin sits right where the current grid sits so nothing changes
        int[] A_Origin = new int[] { 0, 0 };
        // However the B Origin needs to start,
        // AFTER the split
        int[] B_Origin = SplitX ? new int[] { 0, A_Size + 1 } : new int[] { A_Size + 1, 0 };

        /// NOTE: As stated before, A_Size is the length of 
        /// A's grid AND the INDEX of the split. Since this
        /// is true, we can simply add one to A_Size to get
        /// the INDEX of B's origin.

        // Side A
        Tile[,] A_Grid = SplitX ? CreateGrid(Data.GetLength(1), A_Size) : CreateGrid(A_Size, Data.GetLength(0));
        Left = new BinarySpacePartitioningNode(A_Grid, ChildSplitTracking, A_Origin);

        // Side B
        Tile[,] B_Grid = SplitX ? CreateGrid(Data.GetLength(1), B_Size) : CreateGrid(B_Size, Data.GetLength(0));
        Right = new BinarySpacePartitioningNode(B_Grid, ChildSplitTracking, B_Origin);
    }

    private int GetSplitDeviation(int lengthBeingSplit)
    {
        int DeviationMax = lengthBeingSplit / 6;

        return Random.Next(DeviationMax * -1, DeviationMax + 1);
    }

    private Tile[,] CreateGrid(int Le1, int Le0)
    {
        // Create Grid
        Tile[,] Tiles = new Tile[Le0, Le1];

        // Populate Grid
        for (int Y = 0; Y < Le0; Y++)
        {
            for (int X = 0; X < Le1; X++)
            {
                Tile tile = new Tile()
                {
                    X = X,
                    Y = Y,
                };
                Tiles[Y, X] = tile;
            }
        }

        return Tiles;
    }

    private bool canSplit()
    {
        bool result = true;

        if (Data == null)
        {
            result = false;
        } else if (Data.GetLength(0) < 2 || Data.GetLength(1) < 2)
        {
            result = false;
        }

        if (Right != null)
        {
            result = false;
        }

        if (Left != null)
        {
            result = false;
        }

        return result;
    }
}
