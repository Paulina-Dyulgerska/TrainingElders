using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerService1
{
    public class ChatConnector
    {
        private readonly Nested gg;

        public ChatConnector(Nested gg)
        {
            this.gg = gg;
        }

    }

    public class Nested { }

}
