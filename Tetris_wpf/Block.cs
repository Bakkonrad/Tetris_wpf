using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris_wpf
{
    public abstract class Block
    {
        protected abstract Position[][] Tiles { get; } //płytki (każdy blok ma 4)
        protected abstract Position StartOffset { get; } 
        public abstract int Id { get; } 

        private int rotationState = 0; //obecny obrót klocka
        private Position offset; //obecny offset klocka

        public Block()
        {
            offset = new Position(StartOffset.Row, StartOffset.Column); //ustawienie offsetu na początkowy
        }
        public IEnumerable<Position> TitlePositions() //zwraca pozycje klocka
        {
            foreach(Position p in Tiles[rotationState])
            {
                yield return new Position(p.Row + offset.Row, p.Column + offset.Column);
            }
        }

        public void RotateCW() //obrót klocka o 90 stopni w prawo
        {
            rotationState = (rotationState + 1) % Tiles.Length;
        }
        public void RotateCCW() //obrót klocka o 90 stopni w lewo
        {
            if(rotationState == 0)
                rotationState = Tiles.Length - 1;
            else
                rotationState--;
        }
        public void Move(int rows, int columns)
        {
            offset.Row += rows;
            offset.Column += columns;
        }
        public void Reset() //resetuje klocek do pozycji początkowej
        {
            rotationState = 0;
            offset.Row = StartOffset.Row;
            offset.Column = StartOffset.Column;
        }
    }
}
