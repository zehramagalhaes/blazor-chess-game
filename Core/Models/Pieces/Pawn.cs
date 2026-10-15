using BlazorChessWebApp.Core.Models.Enums;

namespace BlazorChessWebApp.Core.Models.Pieces;

public class Pawn : Piece
{
    public Pawn(PieceColor color) : base(color)
    {
        Type = PieceType.Pawn;
    }

    public override List<Position> GetPseudoLegalMoves(Position currentPosition, Board board, Position? enPassantTarget = null)
    {
        var moves = new List<Position>();
        int direction = Color == PieceColor.White ? -1 : 1;
        int startRow = Color == PieceColor.White ? 6 : 1;

        int nextRow = currentPosition.Row + direction;
        if (board.IsValidPosition(nextRow, currentPosition.Column) && board.GetPieceAt(new Position(nextRow, currentPosition.Column)) == null)
        {
            moves.Add(new Position(nextRow, currentPosition.Column));

            int doubleNextRow = currentPosition.Row + (2 * direction);
            if (currentPosition.Row == startRow && board.GetPieceAt(new Position(doubleNextRow, currentPosition.Column)) == null)
            {
                moves.Add(new Position(doubleNextRow, currentPosition.Column));
            }
        }

        int[] captureCols = { currentPosition.Column - 1, currentPosition.Column + 1 };
        foreach (int col in captureCols)
        {
            if (board.IsValidPosition(nextRow, col))
            {
                var targetPos = new Position(nextRow, col);
                var targetPiece = board.GetPieceAt(targetPos);

                // Standard capture
                if (targetPiece != null && targetPiece.Color != Color)
                {
                    moves.Add(targetPos);
                }
                // En Passant capture
                else if (enPassantTarget != null && targetPos == enPassantTarget)
                {
                    moves.Add(targetPos);
                }
            }
        }

        return moves;
    }
}