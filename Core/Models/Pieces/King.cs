using Chess.Core.Enums;

namespace Chess.Core.Models.Pieces;

public class King : Piece
{
    public King(PieceColor color) : base(color)
    {
        Type = PieceType.King;
    }

    public override Piece Clone() => new King(Color) { HasMoved = HasMoved };

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

        // Castling moves
        var opponentColor = Color == PieceColor.White ? PieceColor.Black : PieceColor.White;
        var startRow = Color == PieceColor.White ? 7 : 0;

        if (!HasMoved && currentPosition.Row == startRow && currentPosition.Column == 4 && !board.IsInCheck(Color))
        {
            // Kingside castling (King moves to col 6)
            var kingsideRook = board.GetPieceAt(new Position(startRow, 7));
            if (kingsideRook is Rook && kingsideRook.Color == Color && !kingsideRook.HasMoved)
            {
                if (board.GetPieceAt(new Position(startRow, 5)) == null &&
                    board.GetPieceAt(new Position(startRow, 6)) == null &&
                    !board.IsSquareAttacked(new Position(startRow, 5), opponentColor) &&
                    !board.IsSquareAttacked(new Position(startRow, 6), opponentColor))
                {
                    validMoves.Add(new Position(startRow, 6));
                }
            }

            // Queenside castling (King moves to col 2)
            var queensideRook = board.GetPieceAt(new Position(startRow, 0));
            if (queensideRook is Rook && queensideRook.Color == Color && !queensideRook.HasMoved)
            {
                if (board.GetPieceAt(new Position(startRow, 1)) == null &&
                    board.GetPieceAt(new Position(startRow, 2)) == null &&
                    board.GetPieceAt(new Position(startRow, 3)) == null &&
                    !board.IsSquareAttacked(new Position(startRow, 3), opponentColor) &&
                    !board.IsSquareAttacked(new Position(startRow, 2), opponentColor))
                {
                    validMoves.Add(new Position(startRow, 2));
                }
            }
        }

        return validMoves;
    }

    public override List<Position> GetAttackedSquares(Position currentPosition, Board board)
    {
        var attacked = new List<Position>();
        var rowOffsets = new[] { -1, -1, -1, 0, 0, 1, 1, 1 };
        var colOffsets = new[] { -1, 0, 1, -1, 1, -1, 0, 1 };

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
