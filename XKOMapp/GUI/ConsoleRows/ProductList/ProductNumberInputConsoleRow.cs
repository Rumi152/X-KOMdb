﻿using Spectre.Console.Rendering;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XKOMapp.Models;
using Microsoft.VisualBasic;

namespace XKOMapp.GUI.ConsoleRows.ProductList;

internal class ProductNumberInputConsoleRow : ICustomCursorConsoleRow, ICustomKeystrokeListenerConsoleRow, IHoverConsoleRow, IInteractableConsoleRow
{
    private readonly string markupLabel;
    private readonly int maxLength;
    ConsolePrinter owner = null!;
    public string CurrentInput { get; private set; } = "";
    private bool isHovered;

    public ProductNumberInputConsoleRow(string markupLabel, int maxLength)
    {
        this.markupLabel = markupLabel;
        this.maxLength = maxLength;
    }

    public IRenderable GetRenderContent() => new Markup($"{markupLabel}{CurrentInput.EscapeMarkup()}{(isHovered ? "[blink]_[/]" : "")}");
    public void SetOwnership(ConsolePrinter owner) => this.owner = owner;

    public void ProcessCustomKeystroke(ConsoleKeyInfo keystrokeInfo)
    {
        owner.SetBufferDirty();
        char letter = keystrokeInfo.KeyChar;

        if (keystrokeInfo.Key == ConsoleKey.Backspace)
        {
            if (CurrentInput.Length > 0)
                CurrentInput = CurrentInput[..^1];
            return;
        }

        if (CurrentInput.Length >= maxLength)
            return;

        if (!char.IsDigit(letter))
            return;

        CurrentInput += letter;
    }

    public string GetCustomCursor() => "[blink]\u00BB[/]";
    public string GetCustomCursorBackground() => " ";

    public void OnHoverStart() => isHovered = true;
    public void OnHoverEnd() => isHovered = false;

    public void OnInteraction()
    {
        owner.CursorDown();
    }


    public void ResetInput()
    {
        CurrentInput = "";
        owner.SetBufferDirty();
    }
}



