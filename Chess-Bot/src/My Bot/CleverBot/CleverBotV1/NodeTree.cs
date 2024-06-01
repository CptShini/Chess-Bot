using ChessChallenge.API;
using System;

namespace CleverBot.CleverBotV1;

internal class NodeTree
{
    private Node _root;
    private readonly int _plyCountOffset;

    internal NodeTree(Board board)
    {
        _root = new Node(board.PlyCount, board.IsWhiteToMove);
        _plyCountOffset = board.PlyCount - board.GameMoveHistory.Length;

        Grow(board);
    }

    internal Move GetMove()
    {
        ScoredMoveList moveList = new ScoredMoveList();

        foreach (var connection in _root.Connections)
        {
            moveList.Add(connection.Key, connection.Value.Evaluation);
        }

        Move move = moveList.GetMove(_root.White);
        Console.WriteLine($"{move} | Eval: {moveList.GetEval(_root.White)}");

        return move;
    }

    internal void Update(Board board)
    {
        for (int i = _root.PlyCount - _plyCountOffset; i < board.GameMoveHistory.Length; i++)
        {
            Move move = board.GameMoveHistory[i];
            //Console.WriteLine($"{move} | PlyCount: {_root.PlyCount:00} | Eval: {_root.Connections[move].Evaluation}");

            _root = _root.Connections[move];
        }

        Grow(board);
    }

    private void Grow(Board board, int n = 2)
    {
        for (int i = 0; i < n; i++) _root.Grow(board);
    }

    internal void PrintTree() => Console.WriteLine(_root.ToString());
}