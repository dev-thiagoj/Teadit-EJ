using UnityEngine;

[CreateAssetMenu(menuName = "Joints/Joint Data")]
public class JointData : ScriptableObject
{
    public string jointName;
    public string code;
    public Sprite thumbnail;
    public GameObject prefab;
    public DialogueContent info;
}
