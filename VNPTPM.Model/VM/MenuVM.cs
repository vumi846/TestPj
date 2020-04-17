using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNPTPM.Model.VM
{
    public class MenuVM
    {
        public string Name { get; set; }
        public double? OrderIndex { get; set; }
        public string Url { get; set; }
        public bool IsButton { get; set; }
        public List<MenuVM> Childrens { get; set; }
    }
}
