using System;
using System.Windows;

namespace BaseTool
{
    public sealed class WeakEvent
    {
        private Action _removeEventHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakEvent"/> class.
        /// </summary>
        /// <param name="removeEventHandler">The remove event handler function.</param>
        private WeakEvent(Action removeEventHandler)
        {
            _removeEventHandler = removeEventHandler;
        }

        /// <summary>
        /// Weakly registers the specified subscriber to the the given event of type 
        /// <see cref="EventHandler"/>.
        /// </summary>
        /// <example>
        /// Application application;
        /// WeakEvent.Register{TextBox, TextChangedEventArgs>(
        ///     this,
        ///     eventHandler => textBox.TextChanged += eventHandler,
        ///     eventHandler => textBox.TextChanged -= eventHandler,
        ///     (sender, e) => this.OnTextChanged(sender, e));
        /// </example>
        /// <typeparam name="TS">The type of the subscriber.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="addEventhandler">The add eventhandler.</param>
        /// <param name="removeEventHandler">The remove event handler function.</param>
        /// <param name="action">The event execution function.</param>
        public static WeakEvent Register<TS>(
            TS subscriber,
            Action<EventHandler> addEventhandler,
            Action<EventHandler> removeEventHandler,
            Action<TS, EventArgs> action)
            where TS : class
        {
            return Register(
                subscriber,
                eventHandler => (sender, e) => eventHandler(sender, e),
                addEventhandler,
                removeEventHandler,
                action);
        }

        /// <summary>
        /// Weakly registers the specified subscriber to the the given event of type 
        /// <see cref="EventHandler{T}"/>.
        /// </summary>
        /// <example>
        /// Application application;
        /// WeakEvent.Register{TextBox, TextChangedEventArgs>(
        ///     this,
        ///     eventHandler => textBox.TextChanged += eventHandler,
        ///     eventHandler => textBox.TextChanged -= eventHandler,
        ///     (sender, e) => this.OnTextChanged(sender, e));
        /// </example>
        /// <typeparam name="TS">The type of the subscriber.</typeparam>
        /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="addEventhandler">The add eventhandler.</param>
        /// <param name="removeEventHandler">The remove event handler function.</param>
        /// <param name="action">The event execution function.</param>
        public static WeakEvent Register<TS, TEventArgs>(
            TS subscriber,
            Action<EventHandler<TEventArgs>> addEventhandler,
            Action<EventHandler<TEventArgs>> removeEventHandler,
            Action<TS, TEventArgs> action)
            where TS : class
            where TEventArgs : EventArgs
        {
            return Register(
                subscriber,
                eventHandler => eventHandler,
                addEventhandler,
                removeEventHandler,
                action);
        }

        /// <summary>
        /// Weakly registers the specified subscriber to the the given event.
        /// </summary>
        /// <example>
        /// TextBox textbox;
        /// WeakEvent.Register{TextBox, TextChangedEventHandler, TextChangedEventArgs>(
        ///     this,
        ///     eventHandler => (sender, e) => eventHandler(sender, e),
        ///     eventHandler => textBox.TextChanged += eventHandler,
        ///     eventHandler => textBox.TextChanged -= eventHandler,
        ///     (sender, e) => this.OnTextChanged(sender, e));
        /// </example>
        /// <typeparam name="TS">The type of the subscriber.</typeparam>
        /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
        /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="getEventHandler">The get event handler function.</param>
        /// <param name="addEventHandler">The add event handler function.</param>
        /// <param name="removeEventHandler">The remove event handler function.</param>
        /// <param name="action">The event execution function.</param>
        public static WeakEvent Register<TS, TEventHandler, TEventArgs>(
            TS subscriber,
            Func<EventHandler<TEventArgs>, TEventHandler> getEventHandler,
            Action<TEventHandler> addEventHandler,
            Action<TEventHandler> removeEventHandler,
            Action<TS, TEventArgs> action)
            where TS : class
            where TEventHandler : class
            where TEventArgs : EventArgs

        {
            WeakReference weakReference = new WeakReference(subscriber);

            TEventHandler[] eventHandler = {null};
            eventHandler[0] = getEventHandler(
                (sender, e) =>
                {
                    TS subscriberStrongRef = weakReference.Target as TS;

                    if (subscriberStrongRef != null)
                    {
                        action(subscriberStrongRef, e);
                    }
                    else
                    {
                        removeEventHandler(eventHandler[0]);
                        eventHandler[0] = null;
                    }
                });

            addEventHandler(eventHandler[0]);

            return new WeakEvent(() => removeEventHandler(eventHandler[0]));
        }

        public static WeakEvent Register<TS>(
            TS subscriber,
            Action<DependencyPropertyChangedEventHandler> addEventHandler,
            Action<DependencyPropertyChangedEventHandler> removeEventHandler,
            Action<TS, DependencyPropertyChangedEventArgs> action)
            where TS : class
        {
            WeakReference weakReference = new WeakReference(subscriber);

            DependencyPropertyChangedEventHandler[] eventHandler = {null};
            eventHandler[0] = (sender, e) =>
            {
                TS subscriberStrongRef = weakReference.Target as TS;

                if (subscriberStrongRef != null)
                {
                    action(subscriberStrongRef, e);
                }
                else
                {
                    removeEventHandler(eventHandler[0]);
                    eventHandler[0] = null;
                }
            };

            addEventHandler(eventHandler[0]);

            return new WeakEvent(() => removeEventHandler(eventHandler[0]));
        }

        /// <summary>
        /// Manually unregisters this instance from the event.
        /// </summary>
        public void Unregister()
        {
            if (_removeEventHandler != null)
            {
                _removeEventHandler();
                _removeEventHandler = null;
            }
        }
    }
}
