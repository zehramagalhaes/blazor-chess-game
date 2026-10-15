using Chess.Core.Enums;

namespace Chess.Core.Models;

public class Game
{
    public Board Board { get; private set; } = new Board();
    public PieceColor CurrentTurn { get; private set; } = PieceColor.White;
    public bool IsGameOver { get; private set; } = false;
    public PieceColor? Winner { get; private set; } = null;
    public string? GameOverReason { get; private set; } = null;
    public List<GameLogEntry> Logs { get; } = new();
    public List<Piece> CapturedWhitePieces { get; } = new();
    public List<Piece> CapturedBlackPieces { get; } = new();
    public int MoveCount { get; private set; } = 0;

    public Game()
    {
        Logs.Add(new GameLogEntry
        {
            Type = LogType.Info,
            Message = "Game started. White makes the first move."
        });
    }

    public List<Position> GetLegalMoves(Position position)
    {
        if (IsGameOver) return new List<Position>();

        var piece = Board.GetPieceAt(position);
        if (piece == null || piece.Color != CurrentTurn)
        {
            return new List<Position>();
        }

        return Board.GetLegalMoves(position);
    }

    public bool MakeMove(Position from, Position to)
    {
        if (IsGameOver) return false;

        var piece = Board.GetPieceAt(from);
        if (piece == null || piece.Color != CurrentTurn) return false;

        var legalMoves = Board.GetLegalMoves(from);
        if (!legalMoves.Contains(to)) return false;

        // Execute the move
        var moveResult = Board.ExecuteMove(from, to);
        MoveCount++;

        // 1. Log move from position A to position B
        var moveDescription = $"#{MoveCount} {moveResult.MovedPiece.Color} {moveResult.MovedPiece.Type} moved from {moveResult.From.ToAlgebraic()} to {moveResult.To.ToAlgebraic()}";
        if (moveResult.IsCastling)
        {
            moveDescription += " (Castling)";
        }
        if (moveResult.IsPromotion)
        {
            moveDescription += $" and was promoted to Queen!";
        }
        else
        {
            moveDescription += ".";
        }

        Logs.Add(new GameLogEntry
        {
            Type = LogType.Move,
            Message = moveDescription
        });

        // 2. Log when a piece killed an adversary
        if (moveResult.CapturedPiece != null)
        {
            if (moveResult.CapturedPiece.Color == PieceColor.White)
            {
                CapturedWhitePieces.Add(moveResult.CapturedPiece);
            }
            else
            {
                CapturedBlackPieces.Add(moveResult.CapturedPiece);
            }

            var killDescription = $"⚔️ {moveResult.MovedPiece.Color} {moveResult.MovedPiece.Type} killed {moveResult.CapturedPiece.Color} {moveResult.CapturedPiece.Type} at {moveResult.To.ToAlgebraic()}";
            if (moveResult.IsEnPassant)
            {
                killDescription += " (En Passant)!";
            }
            else
            {
                killDescription += "!";
            }

            Logs.Add(new GameLogEntry
            {
                Type = LogType.Kill,
                Message = killDescription
            });
        }

        var opponentColor = CurrentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;

        // 3. Stop game once the king is dead and declare the winner
        if (moveResult.CapturedPiece?.Type == PieceType.King || Board.IsKingDead(opponentColor))
        {
            IsGameOver = true;
            Winner = CurrentTurn;
            GameOverReason = $"{opponentColor} King is DEAD! {CurrentTurn} wins the game!";

            Logs.Add(new GameLogEntry
            {
                Type = LogType.GameOver,
                Message = $"👑 {GameOverReason}"
            });
            return true;
        }

        // 4. Check for Checkmate
        if (Board.IsCheckmate(opponentColor))
        {
            IsGameOver = true;
            Winner = CurrentTurn;
            GameOverReason = $"Checkmate! {opponentColor} King has no legal escape. {CurrentTurn} wins the game!";

            Logs.Add(new GameLogEntry
            {
                Type = LogType.Checkmate,
                Message = $"🏆 {GameOverReason}"
            });
            return true;
        }

        // 5. Check for Stalemate
        if (Board.IsStalemate(opponentColor))
        {
            IsGameOver = true;
            Winner = null;
            GameOverReason = $"Stalemate! {opponentColor} has no legal moves and is not in check. Game drawn.";

            Logs.Add(new GameLogEntry
            {
                Type = LogType.GameOver,
                Message = $"🤝 {GameOverReason}"
            });
            return true;
        }

        // 6. Check for Insufficient Material (King vs King tie)
        if (Board.HasInsufficientMaterial())
        {
            IsGameOver = true;
            Winner = null;
            GameOverReason = "Draw! Insufficient material to checkmate (King vs King).";

            Logs.Add(new GameLogEntry
            {
                Type = LogType.GameOver,
                Message = $"🤝 TIE! {GameOverReason}"
            });
            return true;
        }

        // 7. Log whenever the king is at risk of a check-mate
        if (Board.IsInCheck(opponentColor))
        {
            Logs.Add(new GameLogEntry
            {
                Type = LogType.Check,
                Message = $"⚠️ {opponentColor} King is in CHECK! (At risk of check-mate)"
            });
        }

        // Switch turn
        CurrentTurn = opponentColor;
        return true;
    }

    public void SwitchTurn()
    {
        CurrentTurn = CurrentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;
    }

    public void RestartGame()
    {
        Board = new Board();
        CurrentTurn = PieceColor.White;
        IsGameOver = false;
        Winner = null;
        GameOverReason = null;
        MoveCount = 0;
        CapturedWhitePieces.Clear();
        CapturedBlackPieces.Clear();
        Logs.Clear();
        Logs.Add(new GameLogEntry
        {
            Type = LogType.Info,
            Message = "New game started. White makes the first move."
        });
    }
}