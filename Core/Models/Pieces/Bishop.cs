using Chess.Core.Enums;

namespace Chess.Core.Models.Pieces;

public class Bishop : Piece
{
    public Bishop(PieceColor color) : base(color)
    {
        Type = PieceType.Bishop;
    }

    public override List<Position> GetValidMoves(Position currentPosition, Board board)
    {
        var validMoves = new List<Position>();

        var directions = new[]
        {
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
}
