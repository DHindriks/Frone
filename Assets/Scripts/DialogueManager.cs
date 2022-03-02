using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Characters
{
    Zero,
    Null
}

public class DialogueManager : MonoBehaviour {

    public static DialogueManager Instance;

    Queue<KeyValuePair<string, GameObject>> Dialoguequeue = new Queue<KeyValuePair<string, GameObject>>();
     
    bool Playing = false;

    [SerializeField]
    Animator DialoguePanel;

    [SerializeField]
    Text DialogueText;

    GameObject CurrentCharacter;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddDialogue(string Text, Characters Character = Characters.Zero)
    {
        GameObject ChosenChar;

        ChosenChar = Resources.Load("Characters/" + Character.ToString()) as GameObject;

        KeyValuePair<string, GameObject> DialogueToAdd = new KeyValuePair<string, GameObject>(Text, ChosenChar);
        Debug.Log("Added: " + Text + " to queue");
        Dialoguequeue.Enqueue(DialogueToAdd);
        CheckQueue();
    }

    int CheckQueue()
    {
        if (Dialoguequeue.Count > 0 && Playing == false)
        {
            StartCoroutine("PlayNext");
        }
        else {
            Debug.LogWarning("Coroutine not started " + Dialoguequeue.Count + " " + Playing);
        }
        return Dialoguequeue.Count;
    }

    IEnumerator PlayNext()
    {
        DialoguePanel.SetBool("Open", true);
        DialoguePanel.gameObject.SetActive(true);
        Playing = true;
        KeyValuePair<string, GameObject> DialogueToPlay = Dialoguequeue.Dequeue();
        if (CurrentCharacter == null)
        {
            CurrentCharacter = Instantiate(DialogueToPlay.Value);
        }

        DialogueText.text = null;

        for (int i = 0; i < DialogueToPlay.Key.Length; i++)
        {
            DialogueText.text += DialogueToPlay.Key[i];
            DialogueText.gameObject.GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(0.075f);
        }
        yield return new WaitForSeconds(3);
        Debug.Log(DialoguePanel.GetBool("Open"));
        DialoguePanel.SetBool("Open", false);
        Playing = false;
        Debug.Log("Closing Window");

        if (Dialoguequeue.Count <= 0)
        {
            Destroy(CurrentCharacter,   0.333f);
        } else if (Dialoguequeue.Peek().Value.Equals(CurrentCharacter))
        {
            Destroy(CurrentCharacter);
        }
        

        CheckQueue();
    }
}
