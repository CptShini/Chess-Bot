﻿using System;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.CleverBot.CleverBotV1;

internal class NodeTree
{
    private Node _root;
    private readonly int _plyCountOffset;

    internal NodeTree(Board board)
    {
        _root = new Node(board.PlyCount, board.IsWhiteToMove);
        _plyCountOffset = board.PlyCount - board.GameMoveHistory.Length;

        Grow(board, 2);
    }

    internal Move GetMove()
    {
        ScoredMoveList moveList = new ScoredMoveList();

        foreach (var connection in _root.Connections)
        {
            moveList.Add(connection.Key, connection.Value.Evaluation);
        }

        Move move = moveList.GetMove(_root.White);
        Console.WriteLine($"CleverBotV1: {move} | Evaluation: {moveList.GetEval(_root.White):0.00}");

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

        Grow(board, 2);
    }

    private void Grow(Board board, int n)
    {
        for (int i = 0; i < n; i++) _root.Grow(board);
    }

    internal void PrintTree() => Console.WriteLine(_root.ToString());
}