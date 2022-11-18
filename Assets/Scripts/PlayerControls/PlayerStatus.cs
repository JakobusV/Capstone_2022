using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.PlayerControls
{
    public static class PlayerStatus
    {
        private const string filePath = "Saves/";
        private const string fileName = "status.mgn";
        public static float Health = 10f;
        public static float Move_Speed = 6f;
        public static float Jump_Force = 5f;
        public static float Position_X = 5f;
        public static float Position_Y = 0f;
        public static float Position_Z = 5f;
        public static string Spectacle_Path = "default";
        public static string Spectacle_Index = "0.lff";

        public static void Write()
        {
            // StringBuilder for writing the file path
            StringBuilder statusPathBuilder = new StringBuilder(filePath);

            // StringBuilder for writing all the preferences
            StringBuilder stringBuilder = new StringBuilder();

            // StringBuilder for writing a preference
            StringBuilder preferenceBuilder = new StringBuilder();

            // Build status path
            statusPathBuilder.Append(Spectacle_Path);
            statusPathBuilder.Append("/");
            statusPathBuilder.Append(fileName);

            foreach (FieldInfo field in typeof(PlayerStatus).GetFields())
            {
                // Append field name
                preferenceBuilder.Append(field.Name);

                // key value split with equal sign
                preferenceBuilder.Append("=");

                // Append Value
                preferenceBuilder.Append(typeof(PlayerStatus).GetField(field.Name).GetValue(null));

                // Append current preference
                stringBuilder.AppendLine(preferenceBuilder.ToString());

                // Clear current preference
                preferenceBuilder.Clear();
            }

            // Write out file
            File.WriteAllText(statusPathBuilder.ToString(), stringBuilder.ToString());
        }

        internal static void Default()
        {
            Health = 10f;
            Move_Speed = 6f;
            Jump_Force = 5f;
            Position_X = 5f;
            Position_Y = 0f;
            Position_Z = 5f;
            Spectacle_Path = "default";
            Spectacle_Index = "0.lff";
        }

        public static void Read(string directory)
        {
            // Build path
            StringBuilder pathBuilder = new StringBuilder(filePath);
            pathBuilder.Append("/");
            pathBuilder.Append(directory);
            pathBuilder.Append("/");
            pathBuilder.Append(fileName);

            // Read from file
            string[] rows = File.ReadAllLines(pathBuilder.ToString());

            // for each preference
            foreach (string row in rows)
            {
                // split key from value
                string[] preference = row.Split('=');

                // get field by key name
                FieldInfo field = typeof(PlayerStatus).GetField(preference[0], BindingFlags.Public | BindingFlags.Static);

                // determine field type
                if (field.FieldType == typeof(float))
                {
                    // get value
                    float value = float.Parse(preference[1]);

                    // set preference
                    field.SetValue(null, value);
                } else
                {
                    field.SetValue(null, preference[1]);
                }
            }
        }

        public static string GetSavePath()
        {
            return filePath + Spectacle_Path + "/";
        }
    }
}
