using FourRowClient.FourRowServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourRowClient 
{
    public class ClientCallback : IFourRowServiceCallback
    {
        public void OtherPlayerConnected()
        {
            throw new NotImplementedException();
        }

        public void OtherPlayerMoved(MoveResult moveResult, int location)
        {
            throw new NotImplementedException();
        }
    }
}
