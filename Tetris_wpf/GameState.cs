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

                //jeśli pierwsze dwa wiersze są puste to gra się spawn bloka jest tak aby ładniej wyglądało

                for(int i = 0; i < 2; i++)
                {
                    currentBlock.Move(1, 0);

                    if (!BlockFits())
                    {
                        currentBlock.Move(-1, 0);
                    }
                }
            }
        }

        public GameGrid GameGrid { get; }
        public BlockQueue BlockQueue { get; }
        public bool GameOver { get; private set; } //zmienna informująca czy gra się skończyła
        public int Score { get; private set; } //zmienna przechowująca wynik
        public Block HeldBlock { get; private set; }
        public bool CanHold { get; private set; } = true; //zmienna informująca czy można zmienić blok

        public GameState() //konstruktor
        {
            GameGrid = new GameGrid(22, 10); //tworzenie planszy
            BlockQueue = new BlockQueue(); //tworzenie kolejki bloków
            CurrentBlock = BlockQueue.GetAndUpdate(); //przypisanie do zmiennej aktualnego bloku
            CanHold = true;
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

        public void HoldBlock() //zmiana bloku
        {
            if (!CanHold) //jeśli można zmienić blok
            {
                return;
            }
            if(HeldBlock == null)
            {
                HeldBlock = CurrentBlock;
                CurrentBlock = BlockQueue.GetAndUpdate();
            }
            else
            {
                Block temp = CurrentBlock;
                CurrentBlock = HeldBlock;
                HeldBlock = temp;
            }

            CanHold = false;
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

        public void MoveBlockLeft() //przesuwanie bloku
        {
            CurrentBlock.Move(0, -1);
            if (!BlockFits()) //jeśli blok nie mieści się na planszy to cofa ruch
            {
                CurrentBlock.Move(0, 1);
            }
        }

        public void MoveBlockRight() //przesuwanie bloku
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

            Score += GameGrid.ClearFullRows(); //usuwa pełne rzędy i dodaje na tej podstawie punkty do wyniku

            if (IsGameOver()) //jeśli gra się skończyła to ustawia zmienną GameOver na true
            {
                GameOver = true;
            }
            else
            {
                CurrentBlock = BlockQueue.GetAndUpdate(); //przypisuje do zmiennej aktualny blok
                CanHold = true;
            }
        }

        public void MoveBlockDown()
        {
            CurrentBlock.Move(1, 0);
            if (!BlockFits()) //jeśli blok nie mieści się na planszy to 
            {
                CurrentBlock.Move(-1, 0);
                PlaceBlock();
            }
        }

        private int TileDropDistance(Position p) //zwraca odległość o jaką można opuścić część bloku
        {
            int drop = 0;

            while (GameGrid.IsEmpty(p.Row + drop + 1, p.Column))
            {
                drop++;
            }
            return drop;
        }

        public int BlockDropDistance() //zwraca najmniejszą odległość o jaką można opuścić blok
        {
            int drop = GameGrid.Rows;

            foreach (Position p in CurrentBlock.TitlePositions())
            {
                drop = Math.Min(drop, TileDropDistance(p)); //zwraca najmniejszą odległość o jaką można opuścić blok na podstawie pozycji każdego z klocków
            }
            return drop;
        }

        public void DropBlock() //opuszcza blok na dół
        {
            CurrentBlock.Move(BlockDropDistance(), 0);
            PlaceBlock();
        }
    }
}
