using Spectre.Console.Rendering;

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
/// ConsoleRow that can override action on press of standard keys
/// </summary>
public interface IStandardKeystrokeOverrideConsoleRow : IConsoleRow
{
    /// <summary>
    /// Processes user's standard pressed key
    /// </summary>
    /// <param name="keystrokeInfo">ConsoleKeyInfo of pressed key</param>
    /// <returns>Whether keystroke was processed specifically or it should use standard processing</returns>
    bool ProcessStandardKeystroke(ConsoleKeyInfo keystrokeInfo);
}

/// <summary>
/// ConsoleRow that can listen for pressed keys, other than standard ones
/// </summary>
public interface ICustomKeystrokeListenerConsoleRow : IConsoleRow
{
    void ProcessCustomKeystroke(ConsoleKeyInfo keystrokeInfo);
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
/// ConsoleRow that can have multiple modes
/// </summary>
public interface IModesConsoleRow : IConsoleRow
{
    public int ModeIndex { get; protected set; }
    public int ModesCount { get;}

    public void IncrementModeIndex()
    {
        if (ModeIndex + 1 >= ModesCount)
            return;

        ModeIndex++;
        OnModeChange();
    }
    public void DecrementModeIndex()
    {
        if (ModeIndex <= 0)
            return;

        ModeIndex--;
        OnModeChange();
    }

    protected void OnModeChange();
}

/// <summary>
/// ConsoleRow which can be switched between two states
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
    public void Switch()
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
/// ConsoleRow which can be activated, to switch printer to focus mode
/// <para>Focus mode means that only currently activated IFocusableConsoleRows can bo available for cursor</para>
/// </summary>
public interface IFocusableConsoleRow : ISwitchableConsoleRow
{

}

/// <summary>
/// Delegate for action invoked by ConsoleRow
/// </summary>
/// <param name="row">Row invoking action</param>
/// <param name="rowOwner">Printer owning row</param>
public delegate void ConsoleRowAction(IConsoleRow row, ConsolePrinter? rowOwner);