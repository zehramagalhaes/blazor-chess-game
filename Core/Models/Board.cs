using Chess.Core.Enums;
using Chess.Core.Models.Pieces;

namespace Chess.Core.Models;

public class Board
{
    public Piece?[,] Squares { get; private set; } = new Piece?[8, 8];
    public Position? EnPassantTarget { get; set; } = null;

    public Board()
    {
        InitBoard();
    }

    public Board(bool init)
    {
        if (init) InitBoard();
    }

    public bool IsValidPosition(int row, int column)
    {
        return row is >= 0 and < 8 && column is >= 0 and < 8;
    }

    public Piece? GetPieceAt(Position position)
    {
        return !IsValidPosition(position.Row, position.Column) ? null : Squares[position.Row, position.Column];
    }

    public Position? FindKing(PieceColor color)
    {
        for (var r = 0; r < 8; r++)
        {
            for (var c = 0; c < 8; c++)
            {
                var piece = Squares[r, c];
                if (piece != null && piece.Type == PieceType.King && piece.Color == color)
                {
                    return new Position(r, c);
                }
            }
        }
        return null;
    }

    public bool IsSquareAttacked(Position position, PieceColor attackingColor)
    {
        for (var r = 0; r < 8; r++)
        {
            for (var c = 0; c < 8; c++)
            {
                var piece = Squares[r, c];
                if (piece != null && piece.Color == attackingColor)
                {
                    var attackedSquares = piece.GetAttackedSquares(new Position(r, c), this);
                    if (attackedSquares.Contains(position))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool IsInCheck(PieceColor color)
    {
        var kingPosition = FindKing(color);
        if (kingPosition == null) return false;

        var opponentColor = color == PieceColor.White ? PieceColor.Black : PieceColor.White;
        return IsSquareAttacked(kingPosition, opponentColor);
    }

    public List<Position> GetLegalMoves(Position position)
    {
        var piece = GetPieceAt(position);
        if (piece == null) return new List<Position>();

        var validMoves = piece.GetValidMoves(position, this);
        var legalMoves = new List<Position>();

        foreach (var move in validMoves)
        {
            // Castling moves are already pre-validated for checks and attacked path squares
            if (piece.Type == PieceType.King && Math.Abs(move.Column - position.Column) == 2)
            {
                legalMoves.Add(move);
                continue;
            }

            // Simulate the move and verify king is not left in check
            var simBoard = Clone();
            simBoard.ExecuteMove(position, move);

            if (!simBoard.IsInCheck(piece.Color))
            {
                legalMoves.Add(move);
            }
        }

        return legalMoves;
    }

    public bool HasAnyLegalMoves(PieceColor color)
    {
        for (var r = 0; r < 8; r++)
        {
            for (var c = 0; c < 8; c++)
            {
                var piece = Squares[r, c];
                if (piece != null && piece.Color == color)
                {
                    var moves = GetLegalMoves(new Position(r, c));
                    if (moves.Count > 0)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool IsCheckmate(PieceColor color)
    {
        return IsInCheck(color) && !HasAnyLegalMoves(color);
    }

    public bool IsStalemate(PieceColor color)
    {
        return !IsInCheck(color) && !HasAnyLegalMoves(color);
    }

    public bool IsKingDead(PieceColor color)
    {
        return FindKing(color) == null;
    }

    public bool HasInsufficientMaterial()
    {
        var pieces = new List<Piece>();
        for (var r = 0; r < 8; r++)
        {
            for (var c = 0; c < 8; c++)
            {
                var piece = Squares[r, c];
                if (piece != null)
                {
                    pieces.Add(piece);
                }
            }
        }

        // King vs King
        if (pieces.Count == 2 && pieces.All(p => p.Type == PieceType.King))
        {
            return true;
        }

        // King + Minor Piece (Bishop or Knight) vs King
        if (pieces.Count == 3)
        {
            var nonKings = pieces.Where(p => p.Type != PieceType.King).ToList();
            if (nonKings.Count == 1 && (nonKings[0].Type == PieceType.Bishop || nonKings[0].Type == PieceType.Knight))
            {
                return true;
            }
        }

        return false;
    }

    public MoveResult ExecuteMove(Position from, Position to)
    {
        var piece = Squares[from.Row, from.Column]!;
        var captured = Squares[to.Row, to.Column];
        var isEnPassant = false;
        var isCastling = false;
        var isPromotion = false;
        Piece? promotedTo = null;

        // 1. En Passant capture check
        if (piece.Type == PieceType.Pawn && EnPassantTarget != null && to == EnPassantTarget)
        {
            var capturedPawnRow = from.Row;
            var capturedPawnCol = to.Column;
            captured = Squares[capturedPawnRow, capturedPawnCol];
            Squares[capturedPawnRow, capturedPawnCol] = null;
            isEnPassant = true;
        }

        // 2. Castling rook relocation
        if (piece.Type == PieceType.King && Math.Abs(to.Column - from.Column) == 2)
        {
            isCastling = true;
            if (to.Column == 6) // Kingside
            {
                var rook = Squares[from.Row, 7];
                Squares[from.Row, 5] = rook;
                Squares[from.Row, 7] = null;
                if (rook != null) rook.HasMoved = true;
            }
            else if (to.Column == 2) // Queenside
            {
                var rook = Squares[from.Row, 0];
                Squares[from.Row, 3] = rook;
                Squares[from.Row, 0] = null;
                if (rook != null) rook.HasMoved = true;
            }
        }

        // 3. Update EnPassantTarget for next turn
        if (piece.Type == PieceType.Pawn && Math.Abs(to.Row - from.Row) == 2)
        {
            var skippedRow = from.Row + (to.Row - from.Row) / 2;
            EnPassantTarget = new Position(skippedRow, from.Column);
        }
        else
        {
            EnPassantTarget = null;
        }

        // 4. Place piece at destination
        Squares[to.Row, to.Column] = piece;
        Squares[from.Row, from.Column] = null;
        piece.HasMoved = true;

        // 5. Pawn Promotion (default to Queen)
        if (piece.Type == PieceType.Pawn && (to.Row == 0 || to.Row == 7))
        {
            isPromotion = true;
            promotedTo = new Queen(piece.Color) { HasMoved = true };
            Squares[to.Row, to.Column] = promotedTo;
        }

        return new MoveResult
        {
            MovedPiece = piece,
            From = from,
            To = to,
            CapturedPiece = captured,
            IsEnPassant = isEnPassant,
            IsCastling = isCastling,
            IsPromotion = isPromotion,
            PromotedTo = promotedTo
        };
    }

    public void MovePiece(Position from, Position to)
    {
        ExecuteMove(from, to);
    }

    public Board Clone()
    {
        var clone = new Board(init: false);
        for (var r = 0; r < 8; r++)
        {
            for (var c = 0; c < 8; c++)
            {
                var piece = Squares[r, c];
                if (piece != null)
                {
                    clone.Squares[r, c] = piece.Clone();
                }
            }
        }
        clone.EnPassantTarget = EnPassantTarget;
        return clone;
    }

    public void InitBoard()
    {
        Squares = new Piece?[8, 8];
        EnPassantTarget = null;

        // Black major & minor pieces
        Squares[0, 0] = new Rook(PieceColor.Black);
        Squares[0, 1] = new Knight(PieceColor.Black);
        Squares[0, 2] = new Bishop(PieceColor.Black);
        Squares[0, 3] = new Queen(PieceColor.Black);
        Squares[0, 4] = new King(PieceColor.Black);
        Squares[0, 5] = new Bishop(PieceColor.Black);
        Squares[0, 6] = new Knight(PieceColor.Black);
        Squares[0, 7] = new Rook(PieceColor.Black);

        // Black pawns
        for (var col = 0; col < 8; col++)
        {
            Squares[1, col] = new Pawn(PieceColor.Black);
        }

        // White pawns
        for (var col = 0; col < 8; col++)
        {
            Squares[6, col] = new Pawn(PieceColor.White);
        }

        // White major & minor pieces
        Squares[7, 0] = new Rook(PieceColor.White);
        Squares[7, 1] = new Knight(PieceColor.White);
        Squares[7, 2] = new Bishop(PieceColor.White);
        Squares[7, 3] = new Queen(PieceColor.White);
        Squares[7, 4] = new King(PieceColor.White);
        Squares[7, 5] = new Bishop(PieceColor.White);
        Squares[7, 6] = new Knight(PieceColor.White);
        Squares[7, 7] = new Rook(PieceColor.White);
    }
}