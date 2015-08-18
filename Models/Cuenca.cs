using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fondef.Models
{
    public class Cuenca
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public double Area { get; set; }
        public double Slope { get; set; }
        public string Coordinates { get; set; }
        public double LonCenter { get; set; }
        public double LatCenter { get; set; }
    }
}