using FourRowClient.FourRowServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FourRowClient 
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ClientCallback : IFourRowServiceCallback
    {
        internal Action<string> answer2Challenge;
        internal Action opponentDecline;
        internal Action opponentQuited;
        internal Action<int, double, double> updateGame;
        internal Action<string> endGame;
        internal Action startGameChoosedGuy;
        internal Action startGameOpponent;
        internal Action okGoBackToLife;

        public void HeyOpponentDeclineToPlay() => opponentDecline();

        public void LetsStart() => startGameOpponent();

        public void NotifyOpponentChallenge(string currentPlayer) => answer2Challenge(currentPlayer);

        public void OpponentAcceptToPlayLetsStart() => startGameChoosedGuy();

        public void OpponentDisconnectedBeforeTheGame()
        {
            MessageBox.Show(" You opponent is disconnected\n    please cancel the thing\n please press - yes/no - ");
            okGoBackToLife();
        }

        public void OpponentDisconnectedThrowGameYouWon() => opponentQuited();

        public void OtherPlayerConnected() => throw new NotImplementedException();

        public void OtherPlayerMoved(Tuple<MoveResult, int> moveResult, double pointX, double pointY)
        {
            updateGame(moveResult.Item2, pointX, pointY);
            if (moveResult.Item1 == MoveResult.Draw)
                endGame("draw");
            if (moveResult.Item1 != MoveResult.YouWon)
                return;
            endGame("you lost");
        }

        public void OtherPlayerMoved(MoveResult moveResult, int location)
        {
            throw new NotImplementedException();
        }
    }
}
