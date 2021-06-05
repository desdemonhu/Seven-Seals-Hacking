using UnityEngine;


public class SealCreator : MonoBehaviour {

    public float timeRemaining = 15;
    public bool timerIsRunning = false;
    public SealHackingBoard Board { get; set; }
    public BoardConfigSettings.ConfigTypes ConfigType { get; set; }
    public BoardConfigSettings.ConfigTypes SealPieceConfig { get; set;}
    public GenerateSealRandomizer CurrentPiece { get; set; }
    public bool GameRunning { get; set; } = false;

    void Awake(){

        ConfigType = BoardConfigSettings.ConfigTypes.Board_Default;
        SealPieceConfig = BoardConfigSettings.ConfigTypes.SealPiece_Default;

        Board = SealHackingBoard.CreateSealHackingBoard(ConfigType);

        timerIsRunning = true;
        ///TODO: set GameRunning on a timer for testing
        
        Debug.Log(string.Format("In Awake - Board.GeneratedBoard.Board[0].SealPieces[0].SealPiece.GetNeighbors().Length: {0}", Board.GeneratedBoard.Board[0].SealPieces[0].SealPiece.GetNeighbors().Length.ToString()));
        Debug.Log(string.Format("In Awake - Board.GeneratedBoard.Board[0].SealPieces[0].SealPiece.NeighborValidation(0)[1]: {0}", Board.GeneratedBoard.Board[0].SealPieces[0].SealPiece.NeighborValidation(0)[1].ToString()));
        // Debug.Log(string.Format("In Awake - generatedPlayPiece.ActivePieces[1]: [{0},{1}] ", CurrentPiece.ActivePieces[1][0], CurrentPiece.ActivePieces[1][1]));

    }

    void Update(){

        if (timerIsRunning) {
            TimerGameRunning();
        }
              
    }

    public void TimerGameRunning(){

        GameRunning = true;

        if (timeRemaining > 0)
        {
            InPlay();
            timeRemaining -= Time.deltaTime;
        }
        else
        {
            timeRemaining = 0;
            GameRunning = false;
            
            Debug.Log(string.Format("in TimerGameRunning - Time has run out according to GameRunning: {0}", GameRunning));

            timerIsRunning = false;
        }
    }


    public void InPlay(){

        while (!Board.Full)
        {
            CurrentPiece = CreateSealPiece(SealPieceConfig);
            Board = CurrentPiece.FallingToSettled(CurrentPiece, Board);
        }

        GameRunning = false;
        Debug.Log(string.Format("in InPlay - Board is filled! Time Remaining: {0}", Mathf.FloorToInt(timeRemaining % 60))); 
    }


    public GenerateSealRandomizer CreateSealPiece(BoardConfigSettings.ConfigTypes type){

        return new GenerateSealRandomizer(type);
    }

}

public class SealHackingBoard
{
    public SealHackingBoard GeneratedSealHackingBoard { get; set; }
    public BoardConfigSettings BoardSettings { get; set;}
    public CurrentSealBoard GeneratedBoard { get; set; }
    public GenerateSealPiece CurrentPlayPiece { get; set; }
    public bool Full { get; set; }

    public SealHackingBoard(BoardConfigSettings.ConfigTypes type){
        
        BoardSettings = BoardConfigSettings.GetStandardConfigSettings(type);
        GeneratedBoard = GenerateBoard(this.BoardSettings);
        Full = false;

    } 

    public CurrentSealBoard GenerateBoard(BoardConfigSettings settings) {
                
        CurrentSealBoard newBoard = new CurrentSealBoard(settings);
    
        if (settings.BoardType != BoardConfigSettings.ConfigTypes.SealPiece_Custom || settings.BoardType != BoardConfigSettings.ConfigTypes.SealPiece_Custom) {
            
            newBoard = newBoard.AddNeighborsToBoard(newBoard);
        }

        for (int index = 0; index < newBoard.NumOfRows; index++) {

            CurrentSealRow row = newBoard.Board[index];
        }
        
        return newBoard;
    }

    public static SealHackingBoard CreateSealHackingBoard(BoardConfigSettings.ConfigTypes type){

        SealHackingBoard sealHackingBoard = new SealHackingBoard(type);

        return sealHackingBoard;
    } 

}


public class BoardConfigSettings
{
    private static int[] boardDefault = new int[] {25, 12};
    private static int[] boardCustom = new int[] {25, 12};
    private static int[] sealPieceDefault = new int[] {4, 4};
    private static int[] sealPieceCustom = new int[] {4, 4};

    public int BlocksPerRow { get ; set ; }
    public int NumRows { get ; set; }
    public ConfigTypes BoardType { get; set; }

    public BoardConfigSettings(int[] settings, ConfigTypes type)
    {
        NumRows = settings[0];
        BlocksPerRow = settings[1];
        BoardType = type;
    }

    public enum ConfigTypes {
        Board_Default,
        Board_Custom,
        SealPiece_Default,
        SealPiece_Custom,
        None        
    }

    public static BoardConfigSettings GetStandardConfigSettings(ConfigTypes type){

        return type switch {

            ConfigTypes.Board_Default => RetrieveConfigSettings(boardDefault, type),
            ConfigTypes.Board_Custom => RetrieveConfigSettings(boardCustom, type),
            ConfigTypes.SealPiece_Default => RetrieveConfigSettings(sealPieceDefault, type),
            ConfigTypes.SealPiece_Custom => RetrieveConfigSettings(sealPieceCustom, type),
            ConfigTypes.None => RetrieveConfigSettings(boardDefault, type),
            _ => RetrieveConfigSettings(boardDefault, type)

        };
    }


    private static BoardConfigSettings RetrieveConfigSettings(int[] settings, ConfigTypes type) {

        return new BoardConfigSettings(settings, type);
    }
}