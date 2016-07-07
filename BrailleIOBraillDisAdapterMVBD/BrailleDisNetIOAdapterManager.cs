using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrailleIO;
using BrailleIO.Interface;

namespace BrailleIOBraillDisAdapter
{
    class BrailleDisNetIOAdapterManager : AbstractBrailleIOAdapterManagerBase
    {


         public BrailleDisNetIOAdapterManager()
            : base()
        { init(); }
         public BrailleDisNetIOAdapterManager(ref BrailleIO.BrailleIOMediator io)
            : base(ref io)
        { init(); }

        void init()
        {
            //push all supported devices and map events 
            IBrailleIOAdapterManager me = this;
            Adapters.Add(new BrailleIOAdapter_BrailleDisNet_MVBD(me));
        }


    }
}
