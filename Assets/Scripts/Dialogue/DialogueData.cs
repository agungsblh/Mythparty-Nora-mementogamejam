using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueData", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public class DialogueLine
    {
        public string characterName;
        [TextArea] public string text;
        public Sprite characterSprite1;
        public Sprite characterSprite2;
    }
    public DialogueLine[] dialogueLines;
}
