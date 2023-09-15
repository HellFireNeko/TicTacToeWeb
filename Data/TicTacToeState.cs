using System.Collections.Concurrent;

namespace TicTacToeWeb;

public class TicTacToeState
{
    public event Action<Guid>? OnStateChanged;
    private ConcurrentDictionary<Guid, TicTacToeGame> games = new();

    public Guid CreateGame()
    {
        Guid guid = Guid.NewGuid();
        games.TryAdd(guid, new TicTacToeGame());
        return guid; 
    }

    public TicTacToeGame? GetGame(Guid guid) => games.TryGetValue(guid, out TicTacToeGame? game) ? game : null;

    public TicTacToeUpdateResponse UpdateGame(Guid guid, TicTacToeSquare currentPlayer, TicTacToeGame game)
    {
        if (!games.TryGetValue(guid, out TicTacToeGame? actualGame))
        {
            return TicTacToeUpdateResponse.DOES_NOT_EXIST;
        }
        if (actualGame.CurrentTurn != currentPlayer)
        {
            return TicTacToeUpdateResponse.NOT_YOUR_TURN;
        }
        games[guid] = game;
        //OnStateChanged?.Invoke(guid);
        return TicTacToeUpdateResponse.UPDATED;
    }

    public void RaiseEvent(Guid guid)
    {
        OnStateChanged?.Invoke(guid);
    }
}

public class TicTacToeGame
{
    public TicTacToeSquare[] GameGrid = new TicTacToeSquare[9];
    public TicTacToeSquare CurrentTurn = TicTacToeSquare.Cross;
    public bool Joinable = true;

    public bool CheckForWin(TicTacToeSquare player)
    {
        // Check rows
        for (int row = 0; row < 3; row++)
        {
            if (GameGrid[row * 3] == player && GameGrid[row * 3 + 1] == player && GameGrid[row * 3 + 2] == player)
            {
                return true; // Player has won in this row
            }
        }

        // Check columns
        for (int col = 0; col < 3; col++)
        {
            if (GameGrid[col] == player && GameGrid[col + 3] == player && GameGrid[col + 6] == player)
            {
                return true; // Player has won in this column
            }
        }

        // Check diagonals
        if (GameGrid[0] == player && GameGrid[4] == player && GameGrid[8] == player)
        {
            return true; // Player has won in the main diagonal
        }

        if (GameGrid[2] == player && GameGrid[4] == player && GameGrid[6] == player)
        {
            return true; // Player has won in the other diagonal
        }

        return false; // No win detected
    }

}

public enum TicTacToeUpdateResponse
{
    UPDATED,
    DOES_NOT_EXIST,
    NOT_YOUR_TURN,
}

public enum TicTacToeSquare
{
    None,
    Cross,
    Circle,
}