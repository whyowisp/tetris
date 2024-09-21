using System.Data;

namespace TetrisGame;

enum UserAction
{
    MoveLeft,
    MoveRight,
    Rotate,
    Drop,
    Pause,
    Quit,
    None
}

enum GameState
{
    Running,

    Paused,
    GameOver
}

class Gameloop
{
    private static Gameloop? instance;

    private const short targetFrameRate = 3;
    private const short frameInterval = 1000 / targetFrameRate;
    private TimeSpan timeElapsedForRender;

    private short updateInterval = 1000;
    private TimeSpan timeElapsedForDrop;

    private bool isRunning = true;

    private BoardController boardController = new BoardController();


    private Gameloop() { }

    public static Gameloop Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Gameloop();
            }
            return instance;
        }
    }

    public void Run()
    {

        Console.CursorVisible = false;
        Console.Clear();

        DateTime currentTime;
        DateTime previousTime = DateTime.Now;
        TimeSpan elapsedTime;

        while (isRunning)
        {
            currentTime = DateTime.Now;
            elapsedTime = currentTime - previousTime;

            UserAction userAction = UserInput.Listen();
            //Console.WriteLine("UserAction: {0}", userAction)

            Update(userAction, elapsedTime);
            Render(elapsedTime);


            previousTime = currentTime;
        }
    }

    private void Update(UserAction userAction, TimeSpan elapsedTime)
    {
        timeElapsedForDrop += elapsedTime;

        if (boardController.Piece == null)
        {
            int spawnX = boardController.board.GetWidth() / 2 - 1;
            boardController.SpawnPiece(spawnX, 0);
        }

        switch (userAction)
        {
            case UserAction.Quit:
                isRunning = false;
                break;
            case UserAction.Pause:
                Console.WriteLine("Game Paused");
                Console.ReadKey();
                break;
            case UserAction.Rotate:
                boardController.RotatePiece();
                break;
            case UserAction.Drop:
                updateInterval = 200;
                break;
            case UserAction.MoveLeft:
                boardController.MovePiece(-1, 0);
                break;
            case UserAction.MoveRight:
                boardController.MovePiece(1, 0);
                break;
            case UserAction.None:
                updateInterval = 1000;
                break;
            default:
                break;
        }

        if (timeElapsedForDrop.TotalMilliseconds >= updateInterval)
        {
            boardController.MovePiece(0, 1);

            timeElapsedForDrop = TimeSpan.Zero;
        }
    }

    private void Render(TimeSpan elapsedTime)
    {
        timeElapsedForRender += elapsedTime;

        if (timeElapsedForRender.TotalMilliseconds >= frameInterval)
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            boardController.Render();
            timeElapsedForRender = TimeSpan.Zero;
        }
    }
}