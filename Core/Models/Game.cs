using BlazorChessWebApp.Core.Models.Enums;
using BlazorChessWebApp.Core.Models.Pieces;

namespace BlazorChessWebApp.Core.Models;

public class Game
{
    public Board Board { get; private set; } = new Board();
    public PieceColor CurrentTurn { get; private set; } = PieceColor.White;
    
    public bool IsInCheck { get; private set; }
    public bool IsCheckmate { get; private set; }
    public bool IsStalemate => DrawReason == DrawReason.Stalemate;
    public DrawReason DrawReason { get; private set; } = DrawReason.None;
    public bool IsDraw => DrawReason != DrawReason.None;
    public bool IsGameOver => IsCheckmate || IsDraw;

    public Position? PendingPromotionPosition { get; private set; }
    public Position? EnPassantTargetPosition { get; private set; }
    public int HalfMoveClock { get; private set; } = 0;

    public List<Piece> WhiteCapturedPieces { get; } = new List<Piece>();
    public List<Piece> BlackCapturedPieces { get; } = new List<Piece>();
    public List<MoveLogEntry> MoveHistory { get; } = new List<MoveLogEntry>();

    public Game()
    {
        AddLog("Battle commenced. White Kingdom to move.", LogType.Info);
    }

    public List<Position> GetLegalMoves(Position from)
    {
        var piece = Board.GetPieceAt(from);
        if (piece == null || piece.Color != CurrentTurn || IsGameOver || PendingPromotionPosition != null)
        {
            return new List<Position>();
        }

        var pseudoMoves = piece.GetPseudoLegalMoves(from, Board, EnPassantTargetPosition);
        var legalMoves = new List<Position>();

        foreach (var move in pseudoMoves)
        {
            // Validação estrita do Roque: o Rei não pode estar em xeque nem passar por casas sob ataque
            if (piece is King && Math.Abs(move.Column - from.Column) == 2)
            {
                if (Board.IsKingInCheck(CurrentTurn)) continue;

                int step = (move.Column - from.Column) > 0 ? 1 : -1;
                var intermediatePos = new Position(from.Row, from.Column + step);
                PieceColor enemyColor = CurrentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;

                if (Board.IsSquareAttacked(intermediatePos, enemyColor)) continue;
                if (Board.IsSquareAttacked(move, enemyColor)) continue;
            }

            Board simBoard = Board.Clone();
            SimulateMove(simBoard, from, move);

            // A jogada só é válida se NÃO deixar o próprio Rei em xeque
            if (!simBoard.IsKingInCheck(CurrentTurn))
            {
                legalMoves.Add(move);
            }
        }

        return legalMoves;
    }

    public bool MakeMove(Position from, Position to)
    {
        var legalMoves = GetLegalMoves(from);
        if (!legalMoves.Contains(to)) return false;

        var sourcePiece = Board.GetPieceAt(from);
        var targetPiece = Board.GetPieceAt(to);

        Position? nextEnPassantTarget = null;

        // 1. Movimento duplo do peão define o alvo de En Passant
        if (sourcePiece is Pawn && Math.Abs(to.Row - from.Row) == 2)
        {
            int skippedRow = (from.Row + to.Row) / 2;
            nextEnPassantTarget = new Position(skippedRow, from.Column);
        }

        // 2. Captura En Passant
        bool isEnPassantCapture = sourcePiece is Pawn && EnPassantTargetPosition != null && to == EnPassantTargetPosition;
        if (isEnPassantCapture)
        {
            var enemyPawnPos = new Position(from.Row, to.Column);
            var capturedPawn = Board.GetPieceAt(enemyPawnPos);
            if (capturedPawn != null)
            {
                if (capturedPawn.Color == PieceColor.White) BlackCapturedPieces.Add(capturedPawn);
                else WhiteCapturedPieces.Add(capturedPawn);

                Board.ReplacePiece(enemyPawnPos, null);
                AddLog($"{sourcePiece.Color} Pawn vanquished {capturedPawn.Color} Pawn en passant at {GetSquareName(to)}!", LogType.Kill);
            }
        }

        // 3. Movimento de Roque
        bool isCastling = sourcePiece is King && Math.Abs(to.Column - from.Column) == 2;
        if (isCastling)
        {
            bool isKingside = to.Column == 6;
            int rookFromCol = isKingside ? 7 : 0;
            int rookToCol = isKingside ? 5 : 3;

            var rook = Board.GetPieceAt(new Position(from.Row, rookFromCol));
            Board.MovePiece(new Position(from.Row, rookFromCol), new Position(from.Row, rookToCol));
            if (rook != null) rook.HasMoved = true;

            string sideText = isKingside ? "kingside" : "queenside";
            AddLog($"{sourcePiece.Color} King castled {sideText}!", LogType.Move);
        }

        // 4. Capturas / Movimentos padrões
        if (!isEnPassantCapture && targetPiece != null)
        {
            if (targetPiece.Color == PieceColor.White)
                BlackCapturedPieces.Add(targetPiece);
            else
                WhiteCapturedPieces.Add(targetPiece);

            AddLog($"{sourcePiece?.Color} {sourcePiece?.Type} vanquished {targetPiece.Color} {targetPiece.Type} at {GetSquareName(to)}!", LogType.Kill);
        }
        else if (!isEnPassantCapture && !isCastling)
        {
            AddLog($"{sourcePiece?.Color} {sourcePiece?.Type} advanced to {GetSquareName(to)}.", LogType.Move);
        }

        // Executa o movimento no tabuleiro real
        Board.MovePiece(from, to);
        sourcePiece.HasMoved = true;
        EnPassantTargetPosition = nextEnPassantTarget;

        // Atualiza a regra de 50 movimentos
        if (sourcePiece is Pawn || targetPiece != null || isEnPassantCapture)
            HalfMoveClock = 0;
        else
            HalfMoveClock++;

        // Verifica promoção de Peão
        bool isPromotion = sourcePiece is Pawn && (to.Row == 0 || to.Row == 7);
        if (isPromotion)
        {
            PendingPromotionPosition = to;
            AddLog($"{sourcePiece?.Color} Pawn reached promotion rank! Select promotion piece.", LogType.Info);
            return true;
        }

        FinalizeTurn();
        return true;
    }

    public bool PromotePawn(PieceType targetType)
    {
        if (PendingPromotionPosition == null) return false;

        if (targetType is PieceType.King or PieceType.Pawn)
        {
            targetType = PieceType.Queen;
        }

        Piece newPiece = targetType switch
        {
            PieceType.Rook => new Rook(CurrentTurn),
            PieceType.Knight => new Knight(CurrentTurn),
            PieceType.Bishop => new Bishop(CurrentTurn),
            _ => new Queen(CurrentTurn)
        };

        newPiece.HasMoved = true;
        Board.ReplacePiece(PendingPromotionPosition, newPiece);
        AddLog($"{CurrentTurn} Pawn promoted to {targetType} at {GetSquareName(PendingPromotionPosition)}!", LogType.Info);

        PendingPromotionPosition = null;
        FinalizeTurn();
        return true;
    }

    private void SimulateMove(Board board, Position from, Position to)
    {
        var piece = board.GetPieceAt(from);

        if (piece is King && Math.Abs(to.Column - from.Column) == 2)
        {
            int rookFromCol = to.Column == 6 ? 7 : 0;
            int rookToCol = to.Column == 6 ? 5 : 3;
            board.MovePiece(new Position(from.Row, rookFromCol), new Position(from.Row, rookToCol));
        }

        if (piece is Pawn && EnPassantTargetPosition != null && to == EnPassantTargetPosition)
        {
            board.ReplacePiece(new Position(from.Row, to.Column), null);
        }

        board.MovePiece(from, to);
    }

    private void FinalizeTurn()
    {
        CurrentTurn = CurrentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;

        IsInCheck = Board.IsKingInCheck(CurrentTurn);
        bool hasLegalMoves = HasAnyLegalMoves(CurrentTurn);

        // Validação de Xeque-mate e Empate
        if (IsInCheck)
        {
            if (!hasLegalMoves)
            {
                IsCheckmate = true;
                string winner = CurrentTurn == PieceColor.White ? "Black Empire" : "White Kingdom";
                AddLog($"CHECKMATE! {winner} wins the realm!", LogType.Checkmate);
            }
            else
            {
                AddLog($"KING IN CHECK! {CurrentTurn} Kingdom must defend!", LogType.Check);
            }
        }
        else if (!hasLegalMoves)
        {
            DrawReason = DrawReason.Stalemate;
            AddLog("STALEMATE! The battle ends in a draw.", LogType.GameOver);
        }
        else if (HalfMoveClock >= 100)
        {
            DrawReason = DrawReason.FiftyMoveRule;
            AddLog("DRAW! 50-move rule limit reached.", LogType.GameOver);
        }
        else if (Board.IsInsufficientMaterial())
        {
            DrawReason = DrawReason.InsufficientMaterial;
            AddLog("DRAW! Insufficient material to checkmate.", LogType.GameOver);
        }
    }

    private bool HasAnyLegalMoves(PieceColor color)
    {
        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                var piece = Board.GetPieceAt(new Position(r, c));
                if (piece != null && piece.Color == color)
                {
                    if (GetLegalMoves(new Position(r, c)).Count > 0)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void AddLog(string message, LogType type = LogType.Info)
    {
        MoveHistory.Insert(0, new MoveLogEntry
        {
            Timestamp = DateTime.Now,
            Message = message,
            Type = type
        });
    }

    private string GetSquareName(Position pos)
    {
        char file = (char)('A' + pos.Column);
        int rank = 8 - pos.Row;
        return $"{file}{rank}";
    }
}