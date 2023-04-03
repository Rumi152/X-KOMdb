using Spectre.Console.Rendering;

namespace XKOMapp.GUI.ConsoleRows
{
    public class HideOnClickConsoleRow : HideableConsoleRow, IInteractableConsoleRow
    {
        public HideOnClickConsoleRow(IRenderable renderable) : base(renderable)
        {
        }


        public void OnInteraction() => ((IHideableConsoleRow)this).TurnOff();
    }
}
