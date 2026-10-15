using Chess.Core.Enums;

namespace Chess.Core.Models.Pieces;

public class King : Piece
{
    public King(PieceColor color) : base(color)
    {
        Type = PieceType.King;
    }

    public override List<Position> GetValidMoves(Position currentPosition, Board board)
    {
        var validMoves = new List<Position>();

        var rowOffsets = new[] { -1, -1, -1, 0, 0, 1, 1, 1 };
        var colOffsets = new[] { -1, 0, 1, -1, 1, -1, 0, 1 };

        for (var i = 0; i < 8; i++)
        {
            var newRow = currentPosition.Row + rowOffsets[i];
            var newCol = currentPosition.Column + colOffsets[i];

            if (!board.IsValidPosition(newRow, newCol)) continue;
            var targetPiece = board.GetPieceAt(new Position(newRow, newCol));

            // Move is valid if the square is empty or has an enemy piece
            if (targetPiece == null || targetPiece.Color != Color)
            {
                validMoves.Add(new Position(newRow, newCol));
            }
        }

        return validMoves;
    }
}
