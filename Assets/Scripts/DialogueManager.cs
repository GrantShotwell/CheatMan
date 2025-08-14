using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Device;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;
using System;
using DG.Tweening;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] PlayerController player;

    [SerializeField] GameObject dialogueCanvas;
    [SerializeField] GameObject grayBackground;
    [SerializeField] GameObject playerImg;
    [SerializeField] GameObject playerUI;
    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] TextMeshProUGUI playerDialogue;
    [SerializeField] GameObject npcImg;
    [SerializeField] GameObject npcUI;
    [SerializeField] TextMeshProUGUI npcName;
    [SerializeField] TextMeshProUGUI npcDialogue;

    [SerializeField] string playerDialogueName = "Kuho";

    [SerializeField] private string[] nameOfTalker;
    [SerializeField] private string[] wordsOfTalker;

    BoxCollider2D boxCollider;

    public string[] NameOfTalker { get => nameOfTalker; set => nameOfTalker = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boxCollider= GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DialogueStart()
    {
        StartCoroutine("DialogueCoroutine");
    }
    private IEnumerator DialogueCoroutine()
    {
        var playerInput = player.GetComponent<PlayerInput>();
        playerInput.actions.FindActionMap("Player").Disable();

        dialogueCanvas.SetActive(true);
        playerImg.SetActive(true);
        npcImg.SetActive(true);
        playerName.text = "";
        npcName.text = "";
        for (int i = 0; i < wordsOfTalker.Length; i++)
        {
            if (nameOfTalker[i] == playerDialogueName)
            {
                Debug.Log("Kuho area");
                npcName.text = "";
                npcDialogue.text = "";
                npcUI.SetActive(false);
                playerImg.SetActive(true);
                playerUI.SetActive(true);
                playerName.text = nameOfTalker[i].ToString();
                playerDialogue.text = wordsOfTalker[i].ToString();
            }
            else
            {
                playerName.text = "";
                playerDialogue.text = "";
                playerUI.SetActive(false);
                npcImg.SetActive(true);
                npcUI.SetActive(true);
                npcName.text = nameOfTalker[i].ToString();
                npcDialogue.text = wordsOfTalker[i].ToString();
            }
            yield return null;
            while (!Input.GetMouseButtonDown(0)) yield return null;
        }
        playerImg.SetActive(false);
        playerUI.SetActive(false);
        npcImg.SetActive(false);
        npcUI.SetActive(false);

        playerName.text = "";
        playerDialogue.text = "";
        npcName.text = "";
        npcDialogue.text = "";

        dialogueCanvas.SetActive(false);
        playerInput.actions.FindActionMap("Player").Enable();
    }

    private IEnumerator DisableInput()
    {
        var playerInput = player.GetComponent<PlayerInput>();
        playerInput.actions.FindActionMap("Player").Disable();
        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Kuho"))
        {
            DialogueStart();
        }
        //DialogueStart();
        //boxCollider.gameObject.SetActive(false);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Kuho"))
        {
            boxCollider.gameObject.SetActive(false);
        }
    }
}
