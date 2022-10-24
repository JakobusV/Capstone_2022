using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface Algorithm
{
    public Tile[,] Tiles { get; set; }

    string GetFileType();

    public Tile[,] Run();

    public Tile[,] Read(string file);

    public void Write(string file);
}

