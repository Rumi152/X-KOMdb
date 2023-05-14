using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XKOMapp.GUI.ConsoleRows;
internal class DynamicConsoleRowPacket : IConsoleRowPacket
{
    private readonly Func<List<IConsoleRow>> packetGetter;
    private ConsolePrinter owner = null!;

    public DynamicConsoleRowPacket(Func<List<IConsoleRow>> packetGetter)
    {
        this.packetGetter = packetGetter;
    }

    public List<IConsoleRow> GetPacket() => packetGetter();
    public IRenderable GetRenderContent() => throw new InvalidOperationException();
    public void SetOwnership(ConsolePrinter owner) => this.owner = owner;
}
