using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finansu.Model
{
    public class Urisidikciiy
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsClosing { get; set; } = false;
    }
}
