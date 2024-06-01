using ChessChallenge.API;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleverBot.CleverBotV1;

internal class Node
{
    internal Dictionary<Move, Node> Connections { get; private set; }
    internal float Evaluation { get; private set; }

    internal readonly int PlyCount;
    internal readonly bool White;

    internal Node(int plyCount, bool white, float score = 0f)
    {
        PlyCount = plyCount;
        White = white;

        Evaluation = score;
    }

    #region Growing

    internal void Grow(Board board)
    {
        if (IsLeaf) Sprout(board);
        else Branch(board);

        if (!IsTerminal) Evaluate();
    }

    private void Sprout(Board board)
    {
        Connections = new();

        Move[] legalMoves = board.GetLegalMoves();
        foreach (Move move in legalMoves)
        {
            board.MakeMove(move);

            float moveScore = Evaluator.EvaluateBoard(board);
            Node node = new Node(PlyCount + 1, !White, moveScore);
            Connections.Add(move, node);

            board.UndoMove(move);
        }
    }

    private void Branch(Board board)
    {
        foreach (var connection in Connections)
        {
            Move connectionMove = connection.Key;
            board.MakeMove(connectionMove);

            connection.Value.Grow(board);

            board.UndoMove(connectionMove);
        }
    }

    #endregion

    #region Evaluation

    private void Evaluate()
    {
        Evaluation = White ?
            Connections.Values.Max(n => n.Evaluation) :
            Connections.Values.Min(n => n.Evaluation);
    }

    #endregion

    #region Node Types

    private bool IsLeaf => Connections == null;
    private bool IsTerminal => Connections.Count == 0;

    #endregion

    #region Printing

    private static readonly int _printDepth = 1;

    public string ToString(int depth = 0)
    {
        StringBuilder sb = new();
        sb.AppendLine($"PlyCount: {PlyCount} | {(White ? "White" : "Black")} to move | Evaluation: {Evaluation}");

        if (!IsLeaf && depth < _printDepth)
        {
            foreach (var c in Connections)
            {
                for (int i = 0; i < depth; i++) sb.Append("   ");
                sb.Append($"{c.Key} -> {c.Value.ToString(depth + 1)}");
            }
        }

        return sb.ToString();
    }

    #endregion
}