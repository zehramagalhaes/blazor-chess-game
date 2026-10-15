using Chess.Core.Enums;

namespace Chess.Core.Models;

public class Game
{
    public Board Board { get; private set; } = new Board();
    public PieceColor CurrentTurn { get; set; } = PieceColor.White;

    public void SwitchTurn()
    {
        CurrentTurn = CurrentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;
    }
}