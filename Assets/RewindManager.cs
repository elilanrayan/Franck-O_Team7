using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Collections;

public class RewindManager : MonoBehaviour
{
    [SerializeField] List<GameObject> rewindableGameObjects;

    public int maxRewindableTime = 300;

    public event Action<bool> OnToggleRecord;
    public event Action<int> OnRewind;


    [Button]
    void startRewind()
    {
        StartCoroutine(StartRewind());
    }

  
    IEnumerator StartRewind()
    {
        OnToggleRecord?.Invoke(false);

        for (int i = Time.frameCount; i > Time.frameCount - maxRewindableTime; i--)
        {
            OnRewind?.Invoke(i);
            yield return new WaitForEndOfFrame();
        }

        OnToggleRecord?.Invoke(true);
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
