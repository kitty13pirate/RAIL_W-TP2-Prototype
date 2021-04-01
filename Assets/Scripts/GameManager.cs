using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;

    public RbCharacterMovements playerMovements;

    public bool isGameOver;

    void Awake()
    {
        if (singleton != null)
            return;

        singleton = this;
    }

    public void GameOver()
    {
        if (isGameOver)
            return;

        isGameOver = true;

        // Empecher les mouvements du joueur
        playerMovements.enabled = false;

        // Empecher les mouvements du NPC

        enemySwat[] swats = FindObjectsOfType<enemySwat>();

        foreach (enemySwat enemy in swats)
        {
            Destroy(enemy);
            enemy.enabled = false;
            enemy.isDead = true;
            enemy.GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped = true;
        }
        // Message de fin de jeu
        Debug.Log($"Fin du jen en {Time.time}s");
    }
}
