using UnityEngine;
using Unity.GraphToolkit.Editor;
using UnityEditor;
using System;

[Serializable]
[Graph(AssetExtension)]
public class DialogGraph : Graph
{
    public const string AssetExtension = "dialoggraph";

    [MenuItem("Assets/Create/Guillaume/Dialog Graph", false)]
    private static void CreateAssetFile()
    {
        GraphDatabase.PromptInProjectBrowserToCreateNewAsset<DialogGraph>();
    }
}
