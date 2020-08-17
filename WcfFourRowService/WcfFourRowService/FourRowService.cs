using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows.Forms;

/*WcfFourRowService namespace*/
namespace WcfFourRowService
{
    /*FourRowService class*/
    /*credit to dani's code for some methods implementations*/
    /// <summary>
    /// class that implements the IFourRowService - same brief as there
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Multiple,
        IncludeExceptionDetailInFaults =true, 
        UseSynchronizationContext = false)]
   
    public class FourRowService : IFourRowService
    {
        /*data members*/
        private int NumClients { get; set; }
        private int ConnectedClientsCnt { get;  set; }
        public Dictionary<string, IFourRowServiceCallback> Clients = new Dictionary<string, IFourRowServiceCallback>();
        public Dictionary<string, IFourRowServiceCallback> ConnectedClient = new Dictionary<string, IFourRowServiceCallback>();
        private readonly List<string> _clientsThatNotPlay = new List<string>();
        private readonly List<Tuple<string, string>> _sideModeClients = new List<Tuple<string, string>>();
        private readonly Dictionary<Tuple<string, string>, Game> _games = new Dictionary<Tuple<string, string>, Game>();


        #region Register&login

        public void ClientConnected(string userName, string hashedPasswd)
        {
            if (ConnectedClient.ContainsKey(userName))
                throw new FaultException<UserAlreadyConnectedFault>(new UserAlreadyConnectedFault()
                {
                    Details = "User name " + userName + " Already Connected"
                });
            using (var ctx = new FourinrowDBContext())
            {
                var user = (from u in ctx.Users
                            where u.UserName == userName
                            select u).FirstOrDefault();
                if (user == null)
                    throw new FaultException<UserDoesntExistsFault>(new UserDoesntExistsFault()
                    {
                        Details = "User name " + userName + " Doesnt exists. Please register"
                    });
                if (user.HashedPassword != hashedPasswd)
                    throw new FaultException<WrongPasswordFault>(new WrongPasswordFault()
                    {
                        Details = "Worng PassWord!"
                    });
                if (!Clients.ContainsKey(userName))
                {
                    IFourRowServiceCallback callbackChannel = OperationContext.Current.GetCallbackChannel<IFourRowServiceCallback>();
                    Clients.Add(userName, callbackChannel);
                }
                if (!ConnectedClient.ContainsKey(userName))
                    ConnectedClient.Add(userName, Clients[userName]);
                if (!_clientsThatNotPlay.Contains(userName))
                    _clientsThatNotPlay.Add(userName);
                ++ConnectedClientsCnt;
            }
        }

        public void ClientRegistered(string userName, string hashedPasswd)
        {
            if (Clients.ContainsKey(userName))
                throw new FaultException<UserExistsFault>(new UserExistsFault()
                {
                    Details = "User name " + userName + " already exists. Try something else"
                });
            using (var ctx = new FourinrowDBContext())
            {
                var user = (from u in ctx.Users
                            where u.UserName == userName
                            select u).FirstOrDefault();
                if (user == null)
                {
                    User newUser = new User()
                    {
                        UserName = userName,
                        HashedPassword = hashedPasswd
                    };
                    ctx.Users.Add(newUser);
                    ctx.SaveChanges();
                }
                ++NumClients;
                IFourRowServiceCallback callbackChannel = OperationContext.Current.GetCallbackChannel<IFourRowServiceCallback>();
                Clients.Add(userName, callbackChannel);
            }
        }

        public void ClientDisconnected(string userName)
        {
            try
            {
                if (ConnectedClient.ContainsKey(userName))
                    ConnectedClient.Remove(userName);
                if (_clientsThatNotPlay.Contains(userName))
                    _clientsThatNotPlay.Remove(userName);
                --ConnectedClientsCnt;
            }
            catch (Exception ex)
            {
                throw new FaultException<Exception>(ex);
            }
        }

        #endregion

        #region DB & Search functions

        public List<string> GetClientsThatNotPlayNow()
        {
            return new List<string>(_clientsThatNotPlay);
        }

        public List<string> GetAllUsersInDb()
        {
            try
            {
                List<string> res = new List<string>();
                using (var ctx = new FourinrowDBContext())
                {
                    List<string> users = (from u in ctx.Users
                                          select u.UserName).ToList();
                    if (users.FirstOrDefault() == null)
                    {
                        res.Add("DB is empty");
                        return res;
                    }
                    res.Add($"There is {users.Count} users in db");

                    foreach (var str in users)
                        res.Add(str);
                    return res;
                }
            }
            catch (DbException ex)
            {
                throw new FaultException<DbException>(ex);
            }
            catch (Exception ex)
            {
                throw new FaultException<Exception>(ex);
            }
        }

        public List<string> GetAllUsersGamesHistory()
        {
            List<string> allUsersGamesHistory = new List<string>();

            try
            {
                List<UserHistory> usersHistory = AllUsersGamesHistoryPrivately();
                foreach (var uh in usersHistory)
                    allUsersGamesHistory.Add(uh.ToString());
                return allUsersGamesHistory;
            }
            catch (DbException ex)
            {
                throw new FaultException<DbException>(ex);
            }
            catch (Exception ex)
            {
                throw new FaultException<Exception>(ex);
            }
        }

        public List<string> GetAllUsersGamesHistoryOrderedByName()
        {
            List<string> allUsersGamesHistoryOrderedByName = new List<string>();

            try
            {
                List<UserHistory> usersHistory = AllUsersGamesHistoryPrivately();

                /*order it by name*/
                var usersHistoryOrderdByName = (from uh in usersHistory
                                                orderby uh.UserName
                                                select uh).ToList();

                foreach (var uhobn in usersHistoryOrderdByName)
                {
                    allUsersGamesHistoryOrderedByName.Add(uhobn.ToString());
                }

                return allUsersGamesHistoryOrderedByName;
            }
            catch (DbException dex)
            {
                throw new FaultException<DbException>(dex);
            }
            catch (Exception ex)
            {
                throw new FaultException<Exception>(ex);
            }
        }

        public List<string> GetAllUsersGamesHistoryOrderedByGames()
        {
            List<string> allUsersGamesHistoryOrderedByGames = new List<string>();

            try
            {
                List<UserHistory> usersHistory = AllUsersGamesHistoryPrivately();

                /*order it by games*/
                var usersHistoryOrderdByGames = (from uh in usersHistory
                                                 orderby uh.NumberOfGames descending
                                                 select uh).ToList();

                foreach (var uhobg in usersHistoryOrderdByGames)
                {
                    allUsersGamesHistoryOrderedByGames.Add(uhobg.ToString());
                }

                return allUsersGamesHistoryOrderedByGames;
            }
            catch (DbException dex)
            {
                throw new FaultException<DbException>(dex);
            }
            catch (Exception ex)
            {
                throw new FaultException<Exception>(ex);
            }
        }

        public List<string> GetAllUsersGamesHistoryOrderedByWins()
        {
            List<string> allUsersGamesHistoryOrderedByWins = new List<string>();

            try
            {
                List<UserHistory> usersHistory = AllUsersGamesHistoryPrivately();

                /*order it by wins*/
                var usersHistoryOrderdByWins = (from uh in usersHistory
                                                orderby uh.NumberOfWinnings descending
                                                select uh).ToList();

                foreach (var uhobw in usersHistoryOrderdByWins)
                {
                    allUsersGamesHistoryOrderedByWins.Add(uhobw.ToString());
                }

                return allUsersGamesHistoryOrderedByWins;
            }
            catch (DbException dex)
            {
                throw new FaultException<DbException>(dex);
            }
            catch (Exception ex)
            {
                throw new FaultException<Exception>(ex);
            }
        }

        public List<string> GetAllUsersGamesHistoryOrderedByLoses()
        {
            List<string> allUsersGamesHistoryOrderedByLoses = new List<string>();

            try
            {
                List<UserHistory> usersHistory = AllUsersGamesHistoryPrivately();

                /*order it by loses*/
                var usersHistoryOrderdByLoses = (from uh in usersHistory
                                                 orderby uh.NumberOfLoses
                                                 select uh).ToList();

                foreach (var uhobl in usersHistoryOrderdByLoses)
                {
                    allUsersGamesHistoryOrderedByLoses.Add(uhobl.ToString());
                }

                return allUsersGamesHistoryOrderedByLoses;
            }
            catch (DbException dex)
            {
                throw new FaultException<DbException>(dex);
            }
            catch (Exception ex)
            {
                throw new FaultException<Exception>(ex);
            }
        }

        public List<string> GetAllUsersGamesHistoryOrderedByPoints()
        {
            List<string> allUsersGamesHistoryOrderedByPoints = new List<string>();

            try
            {
                List<UserHistory> usersHistory = AllUsersGamesHistoryPrivately();

                /*order it by points*/
                var usersHistoryOrderdByPoints = (from uh in usersHistory
                                                  orderby uh.NumberOfPoints descending
                                                  select uh).ToList();

                foreach (var uhobp in usersHistoryOrderdByPoints)
                {
                    allUsersGamesHistoryOrderedByPoints.Add(uhobp.ToString());
                }

                return allUsersGamesHistoryOrderedByPoints;
            }
            catch (DbException dex)
            {
                throw new FaultException<DbException>(dex);
            }
            catch (Exception ex)
            {
                throw new FaultException<Exception>(ex);
            }
        }

        public List<string> AllTheGamesThatPlayesSoFar()
        {
            List<string> gamesthatPlayedSofar = new List<string>();

            try
            {
                using (var ctx = new FourinrowDBContext())
                {
                    var gamesSoFar = (from gd in ctx.GameDetails
                                      select gd).ToList();
                    int i = 1;

                    /*arrange the things*/
                    foreach (var gsf in gamesSoFar)
                    {
                        string someString = $" {i++}: {gsf.User1Name} against {gsf.User2Name}, ";

                        if (gsf.WinningUserName == "draw")
                            someString += "draw, ";
                        else
                        {
                            int winnerPoints = gsf.WinningUserName == gsf.User1Name ? gsf.PointsUser1 : gsf.PointsUser2;
                            someString += $"winner: {gsf.WinningUserName},  winner points: {winnerPoints}, ";
                        }
                        someString += $"game date: {gsf.GameDateStart.ToString("d", DateTimeFormatInfo.InvariantInfo)}";

                        gamesthatPlayedSofar.Add(someString);

                    }/*end of loop*/

                    return gamesthatPlayedSofar;

                }/*end of using*/
            }
            catch (DbException dex)
            {
                throw new FaultException<DbException>(dex);
            }
            catch (Exception ex)
            {
                throw new FaultException<Exception>(ex);
            }

        }

        public List<string> AllTheGamesThatPlayesNow()
        {
            List<string> gamesThatPlaysNow = new List<string>();

            try
            {
                using (var ctx = new FourinrowDBContext())
                {
                    var gamesNow = (from gdn in ctx.GameDetailsNows
                                    select gdn).ToList();

                    int i; i = 1;

                    /*doing the thing*/
                    foreach (var gn in gamesNow)
                    {
                        gamesThatPlaysNow.Add($" {i++}: {gn.User1Name} against " +
                            $"{gn.User2Name}, start time: " +
                            $"{gn.GameDateStart.ToString("t", DateTimeFormatInfo.InvariantInfo)}");

                    }/*end of loop*/

                    return gamesThatPlaysNow;

                }/*end of using*/
            }
            catch (DbException dex)
            {
                throw new FaultException<DbException>(dex);
            }
            catch (Exception ex)
            {
                throw new FaultException<Exception>(ex);
            }

        }

        public List<string> AllTheGamesBetweenTwoClients(string userName1, string userName2)
        {
            List<string> gamesBetweenTwoClients = new List<string>();

            try
            {
                using (var ctx = new FourinrowDBContext())
                {
                    var thoseGames = (from gd in ctx.GameDetails
                                      where ((gd.User1Name == userName1 && gd.User2Name == userName2) ||
                                             (gd.User1Name == userName2 && gd.User2Name == userName1))
                                      select gd).ToList();

                    if (thoseGames.Count == 0)
                        return gamesBetweenTwoClients;

                    gamesBetweenTwoClients.Add($"           the games between {userName1} and {userName2}   \n" +
                                              $"                        total games: {thoseGames.Count}   ");

                    int i = 1;
                    int numOfWinsUser1, numOfWinsUser2;
                    numOfWinsUser1 = numOfWinsUser2 = 0;

                    foreach (var tg in thoseGames)
                    {
                        string someString = $" {i++}: game date: {tg.GameDateStart.ToString("g", DateTimeFormatInfo.InvariantInfo)}, ";

                        if (tg.WinningUserName == "draw")
                            someString += "draw";
                        else
                        {
                            someString += $"winner: {tg.WinningUserName}";

                            if (tg.WinningUserName == userName1)
                                numOfWinsUser1++;
                            else
                                numOfWinsUser2++;
                        }

                        gamesBetweenTwoClients.Add(someString);

                    }/*end of loop*/

                    string someOtherString = "\n           wins ratio (percentage)   \n";

                    someOtherString += $"{userName1}: {String.Format("{0:0.00}", Convert.ToDecimal(numOfWinsUser1) / thoseGames.Count * 100)}%      " +
                                      $"{userName2}: {String.Format("{0:0.00}", Convert.ToDecimal(numOfWinsUser2) / thoseGames.Count * 100)}%";

                    gamesBetweenTwoClients.Add(someOtherString);

                    return gamesBetweenTwoClients;

                }/*end of using*/
            }
            catch (DbException dex)
            {
                throw new FaultException<DbException>(dex);
            }
            catch (Exception ex)
            {
                throw new FaultException<Exception>(ex);
            }

        }

        public List<string> AllTheGamesOfSomeClient(string userName)
        {
            List<string> theGamesOfSomeClient = new List<string>();

            try
            {
                using (var ctx = new FourinrowDBContext())
                {
                    var gamesOfSomeClient = (from gd in ctx.GameDetails
                                             where (gd.User1Name == userName ||
                                                  gd.User2Name == userName)
                                             select gd).ToList();

                    if (gamesOfSomeClient.Count == 0)
                        return theGamesOfSomeClient;

                    theGamesOfSomeClient.Add($"             all the games of {userName}   \n" +
                                             $"                   total games: {gamesOfSomeClient.Count}   ");

                    int i = 1;
                    int totalWinnings, totalLoses, totalPoints;
                    totalWinnings = totalLoses = totalPoints = 0;

                    foreach (var gosc in gamesOfSomeClient)
                    {
                        string someString = $" {i++}: {userName} against ";

                        if (gosc.User1Name == userName)
                            someString += $"{gosc.User2Name}";
                        else
                            someString += $"{gosc.User1Name}";

                        someString += $", game date: {gosc.GameDateStart.ToString("g", DateTimeFormatInfo.InvariantInfo)}, ";

                        if (gosc.WinningUserName == "draw")
                            someString += "draw";
                        else
                        {
                            someString += $"winner: {gosc.WinningUserName}";

                            if (gosc.WinningUserName == userName)
                                totalWinnings++;
                            else
                                totalLoses++;
                        }

                        if (gosc.User1Name == userName)
                            totalPoints += gosc.PointsUser1;
                        else
                            totalPoints += gosc.PointsUser2;

                        theGamesOfSomeClient.Add(someString);

                    }/*end of loop*/

                    string someOtherString = "\n            conclusins   \n";

                    someOtherString += $"•winnings: {totalWinnings},    •losses: {totalLoses}\n";
                    someOtherString += $"win ratio (precentage): {String.Format("{0:0.00}", Convert.ToDecimal(totalWinnings) / gamesOfSomeClient.Count * 100)}%\n";
                    someOtherString += $"total points: {totalPoints}";

                    theGamesOfSomeClient.Add(someOtherString);

                    return theGamesOfSomeClient;

                }/*end of using*/
            }
            catch (DbException dex)
            {
                throw new FaultException<DbException>(dex);
            }
            catch (Exception ex)
            {
                throw new FaultException<Exception>(ex);
            }

        }

        /*getAllUsersEver method*/
        public List<string> GetAllUsersEver()
        {
            List<string> allUsers = new List<string>();

            try
            {
                using (var ctx = new FourinrowDBContext())
                {
                    var theUsersRequested = (from u in ctx.Users
                                             select u).ToList();

                    foreach (var tur in theUsersRequested)
                    {
                        allUsers.Add(tur.UserName);
                    }

                    return allUsers;

                }/*end of using*/

            }
            catch (DbException dex)
            {
                throw new FaultException<DbException>(dex);
            }
            catch (Exception ex)
            {
                throw new FaultException<Exception>(ex);
            }

        }/*end of -getAllUsersEver- method*/
        public List<UserHistory> AllUsersGamesHistoryPrivately()
        {
            List<UserHistory> allUsersGamesHistory = new List<UserHistory>();
            int numOfPlays, numOfWins, numOfLoses, numOfPoints;

            using (var ctx = new FourinrowDBContext())
            {
                var allUsers = (from u in ctx.Users
                                select u.UserName).ToList();

                var allGames = (from gd in ctx.GameDetails
                                select gd).ToList();

                /*get the details of each user*/
                foreach (var user in allUsers)
                {
                    numOfPlays = numOfWins = numOfLoses = numOfPoints = 0;

                    foreach (var game in allGames)
                    {
                        if (!(user == game.User1Name || user == game.User2Name))
                            continue;

                        numOfPlays++;
                        numOfWins += (user == game.WinningUserName ? 1 : 0);
                        numOfLoses += (user == game.WinningUserName ? 0 : (game.WinningUserName == "draw" ? 0 : 1));
                        numOfPoints += (user == game.User1Name ? game.PointsUser1 : game.PointsUser2);

                    }/*end of inner loop*/

                    UserHistory userHistory = new UserHistory()
                    {
                        UserName = user,
                        NumberOfGames = numOfPlays,
                        NumberOfWinnings = numOfWins,
                        NumberOfLoses = numOfLoses,
                        NumberOfPoints = numOfPoints
                    };

                    allUsersGamesHistory.Add(userHistory);

                }/*end of outer loop*/

                return allUsersGamesHistory;
            }

        }

        public bool ClearUsers()
        {
            try
            {
                using (var ctx = new FourinrowDBContext())
                {
                    string res = "elements: ";
                    int cnt = 0;
                    var x = (from u in ctx.Users
                        select u); // clear db
                    foreach (var item in x)
                    {
                        if (item.UserName != null)
                            res += item.UserName + " ";
                        cnt++;
                        if (x.FirstOrDefault() == null)
                            break;
                        ctx.Users.Remove(item);
                        //ctx.SaveChanges();
                    }
                    MessageBox.Show(res + " element count: " + cnt.ToString());

                    ctx.SaveChanges();

                    var x1 = (from u in ctx.Users
                        orderby u.UserName descending
                        select u).FirstOrDefault();
                    if (x1 == null)
                    {
                        Clients.Clear();
                        return true;
                    }

                }
            }
            catch (DbException dex)
            {
                throw new FaultException<DbException>(dex);
            }
            catch (Exception ex)
            {
                throw new FaultException<Exception>(ex);
            }
            return false;
        }
        #endregion

        #region WaitingRoomFunctions


        public void WantToPlayWithClient(string currentPlayer, string opponent)
        {
            try
            {
                if (_clientsThatNotPlay.Contains(currentPlayer))
                    _clientsThatNotPlay.Remove(currentPlayer);

                if (_clientsThatNotPlay.Contains(opponent))
                    _clientsThatNotPlay.Remove(opponent);

                _sideModeClients.Add(Tuple.Create(currentPlayer, opponent));

                if (!ConnectedClient.ContainsKey(opponent))
                    return;

                Thread notifyOpponentChallengeThread = new Thread(() => Clients[opponent].NotifyOpponentChallenge(currentPlayer))
                {
                    Name = "notifyOpponentChallenge"
                };
                notifyOpponentChallengeThread.Start();
            }
            catch (Exception ex)
            {
                throw new FaultException<Exception>(ex);
            }
        }

        public void OpponentAcceptToPlay(string currentPlayer, string opponent)
        {
            try
            {
                if (!(Clients.ContainsKey(currentPlayer) && Clients.ContainsKey(opponent)))
                    return;

                if (_clientsThatNotPlay.Contains(currentPlayer))
                    _clientsThatNotPlay.Remove(currentPlayer);

                if (_clientsThatNotPlay.Contains(opponent))
                    _clientsThatNotPlay.Remove(opponent);

                Tuple<string, string> thisGamePlayers = Tuple.Create(currentPlayer, opponent);

                if (_sideModeClients.Contains(thisGamePlayers))
                    _sideModeClients.Remove(thisGamePlayers);

                if (!_games.ContainsKey(thisGamePlayers))
                    _games.Add(thisGamePlayers, new Game());

                using (var ctx = new FourinrowDBContext())
                {
                    ctx.GameDetailsNows.Add(new GameDetailsNow()
                    {
                        User1Name = currentPlayer,
                        User2Name = opponent,
                        GameDateStart = DateTime.Now
                    });


                    ctx.SaveChanges();

                    ctx.GameDetails.Add(new GameDetail()
                    {
                        User1Name = currentPlayer,
                        User2Name = opponent,
                        WinningUserName = "---",
                        PointsUser1 = -1,
                        PointsUser2 = -1,
                        GameDateStart = DateTime.Now,
                        GameDateEnd = DateTime.Now
                    });
                    ctx.SaveChanges();
                }
                if (ConnectedClient.ContainsKey(currentPlayer))
                {
                    Thread updateOtherPlayerThread = new Thread(() =>
                    {
                        Clients[currentPlayer].OpponentAcceptToPlayLetsStart();
                    }
);

                    updateOtherPlayerThread.Start();
                }

                if (ConnectedClient.ContainsKey(opponent))
                {
                    Thread updateOtherPlayerThread = new Thread(() =>
                    {
                        Clients[opponent].LetsStart();
                    }
                );

                    updateOtherPlayerThread.Start();
                }
            }
            catch (DbException ex)
            {
                throw new FaultException<DbException>(ex);
            }
            catch (Exception ex)
            {
                throw new FaultException<Exception>(ex);
            }
        }

        public void OpponentDeclineToPlay(string currentPlayer, string opponent)
        {
            try
            {
                if (Clients.ContainsKey(currentPlayer))
                {
                    Thread notifyChallengeAcceptedThread = new Thread(() =>
                    {
                        ConnectedClient[currentPlayer].HeyOpponentDeclineToPlay();
                    });
                    notifyChallengeAcceptedThread.Start();
                }

                Tuple<string, string> thisGamePlayers = Tuple.Create(currentPlayer, opponent);

                if (_sideModeClients.Contains(thisGamePlayers))
                    _sideModeClients.Remove(thisGamePlayers);

                if (!_clientsThatNotPlay.Contains(currentPlayer) && ConnectedClient.ContainsKey(currentPlayer))
                    _clientsThatNotPlay.Add(currentPlayer);

                if (!_clientsThatNotPlay.Contains(opponent) && ConnectedClient.ContainsKey(opponent))
                    _clientsThatNotPlay.Add(opponent);
            }
            catch (Exception ex)
            {
                throw new FaultException<Exception>(ex);
            }
        }

        public void ClientDisconnectedBeforeGame(string userName, string opponent, string whosOut)
        {
            try
            {
                ClientDisconnected(whosOut);
                Tuple<string, string> option1Key = Tuple.Create(userName, opponent);
                Tuple<string, string> option2Key = Tuple.Create(opponent, userName);

                if (_games.ContainsKey(option1Key))
                    _games.Remove(option2Key);

                if (_games.ContainsKey(option2Key))
                    _games.Remove(option1Key);

                if (_sideModeClients.Contains(option1Key))
                    _sideModeClients.Remove(option2Key);

                if (_sideModeClients.Contains(option2Key))
                    _sideModeClients.Remove(option1Key);

                string player2Notify = whosOut == userName ? opponent : userName;
                if (ConnectedClient.ContainsKey(player2Notify))
                {
                    Thread notifyOpponentThread = new Thread(() => Clients[player2Notify].OpponentDisconnectedBeforeTheGame());
                    notifyOpponentThread.Start();
                }

                if (_clientsThatNotPlay.Contains(player2Notify))
                    return;

                _clientsThatNotPlay.Add(player2Notify);
            }
            catch (Exception ex)
            {
                throw new FaultException<Exception>(ex);
            }
        }

        #endregion

        #region GameFunctions

        public void ClientDisconnectedThrowGame(string currentPlayer, string opponent, string whosOut)
        {
            try
            {
                Tuple<string, string> thisGamePlayers = Tuple.Create(currentPlayer, opponent);
                Game theGame = null;
                if (_games.ContainsKey(thisGamePlayers))
                {
                    theGame = _games[thisGamePlayers];
                    _games.Remove(thisGamePlayers);
                }
                using (var ctx = new FourinrowDBContext())
                {
                    var removeGameNow = (from gdw in ctx.GameDetailsNows
                                         where ((gdw.User1Name == currentPlayer && gdw.User2Name == opponent) ||
                                               (gdw.User1Name == opponent && gdw.User2Name == currentPlayer))
                                         select gdw).FirstOrDefault();

                    if (removeGameNow != null)
                    {
                        ctx.GameDetailsNows.Remove(removeGameNow);
                        ctx.SaveChanges();
                    }

                    string winnerName = whosOut == currentPlayer ? opponent : currentPlayer;

                    UpdatePointAndHistory(currentPlayer, opponent, winnerName, theGame, ctx);
                    ctx.SaveChanges();

                    string player2Notify = whosOut == currentPlayer ? opponent : currentPlayer;

                    if (ConnectedClient.ContainsKey(player2Notify))
                        new Thread(() =>
                        {
                            Clients[player2Notify].OpponentDisconnectedThrowGameYouWon();
                        }).Start();

                    if (_clientsThatNotPlay.Contains(player2Notify))
                        return;

                    _clientsThatNotPlay.Add(player2Notify);
                }
            }
            catch (DbException ex)
            {
                throw new FaultException<DbException>(ex);
            }
            catch (Exception ex)
            {
                throw new FaultException<Exception>(ex);
            }
        }

        public Tuple<MoveResult, int> ReportMove(
            string currentPlayer,
            string opponent,
            string whoMoved,
            int location,
            double pointX,
            double pointY)
        {
            try
            {
                string player = whoMoved == currentPlayer ? currentPlayer : opponent;
                string player2Notify = whoMoved == currentPlayer ? opponent : currentPlayer;

                Game theGame = _games[Tuple.Create(currentPlayer, opponent)];
                IFourRowServiceCallback callback = ConnectedClient.ContainsKey(player2Notify) ? ConnectedClient[player2Notify] : null;

                Tuple<MoveResult, int> result = theGame.VerifyMove(currentPlayer, opponent, whoMoved, location);

                switch (result.Item1)
                {
                    case MoveResult.NotYourTurn:
                    case MoveResult.IllegalMove:
                        return result;
                    case MoveResult.YouWon:
                    case MoveResult.Draw:
                    {
                        Thread notifyOppnentWinOrTieThread = new Thread(() => WinOrDraw(currentPlayer, opponent, theGame, result.Item1, player));
                        notifyOppnentWinOrTieThread.Start();
                        break;
                    }
                }

                Thread notifyOppnentMoveThread = new Thread(() => callback?.OtherPlayerMoved(result, pointX, pointY));
                notifyOppnentMoveThread.Start();

                return result;
            }
            catch (DbException ex)
            {
                throw new FaultException<DbException>(ex);
            }
            catch (Exception ex)
            {
                throw new FaultException<Exception>(ex);
            }
        }

        private void WinOrDraw(
            string currentPlayer,
            string opponent,
            Game theGame,
            MoveResult result,
            string whoMoved)
        {
            using (FourinrowDBContext ctx = new FourinrowDBContext())
            {
                GameDetailsNow gameDetailsNow = (from gdn in ctx.GameDetailsNows
                    where ((gdn.User1Name == currentPlayer && gdn.User2Name == opponent) ||
                           (gdn.User1Name == opponent && gdn.User2Name == currentPlayer))
                    select gdn).FirstOrDefault();
                if (gameDetailsNow != null)
                {
                    ctx.GameDetailsNows.Remove(gameDetailsNow);
                    ctx.SaveChanges();
                }
                string winnerName = result == MoveResult.Draw ? "draw" : whoMoved;
                UpdatePointAndHistory(currentPlayer, opponent, winnerName, theGame, ctx);
                ctx.SaveChanges();
            }
        }

        public void GetMeBackToWaitingList(string userName)
        {
            if (!_clientsThatNotPlay.Contains(userName))
                _clientsThatNotPlay.Add(userName);
        }

        public int GetMyPos(string currentPlayer, string opponent, string whoWantIt)
        {
            try
            {
                Tuple<string, string> thisGamePlayers = Tuple.Create(currentPlayer, opponent);
                Game game = _games.ContainsKey(thisGamePlayers) ? _games[thisGamePlayers] : null;
                int pos = -1;
                if (game != null)
                    pos = game.GetPos(currentPlayer, whoWantIt);
                return pos;
            }
            catch (Exception ex)
            {
                throw new FaultException<Exception>(ex);
            }
        }

        public void SetMyPos(string currentPlayer, string opponent, string whoWantIt, int pos)
        {
            try
            {
                Tuple<string, string> thisGamePlayers = Tuple.Create(currentPlayer, opponent);
                if (_games.ContainsKey(thisGamePlayers))
                    _games[thisGamePlayers].SetPos(currentPlayer, whoWantIt, pos);
            }
            catch (Exception ex)
            {
                throw new FaultException<Exception>(ex);
            }
        }

        public void KillTheGame(string currentPlayer, string opponent)
        {
            Tuple<string, string> thisGamePlayers = Tuple.Create(currentPlayer, opponent);
            if (_games.ContainsKey(thisGamePlayers))
                _games.Remove(thisGamePlayers);
        }


        private void UpdatePointAndHistory(
            string currentPlayer,
            string opponent,
            string winnerName,
            Game theGame,
            FourinrowDBContext ctx)
        {
            Tuple<int, int> points = theGame.CalcPoints(currentPlayer, opponent, winnerName);
            DateTime endGameDate = DateTime.Now;
            GameDetail updateGameHistory = (from gd in ctx.GameDetails
                where ((gd.User1Name == currentPlayer && gd.User2Name == opponent &&
                        gd.PointsUser1 == -1 && gd.PointsUser2 == -1) ||
                       (gd.User1Name == opponent && gd.User2Name == currentPlayer &&
                        gd.PointsUser1 == -1 && gd.PointsUser2 == -1))
                select gd).FirstOrDefault();
            if (updateGameHistory == null)
                return;
            updateGameHistory.WinningUserName = winnerName;
            updateGameHistory.PointsUser1 = points.Item1;
            updateGameHistory.PointsUser2 = points.Item2;
            updateGameHistory.GameDateEnd = endGameDate;
            ctx.SaveChanges();
        }

        #endregion

        public bool Ping() => true;






    }/*end of -FourRowService- class*/

}/*end of -WcfFourRowService- namespace*/
