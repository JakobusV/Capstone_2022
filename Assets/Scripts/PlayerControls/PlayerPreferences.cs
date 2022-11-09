using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.PlayerControls
{
    public static class PlayerPreferences
    {
        private static string filePath = "Prefs/PlayerPreferences.txt";
        public static float Vertical_Sensitivity = 2f;
        public static float Horizontal_Sensitivity = 3f;
        public static float Sound = 1f;

        public static void Write()
        {
            // StringBuilder for writing all the preferences
            StringBuilder stringBuilder = new StringBuilder();

            // StringBuilder for writing a preference
            StringBuilder preferenceBuilder = new StringBuilder();

            foreach (FieldInfo field in typeof(PlayerPreferences).GetFields())
            {
                // Append field name
                preferenceBuilder.Append(field.Name);

                // key value split with equal sign
                preferenceBuilder.Append("=");

                // Append Value
                preferenceBuilder.Append(typeof(PlayerPreferences).GetField(field.Name).GetValue(null));

                // Append current preference
                stringBuilder.AppendLine(preferenceBuilder.ToString());

                // Clear current preference
                preferenceBuilder.Clear();
            }

            // Write out file
            File.WriteAllText(filePath, stringBuilder.ToString());
        }

        public static void Read()
        {
            // Read from file
            string[] rows = File.ReadAllLines(filePath);

            // for each preference
            foreach (string row in rows)
            {
                // split key from value
                string[] preference = row.Split('=');

                // get value
                float value = float.Parse(preference[1]);

                // get field by key name
                FieldInfo field = typeof(PlayerPreferences).GetField(preference[0], BindingFlags.Public | BindingFlags.Static);

                // set preference
                field.SetValue(null, value);
            }
        }
    }
}
