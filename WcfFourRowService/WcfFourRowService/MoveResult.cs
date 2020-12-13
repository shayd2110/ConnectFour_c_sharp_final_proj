using System.Runtime.Serialization;

/*WcfFourRowService namespace*/
namespace WcfFourRowService
{
    /*MoveResult enum*/
    /*credit to - dani's code*/
    /// <summary>
    /// enum that represent result of the four row game
    /// </summary>
    [DataContract]
    public enum MoveResult
    {
        [EnumMember] YouWon,
        [EnumMember] YouLose,
        [EnumMember] Draw,
        [EnumMember] NotYourTurn,
        [EnumMember] GameOn,
        [EnumMember] Nothing,
        [EnumMember] IllegalMove,

    }/*end of -MoveResult- enum*/

}/*end of -WcfFourRowService- namespace*/
