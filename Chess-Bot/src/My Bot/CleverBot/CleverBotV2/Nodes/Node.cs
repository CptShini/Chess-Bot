using ChessChallenge.API;
using CleverBot.CleverBotV2.Evaluation;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleverBot.CleverBotV2.Nodes;

internal class Node
{
    internal Dictionary<Move, Node> Connections { get; private set; }
    internal float Evaluation { get; private set; }

    internal readonly int PlyCount;
    internal readonly bool White;

    private readonly float _score;

    internal Node(int plyCount, bool white, float score = 0f)
    {
        PlyCount = plyCount;
        White = white;

        _score = score;
        Evaluation = score;
    }

    #region Growing

    internal void Grow(Board board)
    {
        if (IsLeaf) Sprout(board);
        else if (!IsTerminal) Branch(board);
    }

    #region Leaf Nodes

    private void Sprout(Board board)
    {
        Connections = new();

        Move[] legalMoves = board.GetLegalMoves();
        TraverseAndExecute(board, legalMoves, RegisterConnection);
    }

    private void RegisterConnection(Board board, Move move)
    {
        float moveScore = EvaluateNode(board, move);
        Node node = new Node(PlyCount + 1, !White, moveScore);
        Connections.Add(move, node);
    }

    private float EvaluateNode(Board board, Move move)
    {
        float moveEvaluation = Evaluator.Evaluate(board, move, White, _score);
        UpdateEvaluation(moveEvaluation);

        return moveEvaluation;
    }

    #endregion

    #region Interior Nodes

    private void Branch(Board board)
    {
        Evaluation = White ? float.MinValue : float.MaxValue;

        TraverseAndExecute(board, Connections.Keys, GrowSubNode);
    }

    private void GrowSubNode(Board board, Move move)
    {
        Node subNode = Connections[move];
        subNode.Grow(board);

        UpdateEvaluation(subNode.Evaluation);
    }

    #endregion

    #endregion

    #region Evaluation

    private void UpdateEvaluation(float newEvaluation)
    {
        if (White)
        {
            if (newEvaluation > 80) newEvaluation--;

            bool greater = newEvaluation > Evaluation;
            if (greater) Evaluation = newEvaluation;
        }
        else
        {
            if (newEvaluation < -80) newEvaluation++;

            bool lower = newEvaluation < Evaluation;
            if (lower) Evaluation = newEvaluation;
        }
    }

    #endregion

    #region Node Types

    private bool IsLeaf => Connections == null;
    private bool IsTerminal => Connections.Count == 0;

    #endregion

    #region Utilities

    private static void TraverseAndExecute(Board board, IEnumerable<Move> moves, Action<Board, Move> onTraverse)
    {
        foreach (Move move in moves)
        {
            board.MakeMove(move);

            onTraverse.Invoke(board, move);

            board.UndoMove(move);
        }
    }

    #region Printing

    public string ToString(int maxDepth, int depth = 0)
    {
        StringBuilder sb = new();
        sb.AppendLine($"PlyCount: {PlyCount} | {(White ? "White" : "Black")} to move | Evaluation: {Evaluation:0.00}");

        if (!IsLeaf && depth < maxDepth)
        {
            foreach (var c in Connections)
            {
                for (int i = 0; i < depth; i++) sb.Append("   ");
                sb.Append($"{c.Key} -> {c.Value.ToString(maxDepth, depth + 1)}");
            }
        }

        return sb.ToString();
    }

    #endregion

    #endregion
}