using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using AvaloniaTetris.Models;
using AvaloniaTetris.Models.Blocks;
using AvaloniaTetris.ViewModels;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AvaloniaTetris.Views;

public partial class MainWindow : Window
{
    private readonly Bitmap[] tileImages = new Bitmap[]
    {
        new Bitmap(AssetLoader.Open(new Uri("avares://AvaloniaTetris/Assets/TileEmpty.png"))),
        new Bitmap(AssetLoader.Open(new Uri("avares://AvaloniaTetris/Assets/TileCyan.png"))),
        new Bitmap(AssetLoader.Open(new Uri("avares://AvaloniaTetris/Assets/TileBlue.png"))),
        new Bitmap(AssetLoader.Open(new Uri("avares://AvaloniaTetris/Assets/TileOrange.png"))),
        new Bitmap(AssetLoader.Open(new Uri("avares://AvaloniaTetris/Assets/TileYellow.png"))),
        new Bitmap(AssetLoader.Open(new Uri("avares://AvaloniaTetris/Assets/TileGreen.png"))),
        new Bitmap(AssetLoader.Open(new Uri("avares://AvaloniaTetris/Assets/TilePurple.png"))),
        new Bitmap(AssetLoader.Open(new Uri("avares://AvaloniaTetris/Assets/TileRed.png"))),
    };

    private readonly Bitmap[] blockImages = new Bitmap[]
    {
        new Bitmap(AssetLoader.Open(new Uri("avares://AvaloniaTetris/Assets/Block-Empty.png"))),
        new Bitmap(AssetLoader.Open(new Uri("avares://AvaloniaTetris/Assets/Block-I.png"))),
        new Bitmap(AssetLoader.Open(new Uri("avares://AvaloniaTetris/Assets/Block-J.png"))),
        new Bitmap(AssetLoader.Open(new Uri("avares://AvaloniaTetris/Assets/Block-L.png"))),
        new Bitmap(AssetLoader.Open(new Uri("avares://AvaloniaTetris/Assets/Block-O.png"))),
        new Bitmap(AssetLoader.Open(new Uri("avares://AvaloniaTetris/Assets/Block-S.png"))),
        new Bitmap(AssetLoader.Open(new Uri("avares://AvaloniaTetris/Assets/Block-T.png"))),
        new Bitmap(AssetLoader.Open(new Uri("avares://AvaloniaTetris/Assets/Block-Z.png"))),
    };

    private readonly Image[,] imageControls;
    private GameState gameState = new GameState();

    public MainWindow()
    {
        InitializeComponent();
        imageControls = SetupGameCanvas(gameState.GameGrid);

        mainGameWindow.AddHandler(Window.KeyDownEvent, Window_KeyDown, RoutingStrategies.Tunnel);
        playAgainButton.AddHandler(Button.ClickEvent, PlayAgain_Click, RoutingStrategies.Bubble);
    }

    private Image[,] SetupGameCanvas(GameGrid grid)
    {
        Image[,] imageControls = new Image[grid.Rows, grid.Columns];
        int cellSize = 25;
        for (int r = 0; r < grid.Rows; r++)
        {
            for (int c = 0; c < grid.Columns; c++)
            {
                Image imageControl = new Image() 
                { 
                    Width = cellSize, Height = cellSize 
                };
                Canvas.SetTop(imageControl, (r - 2) * cellSize + 10);
                Canvas.SetLeft(imageControl, c * cellSize);
                GameCanvas.Children.Add(imageControl);
                imageControls[r, c] = imageControl;
            }
        }
        return imageControls;
    }
    private void DrawGrid(GameGrid grid)
    {
        for (int r = 0; r < grid.Rows; r++)
        {
            for (int c = 0; c < grid.Columns; c++)
            {
                int id = grid[r, c];
                imageControls[r, c].Source = tileImages[id];
            }
        }
    }

    private void DrawBlock(Block block)
    {
        foreach (Position p in block.TilePositions())
        {
            imageControls[p.Row, p.Column].Source = tileImages[block.Id];
        }
    }

    private void Draw(GameState gameState)
    {
        DrawGrid(gameState.GameGrid);
        DrawBlock(gameState.CurrentBlock);
    }

    private async Task GameLoop()
    {
        Draw(gameState);
        while(!gameState.GameOver)
        {
            await Task.Delay(500);
            gameState.MoveBlockDown();
            Draw(gameState);
        }
    }

    private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
    {
        await GameLoop();
    }

    public void Window_KeyDown(object? sender, KeyEventArgs e)
    {
        if (gameState.GameOver)
        {
            GameOverMenu.IsVisible = true;
            return;
        }
        
        switch (e.Key) 
        {
            case Key.Left:
                {
                    gameState.MoveBlockLeft();
                    break;
                }
            case Key.Right:
                {
                    gameState.MoveBlockRight();
                    break;
                }
            case Key.Down:
                {
                    gameState.MoveBlockDown();
                    break;
                }
            case Key.Up:
                {
                    gameState.RotateBlockCW();
                    break;
                }
            case Key.Z:
                {
                    gameState.RotateBlockCCW();
                    break;
                }
            default:
                {
                    return;
                }
        }

        Draw(gameState);
    }

    public async void PlayAgain_Click(object? sender, RoutedEventArgs e)
    {
        gameState = new GameState();
        GameOverMenu.IsVisible = false;
        await GameLoop();
    }
}
