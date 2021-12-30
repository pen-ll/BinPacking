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
        /// 适配至少3个不规则图片，且刚好齐平【首次】=>适配结果最多6个在一个容器内
        /// </summary>
        /// <param name="blocks"></param>
        /// <returns>未能适配false</returns>
        private bool JustFitMultiple(List<Block> blocks)
        {
            if (blocks == null || blocks.Count == 0 || blocks.Count < 2)//多图适配至少3个
            {
                return false;
            }

            //==6 最多支持6张，单页
            var resultBlock = blocks.Count > 6 ? blocks.Take(6) : blocks;

            bool isCanFit = CanFitPerfectly(resultBlock.ToList(), isCalcFitPosition: true);

            return isCanFit;
        }

        /// <summary>
        /// 适配不规则图片【首次】=>适配结果最多6个在一个容器内
        /// </summary>
        /// <param name="blocks"></param>
        /// <returns>未能适配false</returns>
        private void LastFitMultiple(List<Block> blocks)
        {
            if (blocks == null || blocks.Count == 0)//多图适配至少3个
            {
                throw new ArgumentNullException(nameof(blocks));
            }


            //==6 最多支持6张，单页
            var resultBlock = blocks.Count > 6 ? blocks.Take(6).ToList() : blocks;
            double useH = 0;
            for (int i = 0; i < resultBlock.Count; i++)
            {
                var block = resultBlock[i];

                if (block.IsScaled)
                {
                    continue;
                }

                if (block.W / block.H > 3)//超长横图直接独立一行
                {
                    ScaleBlockTargetWidth(block, PackerOptions.Width, isOnlyOneBlockFit: true, 0, useH, PackerOptions.Height);
                    useH += block.H;
                    continue;
                }

                bool isLast = i + 1 == resultBlock.Count;

                if (isLast)//奇数最后一个，未适配，独立一行
                {
                    ScaleBlockTargetWidth(block, PackerOptions.Width, PackerOptions.Height);
                    if (useH + block.H > PackerOptions.Height)
                    {
                        //差一点 齐平
                        if ((useH + block.H - PackerOptions.Height) / PackerOptions.Height < PackerOptions.HeightDeviationRatio)
                        {
                            block.Fit = new Block(block.W, block.H, 0, useH);//暂不挤变形
                            LeftHeight = 0;
                            return;
                        }
                        else
                        {
                            return;
                        }
                    }
                    break;
                }

                var block1 = resultBlock[i + 1];

                double h1 = block.H;
                block.H = block.H / (block.W / block1.W);
                block.W = block.W / (h1 / block.H);
                //再缩放回正常大小
                ScaleBlocksOneLineWithSmooth(block, block1);

                if (useH + block.H > PackerOptions.Height)
                {
                    //齐平容错超过0.02
                    if ((useH + block.H - PackerOptions.Height) / PackerOptions.Height > PackerOptions.HeightDeviationRatio)
                    {
                        if (i == 0)//只有两张图，发现一行是超长图，直接跳过，第一张图独立一行
                        {
                            ScaleBlockTargetWidth(block, PackerOptions.Width, isOnlyOneBlockFit: true, 0, useH, PackerOptions.Height);
                            ResetSourceWidthHeightFit(block1);
                            LeftHeight = PackerOptions.Height - block.H;
                            return;
                        }
                        else
                        {
                            LeftHeight = PackerOptions.Height - useH;
                            return;
                        }
                    }
                }

                if (block.W < MinBlockWidth)//小于最小宽度 独立一行
                {
                    ScaleBlockTargetWidth(block, PackerOptions.Width, isOnlyOneBlockFit: true, 0, useH, PackerOptions.Height);
                    useH += block.H;
                    ResetSourceWidthHeightFit(block1);
                    continue;
                }

                if (block1.W < MinBlockWidth)//小于最小宽度 独立一行
                {
                    ScaleBlockTargetWidth(block, PackerOptions.Width, isOnlyOneBlockFit: true, 0, useH, PackerOptions.Height);
                    useH += block.H;
                    ResetSourceWidthHeightFit(block1);
                    continue;
                }

                block.Fit = new Block(block.W, block.H, 0, useH);
                block1.Fit = new Block(block1.W, block1.H, block.W, useH);

                useH += block.H;
            }

            LeftHeight = PackerOptions.Height - useH;
        }

        /// <summary>
        /// 适配4个特殊规则图片
        /// 1、比例一样得横图【一上俩中一下】
        /// </summary>
        /// <param name="blocks"></param>
        /// <param name="containerWidth">适配容器宽</param>
        /// <param name="maxHeight">适配容器最大高度</param>
        /// <returns>未能适配false</returns>
        private bool FitFour(List<Block> blocks, double containerWidth = 0, double maxHeight = 0)
        {
            if (blocks == null || blocks.Count == 0 || blocks.Count < 4)
            {
                return false;
            }

            if (containerWidth == 0)
            {
                containerWidth = PackerOptions.Width;
            }
            if (maxHeight == 0)
            {
                maxHeight = PackerOptions.Height;
            }

            var resultBlock = blocks.Count > 4 ? blocks.Take(4).ToList() : blocks;//==4


            //非横图
            if (resultBlock[0].W <= resultBlock[0].H || resultBlock[1].W <= resultBlock[1].H || resultBlock[2].W <= resultBlock[2].H || resultBlock[3].W <= resultBlock[3].H)
            {
                return false;
            }

            var block1Ratio = resultBlock[0].W / resultBlock[0].H;

            var block2Ratio = resultBlock[1].W / resultBlock[1].H;
            if (block1Ratio != block2Ratio && Math.Abs(block1Ratio - block2Ratio) > PackerOptions.WidthDeviationRatio)
            {
                return false;
            }

            var block3Ratio = resultBlock[2].W / resultBlock[2].H;
            if (block1Ratio != block3Ratio && Math.Abs(block1Ratio - block3Ratio) > PackerOptions.WidthDeviationRatio)
            {
                return false;
            }

            var block4Ratio = resultBlock[3].W / resultBlock[3].H;
            if (block1Ratio != block4Ratio && Math.Abs(block1Ratio - block4Ratio) > PackerOptions.WidthDeviationRatio)
            {
                return false;
            }

            //缩放
            ScaleBlockTargetWidth(resultBlock[0], containerWidth);
            ScaleBlockTargetWidth(resultBlock[1], containerWidth / 2);
            resultBlock[2].W = resultBlock[1].W;
            resultBlock[2].H = resultBlock[1].H;

            resultBlock[3].W = resultBlock[0].W;
            resultBlock[3].H = resultBlock[0].H;

            //定位
            resultBlock[0].Fit = new Block(resultBlock[0].W, resultBlock[0].H) { X = 0, Y = 0 };

            double needChangeValue = resultBlock[0].H + resultBlock[1].H - maxHeight;
            bool isOver = false;
            if (needChangeValue > 0)
            {
                if (needChangeValue / maxHeight <= PackerOptions.WidthDeviationRatio)
                {
                    isOver = false;
                }
                else
                {
                    isOver = true;
                }
            }
            if (isOver)
            {
                //LeftBlocks = resultBlock.Skip(1).ToList();
                LeftHeight = maxHeight - resultBlock[0].H;
                return true;
            }

            if (!isOver && needChangeValue > 0)
            {
                double avChangeValue = needChangeValue / 2;
                resultBlock[0].H = resultBlock[0].H - avChangeValue;//差距0.02直接变形
                resultBlock[0].IsOverChange = true;
                resultBlock[1].H = resultBlock[1].H - avChangeValue;
                resultBlock[1].IsOverChange = true;
                resultBlock[2].H = resultBlock[2].H - avChangeValue;
                resultBlock[2].IsOverChange = true;
            }

            resultBlock[1].Fit = new Block(resultBlock[1].W, resultBlock[1].H) { X = 0, Y = resultBlock[0].H };
            resultBlock[2].Fit = new Block(resultBlock[2].W, resultBlock[2].H) { X = resultBlock[1].W, Y = resultBlock[0].H };

            double needChangeValue1 = resultBlock[0].H + resultBlock[1].H + resultBlock[3].H - maxHeight;
            bool isOver1 = false;
            if (needChangeValue1 > 0)
            {
                if (needChangeValue1 / maxHeight <= PackerOptions.WidthDeviationRatio)
                {
                    isOver1 = false;
                }
                else
                {
                    isOver1 = true;
                }
            }

            if (isOver1)//(resultBlock[0].H + resultBlock[1].H + resultBlock[3].H > maxHeight)
            {
                LeftHeight = maxHeight - resultBlock[0].H - resultBlock[1].H;//[2]跟[1]同一行
                return true;
            }

            if (!isOver1 && needChangeValue1 > 0)
            {
                double avChangeValue = needChangeValue1 / 3;//变形高度，平均平分
                resultBlock[0].H = resultBlock[0].H - avChangeValue;//差距0.02直接变形
                resultBlock[0].IsOverChange = true;
                resultBlock[1].H = resultBlock[1].H - avChangeValue;
                resultBlock[1].IsOverChange = true;
                resultBlock[2].H = resultBlock[2].H - avChangeValue;
                resultBlock[2].IsOverChange = true;
                resultBlock[3].H = resultBlock[3].H - avChangeValue;
                resultBlock[3].IsOverChange = true;
            }


            resultBlock[3].Fit = new Block(resultBlock[3].W, resultBlock[3].H) { X = 0, Y = resultBlock[0].H + resultBlock[1].H };

            LeftHeight = maxHeight - resultBlock[0].H - resultBlock[1].H - resultBlock[3].H;//[2]跟[1]同一行
            return true;
        }


        /// <summary>
        /// 适配3个正方形
        /// </summary>
        /// <param name="blocks"></param>
        /// <param name="containerWidth">适配容器宽</param>
        /// <param name="maxHeight">适配容器最大高度</param>
        /// <returns>未能适配false</returns>
        private bool FitThreeSquare(List<Block> blocks, double containerWidth = 0, double maxHeight = 0)
        {
            if (blocks == null || blocks.Count == 0 || blocks.Count < 3)
            {
                return false;
            }

            if (containerWidth == 0)
            {
                containerWidth = PackerOptions.Width;
            }
            if (maxHeight == 0)
            {
                maxHeight = PackerOptions.Height;
            }

            var resultBlock = blocks.Count > 3 ? blocks.Take(3).ToList() : blocks;//==3

            //非正方形
            if ((resultBlock[0].W != resultBlock[0].H && Math.Abs(resultBlock[0].W - resultBlock[0].H) > PackerOptions.WidthDeviationRatio)
                || (resultBlock[1].W != resultBlock[1].H && Math.Abs(resultBlock[1].W - resultBlock[1].H) > PackerOptions.WidthDeviationRatio)
                || (resultBlock[2].W != resultBlock[2].H && Math.Abs(resultBlock[2].W - resultBlock[2].H) > PackerOptions.WidthDeviationRatio)
                )
            {
                return false;
            }

            double expectWidth = containerWidth / 3 * 2;
            if (expectWidth > maxHeight)
            {
                return false;
            }

            //缩放
            ScaleBlockTargetWidth(resultBlock[0], expectWidth);

            if (resultBlock[0].W < expectWidth)//高度太少，导致第一个大正方形 进行了缩小
            {
                return false;
            }

            double needChangeValue = resultBlock[0].H - maxHeight;
            bool isOver = false;
            if (needChangeValue > 0)
            {
                if (needChangeValue / maxHeight <= PackerOptions.WidthDeviationRatio)
                {
                    isOver = false;
                }
                else
                {
                    isOver = true;
                }
            }

            if (isOver)
            {
                return false;
            }

            ScaleBlockTargetWidth(resultBlock[1], containerWidth / 3);
            resultBlock[2].W = resultBlock[1].W;
            resultBlock[2].H = resultBlock[1].H;

            if (!isOver && needChangeValue > 0)
            {
                double avChangeValue = needChangeValue / 2;//变形高度，平均平分
                resultBlock[0].H -= needChangeValue;//差距0.02直接变形
                resultBlock[0].IsOverChange = true;
                resultBlock[1].H -= avChangeValue;
                resultBlock[1].IsOverChange = true;
                resultBlock[2].H -= avChangeValue;
                resultBlock[2].IsOverChange = true;
            }

            //定位
            resultBlock[0].Fit = new Block(resultBlock[0].W, resultBlock[0].H) { X = 0, Y = 0 };
            resultBlock[1].Fit = new Block(resultBlock[1].W, resultBlock[1].H) { X = resultBlock[0].W, Y = 0 };
            resultBlock[2].Fit = new Block(resultBlock[2].W, resultBlock[2].H) { X = resultBlock[0].W, Y = resultBlock[1].H };
            LeftHeight = maxHeight - resultBlock[0].H;
            Console.WriteLine(" 3Q -Height:" + resultBlock[0].H);
            return true;
        }


        /// <summary>
        /// 适配仅2张图，任意图形
        /// 1、两个正方形{上下}
        /// 2、两个横图{上下}
        /// 3、一竖一横{上下}
        /// 4、两竖{左右}
        /// </summary>
        /// <param name="blocks"></param>
        /// <param name="containerWidth">适配容器宽</param>
        /// <param name="maxHeight">适配容器最大高度</param>
        /// <returns>未能适配false</returns>
        private bool FitOnlyTwo(List<Block> blocks, double containerWidth = 0, int maxHeight = 0)
        {
            if (blocks == null || blocks.Count == 0 || blocks.Count != 2)
            {
                return false;
            }

            if (containerWidth == 0)
            {
                containerWidth = PackerOptions.Width;
            }
            if (maxHeight == 0)
            {
                maxHeight = PackerOptions.Height;
            }


            //两个正方形
            if ((blocks[0].W == blocks[0].H || Math.Abs(blocks[0].W - blocks[0].H) < PackerOptions.WidthDeviationRatio)
                && (blocks[1].W == blocks[1].H || Math.Abs(blocks[1].W - blocks[1].H) < PackerOptions.WidthDeviationRatio))
            {
                //目前直接 只适配一个正方形剩余放入下页
                ScaleBlockTargetWidth(blocks[0], containerWidth, isOnlyOneBlockFit: true, 0, 0, maxHeight);
                return true;
            }
            else
            {
                bool isBlock1W = blocks[0].W > blocks[0].H;
                bool isBlock2W = blocks[1].W > blocks[1].H;

                if ((isBlock1W && isBlock2W) || (isBlock1W && !isBlock2W) || (!isBlock1W && isBlock2W))//俩横图 || 横竖 || 竖横
                {
                    ScaleBlockTargetWidth(blocks[0], containerWidth);
                    ScaleBlockTargetWidth(blocks[1], containerWidth);
                    if (blocks[0].H + blocks[1].H > maxHeight)
                    {
                        ScaleBlockTargetWidth(blocks[0], containerWidth, isOnlyOneBlockFit: true, 0, 0, maxHeight);
                        return true;
                    }

                    blocks[0].Fit = new Block(blocks[0].W, blocks[0].H, 0, 0);
                    blocks[1].Fit = new Block(blocks[1].W, blocks[1].H, 0, blocks[0].H);
                    LeftHeight = maxHeight - blocks[0].H - blocks[1].H;
                    return true;
                }
                else//俩竖图
                {
                    //俩图比例一致
                    if (blocks[0].W / blocks[0].H == blocks[1].W / blocks[1].H || Math.Abs(blocks[0].W / blocks[0].H - blocks[1].W / blocks[1].H) < PackerOptions.WidthDeviationRatio)
                    {
                        //缩放
                        ScaleBlockTargetWidth(blocks[0], containerWidth / 2);
                        if (blocks[0].H > maxHeight)
                        {
                            if ((blocks[0].H - maxHeight) / maxHeight < PackerOptions.HeightDeviationRatio)//isBeforeSmooth外层已经齐平
                            {
                                ScaleBlockTargetWidth(blocks[1], containerWidth / 2);
                                blocks[0].Fit = new Block(blocks[0].W, blocks[0].H, 0, 0);
                                blocks[1].Fit = new Block(blocks[1].W, blocks[1].H, blocks[0].W, 0);
                                return true;
                            }
                            else
                            {
                                ScaleBlockTargetWidth(blocks[0], containerWidth, isOnlyOneBlockFit: true, 0, 0, maxHeight);
                                return true;
                            }
                        }
                        else
                        {
                            if (blocks[0].H < maxHeight / 8 * 5)
                            {
                                ScaleBlockTargetWidth(blocks[0], containerWidth, isOnlyOneBlockFit: true, 0, 0, maxHeight);
                                return true;
                            }

                            ScaleBlockTargetWidth(blocks[1], containerWidth / 2);
                            blocks[0].Fit = new Block(blocks[0].W, blocks[0].H, 0, 0);
                            blocks[1].Fit = new Block(blocks[1].W, blocks[1].H, blocks[0].W, 0);
                            return true;
                        }
                    }
                    else
                    {
                        //由 两块一行的模板处理
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// 判断对应数量block原分辨率缩放后，刚好找齐
        /// 支持处理1个图片
        /// PS:失败会reset坐标 block.Fit=null;
        /// </summary>
        /// <param name="blocks">适配blocks</param>
        /// <param name="containerWidth">容器宽度</param>
        /// <param name="maxHeight">容器上限高度</param>
        /// <param name="isCalcFitPosition">是否计算坐标xy</param>
        /// <returns></returns>
        private bool CanFitPerfectly(List<Block> blocks, int containerWidth = 0, int maxHeight = 0, bool isCalcFitPosition = false)
        {
            if (containerWidth == 0)
            {
                containerWidth = PackerOptions.Width;
            }

            int totalCount = blocks.Count();
            if (totalCount < 2)//少量图直接不在此方法内适配
            {
                return false;
            }

            if (maxHeight == 0)
            {
                maxHeight = PackerOptions.Height;
            }

            double leftH = 0;
            double rightH = 0;

            double leftWidth = 0;
            double rightWidth = 0;

            for (int i = 0; i < totalCount; i++)
            {
                if (blocks[i].IsScaled)
                {
                    continue;
                }
                bool isInit = leftH == 0 || rightH == 0;
                bool isBeforeSmooth = (leftH == rightH || Math.Abs(leftH - rightH) / maxHeight < PackerOptions.HeightDeviationRatio) && !isInit;
                bool isLast = i + 1 == totalCount;


                //最后一个
                if (isLast && isBeforeSmooth)//例如：3个，前两个找齐了，第三个直接单独一行可以当作找齐了
                {
                    ScaleBlockTargetWidth(blocks[i], containerWidth, maxHeight);
                    if (leftH + blocks[i].H > maxHeight)
                    {
                        //剩余高度少于0.15比例也当作可适配
                        if ((maxHeight - leftH) / maxHeight < PackerOptions.LeftMinHeightRatio)//isBeforeSmooth外层已经齐平
                        {
                            LeftHeight = maxHeight - leftH;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    if (isCalcFitPosition)
                    {
                        blocks[i].Fit = new Block(blocks[i].W, blocks[i].H, 0, leftH);
                    }
                    leftH += blocks[i].H;
                    //rightH += blocks[i].H;//没用到，因为已经齐平左右相等
                    LeftHeight = maxHeight - leftH;

                    return true;
                }

                //齐平或者初始化
                if ((isInit || isBeforeSmooth) && !isLast)
                {
                    ScaleBlocksOneLineWithoutSmooth(blocks[i], blocks[i + 1]);

                    if (isInit)
                    {
                        if (isCalcFitPosition)
                        {
                            blocks[i].Fit = new Block(blocks[i].W, blocks[i].H, 0, 0);
                            blocks[i + 1].Fit = new Block(blocks[i + 1].W, blocks[i + 1].H, blocks[i].W, 0);
                        }
                        leftH = blocks[i].H;
                        leftWidth = blocks[i].W;
                        rightH = blocks[i + 1].H;
                        rightWidth = blocks[i + 1].W;
                    }
                    else
                    {

                        if (leftH + blocks[i].H > maxHeight)
                        {
                            //剩余高度少于0.15比例也当作可适配
                            if ((maxHeight - leftH) / maxHeight < PackerOptions.LeftMinHeightRatio)//isBeforeSmooth外层已经齐平
                            {
                                LeftHeight = maxHeight - leftH;
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }


                        if (isCalcFitPosition)
                        {
                            blocks[i].Fit = new Block(blocks[i].W, blocks[i].H, 0, leftH);
                            blocks[i + 1].Fit = new Block(blocks[i + 1].W, blocks[i + 1].H, blocks[i].W, leftH);//齐平所以y的 i.H==i+1.H
                        }
                        leftH += blocks[i].H;
                        rightH += blocks[i + 1].H;
                    }


                    //剩余高度少于0.15比例也当作可适配
                    if ((leftH == rightH || Math.Abs(leftH - rightH) / maxHeight < PackerOptions.HeightDeviationRatio)//放置完最新两个需要齐平
                        && (maxHeight - leftH) / maxHeight < PackerOptions.LeftMinHeightRatio)
                    {
                        LeftHeight = maxHeight - leftH;

                        return true;
                    }
                }
                else
                {
                    if (leftH < rightH)
                    {
                        ScaleBlockTargetWidth(blocks[i], leftWidth, maxHeight);//leftWidth<=containerWidth - blocks[i - 1].W

                        if (leftH + blocks[i].H > maxHeight)
                        {
                            return false;
                        }


                        if (isCalcFitPosition)
                        {
                            blocks[i].Fit = new Block(blocks[i].W, blocks[i].H, 0, leftH);
                        }
                        leftH += blocks[i].H;
                    }
                    else
                    {
                        ScaleBlockTargetWidth(blocks[i], rightWidth, maxHeight);//rightWidth<=containerWidth - blocks[i - 2].W
                        if (rightH + blocks[i].H > maxHeight)
                        {
                            return false;
                        }

                        if (isCalcFitPosition)
                        {
                            blocks[i].Fit = new Block(blocks[i].W, blocks[i].H, leftWidth, rightH);
                        }
                        rightH += blocks[i].H;
                    }

                    //剩余高度少于0.15比例也当作可适配
                    if ((leftH == rightH || Math.Abs(leftH - rightH) / maxHeight < PackerOptions.HeightDeviationRatio) //放置独立一个 齐平
                        && (maxHeight - leftH) / maxHeight < PackerOptions.LeftMinHeightRatio)//剩余<0.15
                    {
                        LeftHeight = maxHeight - leftH;
                        return true;
                    }
                }

                if (rightH > maxHeight || leftH > maxHeight //超出范围
                    || blocks[i].W < MinBlockWidth)// =>超长图直接失败 留到其他模板再适配
                {
                    return false;
                }
            }

            if (leftH == rightH || Math.Abs(leftH - rightH) / maxHeight < PackerOptions.HeightDeviationRatio)//可以找齐
            {
                LeftHeight = maxHeight - leftH;
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
