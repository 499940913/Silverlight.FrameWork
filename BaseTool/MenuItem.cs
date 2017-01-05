
using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace BaseTool
{
    public class MenuItem : System.Windows.Controls.MenuItem
    {
        public MenuItem()
        {
            DefaultStyleKey = typeof(MenuItem);
            MouseEnter += parent_MouseEnter;
            MouseLeave += SuperMenuItem_MouseLeave;
            Click += SuperMenuItem_Click;
            CanLeave = true;
        }


  #region Fields

        private Popup _popup;
        public bool CanLeave { get; set; }

        #endregion

        #region Properties

        public Visibility HasSubItems
        {
            get { return (Visibility)GetValue(HasSubItemsProperty); }
            set { SetValue(HasSubItemsProperty, value); }
        }

        public static readonly DependencyProperty HasSubItemsProperty =
            DependencyProperty.Register("HasSubItems", typeof(Visibility), typeof(MenuItem), new PropertyMetadata(Visibility.Collapsed));

        public bool IsSubmenuOpen
        {
            get { return (bool)GetValue(IsSubmenuOpenProperty); }
            set { SetValue(IsSubmenuOpenProperty, value); }
        }

        public static readonly DependencyProperty IsSubmenuOpenProperty =
            DependencyProperty.Register("IsSubmenuOpen", typeof(bool), typeof(MenuItem), new PropertyMetadata(false));

        #endregion

        #region Constructor

      

        private void SuperMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = Parent as MenuItem;
            if (item != null)
            {
                item.OnClick();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _popup = (Popup)GetTemplateChild("PART_Popup");
            _popup.Opened += popup_Opened;
            _popup.Closed += popup_Closed;
        }

        private void popup_Opened(object sender, EventArgs e)
        {
            CanLeave = false;
        }

        private void popup_Closed(object sender, EventArgs e)
        {
            if (HasSubItems == Visibility.Visible)
            {
                IsSubmenuOpen = false;
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                if (e.NewItems.Count > 0)
                {
                    HasSubItems = Visibility.Visible;
                }
            }

        }

        private void parent_MouseEnter(object sender, MouseEventArgs e)
        {
            CanLeave = true;
            if (HasSubItems == Visibility.Visible)
            {
                IsSubmenuOpen = true;
            }
            var menu = Parent as ContextMenu;
            if (menu != null)
            {
                foreach (var item in menu.Items)
                {
                    if (item != this)
                    {
                        var menuItem = item as MenuItem;
                        if (menuItem != null) menuItem.IsSubmenuOpen = false;
                    }
                }
            }
        }

        private void SuperMenuItem_MouseLeave(object sender, MouseEventArgs e)
        {
            if (HasSubItems == Visibility.Visible)
            {
                if (CanLeave)
                {
                    IsSubmenuOpen = false;
                }
            }
        }

        #endregion
    }
}
