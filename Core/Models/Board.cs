using BlazorChessWebApp.Core.Models.Enums;
using BlazorChessWebApp.Core.Models.Pieces;

namespace BlazorChessWebApp.Core.Models;

public class Board
{
    private Piece?[,] Squares { get; set; } = new Piece?[8, 8];

    public Board(bool initialize = true)
    {
        if (initialize)
        {
            InitializeBoard();
        }
    }

    public bool IsValidPosition(int row, int col)
    {
        return row is >= 0 and < 8 && col is >= 0 and < 8;
    }

    public Piece? GetPieceAt(Position? pos)
    {
        if (pos == null || !IsValidPosition(pos.Row, pos.Column)) return null;
        return Squares[pos.Row, pos.Column];
    }

    public void MovePiece(Position? from, Position? to)
    {
        if (from == null || to == null) return;
        Piece? piece = Squares[from.Row, from.Column];
        Squares[to.Row, to.Column] = piece;
        Squares[from.Row, from.Column] = null;
    }

    public void ReplacePiece(Position? pos, Piece? newPiece)
    {
        if (pos != null && IsValidPosition(pos.Row, pos.Column))
        {
            Squares[pos.Row, pos.Column] = newPiece;
        }
    }

    public Board Clone()
    {
        var newBoard = new Board(initialize: false);
        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                var piece = Squares[r, c];
                if (piece != null)
                {
                    Piece? clonedPiece = null;
                    
                    if (piece is Pawn) clonedPiece = new Pawn(piece.Color);
                    else if (piece is Queen) clonedPiece = new Queen(piece.Color);
                    else if (piece is Bishop) clonedPiece = new Bishop(piece.Color);
                    else if (piece is Rook) clonedPiece = new Rook(piece.Color);
                    else if (piece is Knight) clonedPiece = new Knight(piece.Color);
                    else if (piece is King) clonedPiece = new King(piece.Color);

                    if (clonedPiece != null)
                    {
                        clonedPiece.HasMoved = piece.HasMoved;
                        newBoard.Squares[r, c] = clonedPiece;
                    }
                }
            }
        }
        return newBoard;
    }

    private Position? FindKingPosition(PieceColor color)
    {
        for (var r = 0; r < 8; r++)
        {
            for (var c = 0; c < 8; c++)
            {
                var piece = Squares[r, c];
                if (piece is King && piece.Color == color)
                {
                    return new Position(r, c);
                }
            }
        }
        return null;
    }

    public bool IsSquareAttacked(Position? target, PieceColor attackerColor)
    {
        if (target == null) return false;

        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                var piece = Squares[r, c];
                if (piece != null && piece.Color == attackerColor)
                {
                    var currentPos = new Position(r, c);
                    if (piece is Pawn)
                    {
                        int direction = piece.Color == PieceColor.White ? -1 : 1;
                        int attackRow = r + direction;
                        if (attackRow == target.Row && (c - 1 == target.Column || c + 1 == target.Column))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        var moves = piece.GetPseudoLegalMoves(currentPos, this);
                        if (moves.Contains(target))
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    public bool IsKingInCheck(PieceColor kingColor)
    {
        Position? kingPos = FindKingPosition(kingColor);
        if (kingPos == null) return false;

        PieceColor attackerColor = kingColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
        return IsSquareAttacked(kingPos, attackerColor);
    }

    public bool IsInsufficientMaterial()
    {
        var pieces = new List<(Piece Piece, Position Position)>();
        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                var piece = Squares[r, c];
                if (piece != null)
                {
                    pieces.Add((piece, new Position(r, c)));
                }
            }
        }

        if (pieces.Count == 2) return true;

        if (pieces.Count == 3)
        {
            return pieces.Any(p => p.Piece is Bishop || p.Piece is Knight);
        }

        if (pieces.Count == 4)
        {
            var bishops = pieces.Where(p => p.Piece is Bishop).ToList();
            if (bishops.Count == 2 && bishops[0].Piece.Color != bishops[1].Piece.Color)
            {
                bool b1IsDark = (bishops[0].Position.Row + bishops[0].Position.Column) % 2 != 0;
                bool b2IsDark = (bishops[1].Position.Row + bishops[1].Position.Column) % 2 != 0;
                return b1IsDark == b2IsDark;
            }
        }

        return false;
    }

    private void InitializeBoard()
    {
        Squares[0, 0] = new Rook(PieceColor.Black);
        Squares[0, 1] = new Knight(PieceColor.Black);
        Squares[0, 2] = new Bishop(PieceColor.Black);
        Squares[0, 3] = new Queen(PieceColor.Black);
        Squares[0, 4] = new King(PieceColor.Black);
        Squares[0, 5] = new Bishop(PieceColor.Black);
        Squares[0, 6] = new Knight(PieceColor.Black);
        Squares[0, 7] = new Rook(PieceColor.Black);

        for (int i = 0; i < 8; i++) Squares[1, i] = new Pawn(PieceColor.Black);
        for (int i = 0; i < 8; i++) Squares[6, i] = new Pawn(PieceColor.White);

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