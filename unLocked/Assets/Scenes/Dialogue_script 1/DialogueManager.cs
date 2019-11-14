using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Animator animator;
    public Text nameText;
    public Text dialogueText;

    private Queue<string> sentences;


    public void StartDialogue(Dialogue dialogue){
        Debug.Log("Starting dialogue with "+dialogue.name);
        animator.SetBool("IsOpen", true);
        
        foreach (string sentence in dialogue.sentences){
            sentences.Enqueue(sentence);
            Debug.Log(sentence);
        }

        nameText.text = dialogue.name;
        DisplayNextSentence();

    }

    public void EndDialogue(){
        Debug.Log("Dialogue ended");
        animator.SetBool("IsOpen", false);

    }

    IEnumerator TypeSentence(string sentence){
        dialogueText.text = "";
        
        foreach (char letter in sentence.ToCharArray()){
            dialogueText.text += letter;
            yield return null;
        }
    }

    public void DisplayNextSentence(){
        string sentence;
        if (sentences.Count == 0){
            EndDialogue();
        }
        else{
            sentence = sentences.Dequeue();
            Debug.Log(sentence);
            // dialogueText.text = sentence;
            StartCoroutine(TypeSentence(sentence));
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
        
    }
}
