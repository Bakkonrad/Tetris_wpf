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
        
        private Block RandomBlock()
        {
            return blocks[random.Next(blocks.Length)]; //losowanie bloku
        }
    }
}
