using UnityEngine;

[CreateAssetMenu(menuName = "Configurações/Dialogue")]
public class DialogueContent : ScriptableObject
{
    [SerializeField] int id;
    [SerializeField, TextArea(5, 10)] string question;
    [SerializeField] string[] answers;

    public int Id => id;
    public string Question => question;
    public string[] Answers => answers;
}
