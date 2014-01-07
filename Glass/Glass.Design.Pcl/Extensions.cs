﻿using System.Collections.Generic;
using System.Linq;
using Glass.Design.Pcl.CanvasItem;
using Glass.Design.Pcl.Core;
using Glass.Design.Pcl.DesignSurface;

namespace Glass.Design.Pcl
{
    public static class Extensions
    {
        public static void SwapCoordinates(this IEnumerable<ICanvasItem> items)
        {
            foreach (var canvasItem in items)
            {
                canvasItem.SwapCoordinates();
            }
        }

        public static void SwapCoordinates(this ICanvasItem item)
        {
            var left = item.Left;
            var top = item.Top;
            Swap(ref left, ref top);
            var width = item.Width;
            var height = item.Height;
            Swap(ref width, ref height);

            item.Left = left;
            item.Top = top;
            item.Width = width;
            item.Height = height;
        }

        private static void Swap<T>(ref T a, ref T b)
        {
            var temp = a;
            a = b;
            b = temp;
        }

        public static IRect Rect(this ICanvasItem item)
        {
            return ServiceLocator.CoreTypesFactory.CreateRect(item.Left, item.Top, item.Width, item.Height);
        }

        public static IPoint MiddlePoint(this IRect item)
        {
            return ServiceLocator.CoreTypesFactory.CreatePoint(item.Left + item.Width / 2, item.Top + item.Height / 2);
        }

        public static IPoint GetLocation(this IPositionable positionable)
        {
            return ServiceLocator.CoreTypesFactory.CreatePoint(positionable.Left, positionable.Top);
        }

        public static ISize GetSize(this ISizable sizable)
        {
            return ServiceLocator.CoreTypesFactory.CreateSize(sizable.Width, sizable.Height);
        }

        public static void SetSize(this ISizable sizable, ISize size)
        {
            sizable.Width = size.Width;
            sizable.Height = size.Height;
        }

        public static void SetLocation(this IPositionable positionable, IPoint location)
        {
            positionable.Left = location.X;
            positionable.Top = location.Y;
        }

        public static void SetLocation(this IPositionable positionable, double x, double y)
        {
            positionable.Left = x;
            positionable.Top = y;
        }

        public static IPoint Subtract(this IPoint point, IVector vector)
        {
            return ServiceLocator.CoreTypesFactory.CreatePoint(point.X - vector.X, point.Y - vector.Y);
        }

        public static IPoint Subtract(this IPoint point, IPoint vector)
        {
            return ServiceLocator.CoreTypesFactory.CreatePoint(point.X - vector.X, point.Y - vector.Y);
        }

        public static IPoint Add(this IPoint point, IPoint vector)
        {
            return ServiceLocator.CoreTypesFactory.CreatePoint(point.X + vector.X, point.Y + vector.Y);
        }


        public static IPoint Add(this IPoint point, IVector vector)
        {
            return ServiceLocator.CoreTypesFactory.CreatePoint(point.X + vector.X, point.Y + vector.Y);
        }

        public static IPoint Swap(this IPoint point)
        {
            return ServiceLocator.CoreTypesFactory.CreatePoint(point.Y, point.X);
        }


        public static IPoint GetOpposite(this IPoint handlePosition, IPoint middlePoint)
        {
            var x = 2 * middlePoint.X - handlePosition.X;
            var y = 2 * middlePoint.Y - handlePosition.Y;

            return ServiceLocator.CoreTypesFactory.CreatePoint(x, y);
        }

        public static IPoint FromParentToLocal(this IPoint origin, IPoint destination)
        {
            var x = origin.X - destination.X;
            var y = origin.Y - destination.Y;
            return ServiceLocator.CoreTypesFactory.CreatePoint(x, y);
        }

        public static IPoint FromLocalToParent(this IPoint origin, IPoint destination)
        {
            var x = origin.X + destination.X;
            var y = origin.Y + destination.Y;
            return ServiceLocator.CoreTypesFactory.CreatePoint(x, y);
        }

        public static Range Swap(this Range range)
        {
            return new Range(range.SegmentEnd, range.SegmentStart);
        }

        public static IPoint GetHandlePoint(this IRect rect, ISize parentSize)
        {
            var middleThumb = rect.MiddlePoint();

            var propX = MathOperations.SquareRounding(middleThumb.X, parentSize.Width, 3) / 3D;
            var propY = MathOperations.SquareRounding(middleThumb.Y, parentSize.Height, 3) / 3D;

            return ServiceLocator.CoreTypesFactory.CreatePoint(propX, propY);
        }

        public static IVector ToVector(this ISize size)
        {
            return ServiceLocator.CoreTypesFactory.CreateVector(size.Width, size.Height);
        }

        public static IVector ToVector(this IPoint point)
        {
            return ServiceLocator.CoreTypesFactory.CreateVector(point.X, point.Y);
        }

        public static ISize ToSize(this IVector vector)
        {
            return ServiceLocator.CoreTypesFactory.CreateSize(vector.X, vector.Y);
        }

        public static IVector Subtract(this IVector vector, IVector another)
        {
            return ServiceLocator.CoreTypesFactory.CreateVector(vector.X - another.X, vector.Y - another.Y);
        }

        public static IVector Add(this IVector vector, IVector another)
        {
            return ServiceLocator.CoreTypesFactory.CreateVector(vector.X + another.X, vector.Y + another.Y);
        }

        public static void SetBounds(this ICanvasItem canvasItem, IRect rect)
        {
            canvasItem.SetLocation(rect.Left, rect.Top);
            canvasItem.SetSize(rect.Size);
        }

        public static void SetLeftKeepingRight(this IRect rect, double left)
        {
            var right = rect.Right;
            rect.X = left;
            rect.Width = right - left;
        }

        public static void SetTopKeepingBottom(this IRect rect, double top)
        {
            var bottom = rect.Bottom;
            rect.Y = top;
            rect.Height = bottom - top;
        }

        public static void SetRightKeepingLeft(this IRect rect, double right)
        {
            rect.Width = right - rect.Left;
        }

        public static void SetBottomKeepingTop(this IRect rect, double bottom)
        {
            rect.Height = bottom - rect.Top;
        }

        public static IRect GetBoundsFromChildren(IEnumerable<ICanvasItem> items)
        {
            var left = items.Min(item => item.Left);
            var top = items.Min(item => item.Top);
            var right = items.Max(item => item.Right);
            var bottom = items.Max(item => item.Bottom);

            return ServiceLocator.CoreTypesFactory.CreateRect(left, top, right - left, bottom - top);
        }

        public static void Offset(this ICanvasItem canvasItem, IPoint point)
        {
            canvasItem.Left += point.X;
            canvasItem.Top += point.Y;
        }

        public static IPoint Negative(this IPoint point)
        {
            return ServiceLocator.CoreTypesFactory.CreatePoint(-point.X, -point.Y);
        }
    }
}