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
        // 计算剩余容器 高度
        private PackerOptions _PackerOptions = null;
        public PackerOptions PackerOptions
        {
            get
            {
                return _PackerOptions;
            }
            set
            {

                _PackerOptions = value;
            }
        }

        public double LastUsingHeight
        {
            get; set;
        }

        public double LeftHeight { get; set; }

        public double MinBlockHeight
        {
            get
            {
                return PackerOptions.Height * PackerOptions.ScaleMinHeightRatio;
            }
        }

        public double MinBlockWidth
        {
            get
            {
                return PackerOptions.Width * PackerOptions.ScaleMinWidthRatio;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public BinFitPacker(PackerOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException();
            }

            _PackerOptions = options;
            _SourcePackerOptions = options;

            LeftHeight = PackerOptions.Height;//初始为容器高度
            LastUsingHeight = 0;

            //// 最小支持1x1像素[doge]
            //if (PackerOptions.Height < 2)
            //{
            //    throw new ArgumentOutOfRangeException("PackerOptions.Height", " 值超出范围，问题值：" + PackerOptions.Height);
            //}
            //if (PackerOptions.Width < 2)
            //{
            //    throw new ArgumentOutOfRangeException("PackerOptions.Width", " 值超出范围，问题值：" + PackerOptions.Width);
            //}
        }


        public BinFitPacker InitInfo()
        {
            LastUsingHeight = 0;
            LeftHeight = 0;
            LeftBlocks = new List<Block>();
            return this;
        }

        private PackerOptions _SourcePackerOptions = null;
        /// <summary>
        /// 多次适配剩余空间时保存首次
        /// </summary>
        public PackerOptions SourcePackerOptions
        {
            get
            {
                return _SourcePackerOptions;
            }
            set
            {
                if (_SourcePackerOptions == null)//首次才会赋值
                {
                    _SourcePackerOptions = value;
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("  ======> !!! 不建议多次赋值SourcePackerOptions");
                    Console.WriteLine();
                }
            }
        }

        /// <summary>
        /// 保存上次使用得高度
        /// </summary>
        /// <param name="packerOptions"></param>
        /// <returns></returns>
        public BinFitPacker ResetLeftFitOptions(PackerOptions packerOptions)
        {
            PackerOptions = packerOptions;
            LeftHeight = 0;
            LeftBlocks = new List<Block>();
            return this;
        }

        /// <summary>
        /// 剩余未能适配 Blocks
        /// </summary>
        public List<Block> LeftBlocks { get; set; } = new List<Block>();

        /// <summary>
        /// 适配各种
        /// </summary>
        /// <param name="blocks"></param>
        /// <param name="isLeftContainer">用剩余空间进行适配</param>
        /// <exception cref="ArgumentException"></exception>
        public void Fit(List<Block> blocks)
        {

            LeftHeight = PackerOptions.Height;
            LastUsingHeight = 0;


            if (blocks == null || blocks.Count == 0)
            {
                Console.WriteLine("!");
                throw new ArgumentException("Fit blocks 里面数据都是错误，如w/h为0");
            }

            if (PackerOptions.Height < 2)
            {
                LeftBlocks = blocks;
                return;
            }
            if (PackerOptions.Width < 2)
            {
                LeftBlocks = blocks;
                return;
            }

            if (blocks.Any(b => b.W < 1 || b.H < 1))
            {
                throw new CannotFitException("无法适配存在 w或h为<1得存在");
            }

            ResetSourceWidthHeightFit(blocks);
            //只有一张图
            if (blocks.Count == 1)
            {
                ScaleBlockTargetWidth(blocks[0], PackerOptions.Width, isOnlyOneBlockFit: true, 0, 0, PackerOptions.Height);
                LastUsingHeight += PackerOptions.Height - LeftHeight;
                blocks[0].FitType = 0;
                Console.WriteLine($" LeftHeight:{LeftHeight},,LastUsingHeight:{LastUsingHeight}");
                return;
            }

            //只有两张图
            if (blocks.Count == 2)
            {
                if (FitOnlyTwo(blocks))
                {
                    Console.WriteLine("   ->可适配 仅2张图 模板，剩余：" + LeftBlocks.Count);
                    LeftBlocks = blocks.Where(o => o.Fit == null).ToList();
                    LastUsingHeight += PackerOptions.Height - LeftHeight;
                    blocks.ForEach(block => { block.FitType = 1; });
                    Console.WriteLine($" LeftHeight:{LeftHeight},,LastUsingHeight:{LastUsingHeight}");
                    return;
                }
                else
                {
                    Console.WriteLine("   -> 无法适配 仅2张图 模板");
                    //不适配，都重置为处理之前
                    ResetSourceWidthHeightFit(blocks);
                }
            }

            //第一张是长图竖图
            if (IsLongHPic(blocks[0]))
            {
                Console.WriteLine("   长图竖图适配...");
                ScaleBlockTargetWidth(blocks[0], PackerOptions.Width, isOnlyOneBlockFit: true, 0, 0, PackerOptions.Height);
                LeftBlocks.AddRange(blocks.Skip(1));
                LastUsingHeight += PackerOptions.Height - LeftHeight;
                blocks.ForEach(block => { block.FitType = 2; });
                Console.WriteLine($" LeftHeight:{LeftHeight},,LastUsingHeight:{LastUsingHeight}");
                return;
            }

            //4张同比例图片 尝试
            if (FitFour(blocks))
            {
                Console.WriteLine("   ->可适配 4同比例 模板，剩余：" + LeftBlocks.Count);
                LeftBlocks = blocks.Where(o => o.Fit == null).ToList();
                LastUsingHeight += PackerOptions.Height - LeftHeight;
                blocks.ForEach(block => { block.FitType = 3; });
                Console.WriteLine($" LeftHeight:{LeftHeight},,LastUsingHeight:{LastUsingHeight}");
                return;
            }
            else
            {
                Console.WriteLine("   -> 无法适配 FitFour 模板");
                //不适配，都重置为处理之前
                ResetSourceWidthHeightFit(blocks);
            }

            //3张正方形 尝试
            if (FitThreeSquare(blocks))
            {
                Console.WriteLine("   ->可适配 3正方形 模板，剩余：" + LeftBlocks.Count);
                LeftBlocks = blocks.Where(o => o.Fit == null).ToList();
                LastUsingHeight += PackerOptions.Height - LeftHeight;
                blocks.ForEach(block => { block.FitType = 4; });
                Console.WriteLine($" LeftHeight:{LeftHeight},,LastUsingHeight:{LastUsingHeight}");
                return;
            }
            else
            {
                Console.WriteLine("   -> 无法适配 3正方形 模板");
                //不适配，都重置为处理之前
                ResetSourceWidthHeightFit(blocks);
            }

            //不规则组合直接排列 内部支持1张直接排版、3-6张不规则排版
            if (JustFitMultiple(blocks))//true 即更新Fit和LeftBlocks
            {
                Console.WriteLine("   ->可适配 3-6不规则 模板，剩余：" + LeftBlocks.Count);
                LeftBlocks = blocks.Where(o => o.Fit == null).ToList();
                LastUsingHeight += PackerOptions.Height - LeftHeight;
                blocks.ForEach(block => { block.FitType = 5; });
                Console.WriteLine($" LeftHeight:{LeftHeight},,LastUsingHeight:{LastUsingHeight}");
                return;
            }
            else
            {
                Console.WriteLine("   -> 无法适配 JustFitMultiple 模板");
                //不适配，都重置为处理之前
                ResetSourceWidthHeightFit(blocks);
            }

            LastFitMultiple(blocks);
            blocks.ForEach(block => { block.FitType = 6; });

            LeftBlocks = blocks.Where(o => o.Fit == null).ToList();
            Console.WriteLine("   -> 全部模板适配结束，剩余：" + LeftBlocks.Count);
            LastUsingHeight += PackerOptions.Height - LeftHeight;
            Console.WriteLine($" LeftHeight:{LeftHeight},,LastUsingHeight:{LastUsingHeight}");

            ResetSourceWidthHeightFit(LeftBlocks);//剩余的，都重置
        }

        ///// <summary>
        ///// 适配各种
        ///// 暂不支持 超过3个得数量 剩余空间进行再适配
        ///// </summary>
        ///// <param name="blocks"></param>
        ///// <param name="isLeftContainer">用剩余空间进行适配</param>
        ///// <exception cref="ArgumentException"></exception>
        //public void FitLeft(List<Block> blocks)
        //{
        //    if (blocks == null || blocks.Count == 0)
        //    {
        //        return;
        //    }


        //    if (PackerOptions.Height < 2)
        //    {
        //        LeftBlocks = blocks;
        //        return;
        //    }
        //    if (PackerOptions.Width < 2)
        //    {
        //        LeftBlocks = blocks;
        //        return;
        //    }

        //    if (LastUsingHeight < 1)
        //    {
        //        throw new ArgumentException("LastUsingHeight不能为<1  适配剩余空间,请不要调用InitInfo，ResetLeftFitOptions");
        //    }

        //    LeftHeight = PackerOptions.Height;

        //    var fits = blocks.Count > 2 ? blocks.Take(2) : blocks;

        //    ScaleBlocksOneLineWithSmooth(fits.ToArray());

        //    if (blocks[0].H > PackerOptions.Height)//因为是齐平直接取第一个（第二个高度跟第一个一样）
        //    {
        //        LeftBlocks = blocks;
        //        ResetSourceWidthHeightFit(blocks);
        //        return;
        //    }

        //    int total = fits.Count();
        //    LeftBlocks = blocks.Skip(total).ToList();

        //    blocks[0].Fit = new Block(blocks[0].W, blocks[0].H, 0, LastUsingHeight);
        //    if (total > 1)
        //    {
        //        blocks[1].Fit = new Block(blocks[1].W, blocks[1].H, blocks[0].W, LastUsingHeight);
        //    }

        //    LeftHeight = PackerOptions.Height - blocks[0].H;
        //    LastUsingHeight += PackerOptions.Height - LeftHeight;

        //    Console.WriteLine($"  ---- LeftHeight:{LeftHeight},,LastUsingHeight:{LastUsingHeight}");
        //}

    }
}
