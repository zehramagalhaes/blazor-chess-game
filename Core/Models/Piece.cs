using Chess.Core.Enums;

namespace Chess.Core.Models;

public abstract class Piece(PieceColor color)
{
    public PieceColor Color { get; } = color;
    public PieceType Type { get; protected set; }
    public bool HasMoved { get; set; } = false;

    public abstract List<Position> GetValidMoves(Position currentPosition, Board board);

    public virtual List<Position> GetAttackedSquares(Position currentPosition, Board board)
    {
        return GetValidMoves(currentPosition, board);
    }

    public abstract Piece Clone();
}