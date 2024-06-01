using ChessChallenge.API;
using CleverBot.CleverBotV3.Evaluations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleverBot.CleverBotV3.Nodes;

internal class Node
{
    internal Dictionary<Move, Node> Connections { get; private set; }
    internal Evaluation Evaluation { get; private set; }

    internal readonly int PlyCount;
    internal readonly bool White;

    internal Node(int plyCount, bool white, float score = 0f)
    {
        PlyCount = plyCount;
        White = white;

        Evaluation = new Evaluation(white, score);
    }

    #region Growing

    internal void Grow(Board board)
    {
        if (IsLeaf) Sprout(board);
        else if (!IsTerminal) Branch(board);
    }

    internal void GrowNext(Board board)
    {
        if (IsLeaf) Sprout(board);
        else if (!IsTerminal)
        {
            foreach (Move move in Connections.Keys)
            {
                Node subNode = Connections[move];
                if (!subNode.IsLeaf) continue;

                board.MakeMove(move);
                subNode.Sprout(board);
                board.UndoMove(move);

                return;
            }
        }
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
        float moveScore = Evaluator.EvaluateMove(board, move, White, Evaluation.Score);
        Node moveNode = new Node(PlyCount + 1, !White, moveScore);
        Connections.Add(move, moveNode);

        Evaluation.AddEvaluation(moveNode.Evaluation);
    }

    #endregion

    #region Interior Nodes

    private void Branch(Board board) => TraverseAndExecute(board, Connections.Keys, GrowSubNode);

    private void GrowSubNode(Board board, Move move) => Connections[move].Grow(board);

    #endregion

    #endregion

    #region Node Types

    private bool IsLeaf => Connections == null;
    private bool IsTerminal => !IsLeaf && Connections.Count == 0;

    #endregion

    #region Utilities

    private static void TraverseAndExecute(Board board, IEnumerable<Move> moves, Action<Board, Move> traversalFunc)
    {
        foreach (Move move in moves)
        {
            board.MakeMove(move);

            traversalFunc.Invoke(board, move);

            board.UndoMove(move);
        }
    }

    #region Printing

    public string ToString(int topN = -1, int maxDepth = -1, int depth = 0)
    {
        StringBuilder sb = new();
        if (depth == 0) sb.AppendLine($"PlyCount: {PlyCount} | {(White ? "White" : "Black")} to move | {Evaluation.ToString()}");

        if (!IsLeaf && !IsTerminal && (maxDepth == -1 || depth < maxDepth))
        {
            var orderedConnections = Connections.OrderBy(con => White ? -con.Value.Evaluation.Score : con.Value.Evaluation.Score);
            if (depth == 0)
            {
                foreach (var conn in topN == -1 ? orderedConnections : orderedConnections.Take(topN))
                {
                    sb.Append($"    {conn.Value.Evaluation.ToString()}  \t | ");
                    sb.Append($"{conn.Key.StartSquare.Name}{conn.Key.TargetSquare.Name} {conn.Value.ToString(topN, maxDepth, depth + 1)}");
                    sb.AppendLine();
                }
            }
            else
            {
                var conn = orderedConnections.First();
                sb.Append($"{conn.Key.StartSquare.Name}{conn.Key.TargetSquare.Name} {conn.Value.ToString(topN, maxDepth, depth + 1)}");
            }
        }

        return sb.ToString();
    }

    #endregion

    #endregion
}