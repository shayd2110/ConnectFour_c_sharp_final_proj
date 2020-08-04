using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
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
        IncludeExceptionDetailInFaults = true,
        UseSynchronizationContext = false)]

    public class FourRowService : IFourRowService
    {
        /*data members*/
        int numClients = 0;
        int connectedClientsCnt = 0;
        public Dictionary<string, IFourRowServiceCallback> clients = new Dictionary<string, IFourRowServiceCallback>();
        public Dictionary<string, IFourRowServiceCallback> connectedClient = new Dictionary<string, IFourRowServiceCallback>();
        Dictionary<int, Game> games = new Dictionary<int, Game>();


        public void clientConnected(string userName, string hashedPasswd)
        {

            if (!clients.ContainsKey(userName))
            {
                UserDoesntExistsFault userExists = new UserDoesntExistsFault
                {
                    Details = "User name " + userName + " Doesnt exists. Please register"
                };
                throw new FaultException<UserDoesntExistsFault>(userExists);
            }
            if (connectedClient.ContainsKey(userName))
            {
                UserAlreadyConnectedFault userAlreadyConnected = new UserAlreadyConnectedFault
                {
                    Details = "User name " + userName + " Already Connected"
                };
                throw new FaultException<UserAlreadyConnectedFault>(userAlreadyConnected);
            }


            using (var ctx = new FourinrowDBContext())
            {
                var user = (from u in ctx.Users
                            where u.UserName == userName
                            select u).FirstOrDefault();

                if (user.HashedPassword != hashedPasswd)
                {
                    WrongPasswordFault fault = new WrongPasswordFault
                    { Details = "Worng PassWord!" };
                    throw new FaultException<WrongPasswordFault>(fault);
                }
                else
                {
                    connectedClientsCnt++;
                    if (connectedClient.ContainsKey(userName))
                        return;
                    connectedClient.Add(userName, clients[userName]);
                }

            }


        }

        public void clientRegisterd(string userName, string hashedPasswd)
        {
            if (clients.ContainsKey(userName))
            {
                UserExistsFault userExists = new UserExistsFault
                {
                    Details = "User name " + userName + " already exists. Try something else"
                };
                throw new FaultException<UserExistsFault>(userExists);
            }


            using (var ctx = new FourinrowDBContext())
            {
                var user = (from u in ctx.Users
                            where u.UserName == userName
                            select u).FirstOrDefault();
                if (user == null)
                {
                    User newUser = new User
                    {
                        UserName = userName,
                        HashedPassword = hashedPasswd
                    };
                    ctx.Users.Add(newUser);
                    ctx.SaveChanges();
                }
                //if (user.HashedPassword != hashedPasswd)
                //{
                //    WrongPasswordFault fault = new WrongPasswordFault
                //    { Details = "Worng PassWord!" };
                //    throw new FaultException<WrongPasswordFault>(fault);
                //}
                numClients++;
                IFourRowServiceCallback callback = OperationContext.Current.GetCallbackChannel<IFourRowServiceCallback>(); // object of client
                clients.Add(userName, callback);
            }

        }

        public void clientDisconnected(string userName)
        {
            connectedClient.Remove(userName);
            connectedClientsCnt--;
        }

        public IEnumerable<string> getClientsThatNotPlayNow()
        {
            return connectedClient.Keys;
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

        #region someMethodsThatConnectedToDataBaseQueries
        /*getAllUsersGamesHistory method*/
        public List<string> getAllUsersGamesHistory()
        {
            List<string> allUsersGamesHistory = new List<string>();

            try
            {
                List<UserHistory> usersHistory = allUsersGamesHistoryPrivately();

                foreach (var uh in usersHistory)
                {
                    allUsersGamesHistory.Add(uh.ToString());
                }

                return allUsersGamesHistory;
            }
            catch (DbException dex)
            {
                throw new FaultException<DbException>(dex);
            }
            catch (Exception ex)
            {
                throw new FaultException<Exception>(ex);
            }

        }/*end of -getAllUsersGamesHistory- method*/

        /*getAllUsersGamesHistoryOrderedByName method*/
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

        }/*end of -getAllUsersGamesHistoryOrderedByName- method*/

        /*getAllUsersGamesHistoryOrderedByGames method*/
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

        }/*end of -getAllUsersGamesHistoryOrderedByGames- method*/

        /*getAllUsersGamesHistoryOrderedByWins method*/
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

        }/*end of -getAllUsersGamesHistoryOrderedByWins- method*/

        /*getAllUsersGamesHistoryOrderedByLoses method*/
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

        }/*end of -getAllUsersGamesHistoryOrderedByLoses- method*/

        /*getAllUsersGamesHistoryOrderedByPoints method*/
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

        }/*end of -getAllUsersGamesHistoryOrderedByPoints- method*/

        /*allTheGamesThatPlayesSoFar method*/
        public List<string> allTheGamesThatPlayesSoFar()
        {
            List<string> gamesthatPlayedSofar = new List<string>();

            try
            {
                using (var ctx = new FourinrowDBContext())
                {
                    var gamesSoFar = (from gd in ctx.GameDetails
                                      select new
                                      {
                                          user1 = gd.User1Name,
                                          user2 = gd.User2Name,
                                          win = gd.WinningUserName,
                                          WinningPoints = (gd.WinningUserName == "draw" ? -1 :
                                                             (gd.WinningUserName == gd.User1Name ? gd.PointsUser1 :
                                                                 gd.PointsUser2)),
                                          gameDate = gd.GameDateStart.ToString("d", DateTimeFormatInfo.InvariantInfo)
                                      }).ToList();

                    int i; i = 1;

                    /*arrange the things*/
                    foreach (var gsf in gamesSoFar)
                    {
                        string someString = $"#{i++}: {gsf.user1} against {gsf.user2}, ";

                        if (gsf.win == "draw")
                            someString += "draw, ";
                        else
                            someString += $"winner: {gsf.win}, #winner points: {gsf.WinningPoints}, ";

                        someString += $"game date: {gsf.gameDate}";

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

        }/*end of -allTheGamesThatPlayesSoFar- method*/

        /*allTheGamesThatPlayesNow method*/
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
                        gamesThatPlaysNow.Add($"#{i++}: {gn.User1Name} against " +
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

        }/*end of -allTheGamesThatPlayesNow- method*/

        /*allTheGamesBetweenTwoClients method*/
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
                                      select new
                                      {
                                          winner = gd.WinningUserName,
                                          dateStarted = gd.GameDateStart
                                      }).ToList();

                    if (thoseGames.Count == 0)
                        return gamesBetweenTwoClients;

                    gamesBetweenTwoClients.Add($"        ***the games between {userName1} and {userName2}***\n" +
                                              $"                     ***total games: {thoseGames.Count}***");

                    int i; i = 1;
                    int numOfWinsUser1, numOfWinsUser2;
                    numOfWinsUser1 = numOfWinsUser2 = 0;

                    foreach (var tg in thoseGames)
                    {
                        string someString = $"#{i++}: game date: {tg.dateStarted.ToString("g", DateTimeFormatInfo.InvariantInfo)}, ";

                        if (tg.winner == "draw")
                            someString += "draw, ";
                        else
                        {
                            someString += $"winner: {tg.winner}";

                            if (tg.winner == userName1)
                                numOfWinsUser1++;
                            else
                                numOfWinsUser2++;
                        }

                        gamesBetweenTwoClients.Add(someString);

                    }/*end of loop*/

                    string someOtherString = "\n        ***wins ratio (percentage)**\n";

                    someOtherString += $"{userName1}: {String.Format("{0:0.00}", numOfWinsUser1 / thoseGames.Count * 100)}%      " +
                                       $"{userName2}: {String.Format("{0:0.00}", numOfWinsUser2 / thoseGames.Count * 100)}%";

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

        }/*end of -allTheGamesBetweenTwoClients- method*/

        public List<string> allTheGamesOfSomeClient(string userName)
        {
            throw new NotImplementedException();
        }

        /*allUsersGamesHistoryPrivately method*/
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

        }/*end of -allUsersGamesHistoryPrivately- method*/
        #endregion


        public bool ping()
        {
            return true;
        }

        public MoveResult ReportMove(int RowLocation, int ColLocation, int player)
        {
            throw new NotImplementedException();
        }
    }/*end of -FourRowService- class*/

}/*end of -WcfFourRowService- namespace*/
