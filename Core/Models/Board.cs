using Chess.Core.Enums;
using Chess.Core.Models.Pieces;

namespace Chess.Core.Models;

public class Board
{
    private Piece?[,] Squares { get; set; } = new Piece? [8, 8];

    public Board()
    {
        InitBoard();
    }

    public bool IsValidPosition(int row, int column)
    {
        return row is >= 0 and < 8 && column is >= 0 and < 8;
    }

    public Piece? GetPieceAt(Position position)
    {
        return !IsValidPosition(position.Row, position.Column) ? null : Squares[position.Row, position.Column];
    }

    public void MovePiece(Position from, Position to)
    {
        var piece = Squares[from.Row, from.Column];
        Squares[to.Row, to.Column] = piece;
        Squares[from.Row, from.Column] = null;
    }

    private void InitBoard()
    {
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