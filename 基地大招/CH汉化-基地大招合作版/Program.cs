using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK.Events;

namespace HumanziedBaseUlt
{
    class Program
    {
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += eventArgs => new Main();
        }
    }
}
