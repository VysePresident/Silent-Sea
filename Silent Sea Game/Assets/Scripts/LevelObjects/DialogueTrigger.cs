/*
 Function coded by Isaac Burns
 Mostly followed a Brackeys tutorial:
 https://www.youtube.com/watch?v=_nRzoTzeyxU
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    private bool inCollider = false;
    public bool HealStation = false;
    public bool SaveStation = false;
    private DialogueManager dialogMng;
    //public GameObject Interact_E;

    void Start()
    {
        dialogMng = FindObjectOfType<DialogueManager>();
    }

    public void TriggerDialogue()
    {
        //dialogMng = FindObjectOfType<DialogueManager>();
        if (DialogueManager.DialogueActive == false)
        {
            dialogMng.StartDialogue(dialogue);
        }
        else
        {
            if (HealStation == true)
            {
                Player_Health_Controller.healingStationUsed = true;
            }
            if (SaveStation == true)
            {
                Player_Health_Controller.saveStationUsed = true;
            }
            dialogMng.DisplayNextSentence();
        }
        //FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        //print("Activate the splash screen!");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && inCollider == true)
        {
            TriggerDialogue();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //print("Player entered the collider.");
            inCollider = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //print("Player left the collider.");
            inCollider = false;
            dialogMng.EndDialogue();
        }
    }
}
