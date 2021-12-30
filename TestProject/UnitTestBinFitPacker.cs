using BinPacking;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace TestProject
{
    [TestClass]
    public class UnitTestBinFitPacker
    {
        const int CONTAINER_WIDTH = 972;
        const int CONTAINER_HEIGHT = 1172;

        private BinFitPacker _BinFitPacker = null;
        private BinFitPacker BinFitPacker
        {
            get
            {
                if (_BinFitPacker == null)
                {
                    _BinFitPacker = new BinFitPacker(new PackerOptions { Width = CONTAINER_WIDTH, Height = CONTAINER_HEIGHT });
                }

                return _BinFitPacker;
            }
            set
            {
                _BinFitPacker = value;
            }
        }

        [TestMethod]
        public void TestDemo()
        {
            //3个刚好齐平
            var temps = new List<Block>
            {
                 new Block (486,1172),
                 new Block (486,586),
                 new Block (486,586),
                 new Block (486,586),
            };

            BinFitPacker.InitInfo().Fit(temps);

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 0);

            Assert.IsTrue(BinFitPacker.LeftHeight == 0, "齐平LeftHeight,h:" + BinFitPacker.LeftHeight);
            Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 1, "LeftBlocks 不是 1，count:" + BinFitPacker.LeftBlocks.Count);
        }

        [TestMethod]
        public void TestDemo1()
        {
            //5个刚好齐平，且有0.02比例以内误差
            var temps = new List<Block>
            {
                 new Block (486,586),
                 new Block (486,390.66),
                 new Block (486,390.66),
                 new Block (486,586),
                 new Block (486,390.66),
                 new Block (486,390.66),
            };

            BinFitPacker.InitInfo().Fit(temps);

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 1);

            Assert.IsTrue(BinFitPacker.LeftHeight == 0, "齐平LeftHeight,h:" + BinFitPacker.LeftHeight);
            Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 1, "LeftBlocks 不是 1，count:" + BinFitPacker.LeftBlocks.Count);
        }

        [TestMethod]
        public void TestDemo2()
        {
            //第一张是超长图
            var temps = new List<Block>
            {
                 new Block (200,1172),
                 new Block (486,586),
            };

            BinFitPacker.InitInfo().Fit(temps);

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 2);

            Assert.IsTrue(BinFitPacker.LeftHeight == 0, "齐平LeftHeight,h:" + BinFitPacker.LeftHeight);
            Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 1, "LeftBlocks 不是 1，count:" + BinFitPacker.LeftBlocks.Count);
        }


        [TestMethod]
        public void TestDemo3()
        {
            //单张图 比例与容器不一致
            var temps = new List<Block>
            {
                 new Block (486,986),
            };

            BinFitPacker.InitInfo().Fit(temps);

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 3);

            if (temps[0].W == CONTAINER_WIDTH)
            {
                Assert.IsTrue(BinFitPacker.LeftHeight != 0, "[单图宽度顶格]齐平LeftHeight");
            }
            else if (temps[0].H == CONTAINER_HEIGHT)
            {
                Assert.IsTrue(BinFitPacker.LeftHeight == 0, "[单图高度顶格]齐平LeftHeight");
            }
            Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 0, "LeftBlocks 不是 0，count:" + BinFitPacker.LeftBlocks.Count);
        }

        [TestMethod]
        public void TestDemo4()
        {
            //单张图 比例与容器一致
            var temps = new List<Block>
            {
                 new Block (486,586),
            };

            BinFitPacker.InitInfo().Fit(temps);

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 4);

            Assert.IsTrue(BinFitPacker.LeftHeight == 0, "齐平LeftHeight,h:" + BinFitPacker.LeftHeight);
            Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 0, "LeftBlocks 不是 0，count:" + BinFitPacker.LeftBlocks.Count);
        }

        [TestMethod]
        public void TestDemo5()
        {
            //4个比例一样竖图
            var temps = new List<Block>
            {
                 new Block (486,586),
                 new Block (486,586),
                 new Block (486,586),
                 new Block (486,586),
            };

            BinFitPacker.InitInfo().Fit(temps);


            //Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(temps));

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 5);

            Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 0, "LeftBlocks 不是 0，count:" + BinFitPacker.LeftBlocks.Count);
        }



        [TestMethod]
        public void TestDemo6()
        {
            //4个比例一样竖图 并两个剩余
            var temps = new List<Block>
            {
                 new Block (486,786),
                 new Block (486,786),
                 new Block (486,786),
                 new Block (486,786),
            };

            BinFitPacker.InitInfo().Fit(temps);


            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 6);

            Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 2, "LeftBlocks 不是 0，count:" + BinFitPacker.LeftBlocks.Count);
            Assert.IsTrue(BinFitPacker.LeftHeight > 0, "LeftHeight 应>0，LeftHeight:" + BinFitPacker.LeftHeight);
        }


        [TestMethod]
        public void TestDemo7()
        {
            //4个比例一样横图
            var temps = new List<Block>
            {
                 new Block (786,326),
                 new Block (786,326),
                 new Block (786,326),
                 new Block (786,326),
            };

            BinFitPacker.InitInfo().Fit(temps);

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 7);

            Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 0, "LeftBlocks 不是 0，count:" + BinFitPacker.LeftBlocks.Count);
        }


        [TestMethod]
        public void TestDemo8()
        {
            //4个比例一样横图
            var temps = new List<Block>
            {
                 new Block (972,391),
                 new Block (972,391),
                 new Block (972,391),
                 new Block (972,391),
            };
            BinFitPacker = new BinFitPacker(new PackerOptions { Width = 972, Height = 977 });
            BinFitPacker.InitInfo().Fit(temps);

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 8);

            Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 0, "LeftBlocks 不是 0，count:" + BinFitPacker.LeftBlocks.Count);
        }

        [TestMethod]
        public void TestDemo9()
        {
            //3个正方形
            var temps = new List<Block>
            {
                 new Block (391,391),
                 new Block (391,391),
                 new Block (391,391),
            };
            BinFitPacker.InitInfo().Fit(temps);

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 9);

            Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 0, "LeftBlocks 不是 0，count:" + BinFitPacker.LeftBlocks.Count);
        }

        [TestMethod]
        public void TestDemo10()
        {
            //3个正方形 容器刚好
            var temps = new List<Block>
            {
                 new Block (391,391),
                 new Block (391,391),
                 new Block (391,391),
            };

            BinFitPacker = new BinFitPacker(new PackerOptions { Width = 972, Height = 648 });
            BinFitPacker.InitInfo().Fit(temps);

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 10);

            Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 0, "LeftBlocks 不是 0，count:" + BinFitPacker.LeftBlocks.Count);
        }


        [TestMethod]
        public void TestDemo11()
        {
            //3个正方形 容器较小 只能适配两个
            var temps = new List<Block>
            {
                 new Block (391,391),
                 new Block (391,391),
                 new Block (391,391),
            };

            BinFitPacker = new BinFitPacker(new PackerOptions { Width = 972, Height = 390 });
            BinFitPacker.InitInfo().Fit(temps);

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 11);

            Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 1, "LeftBlocks 不是 3，count:" + BinFitPacker.LeftBlocks.Count);
        }


        [TestMethod]
        public void TestDemo12()
        {
            //3个横图 ，空间够
            var temps = new List<Block>
            {
                 new Block (800,491),
                 new Block (800,491),
                 new Block (800,491),
            };

            BinFitPacker.InitInfo().Fit(temps);

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 12);

            Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 0, "LeftBlocks 不是0，count:" + BinFitPacker.LeftBlocks.Count);
        }


        [TestMethod]
        public void TestDemo13()
        {
            //2个横图
            var temps = new List<Block>
            {
                 new Block (972,391),
                 new Block (972,391),
            };

            BinFitPacker.InitInfo().Fit(temps);

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 13);

            Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 0, "LeftBlocks 不是 0，count:" + BinFitPacker.LeftBlocks.Count);
        }

        [TestMethod]
        public void TestDemo14()
        {
            //2个正方形
            var temps = new List<Block>
            {
                 new Block (391,391),
                 new Block (391,391),
            };

            BinFitPacker.InitInfo().Fit(temps);

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 14);

            Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 1, "LeftBlocks 不是 1，count:" + BinFitPacker.LeftBlocks.Count);
        }


        [TestMethod]
        public void TestDemo15()
        {
            //2个竖图
            var temps = new List<Block>
            {
                 new Block (391,972),
                 new Block (391,972),
            };

            BinFitPacker.InitInfo().Fit(temps);

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 15);

            Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 0, "LeftBlocks 不是，count:" + BinFitPacker.LeftBlocks.Count);
        }


        [TestMethod]
        public void TestDemo16()
        {
            //2个长竖图 会超出
            var temps = new List<Block>
            {
                 new Block (221,972),
                 new Block (221,972),
            };

            BinFitPacker.InitInfo().Fit(temps);

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 16);

            Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 0, "LeftBlocks 不是，count:" + BinFitPacker.LeftBlocks.Count);
        }

        [TestMethod]
        public void TestDemo17()
        {
            // 剩余空间 刚好fit
            var temps = new List<Block>
            {
                 new Block (400,400),
                 new Block (400,400),
            };

            BinFitPacker.InitInfo().Fit(temps);

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 17);

            Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 1, "LeftBlocks 不是，count:" + BinFitPacker.LeftBlocks.Count);
        }



        [TestMethod]
        public void TestDemo20()
        {
            // 3大图横图
            var temps = new List<Block>
            {
                new Block (4032,3024),
                new Block (4032,3024),
                new Block (4032,3024),
            };

            BinFitPacker.InitInfo().Fit(temps);

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 20);

            Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 0, "LeftBlocks 不是，count:" + BinFitPacker.LeftBlocks.Count);
            Assert.IsTrue(BinFitPacker.LeftHeight == 78.5, "LeftHeight 应该 ==78.5 :" + BinFitPacker.LeftHeight);
        }



        [TestMethod]
        public void TestDemo21()
        {
            // 多图
            var temps = new List<Block>
            {
                 new Block (744,992),
                 new Block (744,992),
                 new Block (593,691),
                 new Block (750,1116),
                 new Block (750,1000),
                 new Block (750,1000),
                 new Block (750,1211),
            };

            BinFitPacker.InitInfo().Fit(temps);

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 21);

            Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 5, "LeftBlocks 不是，count:" + BinFitPacker.LeftBlocks.Count);
            Assert.IsTrue(BinFitPacker.LeftHeight == 524, "LeftHeight 不应该 ==0 :" + BinFitPacker.LeftHeight);
        }


        [TestMethod]
        public void TestDemo22()
        {
            // 多图
            var temps = new List<Block>
            {
                 new Block (750,1000),
                 new Block (600,600),
                 new Block (593,691),
                 new Block (1920,1200),
            };

            BinFitPacker.InitInfo().Fit(temps);

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 22);

            Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 0, "LeftBlocks 不是，count:" + BinFitPacker.LeftBlocks.Count);
            Assert.IsTrue((int)BinFitPacker.LeftHeight == 221, "LeftHeight 不应该 ==0 :" + BinFitPacker.LeftHeight);
        }


        [TestMethod]
        public void TestDemo23()
        {
            // 多图
            var temps = new List<Block>
            {
                 new Block (600,600),
                 new Block (600,600),
                 new Block (450,525),
                 new Block (380,416),
                 new Block (750,1000),
                 new Block (600,600),
            };

            BinFitPacker.InitInfo().Fit(temps);

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 23);

            Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 2, "LeftBlocks 不是，count:" + BinFitPacker.LeftBlocks.Count);
            Assert.IsTrue(BinFitPacker.LeftHeight > 0, "LeftHeight 不应该 ==0 :" + BinFitPacker.LeftHeight);
        }



        [TestMethod]
        public void TestDemo24()
        {
            // 多图
            var temps = new List<Block>
            {
                 new Block (756,568),
                 new Block (750,1000),
                 new Block (748,1000),
                 new Block (593,691),
                 new Block (744,992),
                 new Block (348,250),
                 new Block (1080,1440),
                 new Block (2880,3840),
            };

            BinFitPacker.InitInfo().Fit(temps);

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 24);

            Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 4, "LeftBlocks 不是，count:" + BinFitPacker.LeftBlocks.Count);
            Assert.IsTrue(BinFitPacker.LeftHeight > 0, "LeftHeight 不应该 ==0 :" + BinFitPacker.LeftHeight);
        }




        [TestMethod]
        public void TestDemo25()
        {
            // 多图
            var temps = new List<Block>
            {
                 new Block (748,1000),
                 new Block (748,1000),
                 new Block (744,992),
                 new Block (750,1000),
            };

            BinFitPacker.InitInfo().Fit(temps);

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 25);

            Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 2, "LeftBlocks 不是，count:" + BinFitPacker.LeftBlocks.Count);
            Assert.IsTrue(BinFitPacker.LeftHeight > 0, "LeftHeight 不应该 ==0 :" + BinFitPacker.LeftHeight);
        }



        [TestMethod]
        public void TestDemo26()
        {
            // 多图
            var temps = new List<Block>
            {
                 new Block (748,1000),
                 new Block (593,691),
                 new Block (744,992),
                 new Block (348,250),
                 new Block (748,1000),
                 new Block (748,1000),
            };

            BinFitPacker.InitInfo().Fit(temps);

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 26);

            Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 2, "LeftBlocks 不是，count:" + BinFitPacker.LeftBlocks.Count);
            Assert.IsTrue(BinFitPacker.LeftHeight > 0, "LeftHeight 不应该 ==0 :" + BinFitPacker.LeftHeight);
        }


        [TestMethod]
        public void TestDemo27()
        {
            // 多图
            var temps = new List<Block>
            {
                 new Block (744,992),
                 new Block (744,992),
                 new Block (756,568),
                 new Block (750,1000),
                 new Block (748,1000),
                 new Block (593,691),
                 new Block (744,992),
                 new Block (348,250),
                 new Block (748,1000),
                 new Block (748,1000),
                 new Block (744,992),
            };

            BinFitPacker.InitInfo().Fit(temps);

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 27);

            Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 7, "LeftBlocks 不是，count:" + BinFitPacker.LeftBlocks.Count);
            Assert.IsTrue(BinFitPacker.LeftHeight > 0, "LeftHeight 不应该 ==0 :" + BinFitPacker.LeftHeight);
        }


        //[TestMethod]
        //public void TestDemo28()
        //{
        //    // 多图 多次适 剩余情况 结果：3方+2方组合  3上2下
        //    var temps = new List<Block>
        //    {
        //         new Block (480,480),
        //         new Block (600,600),
        //         new Block (512,512),
        //         new Block (600,601),
        //         new Block (750,750),
        //         new Block (550,1138),
        //    };

        //    BinFitPacker.InitInfo().Fit(temps);

        //    var leftBlocks = BinFitPacker.LeftBlocks;

        //    BinFitPacker.ResetLeftFitOptions(new PackerOptions { Width = BinFitPacker.PackerOptions.Width, Height = (int)BinFitPacker.LeftHeight }).FitLeft(leftBlocks);

        //    BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 28);
        //    Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 1, "LeftBlocks 不是，count:" + BinFitPacker.LeftBlocks.Count);
        //    Assert.IsTrue(BinFitPacker.LeftHeight < 40, "LeftHeight 应<40 :" + BinFitPacker.LeftHeight);
        //}



        [TestMethod]
        public void TestDemo30()
        {
            // 多图 刚好齐平 与demo1 得比例2倍
            var temps = new List<Block>
            {
                new Block (972,1172),
                new Block (972,781.32),
                new Block (972,781.32),
                new Block (972,1172),
                new Block (972,781.32),
            };

            BinFitPacker.InitInfo().Fit(temps);

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 30);

            Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 0, "LeftBlocks 不是，count:" + BinFitPacker.LeftBlocks.Count);
            Assert.IsTrue(BinFitPacker.LeftHeight == 0, "LeftHeight 应该 ==0 :" + BinFitPacker.LeftHeight);
        }


        [TestMethod]
        public void TestDemo31()
        {
            // 多图
            var temps = new List<Block>
            {
                new Block (456,342.912),
                new Block (456,335.008),
                new Block (456.00000006,456.00000006),
                new Block (456.0000006,363.9338014),
                new Block (456,308.86438614752217),
                new Block (456.00000006,386.15649766),
            };

            BinFitPacker.InitInfo().Fit(temps);

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 31);
            Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 0, "LeftBlocks 不是，count:" + BinFitPacker.LeftBlocks.Count);
            Assert.IsTrue((int)BinFitPacker.LeftHeight == 13, "LeftHeight 应该约等于13:" + BinFitPacker.LeftHeight);
        }


        [TestMethod]
        public void TestDemo32()
        {
            // 多图  第2、3张包含两个超长横图
            var temps = new List<Block>
            {
                 new Block (680,480),
                 new Block (600,60),
                 new Block (600,60),
                 new Block (512,512),
                 new Block (600,601),
            };

            BinFitPacker.InitInfo().Fit(temps);

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 32);
            Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 2, "LeftBlocks 不是，count:" + BinFitPacker.LeftBlocks.Count);
            Assert.IsTrue(BinFitPacker.LeftHeight > 0, "LeftHeight 不应该 ==0 :" + BinFitPacker.LeftHeight);
        }


        [TestMethod]
        public void TestDemo33()
        {
            // 多图  第二张包含一张超长竖图
            var temps = new List<Block>
            {
                 new Block (680,480),
                 new Block (60,600),
                 new Block (512,512),
            };

            BinFitPacker.InitInfo().Fit(temps);

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 33);
            Assert.IsTrue(BinFitPacker.LeftBlocks.Count == 2, "LeftBlocks 不是，count:" + BinFitPacker.LeftBlocks.Count);
            Assert.IsTrue(BinFitPacker.LeftHeight > 0, "LeftHeight 不应该 ==0 :" + BinFitPacker.LeftHeight);
        }


        [TestMethod]
        public void TestDemo34()
        {
            // 多图  第二张包含一张超长竖图
            var temps = new List<Block>
            {
                 new Block (1080,1440),
                 new Block (3024,4032),
                 new Block (1080,1440),
                 new Block (3630,4840),
                 new Block (3068,5454),
                 new Block (1080,1920),
            };

            BinFitPacker.InitInfo().Fit(temps);

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 34);
        }


        [TestMethod]
        public void TestDemo35()
        {
            // 多图 一张正常长图，一张超长图，导致没有任何匹配结果，超出容器（应每个图独立一页）  
            var temps = new List<Block>
            {
                 new Block (1080,2340),
                 new Block (1080,4099),
            };

            BinFitPacker.InitInfo().Fit(temps);

            //居中显示
            temps[0].Fit.X = (BinFitPackerDrawHelper.border + (BinFitPacker.PackerOptions.Width - temps[0].Fit.W - BinFitPackerDrawHelper.border * 2) / 2);

            BinFitPackerDrawHelper.Draw(BinFitPacker.SourcePackerOptions.Width, BinFitPacker.SourcePackerOptions.Height, temps, 35);
        }
    }
}