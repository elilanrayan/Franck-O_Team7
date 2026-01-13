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

    [SerializeField] Vector3 Position;
    [SerializeField] Quaternion Rotation;
    [SerializeField] Vector3 Scale;
}

public class RewindController : MonoBehaviour
{
    [SerializeField]
    public List<Component> components;

    [SerializedDictionary("Frame", "RewindData")]
    public SerializedDictionary<int, FTransformRewindData> rewindData;

    bool bShouldRecord = true;

    [SerializeField]
    int limitDataCount = 300;

    int currentData = 0;

    [ContextMenuItem("Load", "Test")]
    string test;

    private void Reset()
    {
        AddComponentToList(transform);
    }

    void Start()
    {
        rewindData = new(limitDataCount);
        //AddComponentToList(transform);
    }

    void Update()
    {
        if (bShouldRecord) Record(Time.frameCount);
    }

    void Test()
    {

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
            Transform transform = (Transform)component.transform;
            FTransformRewindData transformData = new FTransformRewindData(transform);
            rewindData.Add(actualFrame, transformData);
        }
    }
}