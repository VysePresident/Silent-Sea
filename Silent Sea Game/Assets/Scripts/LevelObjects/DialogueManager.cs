/*
 Function coded by Isaac Burns
 Mostly followed a Brackeys tutorial:
 https://www.youtube.com/watch?v=_nRzoTzeyxU
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Queue<string> sentences;
    public static bool DialogueActive = false;
    public static bool Healing;
    private Text nameText;
    public Text dialogueText;

    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
    }

    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.E) && DialogueActive == true)
        {
            DisplayNextSentence();
        }*/
    }

    public void StartDialogue(Dialogue dialogue)
    {
        animator.SetBool("IsOpen", true);
        DialogueActive = true;
        sentences.Clear();
        //sentences.Enqueue(" ");
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        print("Number of sentences left: " + sentences.Count);
        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;
    }

    /*public new void OnTriggerExit2D(Collider2D collision)
    {
        // Get the Interact tooltip to disappear
        if (collision.CompareTag("Player") && DialogueActive == true)
        {
            print("I left the conversation");
            EndDialogue();
        }
    }*/

    public void EndDialogue()
    {
        //Debug.Log("Exit Dialogue");
        DialogueActive = false;
        animator.SetBool("IsOpen", false);
    }
}
