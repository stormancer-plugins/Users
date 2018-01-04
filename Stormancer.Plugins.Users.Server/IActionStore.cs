﻿using Stormancer.Diagnostics;
using System;
using System.Collections.Concurrent;

namespace Stormancer.Server.Users
{
    /// <summary>
    /// Provides a way to store actions that can be called once accross nodes.
    /// </summary>
    interface IActionStore
    {
        /// <summary>
        /// Runs the action with the id provided as parameter.
        /// </summary>
        /// <param name="id">The id of the action</param>
        /// <returns>true if an action was executed, false otherwise</returns>
        bool TryRun(string id);

        IDisposable RegisterAction(string id, Action action);
    }

    internal class SingleNodeActionStore : IActionStore
    {
        private readonly ConcurrentDictionary<string, Action> _actions = new ConcurrentDictionary<string, Action>();
        private readonly Func<ILogger> _logger;

        private class Disposable : IDisposable
        {
            private string id;
            private ConcurrentDictionary<string, Action> actions;

            public Disposable(ConcurrentDictionary<string, Action> actions, string id)
            {
                this.id = id;
                this.actions = actions;
            }

            public void Dispose()
            {
                Action action;
                actions.TryRemove(id, out action);
            }
        }
        public SingleNodeActionStore(Func<ILogger> logger)
        {
            _logger = logger;
        }
        public IDisposable RegisterAction(string id, Action action)
        {
            if (!_actions.TryAdd(id, action))
            {
                throw new ArgumentException($"An action is already registered with id '{id}'");
            }
            return new Disposable(_actions, id);
        }

        public bool TryRun(string id)
        {
            Action action;
            var success = _actions.TryRemove(id, out action);
            if (success)
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    _logger().Log(LogLevel.Error, "actionStore", $"An error occured while running the action '{id}'", ex);
                }
            }
            return success;
        }
    }

}
