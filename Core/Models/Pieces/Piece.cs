using BlazorChessWebApp.Core.Models.Enums;

namespace BlazorChessWebApp.Core.Models.Pieces;

public abstract class Piece(PieceColor color)
{
    public PieceColor Color { get; } = color;
    public PieceType Type { get; protected set; }
    public bool HasMoved { get; set; } = false;
    
    public abstract List<Position> GetPseudoLegalMoves(Position currentPosition, Board board, Position? enPassantTarget = null);
}