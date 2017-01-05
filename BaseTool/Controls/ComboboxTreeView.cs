using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace BaseTool.Controls
{
    public class ComboBoxTreeView : ComboBox
    {
        private ExtendedTreeView _treeView;
        private ContentPresenter _contentPresenter;

        public ComboBoxTreeView()
        {
            DefaultStyleKey = typeof(ComboBoxTreeView);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            //don't call the method of the base class
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _treeView = (ExtendedTreeView)GetTemplateChild("treeView");
            _treeView.OnHierarchyMouseUp += OnTreeViewHierarchyMouseUp;
            _contentPresenter = (ContentPresenter)GetTemplateChild("ContentPresenter");

            SetSelectedItemToHeader();
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);
            SelectedItem = _treeView.SelectedItem;
            SetSelectedItemToHeader();
        }

        protected override void OnDropDownOpened(EventArgs e)
        {
            base.OnDropDownOpened(e);
            SetSelectedItemToHeader();
        }

        /// <summary>
        /// Handles clicks on any item in the tree view
        /// </summary>
        private void OnTreeViewHierarchyMouseUp(object sender, MouseEventArgs e)
        {
            //This line isn't obligatory because it is executed in the OnDropDownClosed method, but be it so
            SelectedItem = _treeView.SelectedItem;

            IsDropDownOpen = false;
        }

        public new IEnumerable<ITreeViewItemModel> ItemsSource
        {
            get { return (IEnumerable<ITreeViewItemModel>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public new static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<ITreeViewItemModel>), typeof(ComboBoxTreeView), new PropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((ComboBoxTreeView)sender).UpdateItemsSource();
        }

        /// <summary>
        /// Selected item of the TreeView
        /// </summary>
        public new object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public new static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(ComboBoxTreeView), new PropertyMetadata(null, OnSelectedItemChanged));

        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((ComboBoxTreeView)sender).UpdateSelectedItem();
        }

        /// <summary>
        /// Selected hierarchy of the treeview
        /// </summary>
        public IEnumerable<string> SelectedHierarchy
        {
            get { return (IEnumerable<string>)GetValue(SelectedHierarchyProperty); }
            set { SetValue(SelectedHierarchyProperty, value); }
        }

        public static readonly DependencyProperty SelectedHierarchyProperty =
            DependencyProperty.Register("SelectedHierarchy", typeof(IEnumerable<string>), typeof(ComboBoxTreeView), new PropertyMetadata(null, OnSelectedHierarchyChanged));

        private static void OnSelectedHierarchyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((ComboBoxTreeView)sender).UpdateSelectedHierarchy();
        }

        private void UpdateItemsSource()
        {
            var allItems = new List<ITreeViewItemModel>();

            Action<IEnumerable<ITreeViewItemModel>> selectAllItemsRecursively = null;
            selectAllItemsRecursively = items =>
            {
                if (items == null)
                {
                    return;
                }

                foreach (var item in items)
                {
                    allItems.Add(item);
                    selectAllItemsRecursively(item.GetChildren());
                }
            };

            selectAllItemsRecursively(ItemsSource);

            base.ItemsSource = allItems.Count > 0 ? allItems : null;
        }

        private void UpdateSelectedItem()
        {
            if (SelectedItem is TreeViewItem)
            {
                //I would rather use a correct object instead of TreeViewItem
                SelectedItem = ((TreeViewItem)SelectedItem).DataContext;
            }
            else
            {
                //Update the selected hierarchy and displays
                var model = SelectedItem as ITreeViewItemModel;
                if (model != null)
                {
                    SelectedHierarchy = model.GetHierarchy().Select(h => h.SelectedValuePath).ToList();
                }

                SetSelectedItemToHeader();

                base.SelectedItem = SelectedItem;
            }
        }

        private void UpdateSelectedHierarchy()
        {
            if (ItemsSource == null || SelectedHierarchy == null) return;
            var source = ItemsSource.OfType<ITreeViewItemModel>();
            var item = SelectItem(source, SelectedHierarchy);
            SelectedItem = item;
        }

        /// <summary>
        /// Searches the items of the hierarchy inside the items source and selects the last found item
        /// </summary>
        private static ITreeViewItemModel SelectItem(IEnumerable<ITreeViewItemModel> items, IEnumerable<string> selectedHierarchy)
        {
            var enumerable = selectedHierarchy as string[] ?? selectedHierarchy.ToArray();
            var treeViewItemModels = items as ITreeViewItemModel[] ?? items.ToArray();
            if (items == null || selectedHierarchy == null || !treeViewItemModels.Any() || !enumerable.Any())
            {
                return null;
            }

            var hierarchy = enumerable.ToList();
            var currentItems = treeViewItemModels;
            ITreeViewItemModel selectedItem = null;

            for (int i = 0; i < hierarchy.Count; i++)
            {
                // get next item in the hierarchy from the collection of child items
                var currentItem = currentItems.FirstOrDefault(ci => ci.SelectedValuePath == hierarchy[i]);
                if (currentItem == null)
                {
                    break;
                }

                selectedItem = currentItem;
                if (selectedItem.GetChildren()==null) break;
                currentItems = selectedItem.GetChildren().ToArray();
                if (i != hierarchy.Count - 1)
                {
                    selectedItem.IsExpanded = true;
                }
            }

            if (selectedItem != null)
            {
                selectedItem.IsSelected = true;
            }

            return selectedItem;
        }

        /// <summary>
        /// Gets the hierarchy of the selected tree item and displays it at the combobox header
        /// </summary>
        private void SetSelectedItemToHeader()
        {
            string content = null;

            var item = SelectedItem as ITreeViewItemModel;
            if (item != null)
            {
                //Get hierarchy and display it as the selected item
                var hierarchy = item.GetHierarchy().Select(i => i.DisplayValuePath).ToArray();
                if (hierarchy.Length > 0)
                {
                    content = string.Join(".", hierarchy);
                }
            }
            SetContentAsTextBlock(content);
        }

        /// <summary>
        /// Gets the combobox header and displays the specified content there
        /// </summary>
        private void SetContentAsTextBlock(string content)
        {
            if (_contentPresenter == null)
            {
                return;
            }

            var tb = _contentPresenter.Content as TextBlock;
            if (tb == null)
            {
                _contentPresenter.Content = tb = new TextBlock();
            }
            tb.Text = content ?? ' '.ToString();

            _contentPresenter.ContentTemplate = null;
        }
    }


    public interface ITreeViewItemModel : INotifyPropertyChanged
    {
        string SelectedValuePath { get; }

        string DisplayValuePath { get; }

        bool IsExpanded { get; set; }

        bool IsSelected { get; set; }

        IEnumerable<ITreeViewItemModel> GetHierarchy();

        IEnumerable<ITreeViewItemModel> GetChildren();
    }

    #region treeview
    public class ExtendedTreeView : TreeView
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            var childItem = ExtendedTreeViewItem.CreateItemWithBinding();

            childItem.OnHierarchyMouseUp += OnChildItemMouseLeftButtonUp;

            return childItem;
        }

        private void OnChildItemMouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            if (OnHierarchyMouseUp == null) return;
                OnHierarchyMouseUp(this, e);
        }

        public event MouseEventHandler OnHierarchyMouseUp;
    }

    public class ExtendedTreeViewItem : TreeViewItem
    {
        public ExtendedTreeViewItem()
        {
            MouseLeftButtonUp += OnMouseLeftButtonUp;
        }

        void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (OnHierarchyMouseUp == null)return;
                OnHierarchyMouseUp(this, e);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var childItem = CreateItemWithBinding();

            childItem.MouseLeftButtonUp += OnMouseLeftButtonUp;

            return childItem;
        }

        public static ExtendedTreeViewItem CreateItemWithBinding()
        {
            var tvi = new ExtendedTreeViewItem();

            var expandedBinding = new Binding("IsExpanded") {Mode = BindingMode.TwoWay};
            tvi.SetBinding(IsExpandedProperty, expandedBinding);

            var selectedBinding = new Binding("IsSelected") {Mode = BindingMode.TwoWay};
            tvi.SetBinding(IsSelectedProperty, selectedBinding);

            return tvi;
        }

        public event MouseEventHandler OnHierarchyMouseUp;
    }
    #endregion
}
