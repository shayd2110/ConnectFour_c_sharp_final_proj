using System.Runtime.Serialization;

/*WcfFourRowService namespace*/
namespace WcfFourRowService
{
    /*UserExistsFault class*/
    /*credit to - dani's code*/
    /// <summary>
    /// some exception
    /// </summary>
    [DataContract]
    public class UserExistsFault
    {
        [DataMember]
        public string Details { get; set; }

    }/*end of -UserExistsFault- class*/

}/*end of -WcfFourRowService- namespace*/
