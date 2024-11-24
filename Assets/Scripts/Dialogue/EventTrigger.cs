using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    public DialogueManager dialogueManager;
    
    public void TriggerEvent(string dialogueID)
    {

        dialogueManager.TriggerDialogue(dialogueID);
    }
}
