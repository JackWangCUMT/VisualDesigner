﻿using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Glass.Design.Pcl.Canvas;
using Glass.Design.Pcl.Core;
using Glass.Design.Pcl.DesignSurface;
using Glass.Design.Pcl.DesignSurface.VisualAids.Selection;
using Glass.Design.Pcl.PlatformAbstraction;
using PostSharp.Patterns.Model;

using FoundationPoint = Windows.Foundation.Point;

namespace Glass.Design.WinRT.DesignSurface
{
    [NotifyPropertyChanged]
    public sealed class DesignSurface : ItemsControl, IDesignSurface
    {

        public static readonly DependencyProperty CanvasDocumentProperty = DependencyProperty.Register("CanvasDocument",
            typeof(ICanvasItemContainer), typeof(DesignSurface), new PropertyMetadata(null, OnCanvasDocumentChanged));

        private static void OnCanvasDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DesignSurface designSurface = ((DesignSurface)d);
            if (e.NewValue != null)
            {
                designSurface.ItemsSource = ((ICanvasItemContainer)e.NewValue).Children;
            }
            else
            {
                designSurface.ItemsSource = null;
            }
        }

        public DesignSurface()
        {
            this.DefaultStyleKey = typeof(DesignSurface);
            SelectedItems = new ObservableCollection<ICanvasItem>();

            PointerPressed += OnPointerPressed;
            ((INotifyCollectionChanged)SelectedItems).CollectionChanged += OnCollectionChanged;
            DesignAidsProvider = new DesignAidsProvider(this);
            SelectionHandler = new SelectionHandler(this);
            CommandHandler = new DesignSurfaceCommandHandler(this, this);            
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            OnSelectionChanged(notifyCollectionChangedEventArgs);
        }

        private DesignSurfaceCommandHandler CommandHandler { get; set; }


        private SelectionHandler SelectionHandler { get; set; }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
        {
            RaiseNoneSpecified();
        }

        private DesignAidsProvider DesignAidsProvider { get; set; }

        [IgnoreAutoChangeNotification]
        public ICanvasItemContainer CanvasDocument
        {
            get { return (ICanvasItemContainer)GetValue(CanvasDocumentProperty); }
            set { SetValue(CanvasDocumentProperty, value); }
        }


        private void OnSelectionChanged(NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            var newItems = notifyCollectionChangedEventArgs.NewItems;
            var removedItems = notifyCollectionChangedEventArgs.OldItems;

            if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Reset)
            {
                
            }

            else if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ICanvasItem newItem in newItems)
                {
                    DesignAidsProvider.AddItemToSelection(newItem);
                }
            } else if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ICanvasItem removedItem in removedItems)
                {
                    DesignAidsProvider.RemoveItemFromSelection(removedItem);
                }
            }                          
        }

        private void ContainerOnLeftButtonDown(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
        {
            var item = ItemContainerGenerator.ItemFromContainer((DependencyObject)sender);
            OnItemSelected(item);
            pointerRoutedEventArgs.Handled = true;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            var designerItem = (CanvasItemControl)element;
            designerItem.PointerPressed += ContainerOnLeftButtonDown;
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
            var designerItem = (CanvasItemControl)element;
            designerItem.PointerPressed -= ContainerOnLeftButtonDown;
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is CanvasItemControl;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CanvasItemControl();
        }

        #region PlaneOperationMode

        public static readonly DependencyProperty PlaneOperationModeProperty =
            DependencyProperty.Register("PlanePlaneOperationMode", typeof(PlaneOperation), typeof(DesignSurface),
                new PropertyMetadata(PlaneOperation.Resize,
                    OnOperationModeChanged));

        private readonly DesignSurfaceCommandHandler designSurfaceCommandHandler;
        private ICanvasItem _rootCanvasItem;
        private CanvasItemCollection children;

        [IgnoreAutoChangeNotification]
        public PlaneOperation PlaneOperationMode
        {
            get { return (PlaneOperation)GetValue(PlaneOperationModeProperty); }
            set { SetValue(PlaneOperationModeProperty, value); }
        }

        private static void OnOperationModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (DesignSurface)d;
            var oldOperationMode = (PlaneOperation)e.OldValue;
            var newOperationMode = target.PlaneOperationMode;
            target.OnOperationModeChanged(oldOperationMode, newOperationMode);
        }

        private void OnOperationModeChanged(PlaneOperation oldOperationMode, PlaneOperation newOperationMode)
        {
            DesignAidsProvider.PlaneOperation = newOperationMode;
        }

        #endregion

        public IList SelectedItems { get; private set; }
        public event EventHandler<object> ItemSpecified;

        private void OnItemSelected(object e)
        {
            this.LastSelectedItem = e;

            var handler = ItemSpecified;
            if (handler != null) handler(this, e);
        }

        public object LastSelectedItem { get; private set; }

        public event EventHandler SelectionCleared;
        public void UnselectAll()
        {
            SelectedItems.Clear();
        }

        private void RaiseNoneSpecified()
        {
            this.LastSelectedItem = null;

            var handler = SelectionCleared;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        //protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        //{
        //    base.OnPreviewMouseLeftButtonDown(e);
        //    Focus();
            
        //    OnFingerDown(new FingerManipulationEventArgs());
        //}

        //protected override void OnMouseMove(MouseEventArgs e)
        //{
        //    base.OnMouseMove(e);
          
        //    OnFingerMove(new FingerManipulationEventArgs());
        //}

        //protected override void OnPreviewKeyDown(KeyEventArgs e)
        //{
        //    OnKeyDown(e);
        //    if (e.Key == Key.LeftCtrl)
        //    {
        //        SelectionHandler.SelectionMode = SelectionMode.Add;
        //    }
        //}

        //protected override void OnPreviewKeyUp(KeyEventArgs e)
        //{
        //    base.OnKeyUp(e);
        //    if (e.Key == Key.LeftCtrl)
        //    {
        //        SelectionHandler.SelectionMode = SelectionMode.Direct;
        //    }
        //}


        public ICommand GroupCommand { get; private set; }

        //[UsedImplicitly]
        //public DesignSurfaceCommandHandler DesignSurfaceCommandHandler
        //{
        //    get { return designSurfaceCommandHandler; }
        //}


        public event FingerManipulationEventHandler FingerDown;

        private void OnFingerDown(FingerManipulationEventArgs args)
        {
            var handler = FingerDown;
            if (handler != null) handler(this, args);
        }

        public event FingerManipulationEventHandler FingerMove;

        private void OnFingerMove(FingerManipulationEventArgs args)
        {
            var handler = FingerMove;
            if (handler != null) handler(this, args);
        }

        public event FingerManipulationEventHandler FingerUp;

        private void OnFingerUp(FingerManipulationEventArgs args)
        {
            var handler = FingerUp;
            if (handler != null) handler(this, args);
        }

        public void CaptureInput()
        {
            //CapturePointer(new Pointer());
        }

        public void ReleaseInput()
        {
            //ReleasePointerCaptures();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public double GetCoordinate(CoordinatePart part)
        {
            throw new NotImplementedException();
        }

        void ICoordinate.SetCoordinate(CoordinatePart part, double value)
        {
            throw new NotImplementedException();
        }

        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                RaisePropertyChanged(propertyName);
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        double IPositionable.Left { get; set; }
        double IPositionable.Top { get; set; }

        public CanvasItemCollection Children
        {
            get { return children; }            
        }

        public double Right { get; private set; }
        public double Bottom { get; private set; }
        public ICanvasItemContainer Parent { get; private set; }

        public void AddAdorner(IAdorner adorner)
        {
            var popUp = new Popup();
           
            
            var coreInstance = (UIElement) adorner.GetCoreInstance();

            popUp.Child = coreInstance;

            var transform = coreInstance.TransformToVisual(Window.Current.Content);
            var point = transform.TransformPoint(new FoundationPoint(0, 0));
            popUp.HorizontalOffset = adorner.Left;
            popUp.VerticalOffset = adorner.Top;
            popUp.IsOpen = true;
            //var coreInstance = (Visual) GetCoreInstance();
            //var adornerLayer = AdornerLayer.GetAdornerLayer(coreInstance);
            //adornerLayer.Add((Adorner) adorner);
        }

        public void RemoveAdorner(IAdorner adorner)
        {
            //var coreInstance = (Visual)GetCoreInstance();
            //var adornerLayer = AdornerLayer.GetAdornerLayer(coreInstance);
            //adornerLayer.Remove((Adorner) adorner);
        }

        bool IUIElement.IsVisible { get; set; }
        public object GetCoreInstance()
        {
            return this;
        }


        //protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        //{
        //    base.OnPreviewMouseLeftButtonUp(e);

        //    var point = e.GetPosition(null);
        //    var pclPoint = Mapper.Map<Point>(point);
        //    OnFingerUp(new FingerManipulationEventArgs());
        //}
    }
}
