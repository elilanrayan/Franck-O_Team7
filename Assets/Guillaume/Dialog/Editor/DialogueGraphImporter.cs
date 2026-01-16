using UnityEngine;
using UnityEditor.AssetImporters;
using Unity.GraphToolkit.Editor;
using System;
using System.Collections.Generic;
using System.Linq;

[ScriptedImporter(1, DialogGraph.AssetExtension)]
public class DialogueGraphImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        DialogGraph editorGraph = GraphDatabase.LoadGraphForImporter<DialogGraph>(ctx.assetPath);
        RuntimeDialogGraph runtimeGraph = ScriptableObject.CreateInstance<RuntimeDialogGraph>();
        var nodeIDMap = new Dictionary<INode, string>();

        foreach (var node in editorGraph.GetNodes())
        {
            nodeIDMap[node] = Guid.NewGuid().ToString();
        }

        var startNode = editorGraph.GetNodes().OfType<StartNode>().FirstOrDefault();
        if(startNode != null)
        {
            var entryPort = startNode.GetOutputPorts().FirstOrDefault()?.firstConnectedPort;
            if (entryPort != null) 
            {
                runtimeGraph.EntryNodeId = nodeIDMap[entryPort.GetNode()];
            }
        }

        foreach (var iNode in editorGraph.GetNodes())
        {
            if (iNode is StartNode || iNode is EndNode) continue;

            var runtimeNode = new RuntimeDialogNode { NodeId = nodeIDMap[iNode] };
            if(iNode is DialogueNode dialogueNode)
            {
                ProcessDialogueNode(dialogueNode, runtimeNode, nodeIDMap);
            }

            else if(iNode is ChoiceNode choiceNode)
            {
                ProcessChoiceNode(choiceNode, runtimeNode, nodeIDMap);
            }

            runtimeGraph.AllNodes.Add(runtimeNode);
        }

        ctx.AddObjectToAsset("RuntimeData", runtimeGraph);
        ctx.SetMainObject(runtimeGraph);
    }

    private void ProcessDialogueNode(DialogueNode node, RuntimeDialogNode runtimeNode, Dictionary<INode, string> nodeIDMap)
    {
        node.GetNodeOptionByName(DialogueNode.SpeakerOpt).TryGetValue(out DialogKey speakerKey);
        runtimeNode.SpeakerName = speakerKey.ToString();

        node.GetNodeOptionByName(DialogueNode.DialogueOpt).TryGetValue(out DialogKey dialogueKey);
        runtimeNode.DialogueText = dialogueKey.ToString();

        var nextNodePort = node.GetOutputPortByName("out")?.firstConnectedPort;
        if (nextNodePort != null)
            runtimeNode.NextNodeId = nodeIDMap[nextNodePort.GetNode()];

        if (node.GetNodeOptionByName(DialogueNode.modeId).TryGetValue(out DialogueMode mode))
        {
            runtimeNode.Mode = mode;
        }
        else
        {
            runtimeNode.Mode = DialogueMode.Panel;
        }
    }

    private void ProcessChoiceNode(ChoiceNode node, RuntimeDialogNode runtimeNode, Dictionary<INode, string> nodeIDMap)
    {
        node.GetNodeOptionByName(ChoiceNode.SpeakerOpt).TryGetValue(out DialogKey speakerKey);
        runtimeNode.SpeakerName = speakerKey.ToString();

        node.GetNodeOptionByName(ChoiceNode.DialogueOpt).TryGetValue(out DialogKey dialogueKey);
        runtimeNode.DialogueText = dialogueKey.ToString();

        var choiceOutputPorts = node.GetOutputPorts().Where(p => p.name.StartsWith("Choice "));

        foreach (var outputPort in choiceOutputPorts)
        {
            var index = outputPort.name.Substring("Choice ".Length);
            var textPort = node.GetInputPortByName($"Choice Text {index}");
            string choiceKeyString = GetPortEnumString<DialogKey>(textPort);

            var choiceData = new ChoiceData
            {
                ChoiceText = choiceKeyString,
                DestinationNodeId = outputPort.firstConnectedPort != null ? nodeIDMap[outputPort.firstConnectedPort.GetNode()] : null
            };

            runtimeNode.Choices.Add(choiceData);
        }

        if (node.GetNodeOptionByName(ChoiceNode.modeId).TryGetValue(out DialogueMode mode))
        {
            runtimeNode.Mode = mode;
        }
        else
        {
            runtimeNode.Mode = DialogueMode.Panel;
        }

    }

    private string GetPortEnumString<T>(IPort port) where T : Enum
    {
        if (port == null) return string.Empty;

        if (port.isConnected)
        {
            if (port.firstConnectedPort.GetNode() is IVariableNode variableNode)
            {
                return "VARIABLE_NOT_SUPPORTED_FOR_KEYS";
            }
        }
        if (port.TryGetValue(out T enumValue))
        {
            return enumValue.ToString();
        }

        return string.Empty;
    }
}
