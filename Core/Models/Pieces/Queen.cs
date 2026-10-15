using BlazorChessWebApp.Core.Models.Enums;

namespace BlazorChessWebApp.Core.Models.Pieces;

public class Queen : Rook
{
    public Queen(PieceColor color) : base(color)
    {
        Type = PieceType.Queen;
    }

    public override List<Position> GetPseudoLegalMoves(Position currentPosition, Board board, Position? enPassantTarget = null)
    {
        return GetRayMoves(currentPosition, board, [
            (-1, 0), (1, 0), (0, -1), (0, 1),
            (-1, -1), (-1, 1), (1, -1), (1, 1)
        ]);
    }
}