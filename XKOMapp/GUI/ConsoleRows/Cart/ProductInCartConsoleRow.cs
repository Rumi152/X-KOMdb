using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XKOMapp.Models;
using XKOMapp.ViewsFSM.States;

namespace XKOMapp.GUI.ConsoleRows.Cart;

internal class ProductInCartConsoleRow : IInteractableConsoleRow, ICustomCursorConsoleRow, ICustomKeystrokeListenerConsoleRow, IHoverConsoleRow
{
    private readonly Product product;
    private readonly Action<int> onProductAmountChange;
    private readonly Action onInteraction;

    private readonly int productAmount;
    private string currentAmountInput = "";

    private ConsolePrinter owner = null!;

    public ProductInCartConsoleRow(Product product, int productAmount, Action<int> onProductAmountChange, Action onInteraction)
    {
        this.product = product;
        this.productAmount = productAmount;
        this.onProductAmountChange = onProductAmountChange;
        this.onInteraction = onInteraction;
    }

    public IRenderable GetRenderContent()
    {
        string amountString = $"{productAmount}x" + ((currentAmountInput.Length == 0) ? "" : $" -> {currentAmountInput}[blink]x[/]");
        string priceString = product.NumberAvailable > 0 ? $"[lime]{product.Price * productAmount,-9:F2}[/] PLN" : "[red]Unavailable[/]";
        string companyString = product.Company is null ? new string(' ', 32) : ((product.Company.Name.Length <= 29) ? $"{product.Company.Name,-29}" : $"{product.Company.Name[..30]}...");

        string dimStart = productAmount == 0 ? "[dim]" : "";
        string dimEnd = productAmount == 0 ? "[/]" : "";

        string displayString = $"{dimStart}{amountString, -6} {product.Name.EscapeMarkup(),-40}  {priceString + new string(' ', Math.Max(0, 13 - priceString.RemoveMarkup().Length))}  {companyString}{dimEnd}";

        return new Markup(displayString);
    }


    public void SetOwnership(ConsolePrinter owner) => this.owner = owner;
    public string GetCustomCursor() => currentAmountInput.Length > 0 ? "[blink]»[/]" : "»";
    public string GetCustomCursorBackground() => " ";


    public void ProcessCustomKeystroke(ConsoleKeyInfo keystrokeInfo)
    {
        owner.SetBufferDirty();

        if (keystrokeInfo.Key == ConsoleKey.Escape)
        {
            currentAmountInput = "";
            return;
        }

        if (keystrokeInfo.Key == ConsoleKey.Backspace)
        {
            if (currentAmountInput.Length > 0)
                currentAmountInput = currentAmountInput[..^1];

            return;
        }

        char key = keystrokeInfo.KeyChar;

        if (!char.IsDigit(key))
            return;

        if (currentAmountInput.Length >= 5)
            return;

        currentAmountInput += key;
    }

    public void OnInteraction()
    {
        if (currentAmountInput.Length > 0)
            onProductAmountChange(int.Parse(currentAmountInput));
        else
            onInteraction();
    }

    public void OnHoverStart()
    {

    }

    public void OnHoverEnd()
    {
        currentAmountInput = "";
    }
}
