﻿using System.Collections.Generic;
using Glass.Design.Pcl.Canvas;
using Glass.Design.Pcl.DesignSurface.VisualAids.Snapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{

    [TestClass]
    public class SnappingTest
    {
       
        [TestMethod]
        public void MyTestMethod()
        {
            var snappingEngine = new CanvasItemSnappingEngine(8);
            var canvasItems = new List<CanvasItem>
                              {
                                  new CanvasModelItem
                                  {
                                      Left = 10,
                                      Top = 10,
                                      Height = 20,
                                      Width = 20,
                                  },
                                  new CanvasModelItem
                                  {
                                      Left = 33,
                                      Top = 10,
                                      Height = 20,
                                      Width = 20,
                                  }
                              };


            snappingEngine.Magnets = canvasItems;
        }
    }
}