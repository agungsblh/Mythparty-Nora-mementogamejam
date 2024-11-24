using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Items", menuName = "Assets/Items")]
public class Items : ScriptableObject
{
    public string id;
    public string name;
    public bool clue;
    public bool interactable;
    public bool keyItem;
    public bool taken;
    public Sprite spriteChange;
    public bool needKey;
    public bool canChange;
    public AudioClip sfx;

}
