
using BlazorChessWebApp.Core.Models.Enums;

namespace BlazorChessWebApp.Core.Models.Pieces;

public class Bishop : Rook
{
    public Bishop(PieceColor color) : base(color)
    {
        Type = PieceType.Bishop;
    }

    public override List<Position> GetPseudoLegalMoves(Position currentPosition, Board board, Position? enPassantTarget = null)
    {
        return GetRayMoves(currentPosition, board, [(-1, -1), (-1, 1), (1, -1), (1, 1)]);
    }
}