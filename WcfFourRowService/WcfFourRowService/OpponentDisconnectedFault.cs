using System.Runtime.Serialization;

/*WcfFourRowService namespace*/
namespace WcfFourRowService
{
    /*OpponentDisconnectedFault class*/
    /*credit to - dani's code*/
    /// <summary>
    /// some exception
    /// </summary>
    [DataContract]
    public class OpponentDisconnectedFault
    {
        [DataMember]
        public string Details { get; set; }

    }/*end of -OpponentDisconnectedFault- class*/

}/*end of -WcfFourRowService- namespace*/
