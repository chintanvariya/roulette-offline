using DG.Tweening;
using UnityEngine;
namespace RouletteByFinix
{
    public class GameController : MonoBehaviour
    {
        

        public static GameController instance;
        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(instance);
        }

       
        private void Start()
        {
            SplashCanvasMakeOff();
        }

        public GameState gameState;
        [Space(15)]
        public GameMode gameMode;

        public bool IsFTUEgameStateOn()
        {
            return gameState == GameState.FTUE;
        }

        public void GameStateSet(GameState setGameState)
        {
            gameState = setGameState;
            // UiManager.Instance.SettingScreenSet(gameState);
        }

        public void GameModeSet(GameMode currentGameMode)
        {
            gameMode = currentGameMode;
        }
        private void SplashCanvasMakeOff()
        {
            AudioManager.instance.PlayBackGround();

            // DashBoard Panel Make On
            UiManager.Instance.DashBoardPanleOpen();

            // FTUEVerify();

        }
    }
    public enum GameState
    {
        None,
        Splash,
        FTUE,
        DashBoard,
        Playing
    }

    public enum GameMode
    {
        American,
        Europe
    }
}
