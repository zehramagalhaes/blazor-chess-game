using Chess.Core.Enums;

namespace Chess.Core.Models.Pieces;

public class Pawn : Piece
{
    public Pawn(PieceColor color) : base(color)
    {
        Type = PieceType.Pawn;
    }

    public override Piece Clone() => new Pawn(Color) { HasMoved = HasMoved };

    public override List<Position> GetValidMoves(Position currentPosition, Board board)
    {
        var validMoves = new List<Position>();

        var forwardDirection = Color == PieceColor.White ? -1 : 1;
        var startRow = Color == PieceColor.White ? 6 : 1;

        // 1. One square forward
        var oneStepRow = currentPosition.Row + forwardDirection;
        var col = currentPosition.Column;

        if (board.IsValidPosition(oneStepRow, col) && board.GetPieceAt(new Position(oneStepRow, col)) == null)
        {
            validMoves.Add(new Position(oneStepRow, col));

            // 2. Two squares forward from starting rank
            if (!HasMoved && currentPosition.Row == startRow)
            {
                var twoStepRow = currentPosition.Row + 2 * forwardDirection;
                if (board.IsValidPosition(twoStepRow, col) && board.GetPieceAt(new Position(twoStepRow, col)) == null)
                {
                    validMoves.Add(new Position(twoStepRow, col));
                }
            }
        }

        // 3. Diagonal captures & En Passant
        var captureCols = new[] { col - 1, col + 1 };
        foreach (var captureCol in captureCols)
        {
            var captureRow = currentPosition.Row + forwardDirection;
            if (!board.IsValidPosition(captureRow, captureCol)) continue;

            var targetPos = new Position(captureRow, captureCol);
            var targetPiece = board.GetPieceAt(targetPos);

            // Regular capture
            if (targetPiece != null && targetPiece.Color != Color)
            {
                validMoves.Add(targetPos);
            }
            // En Passant capture
            else if (targetPiece == null && board.EnPassantTarget != null && targetPos == board.EnPassantTarget)
            {
                validMoves.Add(targetPos);
            }
        }

        return validMoves;
    }

    public override List<Position> GetAttackedSquares(Position currentPosition, Board board)
    {
        var attacked = new List<Position>();
        var forwardDirection = Color == PieceColor.White ? -1 : 1;
        var captureCols = new[] { currentPosition.Column - 1, currentPosition.Column + 1 };

        foreach (var captureCol in captureCols)
        {
            var captureRow = currentPosition.Row + forwardDirection;
            if (board.IsValidPosition(captureRow, captureCol))
            {
                attacked.Add(new Position(captureRow, captureCol));
            }
        }

        return attacked;
    }
}
