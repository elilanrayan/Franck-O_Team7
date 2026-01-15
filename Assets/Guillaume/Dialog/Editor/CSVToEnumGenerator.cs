using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

public class CSVToEnumGenerator : EditorWindow
{
    private static string enumName = "DialogKey";
    private static string filePath = "Assets/Scripts/Enums/";

    [MenuItem("Tools/Generate Dialog Enum from CSV")]
    public static void GenerateEnum()
    {
        Object selectedObject = Selection.activeObject;
        TextAsset csvFile = selectedObject as TextAsset;

        if (csvFile == null)
        {
            Debug.LogError("Veuillez sélectionner un fichier CSV dans la fenêtre Project avant de lancer cet outil.");
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("public enum " + enumName);
        sb.AppendLine("{");
        sb.AppendLine("\tNone = 0,");

        string[] lines = csvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string key = line.Split(';')[0].Trim();

            if (!string.IsNullOrEmpty(key))
            {
                string sanitizedKey = SanitizeKey(key);
                sb.AppendLine($"\t{sanitizedKey},");
            }
        }

        sb.AppendLine("}");

        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }

        string finalPath = filePath + enumName + ".cs";
        File.WriteAllText(finalPath, sb.ToString());

        AssetDatabase.Refresh();
        Debug.Log($"Enum '{enumName}' généré avec succès dans {finalPath} !");
    }

    private static string SanitizeKey(string key)
    {
        key = key.Replace(" ", "_").Replace("-", "_");
        key = Regex.Replace(key, "[^a-zA-Z0-9_]", "");
        if (char.IsDigit(key[0]))
        {
            key = "_" + key;
        }

        return key;
    }
}