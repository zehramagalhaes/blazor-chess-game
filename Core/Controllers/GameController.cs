using BlazorChessWebApp.Core.Models;
using BlazorChessWebApp.Core.Models.Enums;
using BlazorChessWebApp.Core.Models.Pieces;

namespace BlazorChessWebApp.Core.Controllers;

public class GameController
{
    private Game Game { get; set; } = new Game();
    public Position? SelectedPosition { get; private set; }
    public List<Position> ValidMoves { get; private set; } = new List<Position>();

    public Board Board => Game.Board;
    public PieceColor CurrentTurn => Game.CurrentTurn;
    public bool IsInCheck => Game.IsInCheck;
    public bool IsCheckmate => Game.IsCheckmate;
    public bool IsDraw => Game.IsDraw;
    public DrawReason DrawReason => Game.DrawReason;
    private bool IsGameOver => Game.IsGameOver;
    
    public bool IsPromoting => Game.PendingPromotionPosition != null;

    public IReadOnlyList<Piece> WhiteCapturedPieces => Game.WhiteCapturedPieces;
    public IReadOnlyList<Piece> BlackCapturedPieces => Game.BlackCapturedPieces;
    public IReadOnlyList<MoveLogEntry> MoveHistory => Game.MoveHistory;

    public void HandleSquareClick(int row, int col)
    {
        if (IsGameOver || IsPromoting) return;

        var clickedPos = new Position(row, col);
        var clickedPiece = Board.GetPieceAt(clickedPos);
        
        if (SelectedPosition != null && ValidMoves.Contains(clickedPos))
        {
            var success = Game.MakeMove(SelectedPosition, clickedPos);
            if (success)
            {
                ClearSelection();
                return;
            }
        }

        if (clickedPiece != null && clickedPiece.Color == CurrentTurn)
        {
            SelectedPosition = clickedPos;
            ValidMoves = Game.GetLegalMoves(clickedPos);
        }
        else
        {
            ClearSelection();
        }
    }
    
    public void PromotePawn(PieceType targetType)
    {
        Game.PromotePawn(targetType);
        ClearSelection();
    }

    public void StartNewGame()
    {
        Game = new Game();
        ClearSelection();
    }

    private void ClearSelection()
    {
        SelectedPosition = null;
        ValidMoves.Clear();
    }

    public string GetPieceAssetPath(Piece piece)
    {
        var colorPrefix = piece.Color == PieceColor.White ? "w" : "b";
        var typeName = piece.Type.ToString().ToLower();
        return $"images/pieces/{colorPrefix}_{typeName}.svg";
    }

    public string GetPieceAssetPathByColorAndType(PieceColor color, PieceType type)
    {
        var colorPrefix = color == PieceColor.White ? "w" : "b";
        var typeName = type.ToString().ToLower();
        return $"images/pieces/{colorPrefix}_{typeName}.svg";
    }

    public string GetLogCssClass(LogType type) => type switch
    {
        LogType.Move => "log-move",
        LogType.Kill => "log-kill",
        LogType.Check => "log-kill",
        LogType.Checkmate => "log-kill",
        _ => "log-info"
    };

    public string GetDrawMessage() => DrawReason switch
    {
        DrawReason.Stalemate => "STALEMATE! DRAW",
        DrawReason.FiftyMoveRule => "50-MOVE DRAW",
        DrawReason.InsufficientMaterial => "INSUFFICIENT MATERIAL DRAW",
        _ => "DRAW"
    };
}