namespace Chess.Core.Models;

public class MoveResult
{
    public Piece MovedPiece { get; set; } = null!;
    public Position From { get; set; } = null!;
    public Position To { get; set; } = null!;
    public Piece? CapturedPiece { get; set; }
    public bool IsEnPassant { get; set; }
    public bool IsCastling { get; set; }
    public bool IsPromotion { get; set; }
    public Piece? PromotedTo { get; set; }
}
