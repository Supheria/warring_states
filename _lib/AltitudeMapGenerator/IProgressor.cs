using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltitudeMapGenerator;

public interface IProgressor
{
    public void Reset(int total);

    public void Progress(int addon);
}
