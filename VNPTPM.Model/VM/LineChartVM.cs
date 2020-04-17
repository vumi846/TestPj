using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNPTPM.Model.VM
{
    public class LineChartVM
    {
        public List<string> Categories { get; set; }
        public List<decimal> Data { get; set; }
        public LineChartVM()
        {
            Categories = new List<string>();
            Data = new List<decimal>();
        }
    }
}
