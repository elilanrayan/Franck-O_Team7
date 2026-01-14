using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System;

[Serializable]
public struct FTransformRewindData
{
    public FTransformRewindData(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        Position = position;
        Rotation = rotation;
        Scale = scale;
    }

    public FTransformRewindData(Transform transform)
    {
        Position = transform.position;
        Rotation = transform.rotation;
        Scale = transform.localScale;
    }

     public Vector3 Position;
     public Quaternion Rotation;
     public Vector3 Scale;
}

[DisallowMultipleComponent]
public class RewindController : MonoBehaviour
{
    [SerializeField]
    public List<Component> components = new();

    [SerializedDictionary("Frame", "RewindData")]
    public SerializedDictionary<int, FTransformRewindData> rewindData;

    [SerializeField]
    bool bShouldRecord = true;

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
        rewindData = new(limitDataCount);
        //AddComponentToList(transform);

        rewindManager.OnRewind += Play;
        rewindManager.OnToggleRecord += OnToggleRecord; 
    }

    private void OnToggleRecord(bool bShouldRecord)
    {
        this.bShouldRecord = bShouldRecord;
        this.bIsRewinding = !bShouldRecord;
    }

    void Update()
    {
        if (bShouldRecord) Record(Time.frameCount);
        if (!bIsRewinding)
        {
            TryRemoveData(Time.frameCount - limitDataCount);
        }
    }

    private void TryRemoveData(int frame)
    {
        rewindData.Remove(frame);
    }

    public void Play(int frame)
    {
        if (!rewindData.TryGetValue(frame, out FTransformRewindData transformData)) return;
        
        foreach (Component component in components)
        {
            if ((Transform)component != null)
            {
                transform.SetPositionAndRotation(transformData.Position, transformData.Rotation);
                transform.localScale = transformData.Scale;
            }
            TryRemoveData(frame);
        }
    }

    public void AddComponentToList(Component component)
    {
        components.Add(component);
    }

    // Each frame
    void Record(int actualFrame)
    {
        if (rewindData.Count >= limitDataCount)
        {
            rewindData.Remove(actualFrame - limitDataCount);
        }

        foreach (Component component in components)
        {
            if ((Transform)component != null)
            {
                FTransformRewindData transformData = new FTransformRewindData(transform);
                rewindData.Add(actualFrame, transformData);

            }
        }
    }

   public void BeginDestroy()
    {
        Destroy(this);
    }
}