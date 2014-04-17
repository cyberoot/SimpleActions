using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using cyberoot.SimpleActions.Model;
using cyberoot.SimpleActions.Model.Actions;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;

namespace cyberoot.SimpleActions.Settings
{
    class SettingsStore
    {
        private readonly IServiceProvider _serviceProvider;
        private SettingsManager settingsManager;
        private WritableSettingsStore userSettingsStore;

        private string root;

        public SettingsStore(string _root, IServiceProvider serviceProvider)
        {
            root = _root;
            _serviceProvider = serviceProvider;
            settingsManager = new ShellSettingsManager(_serviceProvider);
            userSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
        }

        public void Write(string path, string key, string value)
        {
            userSettingsStore.SetString(path, key, value);
        }

        public string Read(string path, string key, string def = null)
        {
            return userSettingsStore.GetString(path, key, def);
        }

        public IEnumerable<string> ReadList(string path)
        {
            if (!userSettingsStore.CollectionExists(path))
            {
                return Enumerable.Empty<string>();
            }
            return userSettingsStore.GetSubCollectionNames(path);
        }

        public void WriteActionList(string path, IEnumerable<IAction> actions)
        {
            if (userSettingsStore.CollectionExists(path))
            {
                userSettingsStore.DeleteCollection(path);
            }
            var i = 1;
            foreach (var action in actions)
            {
                var actionKey = "Action #" + i++;
                var actionRoot = path + "\\" + actionKey;
                userSettingsStore.CreateCollection(actionRoot);
                var actionType = action.GetType();
                var properties = actionType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var property in properties)
                {
                    var value = actionType.GetProperty(property.Name).GetValue(action, null);
                    if (value != null)
                    {
                        Write(actionRoot, property.Name, value.ToString());
                    }
                }
                Write(actionRoot, "ActionKind", action.GetKind());
            }
        }

        public IEnumerable<ActionBatch> ReadActionBatches()
        {
            var actionBatches = ReadList(root + "\\Commands");
            foreach (var actionBatch in actionBatches)
            {
                var actionBatchRoot = root + "\\Commands\\" + actionBatch;
                var batch = new ActionBatch
                {
                    Title = Read(actionBatchRoot, "Title"),
                    Description = Read(actionBatchRoot, "Description"),
                    Actions = ReadActionList(actionBatchRoot + "\\Actions").ToList()
                };
                yield return batch;
            }
        }

        public void WriteActionBatches(IEnumerable<ActionBatch> actionBatches)
        {
            var actionBatchRoot = root + "\\Commands";
            if (userSettingsStore.CollectionExists(actionBatchRoot))
            {
                userSettingsStore.DeleteCollection(actionBatchRoot);
            }
            var i = 1;
            foreach (var actionBatch in actionBatches)
            {
                var actionBatchKey = "Command #" + i++;
                var commandRoot = actionBatchRoot + "\\" + actionBatchKey;
                userSettingsStore.CreateCollection(commandRoot);
                Write(commandRoot, "Title", actionBatch.Title);
                Write(commandRoot, "Description", actionBatch.Description);
                WriteActionList(commandRoot + "\\Actions", actionBatch.Actions);
            }
        }

        public IEnumerable<IAction> ReadActionList(string path)
        {
            if (!userSettingsStore.CollectionExists(path))
            {
                yield break;
            }

            var actions = ReadList(path);
            foreach (var actionKey in actions)
            {
                yield return GetAction(path, actionKey);
            }
        }

        private IAction GetAction(string path, string actionKey)
        {
            var properties = userSettingsStore.GetPropertyNames(path + "\\" + actionKey);

            var actionKind = Read(path + "\\" + actionKey, "ActionKind", null);
            if (actionKind == null)
            {
                throw new Exception("Bad config");
            }

            IAction action;

            var availableTypeInfo = Assembly.GetExecutingAssembly().DefinedTypes.SingleOrDefault(t => t.Name == actionKind);

            if (availableTypeInfo == null)
            {
                throw new Exception("Bad config");
            }

            var actionType = Type.GetType(availableTypeInfo.FullName);

            if (actionType == null)
            {
                throw new Exception("Bad config");
            }

            action = (IAction)Activator.CreateInstance(actionType);
            
            foreach (var property in properties.Where(p => p != "ActionKind"))
            {
                var propValue = Read(path + "\\" + actionKey, property, null);
                PropertyInfo propertyInfo = action.GetType().GetProperty(property);
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(action, Convert.ChangeType(propValue, propertyInfo.PropertyType), null);
                }
            }
            return action;
        }
    }
}
