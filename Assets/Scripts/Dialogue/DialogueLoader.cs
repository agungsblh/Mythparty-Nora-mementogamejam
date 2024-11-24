using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueLoader : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public DialogueData[] sceneDialogueData;

    private void Awake()
    {
        foreach (var dialogueData in sceneDialogueData)
        {
            string id = dialogueData.name;
            dialogueManager.RegisterDialogue(id, dialogueData);
        }
    }
}
