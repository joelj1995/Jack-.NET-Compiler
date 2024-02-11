using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NJackOS.Interface
{
    public interface ICurrentKeyObserver
    {
        void OnCurrentKeyChanged(char key);
    }
}
