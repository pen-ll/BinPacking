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
        /// <summary>
        /// 把两个图形缩放【齐平】为一行【行宽度为PackerOptions.Width】
        /// 只有一个时，直接缩放为容器宽度
        /// </summary>
        /// <param name="blocks"></param>
        private void ScaleBlocksOneLineWithSmooth(params Block[] blocks)
        {
            ScaleBlocksOneLineWithSmooth(PackerOptions.Width, PackerOptions.Height, blocks);
        }

        /// <summary>
        /// 把两个图形缩放【齐平】为一行【指定行容器宽度】
        /// 只有一个时，直接缩放为容器宽度
        /// </summary>
        /// <param name="containerWidth">容器宽度</param>
        /// <param name="maxHeight">缩放最大高度，超出该高度会以该高度重算block.H</param>
        /// <param name="blocks">须缩放到一行得blocks</param>
        private void ScaleBlocksOneLineWithSmooth(double containerWidth, int maxHeight, params Block[] blocks)
        {
            if (blocks == null)
            {
                return;
            }

            if (blocks.Length == 1)
            {
                ScaleBlockTargetWidth(blocks[0], containerWidth, maxHeight);//只有缩放，没有赋值Fit定位
                return;
            }

            var firstBlock = blocks[0];
            double h = firstBlock.H;

            //以first为准 ，其他block都缩放为first大小
            foreach (Block block in blocks)
            {
                double h1 = block.H;
                block.H = h;
                block.W = block.W / (h1 / h);
            }

            // 再缩放为 容器大小
            ScaleBlocksOneLineWithoutSmooth(containerWidth, maxHeight, blocks);
        }

        /// <summary>
        /// 把两个图形缩放【非齐平】为一行【行宽度为PackerOptions.Width】
        /// </summary>
        /// <param name="blocks"></param>
        private void ScaleBlocksOneLineWithoutSmooth(params Block[] blocks)
        {
            ScaleBlocksOneLineWithoutSmooth(PackerOptions.Width, PackerOptions.Height, blocks);
        }

        /// <summary>
        /// 把两个图形缩放【非齐平】为一行【指定行容器宽度】
        /// </summary>
        private void ScaleBlocksOneLineWithoutSmooth(double containerWidth, int maxHeight, params Block[] blocks)
        {
            if (blocks == null)
            {
                return;
            }

            // 注意：【非齐平】图片需为还原原始分辨率，不能压缩过一遍之后再压缩，不然非齐平会导致多次缩放过小
            ResetSourceWidthHeightFit(blocks);

            if (blocks.Length == 1)
            {
                ScaleBlockTargetWidth(blocks[0], containerWidth, maxHeight);//只有缩放，没有赋值Fit定位
                return;
            }

            double ratio = blocks.Sum(o => o.W) / containerWidth;

            foreach (Block block in blocks)
            {
                double scaleWidth = block.W / ratio;
                block.H = block.H / (block.W / scaleWidth);
                block.W = scaleWidth;
                block.IsScaled = true;
            }
        }

        private static void ResetSourceWidthHeightFit(IEnumerable<Block> blocks)
        {
            foreach (Block block in blocks)
            {
                ResetSourceWidthHeightFit(block);
            }
        }

        private static void ResetSourceWidthHeightFit(Block block)
        {
            if (block.IsScaled)
            {
                block.W = block.SourceWidth;
                block.H = block.SourceHeight;
                block.IsScaled = false;
            }
            block.Fit = null;
        }

        /// <summary>
        /// 缩放单个到对应容器
        /// </summary>
        /// <param name="block">需要缩放得块</param>
        /// <param name="targetWidth">目标缩放宽度</param>
        /// <param name="maxHeight">最大高度，默认不传为PackerOptions.Height</param>
        /// <exception cref="ArgumentOutOfRangeException">targetWidth不能未配置</exception>
        private void ScaleBlockTargetWidth(Block block, double targetWidth, int maxHeight = 0)
        {
            ScaleBlockTargetWidth(block, targetWidth, false, 0, 0, maxHeight);
        }

        /// <summary>
        /// 缩放单个到对应容器
        /// </summary>
        /// <param name="block">需要缩放得块</param>
        /// <param name="targetWidth">目标缩放宽度</param>
        /// <param name="maxHeight">最大高度，默认不传为PackerOptions.Height</param>
        /// <exception cref="ArgumentOutOfRangeException">targetWidth不能未配置</exception>
        private void ScaleBlockTargetWidth(Block block, double targetWidth, bool isOnlyOneBlockFit, double baseNeedX, double baseNeedY, int maxHeight = 0)
        {
            if (targetWidth < 1)
            {
                throw new ArgumentOutOfRangeException("containerWidth", "ScaleBlockTargetWidth containerWidth不能为0");
            }

            if (maxHeight < 1)
            {
                maxHeight = PackerOptions.Height;
            }

            if (block.W == targetWidth)
            {
                if (block.H > maxHeight)
                {
                    double h1 = maxHeight;
                    double resultWidht1 = ScaleWidth(block.W, h1, block.H);

                    block.H = h1;
                    block.W = resultWidht1;
                    block.IsScaled = true;
                    return;
                }
            }

            double h = ScaleHeight(block.H, targetWidth, block.W);

            if (h < MinBlockHeight && !isOnlyOneBlockFit)
            {
                return;
            }

            if (h > maxHeight)//只有一个时，缩放高度超过 当前容器
            {
                h = maxHeight;
            }

            double resultWidht = ScaleWidth(block.W, h, block.H);
            if (resultWidht < MinBlockWidth && maxHeight != PackerOptions.Height && !isOnlyOneBlockFit)//maxHeight!= PackerOptions.Height 当时最大容器时，不判断超长图，缩放导致过小
            {
                return;
            }

            block.H = h;
            block.W = resultWidht;
            block.IsScaled = true;

            if (isOnlyOneBlockFit)
            {
                block.Fit = new Block(block.W, block.H) { X = baseNeedX, Y = baseNeedY };
                LeftHeight = maxHeight - block.H;
            }
        }

        private double ScaleHeight(double blockHeight, double scaledTargetWidth, double blockWidth)
        {
            return blockHeight / (blockWidth / scaledTargetWidth);
        }

        private double ScaleWidth(double blockWidth, double scaledTargetHeight, double blockHeight)
        {
            return blockWidth / (blockHeight / scaledTargetHeight);
        }

    }
}
