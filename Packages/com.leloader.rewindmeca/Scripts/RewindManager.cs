using NaughtyAttributes;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;


public class RewindManager : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> rewindableGameObjects { get; private set; } = new();

    [HideInInspector]
    public int maxRewindableTime = 600;
    [HideInInspector]
    public int currentStoppedFrame;

    public event Action<bool> OnToggleRecord;
    public event Action<int, bool> OnRewind;
    public event Action<PauseState, int> OnPause;

    bool bIsRewinding;
    [HideInInspector] public bool bIsPaused = false;

    public bool IsInEditor() { return Application.isEditor && !Application.isPlaying; }

    private void Start()
    {
        Debug.Log(rewindableGameObjects.Count);
    }

    private void Awake()
    {
        Debug.Log(rewindableGameObjects.Count);
    }

    [Button, DisableIf(EConditionOperator.Or, "bIsRewinding", "IsInEditor")]
    void startRewind()
    {
        if (!bIsRewinding)
        {
            StartCoroutine(StartRewind());
        }
        else
        {
            Debug.LogError("Rewinding is already in progress");
        }
    }

    [SerializeField, ProgressBar("Frame Remaining", 300, EColor.Green), ShowIf("bIsRewinding")]
    int frameRemaining = 300;

    IEnumerator StartRewind()
    {
        bIsRewinding = true;
        OnToggleRecord?.Invoke(false);
        frameRemaining = maxRewindableTime;
        int frameTarget = Time.frameCount - maxRewindableTime;

        for (int i = Time.frameCount; i > frameTarget; i--)
        {
            OnRewind?.Invoke(i, true);
            frameRemaining = i - frameTarget;
            yield return new WaitForEndOfFrame();
        }

        OnToggleRecord?.Invoke(true);
        bIsRewinding = false;
    }

    public void PlayFrame(int frame, bool delete)
    {
        OnRewind?.Invoke(frame, false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetPause();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            startRewind();
        }
    }

    void SetPause()
    {
        bIsPaused = !bIsPaused;
        if (bIsPaused)
        {
            Time.timeScale = 0f;
            OnPause?.Invoke(PauseState.Paused, currentStoppedFrame);
            currentStoppedFrame = Time.frameCount;
        }
        else
        {
            Time.timeScale = 1f;
            OnPause?.Invoke(PauseState.Unpaused, currentStoppedFrame);
            currentStoppedFrame = -1;
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        GUI.Label(new Rect(10, 10, 200, 200), $"Current frame: {Time.frameCount}");
        GUILayout.EndVertical();
    }
}