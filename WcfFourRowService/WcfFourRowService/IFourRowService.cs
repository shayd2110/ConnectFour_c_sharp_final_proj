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
        void clientConnected(string userName, string hashedPasswd);

        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(UserExistsFault))]
        [FaultContract(typeof(Exception))]
        [OperationContract]
        void clientRegisterd(string userName, string hashedPasswd);

        [FaultContract(typeof(Exception))]
        [FaultContract(typeof(UserAlreadyConnectedFault))]
        [OperationContract]
        void clientDisconnected(string userName);



        [FaultContract(typeof(Exception))]
        [OperationContract]
        void clientDisconnectedBeforeGame(string userName, string opponent, string whosOut);

        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        [OperationContract]
        void clientDisconnectedThrowGame(string currentPlayer, string opponent, string whosOut);


        [FaultContract(typeof(Exception))]
        [OperationContract]
        void wantToPlayWithClient(string currentPlayer, string opponent);

        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        [OperationContract]
        void opponentAcceptToPlay(string currentPlayer, string opponent);

        [FaultContract(typeof(Exception))]
        [OperationContract]
        void opponentDeclineToPlay(string currentPlayer, string opponent);

        [OperationContract]
        List<string> getClientsThatNotPlayNow();

        #region DB methods
        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> getAllUsersGamesHistory();

        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> GetAllUsersInDB();

        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> getAllUsersGamesHistoryOrderedByName();

        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> getAllUsersGamesHistoryOrderedByGames();

        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> getAllUsersGamesHistoryOrderedByWins();

        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> getAllUsersGamesHistoryOrderedByLoses();

        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> getAllUsersGamesHistoryOrderedByPoints();

        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> allTheGamesThatPlayesSoFar();

        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> allTheGamesThatPlayesNow();

        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> allTheGamesBetweenTwoClients(string userName1, string userName2);

        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> allTheGamesOfSomeClient(string userName);

        List<UserHistory> allUsersGamesHistoryPrivately();

        #endregion

        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        [OperationContract]
        Tuple<MoveResult, int> reportMove(
          string currentPlayer,
          string opponent,
          string whoMoved,
          int location,
          double pointX,
          double pointY);

        [OperationContract]
        void getMeBackToWaitingList(string userName);
        

        [FaultContract(typeof(Exception))]
        [OperationContract]
        int getMyPos(string currentPlayer, string opponent, string whoWantIt);

        [FaultContract(typeof(Exception))]
        [OperationContract]
        void setMyPos(string currentPlayer, string opponent, string whoWantIt, int pos);

        [OperationContract]
        void killTheGame(string currentPlayer, string opponent);

        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        bool clearUsers();

        [OperationContract]
        bool ping();


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
