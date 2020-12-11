using System.Runtime.Serialization;

namespace WcfFourRowService
{

    [DataContract]
    public class UserAlreadyConnectedFault
    {
        [DataMember]
        public string Details { get;  set; }
    }
}