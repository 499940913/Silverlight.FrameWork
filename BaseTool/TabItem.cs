
using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace BaseTool
{
    public class TabItem:System.Windows.Controls.TabItem
    {
        public TabItem()
        {
            DefaultStyleKey = typeof(TabItem);
            var viewModel = new TabCloseCommandViewModel(this);
            DataContext = viewModel;
        }

        public static readonly DependencyProperty UnFocusForegroundProperty =

       DependencyProperty.Register("UnFocusForeground", typeof(Brush), typeof(TabItem), null);
        public Brush UnFocusForeground
        {
            get { return (Brush)GetValue(UnFocusForegroundProperty); }

            set
            {
                SetValue(UnFocusForegroundProperty, value);
            }
        }


        public static readonly DependencyProperty FocusForegroundProperty =

      DependencyProperty.Register("FocusForeground", typeof(Brush), typeof(TabItem), null);
        public Brush FocusForeground
        {
            get { return (Brush)GetValue(FocusForegroundProperty); }

            set
            {
                SetValue(FocusForegroundProperty, value);
            }
        }

    }


    [TemplatePart(Name = "BtnShowList", Type = typeof(MacButton))]
    [TemplatePart(Name = "TabPanelTop", Type = typeof(TabPanel))]
    [TemplatePart(Name = "HeadList", Type = typeof(Popup))]
    [TemplatePart(Name = "HeadContent", Type = typeof(ListBox))]
    public class TabControl : System.Windows.Controls.TabControl
    {
        private static Button _btnShowList;
        private static TabPanel _itemPanel;
        private static Popup _headList;
        private static ListBox _headContent;
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private  bool _makePopupOpen;
        public TabControl()
        {
            DefaultStyleKey = typeof(TabControl);
            _makePopupOpen = true;
           
        }


        public static readonly DependencyProperty TabPanelTopBrushProperty =

         DependencyProperty.Register("TabPanelTopBrush", typeof(Brush), typeof(TabControl), null);
        public Brush TabPanelTopBrush
        {
            get { return (Brush)GetValue(TabPanelTopBrushProperty); }

            set
            {
                SetValue(TabPanelTopBrushProperty, value);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Tick += (s, e) =>
            {
                if (!_makePopupOpen)
                {
                    _headList.IsOpen = _makePopupOpen;
                    _timer.Stop();
                }
            };
            _btnShowList = GetTemplateChild("BtnShowList") as Button;
            if (_btnShowList != null)
            {
                _btnShowList.Visibility = Visibility.Collapsed;
                _itemPanel = GetTemplateChild("TabPanelTop") as TabPanel;
                _headList = GetTemplateChild("HeadList") as Popup;
                _btnShowList.Click += (s, e) =>
                {
                    if (_headList.IsOpen == false)
                    {
                        var maxItemWidth = 0.0;
                        _headContent.Items.Clear();
                        foreach (var uiElement in _itemPanel.Children)
                        {
                            var eosItem = (TabItem) uiElement;
                            if (eosItem.Visibility == Visibility.Collapsed)
                            {
                                _headContent.Items.Add(eosItem.Header);
                                if (eosItem.ActualWidth > maxItemWidth)
                                {
                                    maxItemWidth = eosItem.ActualWidth;
                                }
                            }
                        }
                        _headList.HorizontalOffset = ActualWidth - maxItemWidth + 15;
                        _headList.VerticalOffset = 0;
                        _headList.IsOpen = true;
                    }
                    else
                    {
                        var tabItem = SelectedItem as TabItem;
                        if (tabItem != null) tabItem.Focus();
                    }
                };
                SizeChanged += (s,e)=>
                {
                    if (ActualWidth > 0)
                    {
                        var maxWidth = ActualWidth;
                        double itemsWidth = 27;
                        double itemMaxWidth = 0;
                        foreach (var uiElement in _itemPanel.Children)
                        {
                            var eosItem = (TabItem) uiElement;
                            itemsWidth += eosItem.ActualWidth;
                            if (eosItem.ActualWidth > itemMaxWidth)
                            {
                                itemMaxWidth = eosItem.ActualWidth;
                            }
                            eosItem.Visibility = itemsWidth < maxWidth ? Visibility.Visible : Visibility.Collapsed;
                        }
                        if (_itemPanel.Children.Count > 0)
                        {
                            if (_itemPanel.Children[0].Visibility == Visibility.Collapsed)
                            {
                                _itemPanel.Children[0].Visibility = Visibility.Visible;
                            }
                        }
                        foreach (var uiElement in _itemPanel.Children)
                        {
                            var eosItem = (TabItem) uiElement;
                            _btnShowList.Visibility = eosItem.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
                        }
                     _headList.HorizontalOffset = maxWidth - itemMaxWidth;
                    }
                }
                ;
                if (_itemPanel != null) _itemPanel.SizeChanged += (s,e) =>
                {
                    if (ActualWidth > 0)
                    {
                        var maxWidth = ActualWidth;
                        double itemsWidth = 27;
                        double itemMaxWidth = 0;
                        foreach (var uiElement in _itemPanel.Children)
                        {
                            var eosItem = (TabItem) uiElement;
                            itemsWidth += eosItem.ActualWidth;
                            if (eosItem.ActualWidth > itemMaxWidth)
                            {
                                itemMaxWidth = eosItem.ActualWidth;
                            }
                            eosItem.Visibility = itemsWidth < maxWidth ? Visibility.Visible : Visibility.Collapsed;
                        }
                        if (_itemPanel.Children.Count>0)
                        {
                            if (_itemPanel.Children[0].Visibility == Visibility.Collapsed)
                            {
                                _itemPanel.Children[0].Visibility = Visibility.Visible;
                            }
                            foreach (var uiElement in _itemPanel.Children)
                            {
                                var eosItem = (TabItem)uiElement;
                                _btnShowList.Visibility = eosItem.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
                            }
                        }
                       _headList.HorizontalOffset = maxWidth - itemMaxWidth;
                    }
                };
                _btnShowList.LostFocus +=(s,e)=>
                {
                    _headList.IsOpen = false;
                }
                ;
                _btnShowList.MouseLeave += (s,e)=>
                {
                    _makePopupOpen = false;
                    _timer.Start();
                }
                ;
            }
             MouseLeftButtonDown +=(s,e)=>
            {
                var tabItem = SelectedItem as TabItem;
                if (tabItem != null) tabItem.Focus();
            }
            ;
            _headContent = GetTemplateChild("HeadContent") as ListBox;
            if (_headContent != null)
            {
                _headContent.SelectionChanged += (s, e) =>
                {
                    foreach (var uiElement in _itemPanel.Children)
                    {
                        var eosItem = (TabItem) uiElement;
                        var listBox = s as ListBox;
                        if (listBox != null && eosItem.Header == listBox.SelectedItem)
                        {
                            eosItem.Visibility = Visibility.Visible;
                            Items.Remove(eosItem);
                            Items.Insert(0, eosItem);
                            SelectedIndex = 0;
                            break;
                        }
                    }
                   _headList.IsOpen = false;
                    var tabItem = SelectedItem as TabItem;
                    if (tabItem != null) tabItem.Focus();
                    UpdateLayout();
                };
                _headContent.MouseEnter += (s, e) =>
                {
                    _makePopupOpen = false;
                };
                _headContent.MouseLeave += (s, e) =>
                {
                    _makePopupOpen = false;
                    _timer.Start();
                };
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (e.Action == NotifyCollectionChangedAction.Add)
                SelectedItem = Items[Items.Count-1];
        }
    }

}
