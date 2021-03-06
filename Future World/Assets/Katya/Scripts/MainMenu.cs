﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private List<GameObject> enemies;
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }

    public void LoadScene()
    {
        TargetData savedPlayer = SaveSystem.LoadPlayer();
        Vector3 position;
        position.x = savedPlayer.position[0];
        position.y = savedPlayer.position[1];
        position.z = savedPlayer.position[2];

        Debug.Log("Player Level: " + savedPlayer.level);
        Debug.Log("Player Health: " + savedPlayer.health);

        Debug.Log("Position x: " + position.x);
        Debug.Log("Position y: " + position.y);
        Debug.Log("Position z: " + position.z);
        SceneManager.LoadScene(savedPlayer.level);
        PlayerController.instance.level= savedPlayer.level;
        PlayerController.instance.health = savedPlayer.health;
        position.x = savedPlayer.position[0];
        position.y = savedPlayer.position[1];
        position.z = savedPlayer.position[2];
        PlayerController.instance.transform.position = position;



        //    TargetData[] savedEnemies = SaveSystem.LoadEnemies(savedPlayer.level);

        //    for(int i = 0; i < savedEnemies.Length; i++) {
        //       enemies.Level = savedEnemies[i].level;
        //     player.health = savedEnemies[i].health;
        //     Vector3 positionEnemy;
        //    position.x = savedEnemies[i].position[0];
        //    position.y = savedEnemies[i].position[1];
        //    position.z = savedEnemies[i].position[2];

        //    player.transform.position = position;
        //    }



    }
}
