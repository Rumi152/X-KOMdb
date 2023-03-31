using Spectre.Console;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XKOMapp
{
    public class ProductView : IConsoleView
    {
        public Product Product { get; private set; }

        public void PrintView()
        {
            AnsiConsole.Clear();
            AnsiConsoleExtensions.StandardHeader();
            Product = new Product()//TEMP
            {
                Name = "GTX1080Ti",
                Price = 3200.50M,
                IsAvailable = true,
                Properties = new Dictionary<string, string>()
                {
                    { "RAM", "8Gb" },
                    { "Memory", "11Gb" },
                    { "Cooling type", "Active" }
                },
                Company = "Samsung",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.Aenean non orci pulvinar, dictum ligula non,fringilla sapien.Maecenas scelerisque lectus non ipsum tempor,ut faucibus leo gravida.Proin vitae sodales ipsum.Curabitur commodo,ex a ullamcorper eleifend,purus diam tincidunt orci,quis porttitor leo leo accumsan dui.Fusce hendrerit,magna eu ultricies imperdiet,neque elit mattis odio,id cursus lacus libero vel leo.Sed ut lorem tellus.Nullam scelerisque,tellus sed condimentum ultrices,augue tortor ultrices felis,vel fermentum odio sapien et turpis.Donec nec tincidunt ipsum."
            };

            AnsiConsole.MarkupLine($"[#a7e8db]{Product.Name}[/]");
            if(Product.IsAvailable)
                AnsiConsole.MarkupLine($"[#11c408]{Product.Price}zł[/]");
            else
                AnsiConsole.MarkupLine($"[strikethrough]{Product.Price}zł[/] [#de0202]Unavailable[/]");

            AnsiConsole.MarkupLine($"By [#a7e8db]{Product.Company}[/]");

            AnsiConsoleExtensions.StandardRule();

            var grid = new Grid();
            grid.AddColumns(3);
            foreach (var pair in Product.Properties)
            {
                grid.AddRow($"[#c4f0c2]{pair.Key}[/]", ":", pair.Value);
            }
            AnsiConsole.Write(grid);

            AnsiConsole.Write(
                new Panel(Product.Description)
                    .Expand()
                    .DoubleBorder()
                );

            AnsiConsoleExtensions.StandardRule();

            //TODO reviews
            AnsiConsole.MarkupLine("★");
        }
    }
}
