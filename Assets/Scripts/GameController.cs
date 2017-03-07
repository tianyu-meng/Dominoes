﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class GameController : MonoBehaviour
{
    private const int maxScore = 150;

    public TileController tile;
    public HistoryController history;
    public PlayerController player1;
    public PlayerController player2;
    public Text scoreText1;
    public Text scoreText2;

    private bool isPlayer1Blocked;
    private bool isPlayer2Blocked;
    private int scoreOfPlayer1;
    private int scoreOfPlayer2;

    void Start()
    {
        Assert.IsNotNull(tile);
        Assert.IsNotNull(history);
        Assert.IsNotNull(player1);
        Assert.IsNotNull(player2);

        isPlayer1Blocked = false;
        isPlayer2Blocked = false;
        scoreOfPlayer1 = 0;
        scoreOfPlayer2 = 0;

        // Reset a hand
        tile.Shuffle();
        player1.AddDomino();
        player2.AddDomino();
        player1.PlayDomino();
    }

    public void PlayerPlayDomino(PlayerController player, DominoController domino, DominoController anotherDomino)
    {
        Assert.IsNotNull(player);
        Assert.IsNotNull(domino);

        if (player == player1)
        {
            isPlayer1Blocked = false;
        }
        else
        {
            isPlayer2Blocked = false;
        }
        // Put the played domino into history
        history.Add(domino, anotherDomino);
        // Calculate the score of current play
        ScoreByCurrentPlay(player);

        // Ending a hand
        if (player.dominoControllers.Count == 0)
        {
            // Calculate the score by ending a hand
            if (player == player1)
            {
                ScoreByEndingHand(player, GetSumOfDominoInHand(player2));
            }
            else
            {
                ScoreByEndingHand(player, GetSumOfDominoInHand(player1));
            }
            if (scoreOfPlayer1 >= maxScore || scoreOfPlayer2 >= maxScore)
            {
                return;
            }
            // TOFIX: Reset a hand
            tile.Shuffle();
            player1.AddDomino();
            player2.AddDomino();
            if (player == player1)
            {
                player1.PlayDomino();
            }
            else
            {
                player2.PlayDomino();
            }
            return;
        }
        // Or ending a turn
        if (player == player1)
        {
            if (player2.HasCardToPlay())
            {
                player2.PlayDomino();
            }
            else
            {
                player2.DrawDomino();
                player2.PlayDomino();
            }

        }
        else
        {
            if (player1.HasCardToPlay())
            {
                player1.PlayDomino();
            }
            else
            {
                player1.DrawDomino();
                player1.PlayDomino();
            }
        }
    }

    public void PlayerIsBlocked(PlayerController player)
    {
        Assert.IsNotNull(player);

        if (player == player1)
        {
            isPlayer1Blocked = true;
        }
        else
        {
            isPlayer2Blocked = true;
        }

        // Ending a hand
        if (isPlayer1Blocked && isPlayer2Blocked)
        {
            int player1DominoSum = GetSumOfDominoInHand(player1);
            int player2DominoSum = GetSumOfDominoInHand(player2);
            if (player1DominoSum > player2DominoSum)
            {
                ScoreByEndingHand(player2, player1DominoSum - player2DominoSum);
            }
            else
            {
                ScoreByEndingHand(player1, player2DominoSum - player1DominoSum);
            }
            // TODO: Reset a hand
        }
    }

    void ScoreByCurrentPlay(PlayerController player)
    {
        int sum = GetSumOfHistoryDominoes();
        if (sum % 5 == 0)
        {
            if (player == player1)
            {
                scoreOfPlayer1 += sum;
            }
            else
            {
                scoreOfPlayer2 += sum;
            }
            UpdateScore();
        }
    }

    void ScoreByEndingHand(PlayerController player, int score)
    {
        if (score % 5 < 3)
        {
            score = score / 5 * 5;
        }
        else
        {
            score = (score / 5 + 1) * 5;
        }
        if (player == player1)
        {
            scoreOfPlayer1 += score;
        }
        else
        {
            scoreOfPlayer2 += score;
        }
        UpdateScore();
    }

    int GetSumOfHistoryDominoes()
    {
        // Only 1 domino in history
        if (history.horizontalDominoes.Count == 1)
        {
            DominoController domino = history.horizontalDominoes[0];          
            if (domino.direction == DominoController.Direction.Horizontal)
            {
                return domino.leftValue + domino.rightValue;
            }
            else
            {
                return domino.upperValue + domino.lowerValue;
            }
        }
        int sum = 0;
        DominoController leftDomino = history.horizontalDominoes[0];
        if (leftDomino.direction == DominoController.Direction.Horizontal)
        {
            sum += leftDomino.leftValue;
        }
        else
        {
            sum += leftDomino.upperValue + leftDomino.lowerValue;
        }
        DominoController rightDomino = history.horizontalDominoes[history.horizontalDominoes.Count - 1];
        if (rightDomino.direction == DominoController.Direction.Horizontal)
        {
            sum += rightDomino.rightValue;
        }
        else
        {
            sum += rightDomino.upperValue + rightDomino.lowerValue;
        }
        // Have dominoes except spinner
        if (history.verticalDominoes.Count > 1)
        {
            DominoController upperDomino = history.verticalDominoes[0];
            if (upperDomino != history.spinner)
            {
                if (upperDomino.direction == DominoController.Direction.Vertical)
                {
                    sum += upperDomino.upperValue;
                }
                else
                {
                    sum += upperDomino.leftValue + upperDomino.rightValue;
                }
            }
            DominoController lowerDomino = history.verticalDominoes[history.verticalDominoes.Count - 1];
            if (lowerDomino != history.spinner)
            {
                if (lowerDomino.direction == DominoController.Direction.Vertical)
                {
                    sum += lowerDomino.lowerValue;
                }
                else
                {
                    sum += lowerDomino.leftValue + lowerDomino.rightValue;
                }
            }
        }
        return sum;
    }

    int GetSumOfDominoInHand(PlayerController player)
    {
        int sum = 0;
        foreach (DominoController domino in player.dominoControllers)
        {
            sum += domino.upperValue + domino.lowerValue;
        }
        return sum;
    }

    void UpdateScore()
    {
        scoreText1.text = "Player1: " + scoreOfPlayer1;
        scoreText2.text = "Player2: " + scoreOfPlayer2;
    }
    
    void ResetHand()
    {
        // TODO
    }
}
