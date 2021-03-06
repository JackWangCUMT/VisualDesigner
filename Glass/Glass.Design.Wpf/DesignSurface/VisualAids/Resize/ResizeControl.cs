﻿using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AutoMapper;
using Glass.Basics.Wpf.Extensions;
using Glass.Design.Pcl;
using Glass.Design.Pcl.Canvas;
using Glass.Design.Pcl.Core;
using Glass.Design.Pcl.DesignSurface.VisualAids.Resize;
using Glass.Design.Pcl.DesignSurface.VisualAids.Snapping;
using Glass.Design.Pcl.PlatformAbstraction;
using Glass.Design.Wpf.PlatformSpecific;
using Rect = Glass.Design.Pcl.Core.Rect;

namespace Glass.Design.Wpf.DesignSurface.VisualAids.Resize
{
    public sealed class ResizeControl : Control, IUIElement
    {
        public IEdgeSnappingEngine SnappingEngine { get; set; }

        static ResizeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeControl), new FrameworkPropertyMetadata(typeof(ResizeControl)));
        }

        public ResizeControl(ICanvasItem itemToResize, IUserInputReceiver parent, IEdgeSnappingEngine snappingEngine)
        {
            SnappingEngine = snappingEngine;
            FrameOfReference = parent;
            CanvasItem = itemToResize;
        }

        private WindowsSizeCursorsThumbCursorConverter WindowsSizeCursorsThumbCursorConverter { get; set; }
        private UIResizeOperationHandleConnector UIResizeOperationHandleConnector { get; set; }

        #region CreateHostingItem

        public static readonly DependencyProperty CanvasItemProperty =
            DependencyProperty.Register("CreateHostingItem", typeof(ICanvasItem), typeof(ResizeControl),
                new FrameworkPropertyMetadata(null, OnCanvasItemChanged));

        public ICanvasItem CanvasItem
        {
            get { return (ICanvasItem)GetValue(CanvasItemProperty); }
            set { SetValue(CanvasItemProperty, value); }
        }

        private static void OnCanvasItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ResizeControl)d;
            var oldCanvasItem = (ICanvasItem)e.OldValue;
            var newCanvasItem = target.CanvasItem;
            target.OnCanvasItemChanged(oldCanvasItem, newCanvasItem);
        }

        private void OnCanvasItemChanged(ICanvasItem oldCanvasItem, ICanvasItem newCanvasItem)
        {
            if (IsLoaded)
            {
                RegisterHandles();
            }
            else
            {
                Loaded += OnLoaded;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            RegisterHandles();
            Loaded -= OnLoaded;
        }

        private void RegisterHandles()
        {
            UIResizeOperationHandleConnector = new UIResizeOperationHandleConnector(CanvasItem, FrameOfReference, SnappingEngine);
            WindowsSizeCursorsThumbCursorConverter = new WindowsSizeCursorsThumbCursorConverter();

            var thumbContainer = (UIElement)Template.FindName("PART_ThumbContainer", this);


            var enumerable = LogicalTreeHelper.GetChildren(thumbContainer);
            var logicalChildren = enumerable.OfType<FrameworkElement>();
            foreach (var logicalChild in logicalChildren)
            {
                var rectRelativeToParent = Mapper.Map<Rect>(logicalChild.GetRectRelativeToParent());
                var childRect = rectRelativeToParent;

                var parentRect = Mapper.Map<Rect>(CanvasItem.Rect());

                var handlePoint = childRect.GetHandlePoint(parentRect.Size);

                var uiElement = new UIElementAdapter(logicalChild);
                UIResizeOperationHandleConnector.RegisterHandle(uiElement, handlePoint);
                SetCursorToHandle(logicalChild);
            }
        }

        private void SetCursorToHandle(FrameworkElement handle)
        {
            var handleRect = Mapper.Map<Rect>(handle.GetRectRelativeToParent());
            var parentRect = new Rect(0, 0, ActualWidth, ActualHeight);
            handle.Cursor = WindowsSizeCursorsThumbCursorConverter.GetCursor(handleRect, parentRect);
        }

        #endregion

        #region FrameOfReference

        public static readonly DependencyProperty FrameOfReferenceProperty =
            DependencyProperty.Register("FrameOfReference", typeof(IUserInputReceiver), typeof(ResizeControl),
                new FrameworkPropertyMetadata(null, OnFrameOfReferenceChanged));

        public IUserInputReceiver FrameOfReference
        {
            get { return (IUserInputReceiver)GetValue(FrameOfReferenceProperty); }
            set { SetValue(FrameOfReferenceProperty, value); }
        }

        private static void OnFrameOfReferenceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ResizeControl)d;
            var oldFrameOfReference = (IUserInputReceiver)e.OldValue;
            var newFrameOfReference = target.FrameOfReference;
            target.OnFrameOfReferenceChanged(oldFrameOfReference, newFrameOfReference);
        }

        protected void OnFrameOfReferenceChanged(IUserInputReceiver oldFrameOfReference, IUserInputReceiver newFrameOfReference)
        {
            if (IsLoaded)
            {
                RegisterHandles();
            }
            else
            {
                Loaded += OnLoaded;
            }
        }

        #endregion

        public event FingerManipulationEventHandler FingerDown;
        public event FingerManipulationEventHandler FingerMove;
        public event FingerManipulationEventHandler FingerUp;
        public void CaptureInput(object pointer)
        {
            throw new System.NotImplementedException();
        }

        public void ReleaseInput(object pointer)
        {
            throw new System.NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public double GetCoordinate(CoordinatePart part)
        {
            throw new System.NotImplementedException();
        }

        public void SetCoordinate(CoordinatePart part, double value)
        {
            throw new System.NotImplementedException();
        }

        public double Left { get; set; }
        public double Top { get; set; }
        public CanvasItemCollection Children { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }
        public ICanvasItemContainer Parent { get; set; }
        public void AddAdorner(IAdorner adorner)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveAdorner(IAdorner adorner)
        {
            throw new System.NotImplementedException();
        }

        public bool IsVisible { get; set; }
        public object GetCoreInstance()
        {
            return this;
        }
    }
}
