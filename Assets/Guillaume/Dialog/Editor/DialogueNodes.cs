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

    const string optionId = "mode";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort("in").Build();
        context.AddOutputPort("out").Build();

        context.AddInputPort<string>("Speaker").Build();
        context.AddInputPort<string>("Dialogue").Build();
    }

    protected override void OnDefineOptions(IOptionDefinitionContext context)
    {
        context.AddOption<DialogueMode>(optionId).WithDefaultValue(DialogueMode.Panel).Delayed();
    }
}

[Serializable]
public class ChoiceNode : Node
{
    const string optionId = "portCount";
    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort("in").Build();

        context.AddInputPort<string>("Speaker").Build();
        context.AddInputPort<string>("Dialogue").Build();

        var option = GetNodeOptionByName(optionId);
        option.TryGetValue(out int portCount);
        for (int i = 0; i < portCount; i++)
        {
            context.AddInputPort<string>($"Choice Text {i}").Build();
            context.AddOutputPort($"Choice {i}").Build();
        }
    }

    protected override void OnDefineOptions(IOptionDefinitionContext context)
    {
        context.AddOption<int>(optionId).WithDefaultValue(2).Delayed();
    }
}

public enum DialogueMode
{
    Panel,
    Popup,
    Bulle
}
