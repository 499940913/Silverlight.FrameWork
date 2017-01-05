using System.Collections.Generic;
using System.Windows;

namespace BaseTool
{
    public partial class Group2Group 
    {
        public Dictionary<string, object> OriginalList
        {
            get
            {
                return _originalList;
            }
        }

        public Dictionary<string, object> TargetList
        {
           get
           {
               return _targetList;
           }
        }

        private readonly bool _limited;

       private readonly Dictionary<string, object> _originalList;
       private readonly Dictionary<string, object> _targetList;
        public Group2Group(Dictionary<string,object>orgl,string group1Name,Dictionary<string,object>tarlist,string group2Name,bool limited)
        {
            InitializeComponent();
            _limited = limited;
            _originalList = orgl;
            originalGroupName.Text = group1Name;
            TargetGroupName.Text = group2Name;
            _targetList =tarlist;
            UpdateListBox();
        }

        private void UpdateListBox()
        {
            originallistBox.ItemsSource = null;
            targetlistBox.ItemsSource = null;
            originallistBox.ItemsSource = _originalList;
            targetlistBox.ItemsSource = _targetList;
            originallistBox.DisplayMemberPath = "Key";
            originallistBox.SelectedValuePath = "Value";
            targetlistBox.DisplayMemberPath = "Key";
            targetlistBox.SelectedValuePath = "Value";
            originallistBox.UpdateLayout();
            targetlistBox.UpdateLayout();
        }

   
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_limited && _targetList.Count > 0) return;
            if (originallistBox.SelectedItem != null)
            {
                var p = originallistBox.SelectedItem.GetType().GetProperty("Key");
                var key = p.GetValue(originallistBox.SelectedItem, null).ToString();
                _targetList.Add(key, originallistBox.SelectedValue);
                _originalList.Remove(key);
                UpdateListBox();
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (targetlistBox.SelectedItem != null)
            {
                var p = targetlistBox.SelectedItem.GetType().GetProperty("Key");
                var key = p.GetValue(targetlistBox.SelectedItem, null).ToString();
                _originalList.Add(key, targetlistBox.SelectedValue);
                _targetList.Remove(key);
                UpdateListBox();
            }
        }
    }
}

