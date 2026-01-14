using NaughtyAttributes;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class RewindManager : MonoBehaviour
{
    [SerializeField,ReadOnly] List<GameObject> rewindableGameObjects;

    public int maxRewindableTime = 300;

    public event Action<bool> OnToggleRecord;
    public event Action<int> OnRewind;
    public event Action<PauseState> OnPause;

    bool bIsRewinding;
  [HideInInspector] public bool bIsPaused = false;

    public bool IsInEditor() { return Application.isEditor && !Application.isPlaying; }

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
            OnRewind?.Invoke(i);
            frameRemaining = i - frameTarget;
            yield return new WaitForEndOfFrame();
        }

        OnToggleRecord?.Invoke(true);
        bIsRewinding = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetPause();
        }
    }

    void SetPause()
    {
        bIsPaused = !bIsPaused;
        if (bIsPaused)
        {
            Time.timeScale = 0f;
            OnPause?.Invoke(PauseState.Paused);
            OnToggleRecord?.Invoke(false);
        }
        else
        {
            Time.timeScale = 1f;
            OnToggleRecord?.Invoke(true);
        }
         

    }
}
