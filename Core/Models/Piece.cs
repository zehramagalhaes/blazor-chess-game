using Chess.Core.Enums;

namespace Chess.Core.Models;

public abstract class Piece(PieceColor color)
{
    public PieceColor Color { get; } = color;
    public PieceType Type { get; protected set; }

    public abstract List<Position> GetValidMoves(Position currentPosition, Board board);
}