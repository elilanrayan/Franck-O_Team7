using System;
using UnityEngine;
using System.Collections.Generic;

public class RuntimeDialogGraph : ScriptableObject
{
    public string EntryNodeId;
    public List<RuntimeDialogNode> AllNodes = new List<RuntimeDialogNode>();
}


[Serializable]
public class RuntimeDialogNode
{
    public string NodeId;
    public string SpeakerName;
    public string DialogueText;
    public List<ChoiceData> Choices = new List<ChoiceData>();
    public string NextNodeId;

    public DialogueMode Mode;
}

[Serializable]
public class  ChoiceData
{
    public string ChoiceText;
    public string DestinationNodeId;
}

public enum DialogueMode
{
    Panel,
    Popup,
    Bulle
}