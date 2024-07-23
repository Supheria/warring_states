using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Events;

namespace WarringStates.UI;

public class KeyPressArgs(Keys value) : ICallbackArgs
{
    public Keys Value { get; } = value;
}
