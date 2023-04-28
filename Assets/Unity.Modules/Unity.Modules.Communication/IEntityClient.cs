using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IEntityClient
{
    DevInfo GetDevByid(int id);
}
