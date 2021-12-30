namespace BinPacking
{
    public class PackerOptions
    {
        /// <summary>
        /// 【废弃】图片缩放后最小宽度
        /// 如果图片过于细长，是允许突破这个限制，单独一页展示
        /// </summary>
        public int BlockFitMinWidth { get; set; } = 100;

        /// <summary>
        /// 【废弃】图片缩放后最小高度
        /// </summary>
        public int BlockFitMinHeight { get; set; } = 100;

        /// <summary>
        /// 容器宽度
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 容器高度
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 容错高度差
        /// </summary>
        internal const int LIMIT_DIFF_HEIGHT = 30;

        /// <summary>
        /// 允许最小误差百分比 默认0.02，与容器高度
        /// </summary>
        public double HeightDeviationRatio { get; set; } = 0.02;
        /// <summary>
        /// 允许最小误差百分比 默认0.02，与容器宽度
        /// </summary>
        public double WidthDeviationRatio { get; set; } = 0.02;

        /// <summary>
        /// 缩放后允许最小宽度 比例 默认0.125
        /// </summary>
        public double ScaleMinWidthRatio { get; set; } = 0.125;
        /// <summary>
        /// 缩放后允许最小高度 比例 默认0.1
        /// </summary>
        public double ScaleMinHeightRatio { get; set; } = 0.1;

        /// <summary>
        /// 剩余高度 比例 低于这个就直接当作当页已适配完 默认0.15
        /// </summary>
        public double LeftMinHeightRatio { get; set; } = 0.15;

        /// <summary>
        /// 长图比例5倍 即 5：1
        /// </summary>
        public int LongPicRatioTimes { get; set; } = 5;

        /// <summary>
        /// 剩余多少高度时，允许再次进入适配（默认200）
        /// </summary>
        public int LeftAllowFitHeight { get; set; } = 200;
    }
}
