using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public RuntimeDialogGraph RuntimeGraph;

    [Header("UI Components")]
    public GameObject DialogPanel;
    public TextMeshProUGUI SpeakerNameText;
    public TextMeshProUGUI DialogText;

    [Header("Button Choices")]
    public Button ChoiceButtonPrefab;
    public Transform ChoiceButtonContainer;

    private Dictionary<string, RuntimeDialogNode> nodeLookup = new Dictionary<string, RuntimeDialogNode>();
    private RuntimeDialogNode currentNode;

    private void Start()
    {
        foreach (var node in RuntimeGraph.AllNodes)
        {
            nodeLookup[node.NodeId] = node;
        }

        if(!string.IsNullOrEmpty(RuntimeGraph.EntryNodeId))
        {
            ShowNode(RuntimeGraph.EntryNodeId);
        }
        else
        {
            EndDialogue();
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && currentNode != null && currentNode.Choices.Count == 0)
        {
            if (!string.IsNullOrEmpty(currentNode.NextNodeId))
            {
                ShowNode(currentNode.NextNodeId);
            }
            else
            {
                EndDialogue();
            }
        }
    }

    private void ShowNode(string nodeId)
    {
        if(!nodeLookup.ContainsKey(nodeId))
        {
            EndDialogue();
            return;
        }
        currentNode = nodeLookup[nodeId];
        DialogPanel.SetActive(true);
        SpeakerNameText.text = currentNode.SpeakerName;
        DialogText.text = currentNode.DialogueText;

        foreach (Transform child in ChoiceButtonContainer)
        {
            Destroy(child.gameObject);
        }

        if(currentNode.Choices.Count > 0)
        {
            foreach (var choice in currentNode.Choices)
            {
                UnityEngine.UI.Button button = Instantiate(ChoiceButtonPrefab, ChoiceButtonContainer);

                TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                if(buttonText != null)
                {
                    buttonText.text = choice.ChoiceText;
                }

                if(button != null)
                {
                    button.onClick.AddListener(() =>
                    {
                        if (!string.IsNullOrEmpty(choice.DestinationNodeId))
                        {
                            ShowNode(choice.DestinationNodeId);
                        }
                        else
                        {
                            EndDialogue();
                        }
                    });
                }
            }
        }
    }

    private void EndDialogue()
    {
        DialogPanel.SetActive(false);
        currentNode = null;

        foreach (Transform child in ChoiceButtonContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
