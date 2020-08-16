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
        private int numClients = 0;
        private int connectedClientsCnt = 0;
        public Dictionary<string, IFourRowServiceCallback> clients = new Dictionary<string, IFourRowServiceCallback>();
        public Dictionary<string, IFourRowServiceCallback> connectedClient = new Dictionary<string, IFourRowServiceCallback>();
        private List<string> clientsThatNotPlay = new List<string>();
        private List<Tuple<string, string>> sideModeClients = new List<Tuple<string, string>>();
        private Dictionary<Tuple<string, string>, Game> games = new Dictionary<Tuple<string, string>, Game>();

        public void clientConnected(string userName, string hashedPasswd)
        {
            if (connectedClient.ContainsKey(userName))
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
                if (!clients.ContainsKey(userName))
                {
                    IFourRowServiceCallback callbackChannel = OperationContext.Current.GetCallbackChannel<IFourRowServiceCallback>();
                    clients.Add(userName, callbackChannel);
                }
                if (!connectedClient.ContainsKey(userName))
                    connectedClient.Add(userName, clients[userName]);
                if (!clientsThatNotPlay.Contains(userName))
                    clientsThatNotPlay.Add(userName);
                ++connectedClientsCnt;
            }
        }

        public void clientRegisterd(string userName, string hashedPasswd)
        {
            if (clients.ContainsKey(userName))
                throw new FaultException<UserExistsFault>(new UserExistsFault()
                {
                    Details = "User name " + userName + " already exists. Try something else"
                });
            using (var ctx = new FourinrowDBContext())
            {
                var user = (from u in ctx.Users
                            where u.UserName == userName
                            select u).FirstOrDefault();
                if (user  == null)
                {
                    User newUser = new User()
                    {
                        UserName = userName,
                        HashedPassword = hashedPasswd
                    };
                    ctx.Users.Add(user);
                    ctx.SaveChanges();
                }
                ++numClients;
                IFourRowServiceCallback callbackChannel = OperationContext.Current.GetCallbackChannel<IFourRowServiceCallback>();
                clients.Add(userName, callbackChannel);
            }
        }

        public void clientDisconnected(string userName)
        {
            try
            {
                if (connectedClient.ContainsKey(userName))
                    connectedClient.Remove(userName);
                if (clientsThatNotPlay.Contains(userName))
                    clientsThatNotPlay.Remove(userName);
                --connectedClientsCnt;
            }
            catch (Exception ex)
            {
                throw new FaultException<Exception>(ex);
            }
        }

        public List<string> getClientsThatNotPlayNow()
        {
            return new List<string>(clientsThatNotPlay);
        }

        public bool clearUsers()
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
                        res += item.UserName.ToString() + " ";
                        cnt++;
                        if (x == null)
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
                        clients.Clear();
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

        #region DB functions

        public List<string> GetAllUsersInDB()
        {
            try
            {
                List<string> res = new List<string>();
                using (var ctx = new FourinrowDBContext())
                {
                    List<string> users = (from u in ctx.Users
                                          select u.UserName).ToList();
                    if (users == null)
                    {
                        res.Add("DB is empty");
                        return res;
                    }
                    res.Add($"There is {users.Count} users in db");
                    foreach (var str in users)
                        res.Add(str.ToString());
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

        public List<string> getAllUsersGamesHistory()
        {
            List<string> allUsersGamesHistory = new List<string>();

            try
            {
                List<UserHistory> usersHistory = allUsersGamesHistoryPrivately();
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

        public List<string> getAllUsersGamesHistoryOrderedByName()
        {
            List<string> allUsersGamesHistoryOrderedByName = new List<string>();

            try
            {
                List<UserHistory> usersHistory = allUsersGamesHistoryPrivately();

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

        public List<string> getAllUsersGamesHistoryOrderedByGames()
        {
            List<string> allUsersGamesHistoryOrderedByGames = new List<string>();

            try
            {
                List<UserHistory> usersHistory = allUsersGamesHistoryPrivately();

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

        public List<string> getAllUsersGamesHistoryOrderedByWins()
        {
            List<string> allUsersGamesHistoryOrderedByWins = new List<string>();

            try
            {
                List<UserHistory> usersHistory = allUsersGamesHistoryPrivately();

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

        public List<string> getAllUsersGamesHistoryOrderedByLoses()
        {
            List<string> allUsersGamesHistoryOrderedByLoses = new List<string>();

            try
            {
                List<UserHistory> usersHistory = allUsersGamesHistoryPrivately();

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

        public List<string> getAllUsersGamesHistoryOrderedByPoints()
        {
            List<string> allUsersGamesHistoryOrderedByPoints = new List<string>();

            try
            {
                List<UserHistory> usersHistory = allUsersGamesHistoryPrivately();

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

        public List<string> allTheGamesThatPlayesSoFar()
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

        public List<string> allTheGamesThatPlayesNow()
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

        public List<string> allTheGamesBetweenTwoClients(string userName1, string userName2)
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

        public List<string> allTheGamesOfSomeClient(string userName)
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
        public List<string> getAllUsersEver()
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
        public List<UserHistory> allUsersGamesHistoryPrivately()
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
        #endregion

        public bool ping() => true;

        public void wantToPlayWithClient(string currentPlayer, string opponent)
        {
            try
            {
                if (clientsThatNotPlay.Contains(currentPlayer))
                    clientsThatNotPlay.Remove(currentPlayer);

                if (clientsThatNotPlay.Contains(opponent))
                    clientsThatNotPlay.Remove(opponent);

                sideModeClients.Add(Tuple.Create(currentPlayer, opponent));

                if (!connectedClient.ContainsKey(opponent))
                    return;

                Thread notifyOpponentChallengeThread = new Thread(() => clients[opponent].NotifyOpponentChallenge(currentPlayer))
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

        public void opponentAcceptToPlay(string currentPlayer, string opponent)
        {
            try
            {
                if (!(clients.ContainsKey(currentPlayer) && clients.ContainsKey(opponent)))
                    return;

                if (clientsThatNotPlay.Contains(currentPlayer))
                    clientsThatNotPlay.Remove(currentPlayer);

                if (clientsThatNotPlay.Contains(opponent))
                    clientsThatNotPlay.Remove(opponent);

                Tuple<string, string> thisGamePlayers = Tuple.Create(currentPlayer, opponent);

                if (sideModeClients.Contains(thisGamePlayers))
                    sideModeClients.Remove(thisGamePlayers);

                if (!games.ContainsKey(thisGamePlayers))
                    games.Add(thisGamePlayers, new Game());

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
                if (connectedClient.ContainsKey(currentPlayer))
                {
                    Thread updateOtherPlayerThread = new Thread(() =>
                    {
                        clients[currentPlayer].OpponentAcceptToPlayLetsStart();
                    }
);

                    updateOtherPlayerThread.Start();
                }

                if (connectedClient.ContainsKey(opponent))
                {
                    Thread updateOtherPlayerThread = new Thread(() =>
                    {
                        clients[opponent].LetsStart();
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

        public void opponentDeclineToPlay(string currentPlayer, string opponent)
        {
            try
            {
                if (clients.ContainsKey(currentPlayer))
                {
                    Thread notifyChallengeAcceptedThread = new Thread(() =>
                    {
                        connectedClient[currentPlayer].HeyOpponentDeclineToPlay();
                    });
                    notifyChallengeAcceptedThread.Start();
                }

                Tuple<string, string> thisGamePlayers = Tuple.Create(currentPlayer, opponent);

                if (sideModeClients.Contains(thisGamePlayers))
                    sideModeClients.Remove(thisGamePlayers);

                if (!clientsThatNotPlay.Contains(currentPlayer) && connectedClient.ContainsKey(currentPlayer))
                    clientsThatNotPlay.Add(currentPlayer);

                if (!clientsThatNotPlay.Contains(opponent) && connectedClient.ContainsKey(opponent))
                    clientsThatNotPlay.Add(opponent);
            }
            catch (Exception ex)
            {
                throw new FaultException<Exception>(ex);
            }
        }

        public void clientDisconnectedBeforeGame(string userName, string opponent, string whosOut)
        {
            try
            {
                clientDisconnected(whosOut);
                Tuple<string, string> option1Key = Tuple.Create(userName, opponent);
                Tuple<string, string> option2Key = Tuple.Create(opponent, userName);

                if (games.ContainsKey(option1Key))
                    games.Remove(option2Key);

                if (games.ContainsKey(option2Key))
                    games.Remove(option1Key);

                if (sideModeClients.Contains(option1Key))
                    sideModeClients.Remove(option2Key);

                if (sideModeClients.Contains(option2Key))
                    sideModeClients.Remove(option1Key);

                string player2Notify = whosOut == userName ? opponent : userName;
                if (connectedClient.ContainsKey(player2Notify))
                {
                    Thread notifyOpponentThread = new Thread(() => clients[player2Notify].OpponentDisconnectedBeforeTheGame());
                    notifyOpponentThread.Start();
                }

                if (clientsThatNotPlay.Contains(player2Notify))
                    return;

                clientsThatNotPlay.Add(player2Notify);
            }
            catch (Exception ex)
            {
                throw new FaultException<Exception>(ex);
            }
        }

        public void clientDisconnectedThrowGame(string currentPlayer, string opponent, string whosOut)
        {
            try
            {
                Tuple<string, string> thisGamePlayers = Tuple.Create(currentPlayer, opponent);
                Game theGame = null;
                if (games.ContainsKey(thisGamePlayers))
                {
                    theGame = games[thisGamePlayers];
                    games.Remove(thisGamePlayers);
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

                    updatePointAndHistory(currentPlayer, opponent, winnerName, theGame, ctx);
                    ctx.SaveChanges();

                    string player2Notify = whosOut == currentPlayer ? opponent : currentPlayer;

                    if (connectedClient.ContainsKey(player2Notify))
                        new Thread(() =>
                        {
                            clients[player2Notify].OpponentDisconnectedThrowGameYouWon();
                        }).Start();

                    if (clientsThatNotPlay.Contains(player2Notify))
                        return;

                    clientsThatNotPlay.Add(player2Notify);
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

        public Tuple<MoveResult, int> reportMove(
          string currentPlayer,
          string opponent,
          string whoMoved,
          int location,
          double pointX,
          double pointY)
        {
            try
            {
                string _whoMoved = whoMoved == currentPlayer ? currentPlayer : opponent;
                string player2Notify = whoMoved == currentPlayer ? opponent : currentPlayer;

                Game theGame = games[Tuple.Create(currentPlayer, opponent)];
                IFourRowServiceCallback callback = connectedClient.ContainsKey(player2Notify) ? connectedClient[player2Notify] : (IFourRowServiceCallback)null;

                Tuple<MoveResult, int> result = theGame.verifyMove(currentPlayer, opponent, whoMoved, location);

                if (result.Item1 == MoveResult.NotYourTurn || result.Item1 == MoveResult.IllegalMove)
                    return result;

                if (result.Item1 == MoveResult.YouWon || result.Item1 == MoveResult.Draw)
                {
                    Thread notifyOppnentWinOrTieThread = new Thread(() => WinOrDraw(currentPlayer, opponent, theGame, result.Item1, _whoMoved));
                    notifyOppnentWinOrTieThread.Start();
                }

                Thread notifyOppnentMoveThread = new Thread(() => callback.OtherPlayerMoved(result, pointX, pointY));
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
                updatePointAndHistory(currentPlayer, opponent, winnerName, theGame, ctx);
                ctx.SaveChanges();
            }
        }

        private void updatePointAndHistory(
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

        public void getMeBackToWaitingList(string userName)
        {
            if (!clientsThatNotPlay.Contains(userName))
                clientsThatNotPlay.Add(userName);
        }

        public int getMyPos(string currentPlayer, string opponent, string whoWantIt)
        {
            try
            {
                Tuple<string, string> thisGamePlayers = Tuple.Create(currentPlayer, opponent);
                Game game = games.ContainsKey(thisGamePlayers) ? games[thisGamePlayers] : null;
                int pos = -1;
                if (game != null)
                    pos = game.getPos(currentPlayer, whoWantIt);
                return pos;
            }
            catch (Exception ex)
            {
                throw new FaultException<Exception>(ex);
            }
        }

        public void setMyPos(string currentPlayer, string opponent, string whoWantIt, int pos)
        {
            try
            {
                Tuple<string, string> thisGamePlayers = Tuple.Create(currentPlayer, opponent);
                if (games.ContainsKey(thisGamePlayers))
                    games[thisGamePlayers].setPos(currentPlayer, whoWantIt, pos);
            }
            catch (Exception ex)
            {
                throw new FaultException<Exception>(ex);
            }
        }

        public void killTheGame(string currentPlayer, string opponent)
        {
            Tuple<string, string> thisGamePlayers = Tuple.Create(currentPlayer, opponent);
            if (games.ContainsKey(thisGamePlayers))
                games.Remove(thisGamePlayers);
        }


    }/*end of -FourRowService- class*/

}/*end of -WcfFourRowService- namespace*/
