using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System;
using UnityEditor;

[CustomEditor(typeof(RewindController))]
public class RewindControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RewindController controller = (RewindController)target;
        Component[] components = controller.gameObject.GetComponents<Component>();

        if (GUILayout.Button("Track component"))
        {
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < components.Length; i++) { // Chaque component
                Component component = components[i];

                if (controller == component) continue;
                 
                bool checkbox = controller.components.Contains(component);
                menu.AddItem(new GUIContent($"{components[i].ToString()}"), checkbox, () => AddOrRemoveComponentToList(component, controller));
            }

            menu.ShowAsContext();
        }
    }

    private void AddOrRemoveComponentToList(Component component, RewindController controller) { 
        if (controller.components.Contains(component))
        {
            controller.components.Remove(component);
        }
        else
        {
            controller.AddComponentToList(component);
        }
    }
}