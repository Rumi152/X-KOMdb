using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace XKOMapp
{
    public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public Image Picture { get; set; }
        public string Category { get; set; }//TEMP
        public string Company { get; set; }//TEMP
        public bool IsAvailable { get; set; }
        public DateTime IntroductionDate { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }
}
