using System;
using System.ServiceModel;
using System.Windows;
using FourRowClient.FourRowServiceReference;

namespace FourRowClient
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ClientCallback : IFourRowServiceCallback
    {
        internal Action<string> answer2Challenge;
        internal Action<string> endGame;
        internal Action okGoBackToLife;
        internal Action opponentDecline;
        internal Action opponentQuitted;
        internal Action startGameChoosedGuy;
        internal Action startGameOpponent;
        internal Action<int, double, double> updateGame;

        public void HeyOpponentDeclineToPlay()
        {
            opponentDecline();
        }

        public void LetsStart()
        {
            startGameOpponent();
        }

        public void NotifyOpponentChallenge(string currentPlayer)
        {
            answer2Challenge(currentPlayer);
        }

        public void OpponentAcceptToPlayLetsStart()
        {
            startGameChoosedGuy();
        }

        public void OpponentDisconnectedBeforeTheGame()
        {
            MessageBox.Show(" You opponent is disconnected\n    please cancel the thing\n please press - yes/no - ");
            okGoBackToLife();
        }

        public void OpponentDisconnectedThrowGameYouWon()
        {
            opponentQuitted();
        }

        public void OtherPlayerMoved(Tuple<MoveResult, int> moveResult, double pointX, double pointY)
        {
            updateGame(moveResult.Item2, pointX, pointY);
            if (moveResult.Item1 == MoveResult.Draw)
                endGame("draw");
            if (moveResult.Item1 != MoveResult.YouWon)
                return;
            endGame("you lost");
        }

    }
}