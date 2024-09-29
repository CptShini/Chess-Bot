using System;
using Chess_Challenge.My_Bot.CleverBot.CleverBotV2.ScoredMoves;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.CleverBot.CleverBotV2.Nodes;

internal class NodeTree
{
    private Node _root;
    private int _movesMade;

    internal NodeTree(Board board)
    {
        _movesMade = board.GameMoveHistory.Length;

        _root = new Node(board.PlyCount, board.IsWhiteToMove);
        Grow(board, 2);
    }

    internal ScoredMove GetMove()
    {
        ScoredMoveList moveList = new(_root.White);

        foreach (var connection in _root.Connections)
        {
            moveList.Add(connection.Key, connection.Value.Evaluation);
        }

        ScoredMove move = moveList.Move;
        Console.WriteLine($"CleverBotV2: {move}");

        return move;
    }

    internal void Update(Board board)
    {
        while (_movesMade < board.GameMoveHistory.Length)
        {
            Move move = board.GameMoveHistory[_movesMade++];
            _root = _root.Connections[move];
        }

        Grow(board, 2);
    }

    private void Grow(Board board, int n)
    {
        for (int i = 0; i < n; i++) _root.Grow(board);
    }

    internal void PrintTree(int maxDepth = 1) => Console.WriteLine(_root.ToString(maxDepth));
}