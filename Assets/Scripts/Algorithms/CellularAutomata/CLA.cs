using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface CLA : Algorithm
{
    public int Loop { get; set; }
    public Tile[,] PreformCA();

    public List<Tile> MooreNeighborhood(Tile tile);

    public void GenerateRandomGrid();
}

