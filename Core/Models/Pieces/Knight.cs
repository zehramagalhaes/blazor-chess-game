using BlazorChessWebApp.Core.Models.Enums;

namespace BlazorChessWebApp.Core.Models.Pieces;

public class Knight : Piece
{
    public Knight(PieceColor color) : base(color)
    {
        Type = PieceType.Knight;
    }

    public override List<Position> GetPseudoLegalMoves(Position currentPosition, Board board, Position? enPassantTarget = null)
    {
        var moves = new List<Position>();
        int[] dRows = [-2, -2, -1, -1, 1, 1, 2, 2];
        int[] dCols = [-1, 1, -2, 2, -2, 2, -1, 1];

        for (var i = 0; i < 8; i++)
        {
            var r = currentPosition.Row + dRows[i];
            var c = currentPosition.Column + dCols[i];

            if (!board.IsValidPosition(r, c)) continue;
            var targetPiece = board.GetPieceAt(new Position(r, c));
            if (targetPiece == null || targetPiece.Color != Color)
            {
                moves.Add(new Position(r, c));
            }
        }
        return moves;
    }
}