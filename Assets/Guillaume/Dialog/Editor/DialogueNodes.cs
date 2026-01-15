using UnityEngine;
using Unity.GraphToolkit.Editor;
using System;
using UnityEngine.TestTools.Constraints;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.UIElements;

[Serializable]
public class StartNode : Node
{
    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddOutputPort("out").Build();
    }
}

[Serializable]
public class EndNode : Node
{
    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort("in").Build();
    }
}

public class DialogueNode : Node
{

    public const string modeId = "Mode";
    public const string SpeakerOpt = "Speaker Key";
    public const string DialogueOpt = "Dialogue Key";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort("in").Build();
        context.AddOutputPort("out").Build();
    }

    protected override void OnDefineOptions(IOptionDefinitionContext context)
    {
        context.AddOption<DialogueMode>(modeId).WithDefaultValue(DialogueMode.Panel).Delayed();
        context.AddOption<DialogKey>(SpeakerOpt).WithDefaultValue(DialogKey.None);
        context.AddOption<DialogKey>(DialogueOpt).WithDefaultValue(DialogKey.None);
    }
}

[Serializable]
public class ChoiceNode : Node
{
    public const string modeId = "Mode";

    const string portId = "Port Count";
    public const string SpeakerOpt = "Speaker Key";
    public const string DialogueOpt = "Dialogue Key";
    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort("in").Build();

        var option = GetNodeOptionByName(portId);
        option.TryGetValue(out int portCount);
        for (int i = 0; i < portCount; i++)
        {
            context.AddInputPort<DialogKey>($"Choice Text {i}").Build();
            context.AddOutputPort($"Choice {i}").Build();
        }
    }

    protected override void OnDefineOptions(IOptionDefinitionContext context)
    {
        context.AddOption<DialogueMode>(modeId).WithDefaultValue(DialogueMode.Panel).Delayed();
        context.AddOption<int>(portId).WithDefaultValue(2).Delayed();
        context.AddOption<DialogKey>(SpeakerOpt).WithDefaultValue(DialogKey.None);
        context.AddOption<DialogKey>(DialogueOpt).WithDefaultValue(DialogKey.None);
    }
}