using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XKOMapp.GUI.ConsoleRows.ProductDetails;
internal class DeactivatedReviewDescriptionInputConsoleRow : IDeactivableConsoleRow
{
    private readonly IRenderable renderable;

    private ConsolePrinter owner = null!;


    public DeactivatedReviewDescriptionInputConsoleRow(IRenderable renderable)
    {
        this.renderable = renderable;
    }


    bool ISwitchableConsoleRow.IsActive { get => false; set => throw new InvalidOperationException(); }

    public IRenderable GetRenderContent() => renderable;
    public void SetOwnership(ConsolePrinter owner) => this.owner = owner;

    void ISwitchableConsoleRow.OnTurningOff() => throw new InvalidOperationException();
    void ISwitchableConsoleRow.OnTurningOn() => throw new InvalidOperationException();
}
