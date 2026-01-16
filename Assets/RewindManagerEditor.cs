using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEditor.TerrainTools;
using UnityEngine;

[CustomEditor(typeof(RewindManager))]
public class RewindManagerEditor : Editor
{
    float oldValue = 0;
    float currentValue = 0;
    RewindManager manager => (RewindManager)target;

    private void OnEnable()
    {
        //EditorApplication.pauseStateChanged += OnPauseStateChanged;
        manager.OnPause += OnPauseStateChanged;
        EditorApplication.hierarchyChanged += OnHiearchyStateChanged;
    }

    private void OnDisable()
    {
        //EditorApplication.pauseStateChanged -= OnPauseStateChanged;
        manager.OnPause -= OnPauseStateChanged;
        EditorApplication.hierarchyChanged += OnHiearchyStateChanged;
    }

    bool _isOpenned;
    public override void OnInspectorGUI()
    {
        _isOpenned = EditorGUILayout.Foldout(_isOpenned, "Rewindable Object :");
        if (_isOpenned)
        {
            EditorGUI.indentLevel++;
             foreach (var e in manager.rewindableGameObjects)
            {
               
                if (GUILayout.Button(e.name))
                {
                    EditorGUIUtility.PingObject(e.gameObject);
                    Selection.activeGameObject = e.gameObject;
                    SceneView.FrameLastActiveSceneView();
                    Selection.activeGameObject = manager.gameObject;
                }

            }
             EditorGUI.indentLevel--;
        }


            EditorGUILayout.Space(30);
        EditorGUILayout.IntField("Max Time", manager.maxRewindableTime);
       

        if (manager.bIsPaused)
        {
            EditorGUILayout.Space(30);
            EditorGUILayout.LabelField("Rewind Preview", EditorStyles.boldLabel);
            EditorGUILayout.IntField("Last Frame", manager.currentStoppedFrame);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Slider", currentValue.ToString());
            oldValue = currentValue;
            currentValue = GUILayout.HorizontalSlider(currentValue, manager.currentStoppedFrame, manager.currentStoppedFrame - manager.maxRewindableTime);
            if (oldValue != currentValue)
            {
                manager.PlayFrame((int)currentValue, false);
            }
            EditorGUILayout.EndHorizontal();
        }

    }

    private void OnPauseStateChanged(PauseState state, int currentStoppedFrame)
    {
         Repaint();
    }

    private void OnHiearchyStateChanged()
    {
        Repaint();
    }
}