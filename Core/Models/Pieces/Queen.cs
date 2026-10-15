using Chess.Core.Enums;

namespace Chess.Core.Models.Pieces;

public class Queen : Piece
{
    public Queen(PieceColor color) : base(color)
    {
        Type = PieceType.Queen;
    }

    public override Piece Clone() => new Queen(Color) { HasMoved = HasMoved };

    public override List<Position> GetValidMoves(Position currentPosition, Board board)
    {
        var validMoves = new List<Position>();

        var directions = new[]
        {
            (-1, 0),  // Up
            (1, 0),   // Down
            (0, -1),  // Left
            (0, 1),   // Right
            (-1, -1), // Up-Left
            (-1, 1),  // Up-Right
            (1, -1),  // Down-Left
            (1, 1)    // Down-Right
        };

        foreach (var (rowOffset, colOffset) in directions)
        {
            var newRow = currentPosition.Row + rowOffset;
            var newCol = currentPosition.Column + colOffset;

            while (board.IsValidPosition(newRow, newCol))
            {
                var targetPiece = board.GetPieceAt(new Position(newRow, newCol));

                if (targetPiece == null)
                {
                    validMoves.Add(new Position(newRow, newCol));
                }
                else
                {
                    if (targetPiece.Color != Color)
                    {
                        validMoves.Add(new Position(newRow, newCol));
                    }
                    break;
                }

                newRow += rowOffset;
                newCol += colOffset;
            }
        }

        return validMoves;
    }

    public override List<Position> GetAttackedSquares(Position currentPosition, Board board)
    {
        var attacked = new List<Position>();

        var directions = new[]
        {
            (-1, 0),
            (1, 0),
            (0, -1),
            (0, 1),
            (-1, -1),
            (-1, 1),
            (1, -1),
            (1, 1)
        };

        foreach (var (rowOffset, colOffset) in directions)
        {
            var newRow = currentPosition.Row + rowOffset;
            var newCol = currentPosition.Column + colOffset;

            while (board.IsValidPosition(newRow, newCol))
            {
                attacked.Add(new Position(newRow, newCol));
                if (board.GetPieceAt(new Position(newRow, newCol)) != null)
                {
                    break;
                }
                newRow += rowOffset;
                newCol += colOffset;
            }
        }

        return attacked;
    }
}
