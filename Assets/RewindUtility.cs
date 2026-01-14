using System;
using UnityEngine;

public static class RewindUtility
{
    public static IRewindData GetInterface(Component component)
    {
        if (component == null) return null;

        if (component as Transform is not null)
        {
            return new FTransformRewindData();
        }
        //else if (component as Rigidbody is not null)
        //{
        //    return new FRigidBodyRewindData();
        //}
        else
        {
            return null;
        }
    }
}

public interface IRewindData
{
    void GetData(Component component);

    void SetData(Component component);

    Component Component { get; set; }
}

[Serializable]
public struct FTransformRewindData : IRewindData
{
    public void SetData(Component component)
    {
        Component = component;
        Transform transform = (Transform)component;

        transform.SetPositionAndRotation(Position, Rotation);
        transform.localScale = Scale;
    }

    public void GetData(Component component)
    {
        Transform transform = (Transform)component;

        Position = transform.position;
        Rotation = transform.rotation;
        Scale = transform.localScale;
    }

    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;

    public Component Component { get; set; }
}

[Serializable]
public struct FRigidBodyRewindData : IRewindData
{
    public void SetData(Component component)
    {
        Component = component;
        Rigidbody rigidbody = (Rigidbody)component;

        rigidbody.mass = Mass;
        rigidbody.linearDamping = LinearDamping;
        rigidbody.angularDamping = AngularDamping;
        rigidbody.useGravity = bUseGravity;
        rigidbody.constraints = constraints;
    }

    public void GetData(Component component)
    {
        Rigidbody rigidbody = (Rigidbody)component;

        Mass = rigidbody.mass;
        LinearDamping = rigidbody.linearDamping;
        AngularDamping = rigidbody.angularDamping;
        bUseGravity = rigidbody.useGravity;
        constraints = rigidbody.constraints;
    }

    public float Mass;
    public float LinearDamping;
    public float AngularDamping;
    public bool bUseGravity;
    public RigidbodyConstraints constraints;

    public Component Component { get; set; }
}