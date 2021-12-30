namespace BinPacking
{
    /// <summary>
    /// 【新版】待排版块
    /// </summary>
    public class Block
    {
        public Block(double w, double h, double x = 0, double y = 0)
        {
            BlockID = 0;
            X = x;
            Y = y;
            W = w;
            H = h;
            SourceWidth = w;
            SourceHeight = h;
        }

        /// <summary>
        /// 用于唯一标识
        /// </summary>
        public long BlockID { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public bool Used { get; set; } = false;
        public Block Down { get; set; }
        public Block Right { get; set; }
        public Block Fit { get; set; }
        public double BalanceX { get; set; }
        public double BalanceY { get; set; }
        public double W { get; set; }
        public double H { get; set; }
        public readonly double SourceWidth;
        public readonly double SourceHeight;
        public bool IsLastBlocksAutoMiddle { get; set; } = false;
        public bool IsScaled { get; set; } = false;
        /// <summary>
        /// 是否已经进行变形过一次
        /// </summary>
        public bool IsOverChange { get; set; }
        /// <summary>
        /// 匹配类型，0-单图；1-俩图；2-首张竖图；3-4张同比例组合；4-3张正方形组合；5-不规则刚好比例组合；6-最终2张一行
        /// </summary>
        public int FitType { get; set; }
    }
}