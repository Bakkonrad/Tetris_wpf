using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris_wpf
{
    public class BlockQueue // kolejka bloków ktore pojawia sie w grze
    {
        private readonly Block[] blocks = new Block[] //tablica ze wszystkimi blokami
        {
            new IBlock(),
            new JBlock(),
            new LBlock(),
            new OBlock(),
            new SBlock(),
            new TBlock(),
            new ZBlock(),
        };

        private readonly Random random = new Random(); //losownie klocka

        public Block NextBlock { get; private set; } //metoda zwracająca następny blok

        public BlockQueue()
        {
            NextBlock = RandomBlock(); //losowanie następnego bloku
        }
        
        private Block RandomBlock()
        {
            return blocks[random.Next(blocks.Length)]; //losowanie bloku
        }

        public Block GetAndUpdate()
        {
            Block block = NextBlock; //przypisanie następnego bloku do zmiennej

            do
            {
                NextBlock = RandomBlock(); //losowanie następnego bloku
            } 
            while (block.Id == NextBlock.Id); //sprawdzenie czy następny blok nie jest taki sam jak poprzedni

            return block; //zwrócenie bloku
        }
    }
}
