using BinPacking;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace TestProject
{
    internal class BinFitPackerDrawHelper
    {

        static Color[] colors = { Color.AliceBlue, Color.Aqua, Color.Yellow, Color.LemonChiffon, Color.Orchid, Color.OrangeRed, Color.Aquamarine, Color.Black, Color.BlanchedAlmond, Color.Blue, Color.BlueViolet, Color.Brown, Color.BurlyWood, Color.CadetBlue, Color.Chartreuse, Color.Chocolate, Color.Coral, Color.Coral, Color.CornflowerBlue, Color.Cornsilk, Color.Crimson, Color.Cyan, Color.Firebrick, Color.MintCream, Color.Silver, Color.Pink, Color.LightBlue, Color.Green, Color.FloralWhite, Color.Gold, Color.Azure, Color.Beige, Color.Bisque, Color.MistyRose, Color.PaleVioletRed, Color.PeachPuff, Color.SandyBrown, Color.Indigo, Color.PaleGreen };

        private static Color GetColor(int n)
        {
            return colors[n % colors.Length];
        }

        internal const int border = 10;
        internal static void Draw(int w1, int h1, List<Block> nodes, int groupIndex)
        {
            int width = w1;
            int height = h1;

            using (Bitmap bmp = new Bitmap(width + 1, height + 1))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawRectangle(
                        pen: new Pen(
                            color: GetColor(20),
                         width: 2
                        ),
                        rect: new Rectangle(0, 0, width, height)
                    );

                    g.DrawString($"(w{width},h{height})", new Font(FontFamily.GenericSansSerif, 18), new SolidBrush(color: Color.Red), width - 300, height - 300);

                    for (int i = 0; i < nodes.Count; i++)
                    {
                        if (nodes[i].Fit == null)
                        {
                            continue;
                        }

                        g.FillRectangle(
                            brush: new SolidBrush(
                                color: GetColor(i)
                            ),
                            rect: new Rectangle((int)nodes[i].Fit.X + 1, (int)nodes[i].Fit.Y + 1, (int)nodes[i].Fit.W - border, (int)nodes[i].Fit.H - border)//TODO：间距直接用图片缩放来实现
                        );

                        g.DrawRectangle(
                            pen: new Pen(
                                color: GetColor(i),
                             width: 1
                            ),
                            rect: new Rectangle((int)nodes[i].Fit.X + 1, (int)nodes[i].Fit.Y + 1, (int)nodes[i].Fit.W - border, (int)nodes[i].Fit.H - border)
                        );

                        string str = $"[{i}]({nodes[i].Fit.X},{nodes[i].Fit.Y})(w:{(int)nodes[i].Fit.W - border},h:{(int)nodes[i].Fit.H - border})";
                        Console.WriteLine(str);
                        g.DrawString(str, new Font(FontFamily.GenericSansSerif, 18), new SolidBrush(color: Color.Black), (float)nodes[i].Fit.X, (float)nodes[i].Fit.Y);

                    }


                    bmp.Save($"result{groupIndex}.png", ImageFormat.Png);

                }
            }
        }

    }
}
