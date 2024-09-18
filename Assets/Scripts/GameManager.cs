using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ron.Utility;

public class GameManager : MonoBehaviour
{
    [ReadOnly] public int currentRound = 1;
    [ReadOnly] public TurnState gameState = TurnState.PlayerOneTurn;
    public Player[] players = new Player[2];

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SwitchTurn();
        }
    }

    public void StartGame()
    {
        // Basically restart everything here. Anything you can think of.
        currentRound = 1;
        gameState = TurnState.PlayerOneTurn;
        Debug.Log("Game Start.");

        // Initialize every players.
        foreach(Player player in players)
        {
            player.currentHealth = player.initialHealth;
            player.canMove = false;
        }

        // Randomly choose who goes first.
        int random = Random.Range(0, 2);
        players[random].playerOrder = PlayerOrder.First;
        players[(random + 1) % 2].playerOrder = PlayerOrder.Second;
    }

    public void SwitchTurn()
    {
        if(gameState == TurnState.PlayerOneTurn)
        {
            gameState = TurnState.PlayerTwoTurn;
            Debug.Log("Player Two Turn");
        }
        else
        {
            gameState = TurnState.PlayerOneTurn;
            currentRound++;
            Debug.Log("Round " + currentRound + ", Player One Turn");
        }
    }

}

public enum TurnState
{
    PlayerOneTurn,
    PlayerTwoTurn
}
