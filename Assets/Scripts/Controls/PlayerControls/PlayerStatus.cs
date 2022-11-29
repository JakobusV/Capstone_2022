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
        private const string All_Saves_Dir = "Saves/";
        private const string Status_File_Full_Name = "status.mgn";
        private const string Status_File_Extension = ".mgn";
        public static float Health = 10f;
        public static float Move_Speed = 6f;
        public static float Jump_Force = 5f;
        public static float Position_X = 5f;
        public static float Position_Y = 0f;
        public static float Position_Z = 5f;
        public static string Weapon = "";
        public static string Spectacle_Path = "default";
        public static string Spectacle_Index = "0";
        public static string Spectacle_Extension = ".lff";

        public static void Write()
        {
            // StringBuilder for writing the file path
            StringBuilder statusManager = new StringBuilder(All_Saves_Dir);
            StringBuilder spectacleManager = new StringBuilder(All_Saves_Dir);

            // StringBuilder for writing all the preferences for status
            StringBuilder statusTextBuilder = new StringBuilder();
            // StringBuilder for writing all the preferences for status
            StringBuilder spectacleTextBuilder = new StringBuilder();

            // StringBuilder for writing a preference
            StringBuilder preferenceBuilder = new StringBuilder();

            // Build status manager path
            statusManager.Append(Spectacle_Path);
            statusManager.Append("/");
            statusManager.Append(Status_File_Full_Name);

            // Build spectacle manager path
            spectacleManager.Append(Spectacle_Path);
            spectacleManager.Append("/");
            spectacleManager.Append(Spectacle_Index + Status_File_Extension);

            foreach (FieldInfo field in typeof(PlayerStatus).GetFields())
            {
                StringBuilder textBuilder = (IsStatusPreference(field.Name)) ? ref statusTextBuilder : ref spectacleTextBuilder;

                // Append field name
                preferenceBuilder.Append(field.Name);

                // key value split with equal sign
                preferenceBuilder.Append("=");

                // Append Value
                preferenceBuilder.Append(typeof(PlayerStatus).GetField(field.Name).GetValue(null));

                // Append current preference
                textBuilder.AppendLine(preferenceBuilder.ToString());

                // Clear current preference
                preferenceBuilder.Clear();
            }

            // Write out files
            File.WriteAllText(statusManager.ToString(), statusTextBuilder.ToString());
            File.WriteAllText(spectacleManager.ToString(), spectacleTextBuilder.ToString());
        }

        public static void WriteMainMGN()
        {
            // StringBuilder for writing the file path
            StringBuilder statusManager = new StringBuilder(All_Saves_Dir);

            // StringBuilder for writing all the preferences for status
            StringBuilder statusTextBuilder = new StringBuilder();

            // StringBuilder for writing a preference
            StringBuilder preferenceBuilder = new StringBuilder();

            // Build status manager path
            statusManager.Append(Spectacle_Path);
            statusManager.Append("/");
            statusManager.Append(Status_File_Full_Name);

            foreach (FieldInfo field in typeof(PlayerStatus).GetFields())
            {
                if (IsStatusPreference(field.Name))
                {
                    // Append field name
                    preferenceBuilder.Append(field.Name);

                    // key value split with equal sign
                    preferenceBuilder.Append("=");

                    // Append Value
                    preferenceBuilder.Append(typeof(PlayerStatus).GetField(field.Name).GetValue(null));

                    // Append current preference
                    statusTextBuilder.AppendLine(preferenceBuilder.ToString());

                    // Clear current preference
                    preferenceBuilder.Clear();
                }
            }

            // Write out files
            File.WriteAllText(statusManager.ToString(), statusTextBuilder.ToString());
        }

        internal static string GetNextIndex()
        {
            DirectoryInfo spectacle_Dir = new DirectoryInfo(GetSavePath());
            FileInfo[] files = spectacle_Dir.GetFiles(".mgn");
            int largest_index = 0;

            foreach (FileInfo file in files)
            {
                try
                {
                    // Try to get index
                    int new_index = int.Parse(file.Name);
                    // if larger, set
                    largest_index = (new_index > largest_index) ? new_index : largest_index;
                }
                catch
                {
                    // File was most likely status.mgn, that one is expected to come here
                    // But if things start to get weird...
                    // It might just root back to here.
                }
            }

            // send back largest index plus one
            largest_index++;
            return largest_index.ToString();
        }

        /// <summary>
        /// Should be called AFTER current status has been saved. <br/>
        /// Swaps currect status to exact desired index, located by params <b>spectacle_Index</b> + <b>spectacle_Extension</b>.<br/>
        /// If the index doesn't exist, the defaults are loaded, the file is made, and then set as current status.
        /// </summary>
        internal static void SwapStatus(string spectacle_Index, string spectacle_Extension)
        {
            // Set status in main mgn
            Spectacle_Index = spectacle_Index;
            Spectacle_Extension = spectacle_Extension;

            // Create path to file info
            FileInfo file = new FileInfo(GetSavePath() + spectacle_Index + spectacle_Extension);

            // Check for if file exists
            if (!file.Exists)
            {
                // Default status but don't replace path variables
                Default(Spectacle_Path: Spectacle_Path, Spectacle_Index: spectacle_Index, Spectacle_Extension: spectacle_Extension);

                Write();
            }
            else
            {
                WriteMainMGN();

                Read(Spectacle_Path);
            }
        }

        private static bool IsStatusPreference(string fieldName)
        {
            bool result;

            switch (fieldName)
            {
                case "Spectacle_Path":
                    result = true;
                    break;
                case "Spectacle_Index":
                    result = true;
                    break;
                case "Spectacle_Extension":
                    result = true;
                    break;
                default:
                    result = false;
                    break;
            }

            return result;
        }

        internal static void Default(
            float Health = 10f,
            float Move_Speed = 6f,
            float Jump_Force = 5f,
            float Position_X = 5f,
            float Position_Y = 0f,
            float Position_Z = 5f,
            string Weapon = "",
            string Spectacle_Path = "default",
            string Spectacle_Index = "0",
            string Spectacle_Extension = ".lff"
            )
        {
            PlayerStatus.Health = Health;
            PlayerStatus.Move_Speed = Move_Speed;
            PlayerStatus.Jump_Force = Jump_Force;
            PlayerStatus.Position_X = Position_X;
            PlayerStatus.Position_Y = Position_Y;
            PlayerStatus.Position_Z = Position_Z;
            PlayerStatus.Weapon = Weapon;
            PlayerStatus.Spectacle_Path = Spectacle_Path;
            PlayerStatus.Spectacle_Index = Spectacle_Index;
            PlayerStatus.Spectacle_Extension = Spectacle_Extension;
        }

        public static void Read(string directory)
        {
            for (int i = 0; i < 2; i++)
            {
                // Build path
                // MeshGeneration\Saves\fale\status.mgnfale\0.dsq.mgn
                StringBuilder pathBuilder = new StringBuilder(All_Saves_Dir);
                pathBuilder.Append("/");
                pathBuilder.Append(directory);
                pathBuilder.Append("/");
                
                // Read from status then correct spectacle
                if (i == 0)
                {
                    pathBuilder.Append(Status_File_Full_Name);
                }
                else
                {
                    if (string.IsNullOrEmpty(Spectacle_Index))
                    {
                        throw new Exception("Spectacle Index not found in PlayerStatus");
                    }
                    else
                    {
                        pathBuilder.Append(Spectacle_Index + Status_File_Extension);
                    }
                }

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
        }

        public static string GetSavePath()
        {
            return All_Saves_Dir + Spectacle_Path + "/";
        }
    }
}
