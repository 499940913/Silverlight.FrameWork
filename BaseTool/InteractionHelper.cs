using System;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;

namespace BaseTool
{

    [SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic", Justification = "This is not an exception class.")]
    internal interface IUpdateVisualState
    {
        /// <summary>
        /// Update the visual state of the control.
        /// </summary>
        /// <param name="useTransitions">
        /// A value indicating whether to automatically generate transitions to
        /// the new state, or instantly transition to the new state.
        /// </param>
        void UpdateVisualState(bool useTransitions);
    }

    internal sealed partial class InteractionHelper
    {
        // TODO: Consult with user experience experts to validate the double
        // click distance and time thresholds.

        /// <summary>
        /// The threshold used to determine whether two clicks are temporally
        /// local and considered a double click (or triple, quadruple, etc.).
        /// 500 milliseconds is the default double click value on Windows.
        /// This value would ideally be pulled form the system settings.
        /// </summary>
        private const double SequentialClickThresholdInMilliseconds = 500.0;

        /// <summary>
        /// The threshold used to determine whether two clicks are spatially
        /// local and considered a double click (or triple, quadruple, etc.)
        /// in pixels squared.  We use pixels squared so that we can compare to
        /// the distance delta without taking a square root.
        /// </summary>
        private const double SequentialClickThresholdInPixelsSquared = 3.0 * 3.0;

        /// <summary>
        /// Gets the control the InteractionHelper is targeting.
        /// </summary>
        public Control Control { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the control has focus.
        /// </summary>
        public bool IsFocused { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the mouse is over the control.
        /// </summary> 
        public bool IsMouseOver { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the read-only property is set.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Linked file.")]
        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the mouse button is pressed down
        /// over the control.
        /// </summary>
        public bool IsPressed { get; private set; }

        /// <summary>
        /// Gets or sets the last time the control was clicked.
        /// </summary>
        /// <remarks>
        /// The value is stored as Utc time because it is slightly more
        /// performant than converting to local time.
        /// </remarks>
        private DateTime LastClickTime { get; set; }

        /// <summary>
        /// Gets or sets the mouse position of the last click.
        /// </summary>
        /// <remarks>The value is relative to the control.</remarks>
        private Point LastClickPosition { get; set; }

        /// <summary>
        /// Gets the number of times the control was clicked.
        /// </summary>
        public int ClickCount { get; private set; }

        /// <summary>
        /// Reference used to call UpdateVisualState on the base class.
        /// </summary>
        private IUpdateVisualState _updateVisualState;

        /// <summary>
        /// Initializes a new instance of the InteractionHelper class.
        /// </summary>
        /// <param name="control">Control receiving interaction.</param>
        public InteractionHelper(Control control)
        {
            Debug.Assert(control != null, "control should not be null!");
            Control = control;
            _updateVisualState = control as IUpdateVisualState;

            // Wire up the event handlers for events without a virtual override
            control.Loaded += OnLoaded;
            control.IsEnabledChanged += OnIsEnabledChanged;
        }

        #region UpdateVisualState
        /// <summary>
        /// Update the visual state of the control.
        /// </summary>
        /// <param name="useTransitions">
        /// A value indicating whether to automatically generate transitions to
        /// the new state, or instantly transition to the new state.
        /// </param>
        /// <remarks>
        /// UpdateVisualState works differently than the rest of the injected
        /// functionality.  Most of the other events are overridden by the
        /// calling class which calls Allow, does what it wants, and then calls
        /// Base.  UpdateVisualState is the opposite because a number of the
        /// methods in InteractionHelper need to trigger it in the calling
        /// class.  We do this using the IUpdateVisualState internal interface.
        /// </remarks>
        private void UpdateVisualState(bool useTransitions)
        {
            if (_updateVisualState != null)
            {
                _updateVisualState.UpdateVisualState(useTransitions);
            }
        }

        /// <summary>
        /// Update the visual state of the control.
        /// </summary>
        /// <param name="useTransitions">
        /// A value indicating whether to automatically generate transitions to
        /// the new state, or instantly transition to the new state.
        /// </param>
        public void UpdateVisualStateBase(bool useTransitions)
        {
            // Handle the Common states
            if (!Control.IsEnabled)
            {
                VisualStates.GoToState(Control, useTransitions, VisualStates.StateDisabled, VisualStates.StateNormal);
            }
            else if (IsReadOnly)
            {
                VisualStates.GoToState(Control, useTransitions, VisualStates.StateReadOnly, VisualStates.StateNormal);
            }
            else if (IsPressed)
            {
                VisualStates.GoToState(Control, useTransitions, VisualStates.StatePressed, VisualStates.StateMouseOver, VisualStates.StateNormal);
            }
            else if (IsMouseOver)
            {
                VisualStates.GoToState(Control, useTransitions, VisualStates.StateMouseOver, VisualStates.StateNormal);
            }
            else
            {
                VisualStates.GoToState(Control, useTransitions, VisualStates.StateNormal);
            }

            // Handle the Focused states
            if (IsFocused)
            {
                VisualStates.GoToState(Control, useTransitions, VisualStates.StateFocused, VisualStates.StateUnfocused);
            }
            else
            {
                VisualStates.GoToState(Control, useTransitions, VisualStates.StateUnfocused);
            }
        }
        #endregion UpdateVisualState

        /// <summary>
        /// Handle the control's Loaded event.
        /// </summary>
        /// <param name="sender">The control.</param>
        /// <param name="e">Event arguments.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            UpdateVisualState(false);
        }

        /// <summary>
        /// Handle changes to the control's IsEnabled property.
        /// </summary>
        /// <param name="sender">The control.</param>
        /// <param name="e">Event arguments.</param>
        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var enabled = (bool)e.NewValue;
            if (!enabled)
            {
                IsPressed = false;
                IsMouseOver = false;
                IsFocused = false;
            }

            UpdateVisualState(true);
        }

        /// <summary>
        /// Handles changes to the control's IsReadOnly property.
        /// </summary>
        /// <param name="value">The value of the property.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Linked file.")]
        public void OnIsReadOnlyChanged(bool value)
        {
            IsReadOnly = value;
            if (!value)
            {
                IsPressed = false;
                IsMouseOver = false;
                IsFocused = false;
            }

            UpdateVisualState(true);
        }

        /// <summary>
        /// Update the visual state of the control when its template is changed.
        /// </summary>
        public void OnApplyTemplateBase()
        {
            UpdateVisualState(false);
        }

        #region GotFocus
        /// <summary>
        /// Check if the control's GotFocus event should be handled.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        /// <returns>
        /// A value indicating whether the event should be handled.
        /// </returns>
        public bool AllowGotFocus(RoutedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            var enabled = Control.IsEnabled;
            if (enabled)
            {
                IsFocused = true;
            }
            return enabled;
        }

        /// <summary>
        /// Base implementation of the virtual GotFocus event handler.
        /// </summary>
        public void OnGotFocusBase()
        {
            UpdateVisualState(true);
        }
        #endregion GotFocus

        #region LostFocus
        /// <summary>
        /// Check if the control's LostFocus event should be handled.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        /// <returns>
        /// A value indicating whether the event should be handled.
        /// </returns>
        public bool AllowLostFocus(RoutedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            var enabled = Control.IsEnabled;
            if (enabled)
            {
                IsFocused = false;
            }
            return enabled;
        }

        /// <summary>
        /// Base implementation of the virtual LostFocus event handler.
        /// </summary>
        public void OnLostFocusBase()
        {
            IsPressed = false;
            UpdateVisualState(true);
        }
        #endregion LostFocus

        #region MouseEnter
        /// <summary>
        /// Check if the control's MouseEnter event should be handled.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        /// <returns>
        /// A value indicating whether the event should be handled.
        /// </returns>
        public bool AllowMouseEnter(MouseEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            var enabled = Control.IsEnabled;
            if (enabled)
            {
                IsMouseOver = true;
            }
            return enabled;
        }

        /// <summary>
        /// Base implementation of the virtual MouseEnter event handler.
        /// </summary>
        public void OnMouseEnterBase()
        {
            UpdateVisualState(true);
        }
        #endregion MouseEnter

        #region MouseLeave
        /// <summary>
        /// Check if the control's MouseLeave event should be handled.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        /// <returns>
        /// A value indicating whether the event should be handled.
        /// </returns>
        public bool AllowMouseLeave(MouseEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            var enabled = Control.IsEnabled;
            if (enabled)
            {
                IsMouseOver = false;
            }
            return enabled;
        }

        /// <summary>
        /// Base implementation of the virtual MouseLeave event handler.
        /// </summary>
        public void OnMouseLeaveBase()
        {
            UpdateVisualState(true);
        }
        #endregion MouseLeave

        #region MouseLeftButtonDown
        /// <summary>
        /// Check if the control's MouseLeftButtonDown event should be handled.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        /// <returns>
        /// A value indicating whether the event should be handled.
        /// </returns>
        public bool AllowMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            var enabled = Control.IsEnabled;
            if (enabled)
            {
                // Get the current position and time
                var now = DateTime.UtcNow;
                var position = e.GetPosition(Control);

                // Compute the deltas from the last click
                var timeDelta = (now - LastClickTime).TotalMilliseconds;
                var lastPosition = LastClickPosition;
                var dx = position.X - lastPosition.X;
                var dy = position.Y - lastPosition.Y;
                var distance = dx * dx + dy * dy;

                // Check if the values fall under the sequential click temporal
                // and spatial thresholds
                if (timeDelta < SequentialClickThresholdInMilliseconds &&
                    distance < SequentialClickThresholdInPixelsSquared)
                {
                    // TODO: Does each click have to be within the single time
                    // threshold on WPF?
                    ClickCount++;
                }
                else
                {
                    ClickCount = 1;
                }

                // Set the new position and time
                LastClickTime = now;
                LastClickPosition = position;

                // Raise the event
                IsPressed = true;
            }
            else
            {
                ClickCount = 1;
            }

            return enabled;
        }

        /// <summary>
        /// Base implementation of the virtual MouseLeftButtonDown event
        /// handler.
        /// </summary>
        public void OnMouseLeftButtonDownBase()
        {
            UpdateVisualState(true);
        }
        #endregion MouseLeftButtonDown

        #region MouseLeftButtonUp
        /// <summary>
        /// Check if the control's MouseLeftButtonUp event should be handled.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        /// <returns>
        /// A value indicating whether the event should be handled.
        /// </returns>
        public bool AllowMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            var enabled = Control.IsEnabled;
            if (enabled)
            {
                IsPressed = false;
            }
            return enabled;
        }

        /// <summary>
        /// Base implementation of the virtual MouseLeftButtonUp event handler.
        /// </summary>
        public void OnMouseLeftButtonUpBase()
        {
            UpdateVisualState(true);
        }
        #endregion MouseLeftButtonUp

        #region KeyDown
        /// <summary>
        /// Check if the control's KeyDown event should be handled.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        /// <returns>
        /// A value indicating whether the event should be handled.
        /// </returns>
        public bool AllowKeyDown(KeyEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            return Control.IsEnabled;
        }
        #endregion KeyDown

        #region KeyUp
        /// <summary>
        /// Check if the control's KeyUp event should be handled.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        /// <returns>
        /// A value indicating whether the event should be handled.
        /// </returns>
        public bool AllowKeyUp(KeyEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            return Control.IsEnabled;
        }
        #endregion KeyUp

        #region RightToLeft key translation
        /// <summary>
        /// Translates keys for proper RightToLeft mode support.
        /// </summary>
        /// <param name="flowDirection">Control's flow direction mode.</param>
        /// <param name="originalKey">Original key.</param>
        /// <returns>
        /// A translated key code, indicating how the original key should be interpreted.
        /// </returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Linked file.")]
        public static Key GetLogicalKey(FlowDirection flowDirection, Key originalKey)
        {
            var result = originalKey;
            if (flowDirection == FlowDirection.RightToLeft)
            {
                switch (originalKey)
                {
                    case Key.Left:
                        result = Key.Right;
                        break;
                    case Key.Right:
                        result = Key.Left;
                        break;
                }
            }
            return result;
        }
        #endregion
    }

    internal static class VisualStates
    {
        #region GroupCommon
        /// <summary>
        /// Common state group.
        /// </summary>
        public const string GroupCommon = "CommonStates";

        /// <summary>
        /// Normal state of the Common state group.
        /// </summary>
        public const string StateNormal = "Normal";

        /// <summary>
        /// Normal state of the Common state group.
        /// </summary>
        public const string StateReadOnly = "ReadOnly";

        /// <summary>
        /// MouseOver state of the Common state group.
        /// </summary>
        public const string StateMouseOver = "MouseOver";

        /// <summary>
        /// Pressed state of the Common state group.
        /// </summary>
        public const string StatePressed = "Pressed";

        /// <summary>
        /// Disabled state of the Common state group.
        /// </summary>
        public const string StateDisabled = "Disabled";
        #endregion GroupCommon

        #region GroupFocus
        /// <summary>
        /// Focus state group.
        /// </summary>
        public const string GroupFocus = "FocusStates";

        /// <summary>
        /// Unfocused state of the Focus state group.
        /// </summary>
        public const string StateUnfocused = "Unfocused";

        /// <summary>
        /// Focused state of the Focus state group.
        /// </summary>
        public const string StateFocused = "Focused";
        #endregion GroupFocus

        #region GroupSelection
        /// <summary>
        /// Selection state group.
        /// </summary>
        public const string GroupSelection = "SelectionStates";

        /// <summary>
        /// Selected state of the Selection state group.
        /// </summary>
        public const string StateSelected = "Selected";

        /// <summary>
        /// Unselected state of the Selection state group.
        /// </summary>
        public const string StateUnselected = "Unselected";

        /// <summary>
        /// Selected inactive state of the Selection state group.
        /// </summary>
        public const string StateSelectedInactive = "SelectedInactive";
        #endregion GroupSelection

        #region GroupExpansion
        /// <summary>
        /// Expansion state group.
        /// </summary>
        public const string GroupExpansion = "ExpansionStates";

        /// <summary>
        /// Expanded state of the Expansion state group.
        /// </summary>
        public const string StateExpanded = "Expanded";

        /// <summary>
        /// Collapsed state of the Expansion state group.
        /// </summary>
        public const string StateCollapsed = "Collapsed";
        #endregion GroupExpansion

        #region GroupPopup
        /// <summary>
        /// Popup state group.
        /// </summary>
        public const string GroupPopup = "PopupStates";

        /// <summary>
        /// Opened state of the Popup state group.
        /// </summary>
        public const string StatePopupOpened = "PopupOpened";

        /// <summary>
        /// Closed state of the Popup state group.
        /// </summary>
        public const string StatePopupClosed = "PopupClosed";
        #endregion

        #region GroupValidation
        /// <summary>
        /// ValidationStates state group.
        /// </summary>
        public const string GroupValidation = "ValidationStates";

        /// <summary>
        /// The valid state for the ValidationStates group.
        /// </summary>
        public const string StateValid = "Valid";

        /// <summary>
        /// Invalid, focused state for the ValidationStates group.
        /// </summary>
        public const string StateInvalidFocused = "InvalidFocused";

        /// <summary>
        /// Invalid, unfocused state for the ValidationStates group.
        /// </summary>
        public const string StateInvalidUnfocused = "InvalidUnfocused";
        #endregion

        #region GroupExpandDirection
        /// <summary>
        /// ExpandDirection state group.
        /// </summary>
        public const string GroupExpandDirection = "ExpandDirectionStates";

        /// <summary>
        /// Down expand direction state of ExpandDirection state group.
        /// </summary>
        public const string StateExpandDown = "ExpandDown";

        /// <summary>
        /// Up expand direction state of ExpandDirection state group.
        /// </summary>
        public const string StateExpandUp = "ExpandUp";

        /// <summary>
        /// Left expand direction state of ExpandDirection state group.
        /// </summary>
        public const string StateExpandLeft = "ExpandLeft";

        /// <summary>
        /// Right expand direction state of ExpandDirection state group.
        /// </summary>
        public const string StateExpandRight = "ExpandRight";
        #endregion

        #region GroupHasItems
        /// <summary>
        /// HasItems state group.
        /// </summary>
        public const string GroupHasItems = "HasItemsStates";

        /// <summary>
        /// HasItems state of the HasItems state group.
        /// </summary>
        public const string StateHasItems = "HasItems";

        /// <summary>
        /// NoItems state of the HasItems state group.
        /// </summary>
        public const string StateNoItems = "NoItems";
        #endregion GroupHasItems

        #region GroupIncrease
        /// <summary>
        /// Increment state group.
        /// </summary>
        public const string GroupIncrease = "IncreaseStates";

        /// <summary>
        /// State enabled for increment group.
        /// </summary>
        public const string StateIncreaseEnabled = "IncreaseEnabled";

        /// <summary>
        /// State disabled for increment group.
        /// </summary>
        public const string StateIncreaseDisabled = "IncreaseDisabled";
        #endregion GroupIncrease

        #region GroupDecrease
        /// <summary>
        /// Decrement state group.
        /// </summary>
        public const string GroupDecrease = "DecreaseStates";

        /// <summary>
        /// State enabled for decrement group.
        /// </summary>
        public const string StateDecreaseEnabled = "DecreaseEnabled";

        /// <summary>
        /// State disabled for decrement group.
        /// </summary>
        public const string StateDecreaseDisabled = "DecreaseDisabled";
        #endregion GroupDecrease

        #region GroupIteractionMode
        /// <summary>
        /// InteractionMode state group.
        /// </summary>
        public const string GroupInteractionMode = "InteractionModeStates";

        /// <summary>
        /// Edit of the DisplayMode state group.
        /// </summary>
        public const string StateEdit = "Edit";

        /// <summary>
        /// Display of the DisplayMode state group.
        /// </summary>
        public const string StateDisplay = "Display";
        #endregion GroupIteractionMode

        #region GroupLocked
        /// <summary>
        /// DisplayMode state group.
        /// </summary>
        public const string GroupLocked = "LockedStates";

        /// <summary>
        /// Edit of the DisplayMode state group.
        /// </summary>
        public const string StateLocked = "Locked";

        /// <summary>
        /// Display of the DisplayMode state group.
        /// </summary>
        public const string StateUnlocked = "Unlocked";
        #endregion GroupLocked

        #region GroupActive
        /// <summary>
        /// Active state.
        /// </summary>
        public const string StateActive = "Active";

        /// <summary>
        /// Inactive state.
        /// </summary>
        public const string StateInactive = "Inactive";

        /// <summary>
        /// Active state group.
        /// </summary>
        public const string GroupActive = "ActiveStates";
        #endregion GroupActive

        #region GroupWatermark
        /// <summary>
        /// Non-watermarked state.
        /// </summary>
        public const string StateUnwatermarked = "Unwatermarked";

        /// <summary>
        /// Watermarked state.
        /// </summary>
        public const string StateWatermarked = "Watermarked";

        /// <summary>
        /// Watermark state group.
        /// </summary>
        public const string GroupWatermark = "WatermarkStates";
        #endregion GroupWatermark

        #region GroupCalendarButtonFocus
        /// <summary>
        /// Unfocused state for Calendar Buttons.
        /// </summary>
        public const string StateCalendarButtonUnfocused = "CalendarButtonUnfocused";

        /// <summary>
        /// Focused state for Calendar Buttons.
        /// </summary>
        public const string StateCalendarButtonFocused = "CalendarButtonFocused";

        /// <summary>
        /// CalendarButtons Focus state group.
        /// </summary>
        public const string GroupCalendarButtonFocus = "CalendarButtonFocusStates";
        #endregion GroupCalendarButtonFocus

        #region GroupBusyStatus
        /// <summary>
        /// Busy state for BusyIndicator.
        /// </summary>
        public const string StateBusy = "Busy";

        /// <summary>
        /// Idle state for BusyIndicator.
        /// </summary>
        public const string StateIdle = "Idle";

        /// <summary>
        /// Busyness group name.
        /// </summary>
        public const string GroupBusyStatus = "BusyStatusStates";
        #endregion

        #region GroupVisibility
        /// <summary>
        /// Visible state name for BusyIndicator.
        /// </summary>
        public const string StateVisible = "Visible";

        /// <summary>
        /// Hidden state name for BusyIndicator.
        /// </summary>
        public const string StateHidden = "Hidden";

        /// <summary>
        /// BusyDisplay group.
        /// </summary>
        public const string GroupVisibility = "VisibilityStates";
        #endregion

        /// <summary>
        /// Use VisualStateManager to change the visual state of the control.
        /// </summary>
        /// <param name="control">
        /// Control whose visual state is being changed.
        /// </param>
        /// <param name="useTransitions">
        /// A value indicating whether to use transitions when updating the
        /// visual state, or to snap directly to the new visual state.
        /// </param>
        /// <param name="stateNames">
        /// Ordered list of state names and fallback states to transition into.
        /// Only the first state to be found will be used.
        /// </param>
        public static void GoToState(Control control, bool useTransitions, params string[] stateNames)
        {
            Debug.Assert(control != null, "control should not be null!");
            Debug.Assert(stateNames != null, "stateNames should not be null!");
            Debug.Assert(stateNames.Length > 0, "stateNames should not be empty!");

            foreach (var name in stateNames)
            {
                if (VisualStateManager.GoToState(control, name, useTransitions))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Gets the implementation root of the Control.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject.</param>
        /// <remarks>
        /// Implements Silverlight's corresponding internal property on Control.
        /// </remarks>
        /// <returns>Returns the implementation root or null.</returns>
        public static FrameworkElement GetImplementationRoot(DependencyObject dependencyObject)
        {
            Debug.Assert(dependencyObject != null, "DependencyObject should not be null.");
            return 1 == VisualTreeHelper.GetChildrenCount(dependencyObject) ?
                VisualTreeHelper.GetChild(dependencyObject, 0) as FrameworkElement :
                null;
        }

        /// <summary>
        /// This method tries to get the named VisualStateGroup for the 
        /// dependency object. The provided object's ImplementationRoot will be 
        /// looked up in this call.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="groupName">The visual state group's name.</param>
        /// <returns>Returns null or the VisualStateGroup object.</returns>
        public static VisualStateGroup TryGetVisualStateGroup(DependencyObject dependencyObject, string groupName)
        {
            var root = GetImplementationRoot(dependencyObject);
            if (root == null)
            {
                return null;
            }

            return VisualStateManager.GetVisualStateGroups(root)
                .OfType<VisualStateGroup>()
                .Where(group => string.CompareOrdinal(groupName, group.Name) == 0)
                .FirstOrDefault();
        }
    }


    internal sealed partial class ItemsControlHelper
    {
        /// <summary>
        /// Gets or sets the ItemsControl being tracked by the
        /// ItemContainerGenerator.
        /// </summary>
        private ItemsControl ItemsControl { get; set; }

        /// <summary>
        /// A Panel that is used as the ItemsHost of the ItemsControl.  This
        /// property will only be valid when the ItemsControl is live in the
        /// tree and has generated containers for some of its items.
        /// </summary>
        private Panel _itemsHost;

        /// <summary>
        /// Gets a Panel that is used as the ItemsHost of the ItemsControl.
        /// This property will only be valid when the ItemsControl is live in
        /// the tree and has generated containers for some of its items.
        /// </summary>
        internal Panel ItemsHost
        {
            get
            {
                // Lookup the ItemsHost if we haven't already cached it.
                if (_itemsHost == null && ItemsControl != null && ItemsControl.ItemContainerGenerator != null)
                {
                    // Get any live container
                    var container = ItemsControl.ItemContainerGenerator.ContainerFromIndex(0);
                    if (container != null)
                    {
                        // Get the parent of the container
                        _itemsHost = VisualTreeHelper.GetParent(container) as Panel;
                    }
                }

                return _itemsHost;
            }
        }

        /// <summary>
        /// A ScrollViewer that is used to scroll the items in the ItemsHost.
        /// </summary>
        private ScrollViewer _scrollHost;

        /// <summary>
        /// Gets a ScrollViewer that is used to scroll the items in the
        /// ItemsHost.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is linked into multiple projects.")]
        internal ScrollViewer ScrollHost
        {
            get
            {
                if (_scrollHost == null)
                {
                    var itemsHost = ItemsHost;
                    if (itemsHost != null)
                    {
                        for (DependencyObject obj = itemsHost; obj != ItemsControl && obj != null; obj = VisualTreeHelper.GetParent(obj))
                        {
                            var viewer = obj as ScrollViewer;
                            if (viewer != null)
                            {
                                _scrollHost = viewer;
                                break;
                            }
                        }
                    }
                }
                return _scrollHost;
            }
        }

        /// <summary>
        /// Initializes a new instance of the ItemContainerGenerator.
        /// </summary>
        /// <param name="control">
        /// The ItemsControl being tracked by the ItemContainerGenerator.
        /// </param>
        internal ItemsControlHelper(ItemsControl control)
        {
            Debug.Assert(control != null, "control cannot be null!");
            ItemsControl = control;
        }

        /// <summary>
        /// Apply a control template to the ItemsControl.
        /// </summary>
        internal void OnApplyTemplate()
        {
            // Clear the cached ItemsHost, ScrollHost
            _itemsHost = null;
            _scrollHost = null;
        }

        /// <summary>
        /// Prepares the specified container to display the specified item.
        /// </summary>
        /// <param name="element">
        /// Container element used to display the specified item.
        /// </param>
        /// <param name="parentItemContainerStyle">
        /// The ItemContainerStyle for the parent ItemsControl.
        /// </param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is linked into multiple projects.")]
        internal static void PrepareContainerForItemOverride(DependencyObject element, Style parentItemContainerStyle)
        {
            // Apply the ItemContainerStyle to the item
            var control = element as Control;
            if (parentItemContainerStyle != null && control != null && control.Style == null)
            {
                control.SetValue(FrameworkElement.StyleProperty, parentItemContainerStyle);
            }

            // Note: WPF also does preparation for ContentPresenter,
            // ContentControl, HeaderedContentControl, and ItemsControl.  Since
            // we don't have any other ItemsControls using this
            // ItemContainerGenerator, we've removed that code for now.  It
            // should be added back later when necessary.
        }

        /// <summary>
        /// Update the style of any generated items when the ItemContainerStyle
        /// has been changed.
        /// </summary>
        /// <param name="itemContainerStyle">The ItemContainerStyle.</param>
        /// <remarks>
        /// Silverlight does not support setting a Style multiple times, so we
        /// only attempt to set styles on elements whose style hasn't already
        /// been set.
        /// </remarks>
        internal void UpdateItemContainerStyle(Style itemContainerStyle)
        {
            if (itemContainerStyle == null)
            {
                return;
            }

            var itemsHost = ItemsHost;
            if (itemsHost == null || itemsHost.Children == null)
            {
                return;
            }

            foreach (var element in itemsHost.Children)
            {
                var obj = element as FrameworkElement;
                if (obj.Style == null)
                {
                    obj.Style = itemContainerStyle;
                }
            }
        }

        /// <summary>
        /// Scroll the desired element into the ScrollHost's viewport.
        /// </summary>
        /// <param name="element">Element to scroll into view.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "File is linked across multiple projects and this method is used in some but not others.")]
        internal void ScrollIntoView(FrameworkElement element)
        {
            // Get the ScrollHost
            var scrollHost = ScrollHost;
            if (scrollHost == null)
            {
                return;
            }

            // Get the position of the element relative to the ScrollHost
            GeneralTransform transform = null;
            try
            {
                transform = element.TransformToVisual(scrollHost);
            }
            catch (ArgumentException)
            {
                // Ignore failures when not in the visual tree
                return;
            }
            var itemRect = new Rect(
                transform.Transform(new Point()),
                transform.Transform(new Point(element.ActualWidth, element.ActualHeight)));

            // Scroll vertically
            var verticalOffset = scrollHost.VerticalOffset;
            double verticalDelta = 0;
            var hostBottom = scrollHost.ViewportHeight;
            var itemBottom = itemRect.Bottom;
            if (hostBottom < itemBottom)
            {
                verticalDelta = itemBottom - hostBottom;
                verticalOffset += verticalDelta;
            }
            var itemTop = itemRect.Top;
            if (itemTop - verticalDelta < 0)
            {
                verticalOffset -= verticalDelta - itemTop;
            }
            scrollHost.ScrollToVerticalOffset(verticalOffset);

            // Scroll horizontally
            var horizontalOffset = scrollHost.HorizontalOffset;
            double horizontalDelta = 0;
            var hostRight = scrollHost.ViewportWidth;
            var itemRight = itemRect.Right;
            if (hostRight < itemRight)
            {
                horizontalDelta = itemRight - hostRight;
                horizontalOffset += horizontalDelta;
            }
            var itemLeft = itemRect.Left;
            if (itemLeft - horizontalDelta < 0)
            {
                horizontalOffset -= horizontalDelta - itemLeft;
            }
            scrollHost.ScrollToHorizontalOffset(horizontalOffset);
        }
    }
}
