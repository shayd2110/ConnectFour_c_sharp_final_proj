using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

/*WcfFourRowService namespace*/
namespace WcfFourRowService
{
    /*IFourRowService interface*/
    /*credit to dani's code for some methods signatures*/
    /// <summary>
    /// interface that represent all of the operations that service offer to clients
    /// </summary>
    [ServiceContract(CallbackContract = typeof(IFourRowServiceCallback))]
    public interface IFourRowService
    {
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(UserDoesntExistsFault))]
        [FaultContract(typeof(WrongPasswordFault))]
        [FaultContract(typeof(Exception))]
        [OperationContract]
        void ClientConnected(string userName, string hashedPasswd);

        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(UserExistsFault))]
        [FaultContract(typeof(Exception))]
        [OperationContract]
        void ClientRegistered(string userName, string hashedPasswd);

        [FaultContract(typeof(Exception))]
        [FaultContract(typeof(UserAlreadyConnectedFault))]
        [OperationContract]
        void ClientDisconnected(string userName);



        [FaultContract(typeof(Exception))]
        [OperationContract]
        void ClientDisconnectedBeforeGame(string userName, string opponent, string whosOut);

        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        [OperationContract]
        void ClientDisconnectedThrowGame(string currentPlayer, string opponent, string whosOut);


        [FaultContract(typeof(Exception))]
        [OperationContract]
        void WantToPlayWithClient(string currentPlayer, string opponent);

        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        [OperationContract]
        void OpponentAcceptToPlay(string currentPlayer, string opponent);

        [FaultContract(typeof(Exception))]
        [OperationContract]
        void OpponentDeclineToPlay(string currentPlayer, string opponent);

        [OperationContract]
        List<string> GetClientsThatNotPlayNow();

        #region DB methods
        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> GetAllUsersGamesHistory();

        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> GetAllUsersInDb();

        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> GetAllUsersGamesHistoryOrderedByName();

        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> GetAllUsersGamesHistoryOrderedByGames();

        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> GetAllUsersGamesHistoryOrderedByWins();

        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> GetAllUsersGamesHistoryOrderedByLoses();

        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> GetAllUsersGamesHistoryOrderedByPoints();

        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> AllTheGamesThatPlayesSoFar();

        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> AllTheGamesThatPlayesNow();

        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> AllTheGamesBetweenTwoClients(string userName1, string userName2);

        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> AllTheGamesOfSomeClient(string userName);

        List<UserHistory> AllUsersGamesHistoryPrivately();

        #endregion

        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        [OperationContract]
        Tuple<MoveResult, int> ReportMove(
          string currentPlayer,
          string opponent,
          string whoMoved,
          int location,
          double pointX,
          double pointY);

        [OperationContract]
        void GetMeBackToWaitingList(string userName);
        

        [FaultContract(typeof(Exception))]
        [OperationContract]
        int GetMyPos(string currentPlayer, string opponent, string whoWantIt);

        [FaultContract(typeof(Exception))]
        [OperationContract]
        void SetMyPos(string currentPlayer, string opponent, string whoWantIt, int pos);

        [OperationContract]
        void KillTheGame(string currentPlayer, string opponent);

        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        bool ClearUsers();

        [OperationContract]
        bool Ping();


    }/*end of -IFourRowService- interface*/

    /*IFourRowCallback interface*/
    /*credit to dani's code for some methods signatures*/
    /// <summary>
    /// interface that represent all of the operations that service can do throw clients
    /// </summary>
    public interface IFourRowServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void OtherPlayerMoved(Tuple<MoveResult, int> moveResult, double pointX, double pointY); //heyOtherPlayerMoved

        [OperationContract(IsOneWay = true)]
        void NotifyOpponentChallenge(string currentPlayer); //listenSomeoneWantToPlayWithYou

        [OperationContract(IsOneWay = true)]
        void HeyOpponentDeclineToPlay();                //heyOpponentDeclineToPlay

        [OperationContract(IsOneWay = true)] 
        void OpponentAcceptToPlayLetsStart();           //heyOpponentAcceptToPlayLetsStart

        [OperationContract(IsOneWay = true)]
        void LetsStart();                                //okLetsStart

        [OperationContract(IsOneWay = true)]
        void OpponentDisconnectedBeforeTheGame(); //listenTheGuyDisconnectedBeforeTheGame

        [OperationContract(IsOneWay = true)]
        void OpponentDisconnectedThrowGameYouWon(); //listenTheGuyDisconnectedThrowGameYouWon

    }/*end of -IFourRowCallback- interface*/

}/*end of -WcfFourRowService- namespace*/
