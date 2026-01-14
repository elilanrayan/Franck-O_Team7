using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System;
using NaughtyAttributes;

//public interface IRewindableData
//{
//    public abstract void StoreData();
//    public abstract void PlayData();
//}

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
        rewindData = new(limitDataCount);
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
        if (!rewindData.TryGetValue(frame, out FTransformRewindData transformData)) return;

        foreach (Component component in components)
        {
            Transform currentTransfrom = (Transform)component;
            if (currentTransfrom != null)
            {
                currentTransfrom.SetPositionAndRotation(transformData.Position, transformData.Rotation);
                currentTransfrom.localScale = transformData.Scale;
            }
            TryRemoveData(frame);
        }
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
        rewindData.Remove(frame);
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