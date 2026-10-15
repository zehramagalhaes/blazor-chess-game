using BlazorChessWebApp.Core.Models.Enums;

namespace BlazorChessWebApp.Core.Models.Pieces;

public class Rook : Piece
{
    public Rook(PieceColor color) : base(color)
    {
        Type = PieceType.Rook;
    }

    public override List<Position> GetPseudoLegalMoves(Position currentPosition, Board board, Position? enPassantTarget = null)
    {
        return GetRayMoves(currentPosition, board, [(-1, 0), (1, 0), (0, -1), (0, 1)]);
    }

    protected List<Position> GetRayMoves(Position start, Board board, (int dRow, int dCol)[] directions)
    {
        var moves = new List<Position>();
        
        foreach (var (dRow, dCol) in directions)
        {
            var r = start.Row + dRow;
            var c = start.Column + dCol;

            while (board.IsValidPosition(r, c))
            {
                var targetPiece = board.GetPieceAt(new Position(r, c));
                
                if (targetPiece == null)
                {
                    moves.Add(new Position(r, c));
                }
                else
                {
                    if (targetPiece.Color != Color)
                    {
                        moves.Add(new Position(r, c));
                    }
                    break; 
                }
                
                r += dRow;
                c += dCol;
            }
        }
        return moves;
    }
}