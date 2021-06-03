using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISealPiece
{
    bool Active { get; set; }
    int X { get; set; }
    int Y { get; set; }

    SealPiece MarkBoardPiece(SealPiece sealPiece);
}

public class SealPiece : ISealPiece
{
    private const int __numOfNeighbors = 8;
    private const bool __flagged = false;

    public bool Active { get; set; }
    public bool Flagged { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    private int[][] neighbors = new int[__numOfNeighbors][];

    public int[] NeighborValidation(int index){
        
       int[][] retrievedNeighbors = this.GetNeighbors();

        return retrievedNeighbors[index] != null ? retrievedNeighbors[index] : new int[] {-1, -1};
    }
    
    public int[][] GetNeighbors()
    {
        return neighbors;
    }

    public void SetNeighbors(int[][] value)
    {
        neighbors = value;
    }

    public SealPiece(int x = 0, int y = 0, bool active = false)
    {
        X = x;
        Y = y;
        Active = active;
        Flagged = __flagged; 
        SetNeighbors(GetNeighbors());
        
    }

    

    private enum Direction {
        north,
        northeast,
        east,
        southeast,
        south,
        southwest,
        west,
        northwest
    }

    private static int[] GetDirectionCoords(Direction direction){

        int[] north = new int[]{-1, 0};
        int[] northeast = new int[]{-1, 1};
        int[] east = new int[]{0, +1};
        int[] southeast = new int[]{+1, +1};
        int[] south = new int[]{1, 0};
        int[] southwest = new int[]{1, -1};
        int[] west = new int[]{0, -1};
        int[] northwest = new int[]{+1, -1};

        int[] coords = direction switch 
        {
            Direction.north => north,
            Direction.northeast => northeast,
            Direction.east => east,
            Direction.southeast => southeast,
            Direction.south => south,
            Direction.southwest => southwest,
            Direction.west => west,
            Direction.northwest => northwest,
            _ => new int[] {5,5}
        };

        return coords;
    }

    private static bool CheckCoords(int[] coords) => coords[0] != 5;

    public static SealPiece IsCoordsInBounds(int[] coords, CurrentSealBoard board){
        
        return board.Board[coords[0]].SealPieces[coords[1]].SealPiece;
    }

    public static int[][] PossibleOptions(CurrentSealBoard board, int[] startCoords){

        int[][] possibleOptions = new int[__numOfNeighbors][];

        for(int i = 0; i < System.Enum.GetNames(typeof(Direction)).Length ; i++)
        {
            int[] coords = GetCoords(i); //Coords for a direction

            if(CheckCoords(coords))
            {
                int[] nextCoords = new int[] { startCoords[0] + coords[0], startCoords[1] + coords[1] };
                
                if(CheckValidCoords(nextCoords, board)){

                    SealPiece nextPiece = RetriveBoardPiece(nextCoords, board);
                    bool notStartCoords = nextCoords[0] != startCoords[0] && nextCoords[1] != startCoords[1];

                    if(notStartCoords) {
                       
                        possibleOptions[i] = nextCoords;

                    }

                }

             }
        }

        return possibleOptions;
    }


    private static bool CheckValidCoords(int[] coords, CurrentSealBoard settings)
    {
        return (coords[0] < settings.NumOfRows && coords[0] > -1) && (coords[1] < settings.BlocksPerRow && coords[1] > -1);
    }

    private static int[] GetCoords(int i)
        {
            return i switch
            {
                0 => GetDirectionCoords(Direction.north),
                1 => GetDirectionCoords(Direction.northeast),
                2 => GetDirectionCoords(Direction.east),
                3 => GetDirectionCoords(Direction.southeast),
                4 => GetDirectionCoords(Direction.south),
                5 => GetDirectionCoords(Direction.southwest),
                6 => GetDirectionCoords(Direction.west),
                7 => GetDirectionCoords(Direction.northwest),
                _ => new int[] {5,5}
            };
        }


    public static SealPiece RetriveBoardPiece(int[] coords, CurrentSealBoard board){

        SealPiece retrievedPiece = IsCoordsInBounds(coords, board);
        
        return retrievedPiece;
    }


    private static bool LegitPieceCheck(int[] coords){

        bool result = coords[0] > -1 && coords[1] > -1;
        return result;
    }

    public SealPiece MarkBoardPiece(SealPiece sealPiece){
        sealPiece.Active = !(sealPiece.Active);
        return sealPiece;
    }

}


public class Coordinates {

    public int X { get; set; }
    public int Y { get; set; }
    public Coordinates(int x = -1, int y = -1){

        X = x;
        Y = y;
    }
}



public class GenerateSealPiece
{
    public CurrentSealBoard SealParts { get; set; }

    public GenerateSealPiece(){

        //SealParts = GenerateSealRandomizer.GetGeneratePlayPiece();

        //SealParts = GenerateSealRandomizer.GeneratePlayPiece();
        ///Turn Acive and retrive neighbors
        ///Choose next piece from neighbors
        ///While (there aren't 4 pieces selected)


    }

    ///Use SealPiece - What default should be? O,O would put them in conflict with the board, but would need to change as the thing falls (-1, -1)?
    ///Create Basic Block template to choose the pieces from - pick square at random, then look at it's neighbors, pick one at random, until 4 are chosen
    ///Send SealPiece to the Staging Stack
}

public class GenerateSealRandomizer : GenerateSealPiece
{
    public static BoardConfigSettings Settings { get; set;} 
    public static int NumOfBlocksToGenerate { get; set;}

    public SealHackingBoard GeneratedPiece { get; set;}
    public int[][] ActivePieces { get; set; }
    public bool PieceSettled {get; set; }
    public GenerateSealRandomizer(BoardConfigSettings.ConfigTypes config){

        Settings = BoardConfigSettings.GetStandardConfigSettings(config);
        NumOfBlocksToGenerate = GenerateNumOfBlocks();
        GeneratedPiece = GeneratePlayPiece(config);
        ActivePieces = GetActivePieces(this.GeneratedPiece);
        PieceSettled = false;
    }

    public int[][] GetActivePieces(SealHackingBoard generatedPiece)
    {
        CurrentSealRow[] generatedPiecies = generatedPiece.GeneratedBoard.Board;
        List<int[]> activePieces = new List<int[]>();

        foreach(CurrentSealRow row in generatedPiecies){

            for(int block = 0; block < row.SealPieces.Length; block++){

                SealPiece sealPiece = row.SealPieces[block].SealPiece;

                if(sealPiece.Active == true){

                    activePieces.Add(new int[]{sealPiece.X , sealPiece.Y});
                }
            }

        } 
        return activePieces.ToArray();
    }

    public GenerateSealRandomizer SettlePiece(GenerateSealRandomizer currentPiece){

        currentPiece.PieceSettled = !(currentPiece.PieceSettled);
        return currentPiece;
    }

    private static int GenerateNumOfBlocks(){

        return Mathf.RoundToInt(Random.Range(1, (int)GenerateSealRandomizer.Settings.BlocksPerRow * GenerateSealRandomizer.Settings.NumRows));
    }

    private static Coordinates RandomizeCoords(){

        return new Coordinates(Mathf.RoundToInt(Random.Range(0, GenerateSealRandomizer.Settings.BlocksPerRow)), Mathf.RoundToInt(Random.Range(0, GenerateSealRandomizer.Settings.NumRows)));

    }

    ///Check board row if there are any Active Pieces in that row
    ///Slot the piece in based on empty spaces in row (once you hit an active row, adjust relevant rows with the active parts of the falling piece)
    ///Update all Active Pieces in board
    ///When done, update InPlay
    public SealHackingBoard FallingToSettled(GenerateSealRandomizer currentPiece, SealHackingBoard board){

        CurrentSealRow[] rows = board.GeneratedBoard.Board;

        ///TODO: Take into account piece repositioning during fall
        ///Change coords of active pieces to new depth
        for(int i = rows.Length -1 ; i > -1 ; i--)
        {
            CurrentSealRow currentRow = rows[i];
            Debug.Log(string.Format("In FallingToSettled - currentRow{0}.ActivePieces.Length: {1}", currentRow.CurrentRow, currentRow.ActivePieces.Length)); ///TODO: NullReferenceException: Object reference not set to an instance of an object - 2590 times

            if(currentRow.ActivePieces.Length > 0){

                int[][] currentRowActivePieces = currentRow.ActivePieces;
                int[][] fallingActivePieces = currentPiece.ActivePieces;

               for (int index = 0; index < fallingActivePieces.Length; index++){

                   int[] currentPieceCoords = fallingActivePieces[index];
                   Debug.Log(string.Format("In FallingToSettled - currentPieceCoords: [{0},{1}]", currentPieceCoords[0], currentPieceCoords[1]));

                   ///Look in Row active pieces to see if there are any collisions with the currentPieceCoords
                   if (ActivePiecesContains(currentRowActivePieces, currentPieceCoords)){

                       ///reset coords of piece then set piece to Settled
                       int currentPieceRow = currentPieceCoords[0] + 1;
                       currentPieceCoords[0] = currentPieceRow < board.BoardSettings.NumRows ? currentPieceRow : -1;

                       if (currentPieceCoords[0] > -1 ){

                           rows[currentPieceCoords[0]].ActivePieces[index] = currentPieceCoords;

                           currentPiece.PieceSettled = true;
                           board.GeneratedBoard.Board = rows;
                        }
                        else {

                            board.Full = true;

                        }
                   }

               }

            }
        }

        return board;
    }

    private bool ActivePiecesContains(int[][] activePieces, int[] pieceCoords){

        foreach (int[] piece in activePieces){

            Debug.Log(string.Format("In ActivePiecesContains - piece: [{0}]", piece.ToString())); //This is never reached

            if (piece == pieceCoords){

                return true;
            }
        }

        return false;
    }

    public SealHackingBoard GeneratePlayPiece(BoardConfigSettings.ConfigTypes config)
    {

        SealHackingBoard board = SealHackingBoard.CreateSealHackingBoard(config);
        
        for (int i = 0; i < NumOfBlocksToGenerate; i++)
        {

            Coordinates coords = RandomizeCoords();
            SealPiece checkPiece = board.GeneratedBoard.Board[coords.X].SealPieces[coords.Y].SealPiece;

            Debug.Log(string.Format("In GeneratePlayPiece - checkPiece.Active: {0}", checkPiece.Active));

            while(checkPiece.Active || checkPiece.Flagged){

                coords = RandomizeCoords();
                checkPiece = board.GeneratedBoard.Board[coords.X].SealPieces[coords.Y].SealPiece;

                Debug.Log(string.Format("In GeneratePlayPiece - checkPiece.Active - in while loop:{0}", checkPiece.Active));
            }

            checkPiece.MarkBoardPiece(checkPiece);
            board.GeneratedBoard.Board[coords.X].SealPieces[coords.Y].SealPiece = checkPiece;
        }

        return board;
    }
}