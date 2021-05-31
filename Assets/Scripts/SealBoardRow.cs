using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SealBoardRow
{


}

public class CurrentSeal
{
    public static int NumOfNeighbors { get; set; }
    public int CurrentRow { get; set; }
    public SealPiece SealPiece { get; set; }
    public BoardConfigSettings BoardSettings { get; set; }

    public CurrentSeal(int row, int index, int numOfNeighbrs = 8)
    {
        CurrentRow = row;
        SealPiece = CreateSealPiece(row, index);
        NumOfNeighbors = numOfNeighbrs;
    }


    public SealPiece CreateSealPiece(int row, int index, bool active = false){
        int x = row;
        int y = index;
        return new SealPiece(x,y,active);
    }

}


public class CurrentSealBoard {

    public CurrentSealRow[] Board { get; set; }
    public  int NumOfRows { get; set; }
    public  int BlocksPerRow { get; set; }

    public CurrentSealBoard(BoardConfigSettings settings){

        NumOfRows = settings.NumRows;
        BlocksPerRow = settings.BlocksPerRow;
        Board = CreateBoardArray(settings);

    }

    public CurrentSealRow[] CreateBoardArray(BoardConfigSettings settings){

        List<CurrentSealRow> board = new List<CurrentSealRow>();

        for(int i = 0; i <settings.NumRows; i++){

            CurrentSealRow row = new CurrentSealRow(i, settings);
            board.Add(row);
        }

        return board.ToArray();

    }

    public CurrentSealBoard AddNeighborsToBoard(CurrentSealBoard board)
    {     
        for (int row = 0; row < board.NumOfRows; row++){ 
            
            for(int block = 0; block < board.BlocksPerRow; block++){

                int[] startCoords = new int[] {row, block};

                int[][] neighbors = GenerateNeighborsInBoard(board, startCoords);

                board.Board[row].SealPieces[block].SealPiece.SetNeighbors(neighbors);

            }
        }
        return board;
    }

    public  int[][] GenerateNeighborsInBoard(CurrentSealBoard board, int[] startCoords){

        int neighborsLength = CurrentSeal.NumOfNeighbors;

        int[][] neighbors = FindNeighbors(board, startCoords);
        
        return neighbors;
    }

    private int[][] FindNeighbors(CurrentSealBoard board, int[] startCoords){
        if (board is null)
        {
            throw new System.ArgumentNullException(nameof(board));
        }

        if (startCoords is null)
        {
            throw new System.ArgumentNullException(nameof(startCoords));
        }

        int[][] possibleOptions = SealPiece.PossibleOptions(board,startCoords);

        return possibleOptions;
    }
}


public class CurrentSealRow
{
    public int CurrentRow { get; set; }
    public CurrentSeal[] SealPieces { get; set; }
    public int[][] ActivePieces { get; set ;}

    public CurrentSealRow(int row, BoardConfigSettings settings)
    {
        CurrentRow = row;        
        SealPieces = CreateSealRowPieces(row, settings);
        ActivePieces = null;
    }

    public int[][] CheckForActivePiecesInRow(CurrentSeal[] sealPieces){

        List<int[]> activePieces = new List<int[]>();

        foreach (CurrentSeal piece in sealPieces){

            if (piece.SealPiece.Active){
                
                int[] coords = new int[] {piece.SealPiece.X, piece.SealPiece.Y};
                activePieces.Add(coords);
            }

        }

        return activePieces.ToArray();
    }


    public CurrentSeal[] CreateSealRowPieces(int row, BoardConfigSettings settings){

        List<CurrentSeal> sealPieces = new List<CurrentSeal>();

        for(int i = 0; i < settings.BlocksPerRow; i++){

            CurrentSeal seal = new CurrentSeal(row, i);
            sealPieces.Add(seal);
        }

        return sealPieces.ToArray();

    }

}
