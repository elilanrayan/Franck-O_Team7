using UnityEngine;

[CreateAssetMenu(fileName = "DialogueNPC", menuName = "Scriptable Objects/DialogueNPC")]
public class DialogueNPC : ScriptableObject
{
    public float value = 10;

    public void Meet()
    {
        Debug.Log("Coucou");
    }
}
