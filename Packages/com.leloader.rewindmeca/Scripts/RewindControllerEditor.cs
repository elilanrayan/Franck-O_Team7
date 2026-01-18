using NaughtyAttributes.Editor;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using NaughtyAttributes;



[CustomEditor(typeof(RewindController))]
public class RewindControllerEditor : NaughtyInspector
{
    public override void OnInspectorGUI()
    {
        GetTest();

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

        base.OnInspectorGUI();
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

    private static void GetTest()
    {
        //AppDomain domain = AppDomain.CurrentDomain;
        //AssemblyName assemblyName = new AssemblyName("UnityEditor.UIElements");
        //Debug.Log(assemblyName);
        ////TODO: Probably here I need to do something with the Assembly Definition?
        //Assembly assemblyBuilder = domain.Load(assemblyName);
        //Module moduleBuilder = assemblyBuilder.LoadModule(assemblyName.Name);
        //Type editorClass = moduleBuilder.GetType("EditorElement");//, TypeAttributes.Public, typeof(VisualElement)
        //editorClass;
    }


}