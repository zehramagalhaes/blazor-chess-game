using Chess.Core.Enums;

namespace Chess.Core.Models.Pieces;

public class Rook : Piece
{
    public Rook(PieceColor color) : base(color)
    {
        Type = PieceType.Rook;
    }

    public override List<Position> GetValidMoves(Position currentPosition, Board board)
    {
        var validMoves = new List<Position>();

        var directions = new[]
        {
            (-1, 0), // Up
            (1, 0),  // Down
            (0, -1), // Left
            (0, 1)   // Right
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
}
