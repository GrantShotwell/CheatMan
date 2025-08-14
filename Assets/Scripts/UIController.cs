using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Device;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;
using System;
using DG.Tweening;

public class UIController : MonoBehaviour
{
    [SerializeField] Text distanceTraveled;
    [SerializeField] Text collectedCoins;
    [SerializeField] GameObject gameOverScreen;

    [SerializeField] PlayerController player;
    [SerializeField] GameObject gameMusic;
    [SerializeField] GameObject sky;

    public void GameStart()
    {
        Debug.Log("Game Start!");
        var playerInput = player.GetComponent<PlayerInput>();
        playerInput.actions.FindActionMap("Player").Enable();
    }
    public void GameRestart()
    {
        Debug.Log("Restart the Game");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowGameOverScreen()
    {
        //sky.SetActive(false);
        gameOverScreen.SetActive(true);
        //float roundedDistance = Mathf.Ceil(player.distanceTravelled);
        //distanceTraveled.text = roundedDistance.ToString();
        //collectedCoins.text = player.collectedCoins.ToString();
    }
}
