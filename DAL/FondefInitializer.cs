using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Fondef.Models;

namespace Fondef.DAL
{
    public class FondefInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<FondefContext>
    {
        protected override void Seed(FondefContext context)
        {
            var cuencas = new List<Cuenca>
            {
            new Cuenca{Code="C1356744",Name="Cuenca CL1",Area=3000, Slope=25.5, Coordinates="[33.2,38.5]", LatCenter=33.567, LonCenter=-70.457},
            new Cuenca{Code="C1356755",Name="Cuenca CL2",Area=3100, Slope=30, Coordinates="[33.2,38.5]", LatCenter=33.567, LonCenter=-70.457},
            new Cuenca{Code="C1356766",Name="Cuenca CL3",Area=2002, Slope=58.5, Coordinates="[33.2,38.5]", LatCenter=33.567, LonCenter=-70.457},
            new Cuenca{Code="C1356777",Name="Cuenca CL4",Area=5500, Slope=10.5, Coordinates="[33.2,38.5]", LatCenter=33.567, LonCenter=-70.457},
            new Cuenca{Code="C1356788",Name="Cuenca CL5",Area=7090, Slope=0.5, Coordinates="[33.2,38.5]", LatCenter=33.567, LonCenter=-70.457}
            };

            cuencas.ForEach(c => context.Cuencas.Add(c));
            context.SaveChanges();
        }
    }
}