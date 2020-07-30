using System.Runtime.Serialization;

/*WcfFourRowService namespace*/
namespace WcfFourRowService
{
    /*WrongPasswordFault class*/
    /*credit to - dani's code*/
    /// <summary>
    /// some exception
    /// </summary>
    [DataContract]
    public class WrongPasswordFault
    {
        [DataMember]
        public string Details { get; set; }

    }/*end of -WrongPasswordFault- class*/

}/*end of -WcfFourRowService- namespace*/
