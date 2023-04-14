using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XKOMapp.GUI;

namespace XKOMapp.ViewsFSM.States
{
    public class ProductSearchViewState : ViewState
    {
        public ProductSearchViewState(ViewStateMachine stateMachine, string text, decimal price, string company) : base(stateMachine)
        {
            printer = new ConsolePrinter();

            printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
            printer.StartContent();

            //Company search
            //Price range
            //Name search
            //Category search
            //reset filters unavailable
            //orderby: newest, highest ratings, cheapest, most expensive

            //refresh products
            printer.AddRow(new Rule("Click to refresh").RuleStyle(Style.Parse("#0e8f75")).ToBasicConsoleRow());

            //[lime]{Price}[/] or "[red]Unavailable[/]"
            printer.AddRow(new Markup($"{text.EscapeMarkup(),-32} | {company} | [lime]{price,-10}[/] PLN").ToBasicConsoleRow(), "product");
            printer.AddRow(new Markup($"{text.EscapeMarkup(),-32} | {company} | [lime]{price,-10}[/] PLN").ToBasicConsoleRow(), "product");
            printer.AddRow(new Markup($"{text.EscapeMarkup(),-32} | {company} | [lime]{price,-10}[/] PLN").ToBasicConsoleRow(), "product");
            printer.AddRow(new Markup($"{text.EscapeMarkup(),-32} | {company} | [lime]{price,-10}[/] PLN").ToBasicConsoleRow(), "product");
            printer.AddRow(new Markup($"{text.EscapeMarkup(),-32} | {company} | [lime]{1000000.99,-10}[/] PLN").ToBasicConsoleRow(), "product");
        }

        public override void OnEnter()
        {
            base.OnEnter();

            Display();
        }

        protected override void OnKeystrokePassed(ConsoleKeyInfo info)
        {
            base.OnKeystrokePassed(info);

            printer.PassKeystroke(info);
        }

        protected override void OnKeystrokePassedFinally(ConsoleKeyInfo info)
        {
            base.OnKeystrokePassedFinally(info);

            Display();
        }
    }
}
