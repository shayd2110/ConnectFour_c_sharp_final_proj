/*WcfFourRowService namespace*/
namespace WcfFourRowService
{
    /*UserHistory class*/
    /// <summary>
    /// class that represent user history's games
    /// </summary>
    public class UserHistory
    {
        /*properties*/
        public string UserName { get; set; }

        public int NumberOfGames { get; set; }

        public int NumberOfWinnings { get; set; }

        public int NumberOfLoses { get; set; }

        public int NumberOfPoints { get; set; }
        /*end of properties*/

        /*ToString method*/
        public override string ToString()
        {
            return $"{UserName}: •games: {NumberOfGames}, " +
                  $"•wins: {NumberOfWinnings}, •loses: {NumberOfLoses}, " +
                  $"•points: {NumberOfPoints}";

        }/*end of -ToString- method*/

    }/*end of -UserHistory- class*/

}/*end of -WcfFourRowService- namespace*/
