// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;
using Globalization = System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace  BaseTool.Primitives
{
    /// <summary>
    /// Represents the currently displayed month or year on a
    /// <see cref="T:System.Windows.Controls.Calendar" />.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    [TemplatePart(Name = ElementRoot, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = ElementHeaderButton, Type = typeof(Button))]
    [TemplatePart(Name = ElementPreviousButton, Type = typeof(Button))]
    [TemplatePart(Name = ElementNextButton, Type = typeof(Button))]
    [TemplatePart(Name = ElementDayTitleTemplate, Type = typeof(DataTemplate))]
    [TemplatePart(Name = ElementMonthView, Type = typeof(Grid))]
    [TemplatePart(Name = ElementYearView, Type = typeof(Grid))]
    [TemplatePart(Name = ElementQuarterView, Type = typeof(Grid))]
    [TemplatePart(Name = ElementDisabledVisual, Type = typeof(FrameworkElement))]
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    public sealed partial class CalendarItem : Control
    {
        /// <summary>
        /// The number of days per week.
        /// </summary>
        private const int NumberOfDaysPerWeek = 7;

        #region Template Parts
        /// <summary>
        /// The name of the Root template part.
        /// </summary>
        /// <remarks>
        /// TODO: It appears this template part is no longer used.  Verify with
        /// compat whether we can remove the attribute.
        /// </remarks>
        private const string ElementRoot = "Root";

        /// <summary>
        /// The name of the HeaderButton template part.
        /// </summary>
        private const string ElementHeaderButton = "HeaderButton";

        /// <summary>
        /// The name of the PreviousButton template part.
        /// </summary>
        private const string ElementPreviousButton = "PreviousButton";

        /// <summary>
        /// The name of the NextButton template part.
        /// </summary>
        private const string ElementNextButton = "NextButton";

        /// <summary>
        /// The name of the DayTitleTemplate template part.
        /// </summary>
        private const string ElementDayTitleTemplate = "DayTitleTemplate";

        /// <summary>
        /// The name of the MonthView template part.
        /// </summary>
        private const string ElementMonthView = "MonthView";

        /// <summary>
        /// The name of the YearView template part.
        /// </summary>
        private const string ElementYearView = "YearView";

        private const string ElementQuarterView = "QuarterView";

        /// <summary>
        /// The name of the DisabledVisual template part.
        /// </summary>
        private const string ElementDisabledVisual = "DisabledVisual";

        /// <summary>
        /// The button that allows switching between month mode, year mode, and
        /// decade mode. 
        /// </summary>
        private Button _headerButton;

        /// <summary>
        /// Gets the button that allows switching between month mode, year mode,
        /// and decade mode. 
        /// </summary>
        internal Button HeaderButton
        {
            get { return _headerButton; }
            private set
            {
                // TODO: Detach event handler

                _headerButton = value;

                if (_headerButton != null)
                {
                    _headerButton.Click += HeaderButton_Click;
                    _headerButton.IsTabStop = false;
                }
            }
        }

        /// <summary>
        /// The button that displays the next page of the calendar when it is
        /// clicked.
        /// </summary>
        private Button _nextButton;

        /// <summary>
        /// Gets the button that displays the next page of the calendar when it
        /// is clicked.
        /// </summary>
        /// 

        internal void YearViewHide(bool ishide)
        {  

            if (YearView != null)
            {
                if (ishide)
                {
                    if (YearView.Visibility == Visibility.Visible)
                    {
                        YearView.Visibility = Visibility.Collapsed;
                        QuarterView.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        internal Button NextButton
        {
            get { return _nextButton; }
            private set
            {
                // TODO: Detach event handler

                _nextButton = value;

                if (_nextButton != null)
                {
                    // If the user does not provide a Content value in template,
                    // we provide a helper text that can be used in
                    // Accessibility this text is not shown on the UI, just used
                    // for Accessibility purposes
                    if (_nextButton.Content == null)
                    {
                        _nextButton.Content = BaseTool.Resources.Calendar_NextButtonName;
                    }

                    if (_isTopRightMostMonth)
                    {
                        _nextButton.Visibility = Visibility.Visible;
                        _nextButton.Click += NextButton_Click;
                        _nextButton.IsTabStop = false;
                    }
                }
            }
        }

        /// <summary>
        /// The button that displays the previous page of the calendar when it
        /// is clicked.
        /// </summary>
        private Button _previousButton;

        /// <summary>
        /// Gets the button that displays the previous page of the calendar when
        /// it is clicked.
        /// </summary>
        internal Button PreviousButton
        {
            get { return _previousButton; }
            private set
            {
                // TODO: Detach event handler

                _previousButton = value;

                if (_previousButton != null)
                {
                    // If the user does not provide a Content value in template,
                    // we provide a helper text that can be used in
                    // Accessibility this text is not shown on the UI, just used
                    // for Accessibility purposes
                    if (_previousButton.Content == null)
                    {
                        _previousButton.Content = BaseTool.Resources.Calendar_PreviousButtonName;
                    }

                    if (_isTopLeftMostMonth)
                    {
                        _previousButton.Visibility = Visibility.Visible;
                        _previousButton.Click += PreviousButton_Click;
                        _previousButton.IsTabStop = false;
                    }
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private DataTemplate _dayTitleTemplate;

        /// <summary>
        /// Hosts the content when in month mode.
        /// </summary>
        private Grid _monthView;

        /// <summary>
        /// Gets the Grid that hosts the content when in month mode.
        /// </summary>
        internal Grid MonthView
        {
            get { return _monthView; }
            private set
            {
                // TODO: Detach event handler

                _monthView = value;

                if (_monthView != null)
                {
                    _monthView.MouseLeave += MonthView_MouseLeave;
                }
            }
        }

        /// <summary>
        /// Hosts the content when in year or decade mode.
        /// </summary>
        /// 
        private Grid _quarterview;
        internal Grid QuarterView
        {
            get { return _quarterview; }
            private set
            {
                // TODO: Detach event handler

                _quarterview = value;

                if (_quarterview != null)
                {
                    _quarterview.MouseLeave += QuarterView_MouseLeave;
                }
            }
        }

        private Grid _yearView;

        /// <summary>
        /// Gets the Grid that hosts the content when in year or decade mode.
        /// </summary>
        internal Grid YearView
        {
            get { return _yearView; }
            private set
            {
                // TODO: Detach event handler

                _yearView = value;

                if (_yearView != null)
                {
                    _yearView.MouseLeave += YearView_MouseLeave;
                }
            }
        }

        /// <summary>
        /// The overlay for the disabled state.
        /// </summary>
        /// <remarks>
        /// The disabled visual isn't necessary given that we also have a
        /// Disabled visual state.  It's only being left in for compatability
        /// with existing templates.
        /// </remarks>
        private FrameworkElement _disabledVisual;
        #endregion Template Parts

        /// <summary>
        /// Gets or sets Inherited code: Requires comment.
        /// </summary>
        internal Calendar Owner { get; set; }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private CalendarButton  _lastCalendarButton;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private CalendarDayButton  _lastCalendarDayButton;

        /// <summary>
        /// Gets or sets Inherited code: Requires comment.
        /// </summary>
        internal CalendarDayButton  CurrentButton { get; set; }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private MouseButtonEventArgs _downEventArg;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private MouseButtonEventArgs _downEventArgYearView;

        private MouseButtonEventArgs _downEventArgQuarterView;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private bool _isMouseLeftButtonDown;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private bool _isMouseLeftButtonDownYearView;
        private bool _isMouseLeftButtonDownQuarterView;
         
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private bool _isTopLeftMostMonth = true;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private bool _isTopRightMostMonth = true;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private DateTime _currentMonth;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private Globalization.Calendar _calendar = new GregorianCalendar();

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Controls.Primitives.CalendarItem" />
        /// class.
        /// </summary>
        public CalendarItem()
        {
            DefaultStyleKey = typeof(CalendarItem);
        }

        #region Templating
        /// <summary>
        /// Builds the visual tree for the
        /// <see cref="T:System.Windows.Controls.Primitives.CalendarItem" />
        /// when a new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            HeaderButton = GetTemplateChild(ElementHeaderButton) as Button;
            PreviousButton = GetTemplateChild(ElementPreviousButton) as Button;
            NextButton = GetTemplateChild(ElementNextButton) as Button;
            _dayTitleTemplate = GetTemplateChild(ElementDayTitleTemplate) as DataTemplate;
            MonthView = GetTemplateChild(ElementMonthView) as Grid;
            YearView = GetTemplateChild(ElementYearView) as Grid;
            QuarterView = GetTemplateChild(ElementQuarterView) as Grid;
            _disabledVisual = GetTemplateChild(ElementDisabledVisual) as FrameworkElement;

            if (Owner != null)
            {
                UpdateDisabledGrid(Owner.IsEnabled);
            }

            PopulateGrids();

            if (MonthView != null && YearView != null&&QuarterView!=null)
            {
                if (Owner != null)
                {
                    Owner.SelectedMonth = Owner.DisplayDateInternal;
                    Owner.SelectedYear = Owner.DisplayDateInternal;
                    Owner.SelectedQuarter = Owner.DisplayDateInternal;
                    if (Owner.DisplayMode == CalendarMode.Year)
                    {  
                        UpdateYearMode(false);
                    }
                    else if (Owner.DisplayMode == CalendarMode.Quarter)
                    {
                        UpdateYearMode(true);
                    }
                    else if (Owner.DisplayMode == CalendarMode.Decade)
                    {
                        UpdateDecadeMode();
                    }

                    if (Owner.DisplayMode == CalendarMode.Month)
                    {
                        UpdateMonthMode();
                        MonthView.Visibility = Visibility.Visible;
                        YearView.Visibility = Visibility.Collapsed;
                        QuarterView.Visibility = Visibility.Collapsed;
                    }
                    else if (Owner.DisplayMode == CalendarMode.Quarter)
                    {
                        QuarterView.Visibility = Visibility.Visible;
                        MonthView.Visibility = Visibility.Collapsed;
                        YearView.Visibility = Visibility.Collapsed;
                    }
                    else if (Owner.DisplayMode == CalendarMode.Year || Owner.DisplayMode == CalendarMode.Decade)
                    {
                        QuarterView.Visibility = Visibility.Collapsed;
                        MonthView.Visibility = Visibility.Collapsed;
                        YearView.Visibility = Visibility.Visible;
                    }
                    
                   
                }
                else
                {
                    UpdateMonthMode();
                    MonthView.Visibility = Visibility.Visible;
                    YearView.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void PopulateGrids()
        {
            if (MonthView != null)
            {
                for (var i = 0; i < Calendar.RowsPerMonth; i++)
                {
                    if (_dayTitleTemplate != null)
                    {
                        var cell = (FrameworkElement) _dayTitleTemplate.LoadContent();
                        cell.SetValue(Grid.RowProperty, 0);
                        cell.SetValue(Grid.ColumnProperty, i);
                        MonthView.Children.Add(cell);
                    }
                }

                for (var i = 1; i < Calendar.RowsPerMonth; i++)
                {
                    for (var j = 0; j < Calendar.ColumnsPerMonth; j++)
                    {
                        var  cell = new CalendarDayButton();

                        if (Owner != null)
                        {
                            cell.Owner = Owner;
                            if (Owner.CalendarDayButtonStyle != null)
                            {
                                cell.Style = Owner.CalendarDayButtonStyle;
                            }
                        }
                        cell.SetValue(Grid.RowProperty, i);
                        cell.SetValue(Grid.ColumnProperty, j);
                        cell.CalendarDayButtonMouseDown += Cell_MouseLeftButtonDown;
                        cell.CalendarDayButtonMouseUp += Cell_MouseLeftButtonUp;
                        cell.MouseEnter += Cell_MouseEnter;
                        cell.MouseLeave += Cell_MouseLeave;
                        cell.Click += Cell_Click;
                        MonthView.Children.Add(cell);
                    }
                }
            }

            if (QuarterView != null)
            {
                CalendarButton  Quarter;
                var count = 0;
                for (var i = 0; i < Calendar.RowsPerQuarter; i++)
                {
                    for (var j = 0; j < Calendar.ColumnsPerQuarter; j++)
                    {
                        Quarter = new CalendarButton();
                        if (Owner != null)
                        {
                            Quarter.Owner = Owner;
                            if (Owner.CalendarButtonStyle != null)
                            {
                                Quarter.Style = Owner.CalendarButtonStyle;
                            }
                        }
                        Quarter.SetValue(Grid.RowProperty, i);
                        Quarter.SetValue(Grid.ColumnProperty, j);
                        Quarter.CalendarButtonMouseDown += Quarter_CalendarButtonMouseDown;
                        Quarter.CalendarButtonMouseUp += Quarter_CalendarButtonMouseUp;
                        Quarter.MouseEnter += Quarter_MouseEnter;
                        Quarter.MouseLeave += Quarter_MouseLeave;
                        QuarterView.Children.Add(Quarter);
                        count++;
                    }
                }
            }

            if (YearView != null)
            {
                CalendarButton  month;
                var count = 0;
                for (var i = 0; i < Calendar.RowsPerYear; i++)
                {
                    for (var j = 0; j < Calendar.ColumnsPerYear; j++)
                    {
                        month = new CalendarButton();

                        if (Owner != null)
                        {
                            month.Owner = Owner;
                            if (Owner.CalendarButtonStyle != null)
                            {
                                month.Style = Owner.CalendarButtonStyle;
                            }
                        }
                        month.SetValue(Grid.RowProperty, i);
                        month.SetValue(Grid.ColumnProperty, j);
                        month.CalendarButtonMouseDown += Month_CalendarButtonMouseDown;
                        month.CalendarButtonMouseUp += Month_CalendarButtonMouseUp;
                        month.MouseEnter += Month_MouseEnter;
                        month.MouseLeave += Month_MouseLeave;
                        YearView.Children.Add(month);
                        count++;
                    }
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="isEnabled">Inherited code: Requires comment 1.</param>
        internal void UpdateDisabledGrid(bool isEnabled)
        {
            if (isEnabled)
            {
                if (_disabledVisual != null)
                {
                    _disabledVisual.Visibility = Visibility.Collapsed;
                }
                VisualStates.GoToState(this, true, VisualStates.StateNormal);
            }
            else
            {
                if (_disabledVisual != null)
                {
                    _disabledVisual.Visibility = Visibility.Visible;
                }
                VisualStates.GoToState(this, true, VisualStates.StateDisabled, VisualStates.StateNormal);
            }
        }
        #endregion Templating

        #region Month Mode
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal void UpdateMonthMode()
        {
            if (Owner != null)
            {
                Debug.Assert(Owner.DisplayDate != null, "The Owner Calendar's DisplayDate should not be null!");
                _currentMonth = Owner.DisplayDateInternal;
            }
            else
            {
                _currentMonth = DateTime.Today;
            }

            if (_currentMonth != null)
            {
                SetMonthModeHeaderButton();
                SetMonthModePreviousButton(_currentMonth);
                SetMonthModeNextButton(_currentMonth);

                if (MonthView != null)
                {
                    SetDayTitles();
                    SetCalendarDayButtons(_currentMonth);
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void SetDayTitles()
        {
            for (var childIndex = 0; childIndex < Calendar.ColumnsPerMonth; childIndex++)
            {
                var daytitle = MonthView.Children[childIndex] as FrameworkElement;
                if (daytitle != null)
                {
                    if (Owner != null)
                    {
                        daytitle.DataContext = DateTimeHelper.GetCurrentDateFormat().ShortestDayNames[(childIndex + (int) Owner.FirstDayOfWeek) % NumberOfDaysPerWeek];
                    }
                    else
                    {
                        daytitle.DataContext = DateTimeHelper.GetCurrentDateFormat().ShortestDayNames[(childIndex + (int) DateTimeHelper.GetCurrentDateFormat().FirstDayOfWeek) % NumberOfDaysPerWeek];
                    }
                }
            }
        }

        /// <summary>
        /// How many days of the previous month need to be displayed.
        /// </summary>
        /// <param name="firstOfMonth">Inherited code: Requires comment.</param>
        /// <returns>Inherited code: Requires comment 1.</returns>
        private int PreviousMonthDays(DateTime firstOfMonth)
        {
            var day = _calendar.GetDayOfWeek(firstOfMonth);
            int i;

            if (Owner != null)
            {
                i = (day - Owner.FirstDayOfWeek + NumberOfDaysPerWeek) % NumberOfDaysPerWeek;
            }
            else
            {
                i = (day - DateTimeHelper.GetCurrentDateFormat().FirstDayOfWeek + NumberOfDaysPerWeek) % NumberOfDaysPerWeek;
            }

            if (i == 0)
            {
                return NumberOfDaysPerWeek;
            }
            else
            {
                return i;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="childButton">Inherited code: Requires comment 1.</param>
        /// <param name="dateToAdd">Inherited code: Requires comment 2.</param>
        private void SetButtonState(CalendarDayButton   childButton, DateTime dateToAdd)
        {
            if (Owner != null)
            {
                childButton.Opacity = 1;

                // If the day is outside the DisplayDateStart/End boundary, do
                // not show it
                if (DateTimeHelper.CompareDays(dateToAdd, Owner.DisplayDateRangeStart) < 0 || DateTimeHelper.CompareDays(dateToAdd, Owner.DisplayDateRangeEnd) > 0)
                {
                    childButton.IsEnabled = false;
                    childButton.Opacity = 0;
                }
                else
                {
                    // SET IF THE DAY IS SELECTABLE OR NOT
                    if (Owner.BlackoutDates.Contains(dateToAdd))
                    {
                        childButton.IsBlackout = true;
                    }
                    else
                    {
                        childButton.IsBlackout = false;
                    }
                    childButton.IsEnabled = true;

                    // SET IF THE DAY IS INACTIVE OR NOT: set if the day is a
                    // trailing day or not
                    childButton.IsInactive = DateTimeHelper.CompareYearMonth(dateToAdd, Owner.DisplayDateInternal) != 0;

                    // SET IF THE DAY IS TODAY OR NOT
                    childButton.IsToday = Owner.IsTodayHighlighted && dateToAdd == DateTime.Today;

                    // SET IF THE DAY IS SELECTED OR NOT
                    childButton.IsSelected = false;
                    foreach (var item in Owner.SelectedDates)
                    {
                        // Since we should be comparing the Date values not
                        // DateTime values, we can't use
                        // Owner.SelectedDates.Contains(dateToAdd) directly
                        childButton.IsSelected |= DateTimeHelper.CompareDays(dateToAdd, item) == 0;
                    }

                    // SET THE FOCUS ELEMENT
                    if (Owner.LastSelectedDate != null)
                    {
                        if (DateTimeHelper.CompareDays(Owner.LastSelectedDate.Value, dateToAdd) == 0)
                        {
                            if (Owner.FocusButton != null)
                            {
                                Owner.FocusButton.IsCurrent = false;
                            }
                            Owner.FocusButton = childButton;
                            if (Owner.HasFocusInternal)
                            {
                                Owner.FocusButton.IsCurrent = true;
                            }
                        }
                        else
                        {
                            childButton.IsCurrent = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="firstDayOfMonth">Inherited code: Requires comment 1.</param>
        private void SetCalendarDayButtons(DateTime firstDayOfMonth)
        {
            var lastMonthToDisplay = PreviousMonthDays(firstDayOfMonth);
            DateTime dateToAdd;

            if (DateTimeHelper.CompareYearMonth(firstDayOfMonth, DateTime.MinValue) > 0)
            {
                // DisplayDate is not equal to DateTime.MinValue we can subtract
                // days from the DisplayDate
                dateToAdd = _calendar.AddDays(firstDayOfMonth, -lastMonthToDisplay);
            }
            else
            {
                dateToAdd = firstDayOfMonth;
            }

            if (Owner != null && Owner.HoverEnd != null && Owner.HoverStart != null)
            {
                Owner.HoverEndIndex = null;
                Owner.HoverStartIndex = null;
            }

            var count = Calendar.RowsPerMonth * Calendar.ColumnsPerMonth;

            for (var childIndex = Calendar.ColumnsPerMonth; childIndex < count; childIndex++)
            {
                var  childButton = MonthView.Children[childIndex] as CalendarDayButton;
                Debug.Assert(childButton != null, "childButton should not be null!");

                childButton.Index = childIndex;
                SetButtonState(childButton, dateToAdd);

                // Update the indexes of hoverStart and hoverEnd
                if (Owner != null && Owner.HoverEnd != null && Owner.HoverStart != null)
                {
                    if (DateTimeHelper.CompareDays(dateToAdd, Owner.HoverEnd.Value) == 0)
                    {
                        Owner.HoverEndIndex = childIndex;
                    }

                    if (DateTimeHelper.CompareDays(dateToAdd, Owner.HoverStart.Value) == 0)
                    {
                        Owner.HoverStartIndex = childIndex;
                    }
                }

                childButton.IsTabStop = false;
                childButton.Content = dateToAdd.Day.ToString(DateTimeHelper.GetCurrentDateFormat());
                childButton.DataContext = dateToAdd;

                if (DateTime.Compare((DateTime) DateTimeHelper.DiscardTime(DateTime.MaxValue), dateToAdd) > 0)
                {
                    // Since we are sure DisplayDate is not equal to
                    // DateTime.MaxValue, it is safe to use AddDays 
                    dateToAdd = _calendar.AddDays(dateToAdd, 1);
                }
                else
                {
                    // DisplayDate is equal to the DateTime.MaxValue, so there
                    // are no trailing days
                    childIndex++;
                    for (var i = childIndex; i < count; i++)
                    {
                        childButton = MonthView.Children[i] as CalendarDayButton;
                        Debug.Assert(childButton != null, "childButton should not be null!");
                        // button needs a content to occupy the necessary space
                        // for the content presenter
                        childButton.Content = i.ToString(DateTimeHelper.GetCurrentDateFormat());
                        childButton.IsEnabled = false;
                        childButton.Opacity = 0;
                    }
                    return;
                }
            }

            // If the HoverStart or HoverEndInternal could not be found on the
            // DisplayMonth set the values of the HoverStartIndex or
            // HoverEndIndex to be the first or last day indexes on the current
            // month
            if (Owner != null && Owner.HoverStart.HasValue && Owner.HoverEndInternal.HasValue)
            {
                if (!Owner.HoverEndIndex.HasValue)
                {
                    if (DateTimeHelper.CompareDays(Owner.HoverEndInternal.Value, Owner.HoverStart.Value) > 0)
                    {
                        Owner.HoverEndIndex = Calendar.ColumnsPerMonth * Calendar.RowsPerMonth - 1;
                    }
                    else
                    {
                        Owner.HoverEndIndex = Calendar.ColumnsPerMonth;
                    }
                }

                if (!Owner.HoverStartIndex.HasValue)
                {
                    if (DateTimeHelper.CompareDays(Owner.HoverEndInternal.Value, Owner.HoverStart.Value) > 0)
                    {
                        Owner.HoverStartIndex = Calendar.ColumnsPerMonth;
                    }
                    else
                    {
                        Owner.HoverStartIndex = Calendar.ColumnsPerMonth * Calendar.RowsPerMonth - 1;
                    }
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void SetMonthButtonsForYearMode(bool isQuarter)
        {
            var count = 0;
            if (!isQuarter)
            {
                foreach (object child in YearView.Children)
                {
                    var  childButton = child as CalendarButton; 
                    Debug.Assert(childButton != null, "childButton should not be null!");
                    // There should be no time component. Time is 12:00 AM
                    var day = new DateTime(_currentMonth.Year, count + 1, 1);
                    childButton.DataContext = day;

                    childButton.Content = DateTimeHelper.GetCurrentDateFormat().AbbreviatedMonthNames[count];
                    childButton.Visibility = Visibility.Visible;

                    if (Owner != null)
                    {
                        if (day.Year == _currentMonth.Year && day.Month == _currentMonth.Month && day.Day == _currentMonth.Day)
                        {
                            Owner.FocusCalendarButton = childButton;
                            childButton.IsCalendarButtonFocused = Owner.HasFocusInternal;
                        }
                        else
                        {
                            childButton.IsCalendarButtonFocused = false;
                        }

                        Debug.Assert(Owner.DisplayDateInternal != null, "The Owner Calendar's DisplayDateInternal should not be null!");
                        childButton.IsSelected = DateTimeHelper.CompareYearMonth(day, Owner.DisplayDateInternal) == 0;

                        if (DateTimeHelper.CompareYearMonth(day, Owner.DisplayDateRangeStart) < 0 || DateTimeHelper.CompareYearMonth(day, Owner.DisplayDateRangeEnd) > 0)
                        {
                            childButton.IsEnabled = false;
                            childButton.Opacity = 0;
                        }
                        else
                        {
                            childButton.IsEnabled = true;
                            childButton.Opacity = 1;
                        }
                    }

                    childButton.IsInactive = false;
                    count++;
                }
            }
            else
            {
                foreach (object child in QuarterView.Children)
                {
                    var currentquarter = 0;
                    if (_currentMonth.Month >= 1 && _currentMonth.Month <= 3)
                    {
                        currentquarter = 3;
                    }
                    else if (_currentMonth.Month >= 4 && _currentMonth.Month <= 6)
                    {
                        currentquarter = 6;
                    }
                    else if (_currentMonth.Month >= 7 && _currentMonth.Month <= 9)
                    {
                        currentquarter = 9;
                    }
                    else if (_currentMonth.Month >= 10 && _currentMonth.Month <= 12)
                    {
                        currentquarter = 12;
                    }
                    var  childButton = child as CalendarButton; 
                    Debug.Assert(childButton != null, "childButton should not be null!");
                    // There should be no time component. Time is 12:00 AM
                    var day = new DateTime(_currentMonth.Year, count + 3, 1);
                    childButton.DataContext = day;
                    if (DateTimeHelper.GetCurrentDateFormat().AbbreviatedMonthNames[count] == "一月")
                    {
                        childButton.Content = "一季度";
                        childButton.Tag = "Q1";
                    }
                    else if (DateTimeHelper.GetCurrentDateFormat().AbbreviatedMonthNames[count] == "四月")
                    {
                        childButton.Content = "二季度";
                        childButton.Tag = "Q2";
                    }
                    else if (DateTimeHelper.GetCurrentDateFormat().AbbreviatedMonthNames[count] == "七月")
                    {
                        childButton.Content = "三季度";
                        childButton.Tag = "Q3";
                    }
                    else if (DateTimeHelper.GetCurrentDateFormat().AbbreviatedMonthNames[count] == "十月")
                    {
                        childButton.Content = "四季度";
                        childButton.Tag = "Q4";
                    }
                    childButton.Visibility = Visibility.Visible;

                    if (Owner != null)
                    {    
                        //if (day.Year == _currentMonth.Year && day.Month == _currentMonth.Month && day.Day == _currentMonth.Day)
                        //{
                        //    Owner.FocusCalendarButton = childButton;
                        //    childButton.IsCalendarButtonFocused = Owner.HasFocusInternal;
                        //}
                        if (day.Year == _currentMonth.Year && day.Month == currentquarter && day.Day == _currentMonth.Day)
                        {
                            Owner.FocusCalendarButton = childButton;
                            childButton.IsCalendarButtonFocused = Owner.HasFocusInternal;
                        }
                        
                        else
                        {
                            childButton.IsCalendarButtonFocused = false;
                        }

                        Debug.Assert(Owner.DisplayDateInternal != null, "The Owner Calendar's DisplayDateInternal should not be null!");
                        childButton.IsSelected = DateTimeHelper.CompareYearMonth(day, Owner.DisplayDateInternal) == 0;

                        if (DateTimeHelper.CompareYearMonth(day, Owner.DisplayDateRangeStart) < 0 || DateTimeHelper.CompareYearMonth(day, Owner.DisplayDateRangeEnd) > 0)
                        {
                            childButton.IsEnabled = false;
                            childButton.Opacity = 0;
                        }
                        else
                        {
                            childButton.IsEnabled = true;
                            childButton.Opacity = 1;
                        }
                    }

                    childButton.IsInactive = false;
                    count = count + 3;
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void SetMonthModeHeaderButton()
        {
            if (HeaderButton != null)
            {
                if (Owner != null)
                {
                    HeaderButton.Content = Owner.DisplayDateInternal.ToString("Y", DateTimeHelper.GetCurrentDateFormat());
                    HeaderButton.IsEnabled = true;
                }
                else
                {
                    HeaderButton.Content = DateTime.Today.ToString("Y", DateTimeHelper.GetCurrentDateFormat());
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="firstDayOfMonth">Inherited code: Requires comment 1.</param>
        private void SetMonthModeNextButton(DateTime firstDayOfMonth)
        {
            if (Owner != null && NextButton != null)
            {
                // DisplayDate is equal to DateTime.MaxValue
                if (DateTimeHelper.CompareYearMonth(firstDayOfMonth, DateTime.MaxValue) == 0)
                {
                    NextButton.IsEnabled = false;
                }
                else
                {
                    // Since we are sure DisplayDate is not equal to
                    // DateTime.MaxValue, it is safe to use AddMonths  
                    var firstDayOfNextMonth = _calendar.AddMonths(firstDayOfMonth, 1);
                    NextButton.IsEnabled = DateTimeHelper.CompareDays(Owner.DisplayDateRangeEnd, firstDayOfNextMonth) > -1;
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="firstDayOfMonth">Inherited code: Requires comment 1.</param>
        private void SetMonthModePreviousButton(DateTime firstDayOfMonth)
        {
            if (Owner != null && PreviousButton != null)
            {
                PreviousButton.IsEnabled = DateTimeHelper.CompareDays(Owner.DisplayDateRangeStart, firstDayOfMonth) < 0;
            }
        }
        #endregion Month Mode

        #region Year Mode
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal void UpdateYearMode(bool isQuarter)
        {
            if (Owner != null)
            {
                Debug.Assert(Owner.SelectedMonth != null, "The Owner Calendar's SelectedMonth should not be null!");
                _currentMonth = Owner.SelectedMonth;
            }
            else
            {
                _currentMonth = DateTime.Today;
            }

            if (_currentMonth != null)
            {
                SetYearModeHeaderButton();
                SetYearModePreviousButton();
                SetYearModeNextButton();

                if (YearView != null)
                {
                    SetMonthButtonsForYearMode(isQuarter);
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="decade">Inherited code: Requires comment 1.</param>
        /// <param name="decadeEnd">Inherited code: Requires comment 2.</param>
        private void SetYearButtons(int decade, int decadeEnd)
        {
            int year;
            var count = -1;
            foreach (object child in YearView.Children)
            {
                var  childButton = child as CalendarButton; 
                Debug.Assert(childButton != null, "childButton should not be null!");
                year = decade + count;

                if (year <= DateTime.MaxValue.Year && year >= DateTime.MinValue.Year)
                {
                    // There should be no time component. Time is 12:00 AM
                    var day = new DateTime(year, 1, 1);
                    childButton.DataContext = day;
                    childButton.Content = year.ToString(DateTimeHelper.GetCurrentDateFormat());
                    childButton.Visibility = Visibility.Visible;

                    if (Owner != null)
                    {
                        if (year == Owner.SelectedYear.Year)
                        {
                            Owner.FocusCalendarButton = childButton;
                            childButton.IsCalendarButtonFocused = Owner.HasFocusInternal;
                        }
                        else
                        {
                            childButton.IsCalendarButtonFocused = false;
                        }
                        childButton.IsSelected = Owner.DisplayDate.Year == year;

                        if (year < Owner.DisplayDateRangeStart.Year || year > Owner.DisplayDateRangeEnd.Year)
                        {
                            childButton.IsEnabled = false;
                            childButton.Opacity = 0;
                        }
                        else
                        {
                            childButton.IsEnabled = true;
                            childButton.Opacity = 1;
                        }
                    }

                    // SET IF THE YEAR IS INACTIVE OR NOT: set if the year is a
                    // trailing year or not
                    childButton.IsInactive = year < decade || year > decadeEnd;
                }
                else
                {
                    childButton.IsEnabled = false;
                    childButton.Opacity = 0;
                }

                count++;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void SetYearModeHeaderButton()
        {
            if (HeaderButton != null)
            {
                HeaderButton.IsEnabled = true;
                HeaderButton.Content = _currentMonth.Year.ToString(DateTimeHelper.GetCurrentDateFormat());
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void SetYearModePreviousButton()
        {
            if (Owner != null && PreviousButton != null)
            {
                PreviousButton.IsEnabled = Owner.DisplayDateRangeStart.Year != _currentMonth.Year;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void SetYearModeNextButton()
        {
            if (Owner != null && NextButton != null)
            {
                NextButton.IsEnabled = Owner.DisplayDateRangeEnd.Year != _currentMonth.Year;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="calendarButton">Inherited code: Requires comment 1.</param>
        internal void UpdateYearViewSelection(CalendarButton  calendarButton)
        {
            if (Owner != null && calendarButton != null && calendarButton.DataContext != null)
            {
                Owner.FocusCalendarButton.IsCalendarButtonFocused = false;
                Owner.FocusCalendarButton = calendarButton;
                calendarButton.IsCalendarButtonFocused = Owner.HasFocusInternal;

                if (Owner.DisplayMode == CalendarMode.Year)
                {
                    Owner.SelectedMonth = (DateTime) calendarButton.DataContext;
                }
                else
                {
                    Owner.SelectedYear = (DateTime) calendarButton.DataContext;
                }
            }
        }
        #endregion Year Mode

        #region Decade Mode
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal void UpdateDecadeMode()
        {
            DateTime selectedYear;

            if (Owner != null)
            {
                Debug.Assert(Owner.SelectedYear != null, "The owning Calendar's selected year should not be null!");
                selectedYear = Owner.SelectedYear;
                _currentMonth = Owner.SelectedMonth;
            }
            else
            {
                _currentMonth = DateTime.Today;
                selectedYear = DateTime.Today;
            }

            if (_currentMonth != null)
            {
                var decade = DateTimeHelper.DecadeOfDate(selectedYear);
                var decadeEnd = DateTimeHelper.EndOfDecade(selectedYear);

                SetDecadeModeHeaderButton(decade, decadeEnd);
                SetDecadeModePreviousButton(decade);
                SetDecadeModeNextButton(decadeEnd);

                if (YearView != null)
                {
                    SetYearButtons(decade, decadeEnd);
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="decade">Inherited code: Requires comment 1.</param>
        /// <param name="decadeEnd">Inherited code: Requires comment 2.</param>
        private void SetDecadeModeHeaderButton(int decade, int decadeEnd)
        {
            if (HeaderButton != null)
            {
                HeaderButton.Content = decade.ToString(CultureInfo.CurrentCulture) + "-" + decadeEnd.ToString(CultureInfo.CurrentCulture);
                HeaderButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="decadeEnd">Inherited code: Requires comment 1.</param>
        private void SetDecadeModeNextButton(int decadeEnd)
        {
            if (Owner != null && NextButton != null)
            {
                NextButton.IsEnabled = Owner.DisplayDateRangeEnd.Year > decadeEnd;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="decade">Inherited code: Requires comment 1.</param>
        private void SetDecadeModePreviousButton(int decade)
        {
            if (Owner != null && PreviousButton != null)
            {
                PreviousButton.IsEnabled = decade > Owner.DisplayDateRangeStart.Year;
            }
        }
        #endregion Decade Mode

        #region Cell Mouse Events
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        internal void Cell_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Owner != null)
            {
                var  b = sender as CalendarDayButton;
                if (_isMouseLeftButtonDown && b != null && b.IsEnabled && !b.IsBlackout)
                {
                    // Update the states of all buttons to be selected starting
                    // from HoverStart to b
                    switch (Owner.SelectionMode)
                    {
                        case CalendarSelectionMode.SingleDate:
                            {
                                var selectedDate = (DateTime) b.DataContext;
                                Owner.DatePickerDisplayDateFlag = true;
                                if (Owner.SelectedDates.Count == 0)
                                {
                                    Owner.SelectedDates.Add(selectedDate);
                                }
                                else
                                {
                                    Owner.SelectedDates[0] = selectedDate;
                                }
                                return;
                            }
                        case CalendarSelectionMode.SingleRange:
                        case CalendarSelectionMode.MultipleRange:
                            {
                                Debug.Assert(b.DataContext != null, "The DataContext should not be null!");
                                Owner.UnHighlightDays();
                                Owner.HoverEndIndex = b.Index;
                                Owner.HoverEnd = (DateTime) b.DataContext;
                                // Update the States of the buttons
                                Owner.HighlightDays();
                                return;
                            }
                    }
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        internal void Cell_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_isMouseLeftButtonDown)
            {
                var  b = sender as CalendarDayButton;
                // The button is in Pressed state. Change the state to normal.
                b.ReleaseMouseCapture();
                // null check is added for unit tests
                if (_downEventArg != null)
                {
                    b.SendMouseLeftButtonUp(_downEventArg);
                }
                _lastCalendarDayButton = b;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        internal void Cell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Owner != null)
            {
                if (!Owner.HasFocusInternal)
                {
                    Owner.Focus();
                }

                bool ctrl, shift;
                CalendarExtensions.GetMetaKeyState(out ctrl, out shift);
                var  b = sender as CalendarDayButton;

                if (b != null)
                {
                    if (b.IsEnabled && !b.IsBlackout)
                    {
                        var selectedDate = (DateTime) b.DataContext;
                        Debug.Assert(selectedDate != null, "selectedDate should not be null!");
                        _isMouseLeftButtonDown = true;
                        // null check is added for unit tests
                        if (e != null)
                        {
                            _downEventArg = e;
                        }

                        switch (Owner.SelectionMode)
                        {
                            case CalendarSelectionMode.None:
                                {
                                    return;
                                }
                            case CalendarSelectionMode.SingleDate:
                                {
                                    Owner.DatePickerDisplayDateFlag = true;
                                    if (Owner.SelectedDates.Count == 0)
                                    {
                                        Owner.SelectedDates.Add(selectedDate);
                                    }
                                    else
                                    {
                                        Owner.SelectedDates[0] = selectedDate;
                                    }
                                    return;
                                }
                            case CalendarSelectionMode.SingleRange:
                                {
                                    // Set the start or end of the selection
                                    // range
                                    if (shift)
                                    {
                                        Owner.UnHighlightDays();
                                        Owner.HoverEnd = selectedDate;
                                        Owner.HoverEndIndex = b.Index;
                                        Owner.HighlightDays();
                                    }
                                    else
                                    {
                                        Owner.UnHighlightDays();
                                        Owner.HoverStart = selectedDate;
                                        Owner.HoverStartIndex = b.Index;
                                    }
                                    return;
                                }
                            case CalendarSelectionMode.MultipleRange:
                                {
                                    if (shift)
                                    {
                                        if (!ctrl)
                                        {
                                            // clear the list, set the states to
                                            // default
                                            foreach (var item in Owner.SelectedDates)
                                            {
                                                Owner.RemovedItems.Add(item);
                                            }
                                            Owner.SelectedDates.ClearInternal();
                                        }
                                        Owner.HoverEnd = selectedDate;
                                        Owner.HoverEndIndex = b.Index;
                                        Owner.HighlightDays();
                                    }
                                    else
                                    {
                                        if (!ctrl)
                                        {
                                            // clear the list, set the states to
                                            // default
                                            foreach (var item in Owner.SelectedDates)
                                            {
                                                Owner.RemovedItems.Add(item);
                                            }
                                            Owner.SelectedDates.ClearInternal();
                                            Owner.UnHighlightDays();
                                        }
                                        Owner.HoverStart = selectedDate;
                                        Owner.HoverStartIndex = b.Index;
                                    }
                                    return;
                                }
                        }
                    }
                    else
                    {
                        // If a click occurs on a BlackOutDay we set the
                        // HoverStart to be null
                        Owner.HoverStart = null;
                    }
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="b">Inherited code: Requires comment 1.</param>
        private void AddSelection(CalendarDayButton   b)
        {
            if (Owner != null)
            {
                Owner.HoverEndIndex = b.Index;
                Owner.HoverEnd = (DateTime) b.DataContext;

                if (Owner.HoverEnd != null && Owner.HoverStart != null)
                {
                    // this is selection with Mouse, we do not guarantee the
                    // range does not include BlackOutDates.  AddRange method
                    // will throw away the BlackOutDates based on the
                    // SelectionMode
                    Owner.IsMouseSelection = true;
                    Owner.SelectedDates.AddRange(Owner.HoverStart.Value, Owner.HoverEnd.Value);
                    Owner.OnDayClick((DateTime) b.DataContext);
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        internal void Cell_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Owner != null)
            {
                bool ctrl, shift;
                CalendarExtensions.GetMetaKeyState(out ctrl, out shift);
                var  b = sender as CalendarDayButton;
                if (b != null && !b.IsBlackout)
                {
                    Owner.OnDayButtonMouseUp(e);
                }
                _isMouseLeftButtonDown = false;
                if (b != null && b.DataContext != null)
                {
                    if (Owner.SelectionMode == CalendarSelectionMode.None || Owner.SelectionMode == CalendarSelectionMode.SingleDate)
                    {
                        Owner.OnDayClick((DateTime) b.DataContext);
                        return;
                    }
                    if (Owner.HoverStart.HasValue)
                    {
                        switch (Owner.SelectionMode)
                        {
                            case CalendarSelectionMode.SingleRange:
                                {
                                    // Update SelectedDates
                                    foreach (var item in Owner.SelectedDates)
                                    {
                                        Owner.RemovedItems.Add(item);
                                    }
                                    Owner.SelectedDates.ClearInternal();
                                    AddSelection(b);
                                    return;
                                }
                            case CalendarSelectionMode.MultipleRange:
                                {
                                    // add the selection (either single day or
                                    // SingleRange day)
                                    AddSelection(b);
                                    return;
                                }
                        }
                    }
                    else
                    {
                        // If the day is Disabled but a trailing day we should
                        // be able to switch months
                        if (b.IsInactive && b.IsBlackout)
                        {
                            Owner.OnDayClick((DateTime) b.DataContext);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            if (Owner != null)
            {
                bool ctrl, shift;
                CalendarExtensions.GetMetaKeyState(out ctrl, out shift);

                if (ctrl && Owner.SelectionMode == CalendarSelectionMode.MultipleRange)
                {
                    var  b = sender as CalendarDayButton;
                    Debug.Assert(b != null, "The sender should be a non-null CalendarDayButton!");

                    if (b.IsSelected)
                    {
                        Owner.HoverStart = null;
                        _isMouseLeftButtonDown = false;
                        b.IsSelected = false;
                        if (b.DataContext != null)
                        {
                            Owner.SelectedDates.Remove((DateTime) b.DataContext);
                        }
                    }
                }
            }
        }
        #endregion Cell Mouse Events

        #region Mouse Events
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        internal void HeaderButton_Click(object sender, RoutedEventArgs e)
        {
            if (Owner != null)
            {
                if (!Owner.HasFocusInternal)
                {
                    Owner.Focus();
                }
                var b = sender as Button;
                DateTime d;

                if (b.IsEnabled)
                {
                    if (Owner.DisplayMode == CalendarMode.Month)
                    {
                        if (Owner.DisplayDate != null)
                        {
                            d = Owner.DisplayDateInternal;
                            Owner.SelectedMonth = new DateTime(d.Year, d.Month, 1);
                        }
                        Owner.DisplayMode = CalendarMode.Year;
                    }
                    else if (Owner.DisplayMode == CalendarMode.Quarter)
                    {
                        if (Owner.DisplayDate != null)
                        {
                            d = Owner.DisplayDateInternal;
                            Owner.SelectedMonth = new DateTime(d.Year, d.Month, 1);
                        }
                        Owner.DisplayMode = CalendarMode.Decade;
                    }
                    else
                    {
                        Debug.Assert(Owner.DisplayMode == CalendarMode.Year, "The Owner Calendar's DisplayMode should be Year!");

                        if (Owner.SelectedMonth != null)
                        {
                            d = Owner.SelectedMonth;
                            Owner.SelectedYear = new DateTime(d.Year, d.Month, 1);
                        }
                        Owner.DisplayMode = CalendarMode.Decade;
                    }
                }
            }
        }


      

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        internal void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (Owner != null)
            {
                if (!Owner.HasFocusInternal)
                {
                    Owner.Focus();
                }

                var b = sender as Button;
                if (b.IsEnabled)
                {
                    Owner.OnPreviousClick();
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        internal void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (Owner != null)
            {
                if (!Owner.HasFocusInternal)
                {
                    Owner.Focus();
                }
                var b = sender as Button;

                if (b.IsEnabled)
                {
                    Owner.OnNextClick();
                }
            }
        }

        private void Quarter_CalendarButtonMouseDown(object sender, MouseButtonEventArgs e)
        {
            var  b = sender as CalendarButton; 
            Debug.Assert(b != null, "The sender should be a non-null CalendarDayButton!");

            _isMouseLeftButtonDownQuarterView = true;

            if (e != null)
            {
                _downEventArgQuarterView = e;
            }

            UpdateYearViewSelection(b);
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        internal void Quarter_CalendarButtonMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseLeftButtonDownQuarterView = false;

            if (Owner != null)
            {
                var newmonth = (DateTime)((CalendarButton) sender).DataContext;

                if (Owner.DisplayMode == CalendarMode.Quarter)
                {
                    Owner.DisplayDate = newmonth;
                    //Owner.DisplayMode = BaseTool.CalendarMode.Month;
                }
                else
                {
                    Debug.Assert(Owner.DisplayMode == CalendarMode.Decade, "The owning BaseTool.Calendar should be in decade mode!");
                    Owner.SelectedMonth = newmonth;
                    Owner.DisplayMode = CalendarMode.Quarter;
                }
                Owner.DatePickerDisplayDateFlag = true;
                Owner.SelectedQuarter = (DateTime)((CalendarButton) sender).DataContext;
                Owner.OnDayButtonMouseUp(e);
                Owner.OnDayClick((DateTime)((CalendarButton) sender).DataContext);
                return;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void Quarter_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_isMouseLeftButtonDownQuarterView)
            {
                var  b = sender as CalendarButton; 
                Debug.Assert(b != null, "The sender should be a non-null CalendarDayButton!");
                UpdateYearViewSelection(b);
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void Quarter_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_isMouseLeftButtonDownQuarterView)
            {
                var  b = sender as CalendarButton; 
                // The button is in Pressed state. Change the state to normal.
                b.ReleaseMouseCapture();
                if (_downEventArgQuarterView != null)
                {
                    b.SendMouseLeftButtonUp(_downEventArgQuarterView);
                }
                _lastCalendarButton = b;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
       
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void Month_CalendarButtonMouseDown(object sender, MouseButtonEventArgs e)
        {
            var  b = sender as CalendarButton; 
            Debug.Assert(b != null, "The sender should be a non-null CalendarDayButton!");

            _isMouseLeftButtonDownYearView = true;

            if (e != null)
            {
                _downEventArgYearView = e;
            }

            UpdateYearViewSelection(b);
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        internal void Month_CalendarButtonMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseLeftButtonDownYearView = false;

            if (Owner != null)
            {
                var newmonth = (DateTime)((CalendarButton) sender).DataContext;

                if (Owner.DisplayMode == CalendarMode.Year)
                {
                    Owner.DisplayDate = newmonth;
                    if (Owner.Dateformat == DatePickerFormat.Season)
                    {
                        Owner.DisplayMode = CalendarMode.Quarter;
                        YearView.Visibility = Visibility.Collapsed;
                        MonthView.Visibility = Visibility.Collapsed;
                        QuarterView.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        Owner.DisplayMode = CalendarMode.Month;
                    }
                }
                else if (Owner.DisplayMode == CalendarMode.Decade)
                {
                    Owner.SelectedMonth = newmonth;
                    if (Owner.Dateformat == DatePickerFormat.Season)
                    {
                        Owner.DisplayMode = CalendarMode.Quarter;
                        YearView.Visibility = Visibility.Collapsed;
                        MonthView.Visibility = Visibility.Collapsed;
                        QuarterView.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        Owner.DisplayMode = CalendarMode.Year;
                        YearView.Visibility = Visibility.Visible;
                        MonthView.Visibility = Visibility.Collapsed;
                        QuarterView.Visibility = Visibility.Collapsed;
                    }
                }
                else if (Owner.DisplayMode == CalendarMode.Quarter)
                {
                    YearView.Visibility = Visibility.Collapsed;
                    MonthView.Visibility = Visibility.Collapsed;
                    QuarterView.Visibility = Visibility.Visible;
                }

                //else
                //{
                //    Debug.Assert(Owner.DisplayMode == BaseTool.CalendarMode.Decade, "The owning BaseTool.Calendar should be in decade mode!");
                //    Owner.SelectedMonth = newmonth;
                //    Owner.DisplayMode = BaseTool.CalendarMode.Year;
                //}
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void Month_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_isMouseLeftButtonDownYearView)
            {
                var  b = sender as CalendarButton; 
                Debug.Assert(b != null, "The sender should be a non-null CalendarDayButton!");
                UpdateYearViewSelection(b);
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void Month_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_isMouseLeftButtonDownYearView)
            {
                var  b = sender as CalendarButton; 
                // The button is in Pressed state. Change the state to normal.
                b.ReleaseMouseCapture();
                if (_downEventArgYearView != null)
                {
                    b.SendMouseLeftButtonUp(_downEventArgYearView);
                }
                _lastCalendarButton = b;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void MonthView_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_lastCalendarDayButton != null)
            {
                _lastCalendarDayButton.CaptureMouse();
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void YearView_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_lastCalendarButton != null)
            {
                _lastCalendarButton.CaptureMouse();
            }
        }

        private void QuarterView_MouseLeave(object sender, MouseEventArgs e)
        { 
            if (_lastCalendarButton != null)
            {
                _lastCalendarButton.CaptureMouse();
            }
        }
        #endregion Mouse Events
    }
}