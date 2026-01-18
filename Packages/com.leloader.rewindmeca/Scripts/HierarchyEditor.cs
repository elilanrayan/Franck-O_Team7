using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


[InitializeOnLoad]
public class HierarchyIconEditor
{
    static HierarchyIconEditor()
    {
        EditorApplication.hierarchyWindowItemOnGUI += DrawIconOnHierarchy;
    }

    static void DrawIconOnHierarchy(int instanceID, Rect selectionRect)
    {
        GameObject obj = EditorUtility.EntityIdToObject(instanceID) as GameObject;
        RewindManager manager = Object.FindAnyObjectByType<RewindManager>();

        if (obj != null && obj.GetComponent<Transform>() != null && obj.GetComponent<RewindManager>() == null && obj.GetComponent<Camera>() == null )
        {
            Rect iconRect = new Rect(selectionRect.xMin, selectionRect.y, 16, 16);

            GUIContent icon = obj.GetComponent<RewindController>() != null ? EditorGUIUtility.IconContent("Animation.Record") : EditorGUIUtility.IconContent("AudioDistortionFilter Icon");
            if (GUI.Button(iconRect, icon, GUIStyle.none))
            {
                
                    EditorGUI.BeginChangeCheck();
                if (obj.TryGetComponent<RewindController>(out RewindController controller))
                {
                    Object.DestroyImmediate(controller,true);
                    manager.rewindableGameObjects.Remove(obj);
                }
                else
                {

                    manager.rewindableGameObjects.Add(obj);

                    obj.AddComponent<RewindController>();
                }

                    if (EditorGUI.EndChangeCheck())
                    {
                        Debug.Log(manager.rewindableGameObjects.Count);
                        Undo.RecordObject(manager, "Change int");
                        EditorUtility.SetDirty(manager);
                    }
            }
        }
    }

    


}

