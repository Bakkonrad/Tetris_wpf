using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris_wpf
{
    public class GameState
    {
        private Block currentBlock; //aktualny blok

        public Block CurrentBlock //
        {
            get => currentBlock;
            private set
            {
                currentBlock = value; //przypisanie wartości do zmiennej
                currentBlock.Reset(); //resetowanie bloku
            }
        }

        public GameGrid GameGrid { get; }
        public BlockQueue BlockQueue { get; }
        public bool GameOver { get; private set; } //zmienna informująca czy gra się skończyła

        public GameState(int rows, int columns) //konstruktor
        {
            GameGrid = new GameGrid(22, 10); //tworzenie planszy
            BlockQueue = new BlockQueue(); //tworzenie kolejki bloków
            CurrentBlock = BlockQueue.GetAndUpdate(); //przypisanie do zmiennej aktualnego bloku
        }

        public bool BlockFits()
        {
            foreach (Position p in CurrentBlock.TitlePositions()) //sprawdzanie czy blok mieści się na planszy
            {
                if (!GameGrid.IsEmpty(p.Row, p.Column))
                {
                    return false;
                }
            }
            return true;
        }

        public void RotateBlockCW() //obrót bloku o 90 stopni w prawo
        {
            CurrentBlock.RotateCW();
            if (!BlockFits()) //jeśli blok nie mieści się na planszy to cofa obrót
            {
                CurrentBlock.RotateCCW();
            }
        }

        public void RotateBlockCCW() //obrót bloku o 90 stopni w lewo
        {
            CurrentBlock.RotateCCW();
            if (!BlockFits()) //jeśli blok nie mieści się na planszy to cofa obrót
            {
                CurrentBlock.RotateCW();
            }
        }

        public void MoveBlockLeft(int rows, int columns) //przesuwanie bloku
        {
            CurrentBlock.Move(0, -1);
            if (!BlockFits()) //jeśli blok nie mieści się na planszy to cofa ruch
            {
                CurrentBlock.Move(0, 1);
            }
        }

        public void MoveBlockRight(int rows, int columns) //przesuwanie bloku
        {
            CurrentBlock.Move(0, 1);
            if (!BlockFits()) //jeśli blok nie mieści się na planszy to cofa ruch
            {
                CurrentBlock.Move(0, -1);
            }
        }

        private bool IsGameOver()
        {
            return !(GameGrid.IsRowEmpty(0) && GameGrid.IsRowEmpty(1)); //sprawdza czy pierwsze dwa rzędy (ukryte) są puste, jeśli tak to gra się kończy
        }

        private void PlaceBlock() //umieszcza blok na planszy
        {
            foreach (Position p in CurrentBlock.TitlePositions())
            {
                GameGrid[p.Row, p.Column] = CurrentBlock.Id;
            }

            GameGrid.ClearFullRows(); //usuwa pełne rzędy

            if (IsGameOver()) //jeśli gra się skończyła to ustawia zmienną GameOver na true
            {
                GameOver = true;
            }
        }

        public void MoveBlockDown()
        {
            CurrentBlock.Move(1, 0);
            if (!BlockFits()) //jeśli blok nie mieści się na planszy to umieszcza go na planszy i pobiera nowy blok z kolejki
            {
                CurrentBlock.Move(-1, 0);
                PlaceBlock();
            }
        }
    }
}
