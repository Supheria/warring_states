using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalUtilities.GUICore;

public abstract class ViewModelBase : ObservableObject
{
    public virtual void EnableListener()
    {

    }

    public virtual void DisableListener()
    {

    }
}
