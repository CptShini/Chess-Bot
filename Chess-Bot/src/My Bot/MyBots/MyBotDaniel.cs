using System;
using System.Collections.Generic;
using System.Linq;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.MyBots;

public class MyBotDaniel : IChessBot
{
    const int MaxDepth = 4;
    static readonly float[] PieceValues = new float[] { 0, 1, 3, 3, 5, 9, 100 };
    readonly Random rand = new Random();
    Evaluation evaluation;

    public Move Think(Board board, Timer timer)
    {
        evaluation = new(board);
        (float eval, Move move) = MinMax(board, 0);
        Console.WriteLine($"Move: {move}/{move.TargetSquare.Index} | Evaluation: {eval}");
        return move;
    }

    (float, Move) MinMax(Board board, int depth, float alpha = int.MinValue, float beta = int.MaxValue)
    {
        depth++;
        float best = 0;
        Move? bestMove = null;
        Move[] legalMoves = board.GetLegalMoves();
        bool isMax = board.IsWhiteToMove;

        IEnumerable<Move> moves = depth < MaxDepth ? legalMoves.OrderBy(m => MoveScoreGuess(board, m)) : legalMoves;

        foreach (Move move in moves)
        {
            evaluation.MakeMove(move);
            float eval = evalFunc(board);
            evaluation.UndoMove(move);
            if (isMax ? eval > beta : eval < alpha) return (eval, Move.NullMove);
            if ((isMax ? eval > best : eval < best) || bestMove == null || (eval == best && rand.NextDouble() > 0.5))
            {
                best = eval;
                bestMove = move;
            }
        }

        if (bestMove != null) return (best, (Move)bestMove);
        else throw new Exception("No available moves");

        float evalFunc(Board board)
        {
            if (depth == MaxDepth || evaluation.GameHasEnded) return evaluation.Current;
            if (bestMove == null) return MinMax(board, depth, alpha, beta).Item1;
            return isMax
                ? MinMax(board, depth, Math.Max(alpha, best), beta).Item1
                : MinMax(board, depth, alpha, Math.Min(beta, best)).Item1;
        }
    }

    static float EvaluateMaterial(Board board)
    {
        if (board.IsDraw()) return 0;
        if (board.IsInCheckmate())
            return board.IsWhiteToMove ? -PieceValues[(int)PieceType.King] : PieceValues[(int)PieceType.King];

        float material = 0;
        foreach (var pieceList in board.GetAllPieceLists())
        {
            float pieceTypeValue = PieceValues[(int)pieceList.TypeOfPieceInList];
            material += (pieceList.IsWhitePieceList ? pieceTypeValue : -pieceTypeValue) * pieceList.Count;
        }

        return material;
    }

    float MoveScoreGuess(Board board, Move move)
    {
        float scoreGuess = 0;
        float movingPieceValue = PieceValues[(int)move.MovePieceType];
        float targetPieceValue = PieceValues[(int)move.CapturePieceType];

        scoreGuess += targetPieceValue;
        if (board.SquareIsAttackedByOpponent(move.TargetSquare)) scoreGuess -= movingPieceValue;
        else if (board.SquareIsAttackedByOpponent(move.StartSquare)) scoreGuess += movingPieceValue;
        return -scoreGuess;
    }

    struct Evaluation
    {
        private float eval;
        private readonly Stack<float> moveEvaluationsChanges;
        private readonly Board board;
        public float Current => eval;
        public bool GameHasEnded { get; private set; }

        private readonly static float[] KnightValueBoard = CreateValueBoard(KnightPositionalValue);
        private readonly static float[] PawnValueBoard = CreateValueBoard(PawnPositionalValue);
        private readonly static float[] BishopValueBoard = CreateValueBoard(BishopPositionalValue);

        public Evaluation(Board board)
        {
            GameHasEnded = false;
            eval = EvaluateMaterial(board);
            moveEvaluationsChanges = new Stack<float>();
            this.board = board;
        }

        public void MakeMove(Move move)
        {
            board.MakeMove(move);
            float evalChange = EvaluateMove(move);
            eval += evalChange;
            moveEvaluationsChanges.Push(evalChange);
        }

        public void UndoMove(Move move)
        {
            board.UndoMove(move);
            eval -= moveEvaluationsChanges.Pop();
            GameHasEnded = false;
        }

        private float EvaluateMove(Move move)
        {
            bool white = !board.IsWhiteToMove;
            if (board.IsDraw())
            {
                GameHasEnded = true;
                return -eval;
            }

            if (board.IsInCheckmate())
            {
                GameHasEnded = true;
                return -eval + (white ? PieceValues[(int)PieceType.King] : -PieceValues[(int)PieceType.King]);
            }

            float postionalScore = 0f;
            float materialScore = 0f;
            if (move.MovePieceType == PieceType.Knight)
                postionalScore += (white ? 1 : -1) *
                                  (KnightValueBoard[move.TargetSquare.Index] -
                                   KnightValueBoard[move.StartSquare.Index]);
            if (move.MovePieceType == PieceType.Pawn)
                postionalScore += (white ? 1 : -1) *
                                  (PawnValueBoard[move.TargetSquare.Index] - PawnValueBoard[move.StartSquare.Index]);
            if (move.MovePieceType == PieceType.Bishop)
                postionalScore += (white ? 1 : -1) *
                                  (BishopValueBoard[move.TargetSquare.Index] -
                                   BishopValueBoard[move.StartSquare.Index]);
            if (move.IsPromotion) materialScore += (white ? 1 : -1) * (PieceValues[(int)move.PromotionPieceType] - 1);
            if (move.IsCastles) postionalScore += white ? 0.5f : -0.5f;
            materialScore += (white ? 1 : -1) * PieceValues[(int)move.CapturePieceType];
            return materialScore + postionalScore;
        }

        static float mid(int x) => 3.5f - Math.Abs(x - 3.5f);
        static float inner(int x) => x > 0 && x < 7 ? 0.05f : 0f;

        static float KnightPositionalValue(int i, int j) => (mid(i) + mid(j)) * 0.03f;
        static float PawnPositionalValue(int i, int j) => (mid(i) + mid(j)) * 0.05f;
        static float BishopPositionalValue(int i, int j) => inner(i) + inner(j);

        static float[] CreateValueBoard(Func<int, int, float> boardValueFunc)
        {
            float[] valueBoard = new float[64];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    valueBoard[i * 8 + j] = boardValueFunc(i, j);
                }
            }

            return valueBoard;
        }
    }
}