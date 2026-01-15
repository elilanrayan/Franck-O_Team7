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
    }

    private void OnDisable()
    {
        //EditorApplication.pauseStateChanged -= OnPauseStateChanged;
        manager.OnPause -= OnPauseStateChanged;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (manager.bIsPaused)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Rewind Preview", EditorStyles.boldLabel);
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
}