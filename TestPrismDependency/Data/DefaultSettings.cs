using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPrismDependency.Data;

public class DefaultSettings
{
    public class LocationSettings
    {
        public double Latitude { get; set; } = double.NaN;
        public double Longitude { get; set; } = double.NaN;
    }

    public LocationSettings Location { get; } = new LocationSettings();
}
