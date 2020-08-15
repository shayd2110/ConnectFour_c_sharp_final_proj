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
        /*clientConnected method signature*/
        [FaultContract(typeof(UserAlreadyConnectedFault))]
        [FaultContract(typeof(UserDoesntExistsFault))]
        [FaultContract(typeof(WrongPasswordFault))]
        [OperationContract]
        void clientConnected(string userName, string hashedPasswd);

        /*clientRegisterd method signature*/
        [FaultContract(typeof(UserExistsFault))]
        [OperationContract]
        void clientRegisterd(string userName, string hashedPasswd);

        /*clientDisconnected method signature*/
        [FaultContract(typeof(OpponentDisconnectedFault))]
        [OperationContract]
        void clientDisconnected(string userName);

        /*getClientsThatNotPlayNow method signature*/
        [OperationContract]
        IEnumerable<string> getClientsThatNotPlayNow();

        /*some methods signatures that connected to data base queries*/
        
        /*getAllUsersGamesHistory method signature*/
        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> getAllUsersGamesHistory();

        /*getAllUsersGamesHistoryOrderedByName method signature*/
        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> getAllUsersGamesHistoryOrderedByName();

        /*getAllUsersGamesHistoryOrderedByGames method signature*/
        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> getAllUsersGamesHistoryOrderedByGames();

        /*getAllUsersGamesHistoryOrderedByWins method signature*/
        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> getAllUsersGamesHistoryOrderedByWins();

        /*getAllUsersGamesHistoryOrderedByLoses method signature*/
        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> getAllUsersGamesHistoryOrderedByLoses();

        /*getAllUsersGamesHistoryOrderedByPoints method signature*/
        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> getAllUsersGamesHistoryOrderedByPoints();

        /*allTheGamesThatPlayesSoFar method signature*/
        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> allTheGamesThatPlayesSoFar();

        /*allTheGamesThatPlayesNow method signature*/
        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> allTheGamesThatPlayesNow();

        /*allTheGamesBetweenTwoClients method signature*/
        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> allTheGamesBetweenTwoClients(string userName1,string userName2);

        /*allTheGamesOfSomeClient method signature*/
        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        List<string> allTheGamesOfSomeClient(string userName);

        /*allUsersGamesHistoryPrivately method*/
        List<UserHistory> allUsersGamesHistoryPrivately();

        /*end of some methods signatures that connected to data base queries*/

        [OperationContract]
        [FaultContract(typeof(DbException))]
        [FaultContract(typeof(Exception))]
        bool clearUsers();


        [OperationContract]
        [FaultContract(typeof(OpponentDisconnectedFault))]
        MoveResult ReportMove(int RowLocation,int ColLocation, int player);

        /*ping method signature*/
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
        void OtherPlayerConnected();

        [OperationContract(IsOneWay = true)]
        void OtherPlayerMoved(MoveResult moveResult, int location);

    }/*end of -IFourRowCallback- interface*/

}/*end of -WcfFourRowService- namespace*/
