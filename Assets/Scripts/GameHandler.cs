namespace Chess
{
    using UnityEngine;
    using TMPro;

    public class GameHandler : MonoBehaviour
    {
        public TextMeshProUGUI turnToMoveText;
        public TextMeshProUGUI winText;
        public TextMeshProUGUI lastMoveText;
        public TextMeshProUGUI mateText;

        #region  Singleton
        public static GameHandler handler = null;

        private void Awake()
        {
            if (handler != null)
                Debug.LogError("More than one Graphic handler in the scene!", gameObject);
            handler = this;
        }
        #endregion

        void Start()
        {
            GraphicsHandler.handler.StartGame();
        }

        public void InitUI()
        {
            turnToMoveText.text = Essentials.GetTurnToMove();
            winText.text = "";
            lastMoveText.text = "";
            mateText.text = "";
        }

        public void UpdateUI()
        {
            turnToMoveText.text = Essentials.GetTurnToMove();
            if (Essentials.CheckForMate())
            {
                Board.gameEnded = true;
                if (Essentials.InCheck())
                {
                    Board.gameEndReason = Board.GameEndReason.CheckMate;
                    mateText.text = "CheckMate";
                }
                else
                {
                    Board.gameEndReason = Board.GameEndReason.StaleMate;
                    mateText.text = "StaleMate";
                }
                winText.text = (Board.turnToMove == ChessPieceTypes.White ? "Black " : "White ") + "Wins";
                turnToMoveText.text = "";
            }
            else if (Board.pieces.Count == 2)
            {
                Board.gameEnded = true;
                Board.gameEndReason = Board.GameEndReason.Draw;
                mateText.text = "Draw";
            }
            lastMoveText.text = Essentials.GenerateLastMoveLAN();
        }
    }
}