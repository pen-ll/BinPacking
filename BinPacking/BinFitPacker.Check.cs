using System;
using System.Collections.Generic;
using System.Linq;

namespace BinPacking
{
    /// <summary>
    /// 限制数量内的多样式 排列装箱
    /// </summary>
    public partial class BinFitPacker
    {

        private static bool IsSquare(Block block)
        {
            return block.W == block.H && block.W > 0;
        }

        /// <summary>
        /// 是否横图 长图
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private bool IsLongWPic(Block block)
        {
            return block.W > block.H && block.H > 0 && block.W / block.H > PackerOptions.LongPicRatioTimes;
        }

        /// <summary>
        /// 是否竖图 长图
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private bool IsLongHPic(Block block)
        {
            return block.W < block.H && block.W > 0 && block.H / block.W > PackerOptions.LongPicRatioTimes;
        }

        /// <summary>
        /// 过滤错误的Blocks
        /// </summary>
        /// <param name="blocks"></param>
        private List<Block> FilterBlocksWithSetLeftBlock(List<Block> blocks)
        {
            LeftBlocks = blocks.Where(b => b.W < 1 || b.H < 1).ToList();
            return blocks.Except(LeftBlocks).ToList();
        }
    }
}
