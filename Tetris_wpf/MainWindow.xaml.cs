using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tetris_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ImageSource[] tileImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/TileEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileCyan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileBlue.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileOrange.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileYellow.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileGreen.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TilePurple.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileRed.png", UriKind.Relative)),

        };

        private readonly ImageSource[] blockImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/Block-Empty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-I.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-J.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-L.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-O.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-S.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-T.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-Z.png", UriKind.Relative)),
        };

        private readonly Image[,] imageControls;
        private readonly int maxDelay = 1000; //maksymalne opóźnienie między spadaniem klocka - zmiana poziomu trudności
        private readonly int minDelay = 75; //minimalne opóźnienie między spadaniem klocka - zmiana poziomu trudności
        private readonly int delayDecrease = 25; //minimalne opóźnienie między spadaniem klocka - zmiana poziomu trudności

        private GameState gameState = new GameState();
        private HighestScore highestScore = new HighestScore();

        public MainWindow()
        {
            InitializeComponent();
            imageControls = SetupGameCanvas(gameState.GameGrid);
        }

        private Image[,] SetupGameCanvas(GameGrid grid) // ustawia siatkę gry 
        {
            Image[,] imageControls = new Image[grid.Rows, grid.Columns]; // tablica obrazków
            int cellSize = 25; // rozmiar kafelka

            for (int row = 0; row < grid.Rows; row++) 
            {
                for (int column = 0; column < grid.Columns; column++)
                {
                    Image imageControl = new Image 
                    {
                        Width = cellSize,
                        Height = cellSize
                    };
                    Canvas.SetTop(imageControl, (row - 2) * cellSize + 10); // 10 - kawałek ukrytego wiersza aby było lepiej widać
                    Canvas.SetLeft(imageControl, column * cellSize);
                    GameCanvas.Children.Add(imageControl); 
                    imageControls[row, column] = imageControl; 
                }
            }
            return imageControls;
        }

        private void DrawGrid(GameGrid grid) // rysuje siatkę
        {
            for(int row = 2; row < grid.Rows; row++)
            {
                for(int column = 0; column < grid.Columns; column++)
                {
                    int id = grid[row, column]; // pobiera id kafelka
                    imageControls[row, column].Opacity = 1; // 1 - pełna jasność
                    imageControls[row, column].Source = tileImages[id]; //ustawia kolor kafelka
                }
            }
        }

        private void DrawBlock(Block block) // rysuje blok
        {
            foreach(Position p in block.TitlePositions())
            {
                imageControls[p.Row, p.Column].Opacity = 1; // 1 - pełna jasność
                imageControls[p.Row, p.Column].Source = tileImages[block.Id]; //ustawia kolor bloku
            }
        }

        private void DrawNextBlock(BlockQueue blockQueue) // rysuje następny blok
        {
            Block next = blockQueue.NextBlock;
            NextImage.Source = blockImages[next.Id];
        }

        private void DrawHeldBlock(Block heldBlock) // rysuje blok w funkcji hold
        {
            if(heldBlock == null)
            {
                HoldImage.Source = blockImages[0];
            }
            else
            {
                HoldImage.Source = blockImages[heldBlock.Id];
            }
        }

        private void DrawGhostBlock(Block block) // rysuje ghost bloku, który spadnie
        {
            int dropDistance = gameState.BlockDropDistance();

            foreach(Position p in block.TitlePositions())
            {
                imageControls[p.Row + dropDistance, p.Column].Opacity = 0.25; //0.25 - ćwiartka jasności
                imageControls[p.Row + dropDistance, p.Column].Source = tileImages[block.Id];
            }
        }


        private void Draw(GameState gameState) // rysuje wszystko
        {
            DrawGrid(gameState.GameGrid);
            DrawGhostBlock(gameState.CurrentBlock); // rysuje ghost bloku
            DrawBlock(gameState.CurrentBlock);
            DrawNextBlock(gameState.BlockQueue);
            DrawHeldBlock(gameState.HeldBlock);
            ScoreText.Text = $"Wynik: {gameState.Score}";
            HighestScore.Text = $"Najlepszy wynik: {highestScore.LoadScore()}";
        }

        private async Task GameLoop() // pętla gry
        {
            Draw(gameState); // rysuje wszystko

            while(!gameState.GameOver)
            {
                int delay = Math.Max(minDelay, maxDelay - (gameState.Score * delayDecrease)); //zmiana opóźninia w zależności od wyniku - zmiana poziomu trudności
                await Task.Delay(500);
                gameState.MoveBlockDown();
                Draw(gameState);
            }

            GameOverMenu.Visibility = Visibility.Visible;
            FinalScoreText.Text = $"Wynik końcowy: {gameState.Score}";
            if(highestScore.LoadScore() < gameState.Score)
            {
                highestScore.SaveScore(gameState.Score); //zapisywanie nowego najlepszego wyniku
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e) // obsługa przycisków
        {
            if(gameState.GameOver) // jeśli gra się skończyła to nie obsługuje przycisków
            {
                return;
            }

            switch(e.Key)
            {
                case Key.Left:
                    gameState.MoveBlockLeft();
                    break;
                case Key.Right:
                    gameState.MoveBlockRight();
                    break;
                case Key.Up:
                    gameState.RotateBlockCW(); //CW - clockwise
                    break;
                case Key.Down:
                    gameState.MoveBlockDown();
                    break;
                case Key.Z:
                    gameState.RotateBlockCCW(); //CCW - counter clockwise
                    break;
                case Key.C:
                    gameState.HoldBlock();
                    break;
                case Key.Space:
                    gameState.DropBlock();
                    break;
                default: //przycisk, który nie jest obsługiwany
                    return;
            }

            Draw(gameState); //wykonuje się tylko wtedy, gdy wyjdzie ze switcha - dla obsługiwanych przycisków
        }

        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e) // ładowanie gry
        {
            await GameLoop(); //pętla gry
        }

        private async void PlayAgain_Click(object sender, RoutedEventArgs e) // przycisk "zagraj ponownie"
        {
            gameState = new GameState();
            GameOverMenu.Visibility = Visibility.Hidden;
            await GameLoop();  //pętla gry
        }



    }
}
