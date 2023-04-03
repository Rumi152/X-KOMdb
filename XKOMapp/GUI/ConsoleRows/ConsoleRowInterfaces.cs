﻿using Spectre.Console.Rendering;

namespace XKOMapp.GUI.ConsoleRows;

/// <summary>
/// Basic renderable ConsoleRow
/// </summary>
public interface IConsoleRow
{
    IRenderable GetRenderContent();
    void SetOwnership(ConsolePrinter owner);
}

/// <summary>
/// ConsoleRow that can take more than one row
/// </summary>
public interface ICustomLineSpanConsoleRow : IConsoleRow
{
    int GetRenderHeight();
}

/// <summary>
/// ConsoleRow with action on click
/// </summary>
public interface IInteractableConsoleRow : IConsoleRow
{
    void OnInteraction();
}

/// <summary>
/// ConsoleRow with actions on hovering start and end
/// </summary>
public interface IHoverConsoleRow : IConsoleRow
{
    void OnHoverStart();
    void OnHoverEnd();
}

/// <summary>
/// ConsoleRow with custom displayed cursor
/// </summary>
public interface ICustomCursorConsoleRow : IConsoleRow
{
    string GetCustomCursor();
    string GetCustomCursorBackground();
}

/// <summary>
/// ConsoleRow which switched between two states
/// </summary>
public interface ISwitchableConsoleRow : IConsoleRow
{
    public bool IsActive { get; protected set; }

    public void TurnOn()
    {
        if (IsActive)
            return;

        IsActive = true;
        OnTurningOn();
    }
    public void TurnOff()
    {
        if (!IsActive)
            return;

        IsActive = false;
        OnTurningOff();
    }
    public void Swich()
    {
        if (IsActive)
            TurnOff();
        else
            TurnOn();
    }

    protected void OnTurningOff();
    protected void OnTurningOn();
}

/// <summary>
/// ConsoleRow which can be turned on and off
/// </summary>
public interface IDeactivableConsoleRow : ISwitchableConsoleRow
{

}

/// <summary>
/// ConsoleRow which can be hidden
/// </summary>
public interface IHideableConsoleRow : IDeactivableConsoleRow
{

}

/// <summary>
/// Delegate for action invoked by ConsoleRow
/// </summary>
/// <param name="row">Row invoking action</param>
/// <param name="rowOwner">Printer owning row</param>
public delegate void ConsoleRowAction(IConsoleRow row, ConsolePrinter? rowOwner);