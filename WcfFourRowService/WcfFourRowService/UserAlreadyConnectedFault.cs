using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WcfFourRowService
{
    /*UserAlreadyConnectedFault class*/
    /// <summary>
    /// some fault exception
    /// </summary>
    /// 
    [DataContract]
    class UserAlreadyConnectedFault
    {
        [DataMember]
        public string Details { get; set; }
    }/*end of -UserAlreadyConnectedFault- class*/
}/*end of -WcfFourRowService- namespace*/
