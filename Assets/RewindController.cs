using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System;
using NaughtyAttributes;

[Serializable]
public struct FComponentRewindData
{
    public int Frame;
    public List<IRewindData> RewindData;
}

[DisallowMultipleComponent]
public class RewindController : MonoBehaviour
{
    [SerializeField]
    public List<Component> recordedComponents = new();

    //[SerializedDictionary("Frame", "RewindData")]
    public SerializedDictionary<Component, List<FComponentRewindData>> RewindData;

    [SerializeField, ReadOnly]
    bool bShouldRecord = true;

    [SerializeField, ReadOnly]
    bool bIsRewinding = false;

    int limitDataCount;

    int currentData = 0;

    [ContextMenuItem("Load", "Test")]
    string test;

    private void Reset()
    {
        AddComponentToList(transform);
    }

    void Start()
    {
        RewindManager rewindManager = FindAnyObjectByType<RewindManager>();
        limitDataCount = rewindManager.maxRewindableTime;
        RewindData = new(limitDataCount);
        //AddComponentToList(transform);

        rewindManager.OnRewind += Play;
        rewindManager.OnToggleRecord += OnToggleRecord;
    }

    private void OnToggleRecord(bool bShouldRecord)
    {
        this.bShouldRecord = bShouldRecord;
        this.bIsRewinding = !bShouldRecord;

        ToggleGravity(bShouldRecord);
    }

    void Update()
    {
        if (bShouldRecord) Record(Time.frameCount);
        if (!bIsRewinding) TryRemoveData(Time.frameCount - limitDataCount);
    }


    public void Play(int frame)
    {
        if (!RewindData.TryGetValue(frame, out List<FComponentRewindData> componentDatas)) return;

        foreach (FComponentRewindData componentData in componentDatas)
        {
            componentData.RewindData.;
            foreach (Component component in recordedComponents)
            {
                if (componentData.d == component)
                {
                    rewindData.SetData(component);
                    break;
                }
            }
        }
        TryRemoveData(frame);
    }

    private void ToggleGravity(bool newGravity)
    {

        if (TryGetComponent<Rigidbody2D>(out Rigidbody2D rb2d))
        {
            rb2d.gravityScale = newGravity ? 1 : 0;
        }

        if (TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.useGravity = newGravity;
        }
    }

    private void TryRemoveData(int frame)
    {
        RewindData.Remove(frame);
    }

    public void AddComponentToList(Component component)
    {
        recordedComponents.Add(component);
    }

    // Each frame
    void Record(int actualFrame)
    {
        List<IRewindData> rewindDatas = new();

        foreach (Component component in recordedComponents)
        {
            IRewindData data = RewindUtility.GetInterface(component);
            if (data != null)
            {
                data.GetData(component);
                rewindDatas.Add(data);
            }
        }

        RewindData.Add(actualFrame, rewindDatas);
    }

    public void BeginDestroy()
    {
        Destroy(this);
    }
}