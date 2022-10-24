using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGeneration 
{
    public static Texture2D ColorMapTexture(Color[] colors, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        texture.SetPixels(colors);

        texture.Apply();

        return texture;
    }

    public static Texture2D ValueMapTextureBinary(Tile[,] tiles)
    {
        int LengthX = tiles.GetLength(1);
        int LengthY = tiles.GetLength(0);

        Color[] colors = new Color[LengthX * LengthY];

        for (int X = 0; X < LengthX; X++)
        {
            for (int Y = 0; Y < LengthY; Y++)
            {
                int ColorValue = tiles[Y, X].Value;
                bool ySafe = false;

                if (Y < LengthY - 1)
                {
                    ySafe = true;
                    ColorValue += tiles[Y + 1, X].Value;
                }
                if (X < LengthX - 1)
                {
                    ColorValue += tiles[Y, X + 1].Value;

                    if (ySafe)
                    {
                        ColorValue += tiles[Y + 1, X + 1].Value;
                    }
                }

                Color color = Color.white;

                switch (ColorValue)
                {
                    case 0: 
                        color = Color.green;
                        break;
                    case 4:
                        color = Color.red;
                        break;
                    default:
                        color = Color.blue;
                        break;
                }

                colors[X * LengthY + Y] = color;
                //colors[X * LengthY + Y] = Color.Lerp(Color.green, Color.blue, tiles[Y, X].Value);
            }
        }

        return ColorMapTexture(colors, LengthY, LengthX);
    }

    public static Texture2D ValueMapTextureDynamic(Tile[,] tiles, float height)
    {
        int LengthX = tiles.GetLength(1);
        int LengthY = tiles.GetLength(0);

        Color[] colors = new Color[LengthX * LengthY];

        for (int X = 0; X < LengthX; X++)
        {
            for (int Y = 0; Y < LengthY; Y++)
            {
                float range = (float)tiles[Y, X].Value / height;
                colors[X * LengthY + Y] = Color.Lerp(Color.green, Color.blue, range);
            }
        }

        return ColorMapTexture(colors, LengthY, LengthX);
    }
}
