using BlazorChessWebApp.Core.Models.Enums;

namespace BlazorChessWebApp.Core.Models.Pieces;

public class King : Piece
{
    public King(PieceColor color) : base(color)
    {
        Type = PieceType.King;
    }

    public override List<Position> GetPseudoLegalMoves(Position currentPosition, Board board, Position? enPassantTarget = null)
    {
        var moves = new List<Position>();
        int[] dRows = [-1, -1, -1, 0, 0, 1, 1, 1];
        int[] dCols = [-1, 0, 1, -1, 1, -1, 0, 1];

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

        // Castling moves validation
        if (HasMoved) return moves;
        
        // Kingside Castling (Column 7)
        var kingsidePiece = board.GetPieceAt(new Position(currentPosition.Row, 7));
        
        if (kingsidePiece is { Type: PieceType.Rook } && kingsidePiece.Color == Color && !kingsidePiece.HasMoved)
        {
            var conditionX = board.GetPieceAt(new Position(currentPosition.Row, 5));
            var conditionY = board.GetPieceAt(new Position(currentPosition.Row, 6));
            var isFreeToCastling = conditionX == null && conditionY == null;
                     
            if (isFreeToCastling)
            {
                moves.Add(new Position(currentPosition.Row, 6));
            }
        }

        // Queenside Castling (Column 0)
        var queensidePiece = board.GetPieceAt(new Position(currentPosition.Row, 0));

        if (queensidePiece is not { Type: PieceType.Rook } || queensidePiece.Color != Color ||
            queensidePiece.HasMoved) return moves;
        {
            var conditionX = board.GetPieceAt(new Position(currentPosition.Row, 1)) == null;
            var conditionY = board.GetPieceAt(new Position(currentPosition.Row, 2)) == null;
            var conditionZ = board.GetPieceAt(new Position(currentPosition.Row, 3)) == null;
            
            var isFreeToCastling = (conditionX && conditionY && conditionZ);
            if (isFreeToCastling)
            {
                moves.Add(new Position(currentPosition.Row, 2));
            }
        }

        return moves;
    }
}