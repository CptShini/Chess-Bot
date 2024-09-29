using System;
using System.Collections.Generic;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.CleverBot.CleverBotV3.Nodes;

internal class NodeTree
{
    private Node _root;
    private int _movesMade;

    private readonly int _printAmount;

    private readonly Random _random = new();

    internal NodeTree(Board board, int printAmount = 0)
    {
        _movesMade = board.GameMoveHistory.Length;

        _root = new Node(board.PlyCount, board.IsWhiteToMove);
        Grow(board, 2);

        _printAmount = printAmount;
    }

    internal Move GetMove()
    {
        List<KeyValuePair<Move, Node>> bestConnections = new();

        KeyValuePair<Move, Node> bestConnection = new KeyValuePair<Move, Node>(Move.NullMove, null);
        foreach (var connection in _root.Connections)
        {
            int comparer = bestConnection.Value == null
                ? 1
                : connection.Value.Evaluation.CompareTo(bestConnection.Value.Evaluation);

            if (comparer is 1)
            {
                bestConnection = connection;
                bestConnections.Clear();
            }

            if (comparer is 0 or 1) bestConnections.Add(connection);
        }

        var conn = bestConnections[_random.Next(bestConnections.Count)];

        Console.WriteLine($"CleverBotV3: {conn.Key} | Evaluation: {conn.Value.Evaluation:0.00}");

        return conn.Key;
    }

    internal void Update(Board board)
    {
        while (_movesMade < board.GameMoveHistory.Length)
        {
            Move move = board.GameMoveHistory[_movesMade++];
            _root = _root.Connections[move];
        }

        Grow(board, 2);

        ReEvaluate();
        //if (_printAmount != 0) PrintTree();
    }

    internal void Think(Board board, Timer timer)
    {
        int allocatedTime = timer.MillisecondsRemaining / 120;
        while (timer.MillisecondsElapsedThisTurn < allocatedTime)
        {
            _root.GrowNext(board);
        }
    }

    private void ReEvaluate() => _root.Evaluation.ReEvaluate();

    private void Grow(Board board, int n)
    {
        for (int i = 0; i < n; i++) _root.Grow(board);
    }

    internal void PrintTree() => Console.WriteLine(_root.ToString(_printAmount));
}