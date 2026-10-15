namespace BlazorChessWebApp.Core.Models;

public record Position(int Row, int Column)
{
    public string ToAlgebraic() => $"{(char)('A' + Column)}{8 - Row}";
    public override string ToString() => ToAlgebraic();
}