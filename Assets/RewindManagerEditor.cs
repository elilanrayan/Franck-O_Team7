using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEditor.TerrainTools;
using UnityEngine;

[CustomEditor(typeof(RewindManager))]
public class RewindManagerEditor : Editor
{
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
            currentValue = GUILayout.HorizontalSlider(currentValue, Time.frameCount, Time.frameCount - manager.maxRewindableTime);
            EditorGUILayout.EndHorizontal();
            

        }
    }

    private void OnPauseStateChanged(PauseState state)
    {
            Repaint();
    }
}