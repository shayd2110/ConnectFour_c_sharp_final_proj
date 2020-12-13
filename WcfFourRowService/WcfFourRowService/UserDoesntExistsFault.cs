using System.Runtime.Serialization;

/*WcfFourRowService namespace*/
namespace WcfFourRowService
{
    /*UserDoesntExistsFault class*/
    /// <summary>
    /// some fault exception
    /// </summary>
    [DataContract]
    public class UserDoesntExistsFault
    {
        [DataMember]
        public string Details { get; set; }

    }/*end of -UserDoesntExistsFault- class*/

}/*end of -WcfFourRowService- namespace*/
