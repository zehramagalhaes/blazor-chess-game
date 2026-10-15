using Chess.Core.Enums;

namespace Chess.Core.Models.Pieces;

public class Knight : Piece
{
    public Knight(PieceColor color) : base(color)
    {
        Type = PieceType.Knight;
    }

    public override Piece Clone() => new Knight(Color) { HasMoved = HasMoved };

    public override List<Position> GetValidMoves(Position currentPosition, Board board)
    {
        var validMoves = new List<Position>();

        var rowOffsets = new[] { -2, -2, -1, -1, 1, 1, 2, 2 };
        var colOffsets = new[] { -1, 1, -2, 2, -2, 2, -1, 1 };

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

    public override List<Position> GetAttackedSquares(Position currentPosition, Board board)
    {
        var attacked = new List<Position>();
        var rowOffsets = new[] { -2, -2, -1, -1, 1, 1, 2, 2 };
        var colOffsets = new[] { -1, 1, -2, 2, -2, 2, -1, 1 };

        for (var i = 0; i < 8; i++)
        {
            var newRow = currentPosition.Row + rowOffsets[i];
            var newCol = currentPosition.Column + colOffsets[i];

            if (board.IsValidPosition(newRow, newCol))
            {
                attacked.Add(new Position(newRow, newCol));
            }
        }
        return attacked;
    }
}