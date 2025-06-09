using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvaloniaTetris.Models.Blocks;

namespace AvaloniaTetris.Models
{
    public class BlockQueue
    {
        private readonly Block[] blocks = new Block[]
        {
            new IBlock(), new JBlock(), new LBlock(),
            new OBlock(), new SBlock(), new TBlock(),
            new ZBlock()
        };

        private readonly Random random = new Random();
        public BlockQueue() 
        {
            NextBlock = RandomBlock();
        }

        public Block NextBlock { get; private set; }

        private Block RandomBlock()
        {
            return blocks[random.Next(blocks.Length)];
        }

        public Block GetAndUpdate()
        {
            Block block = NextBlock;

            // NOTE: We don't want to return the same
            //       block twice in a row.
            do
            {
                NextBlock = RandomBlock();
            }
            while (block.Id == NextBlock.Id);

            return block;
        }
    }
}
