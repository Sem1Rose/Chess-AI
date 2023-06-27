namespace Chess
{
    using UnityEngine;
    using TMPro;
    using UnityEngine.SceneManagement;

    public class GameSetup : MonoBehaviour
    {
        public TMP_InputField fenInput;

        const string initialFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        string FEN;
        int gameMode = 0;

        public void ResetFENText() => FEN = fenInput.text = initialFEN;
        public void GetFENInput(string txt) => FEN = txt;
        public void GetGameMode(int index) => this.gameMode = index;

        public void Play()
        {
            if (FEN == null || FEN.Split(' ').Length != 6)
                return;

            Board.gameFEN = FEN;
            Board.gameMode = (GameModes)gameMode;

            SceneManager.LoadScene(1);
        }
    }
}