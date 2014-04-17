using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using cyberoot.SimpleActions.Helpers;
using cyberoot.SimpleActions.Model;
using cyberoot.SimpleActions.Settings;
using Microsoft.VisualStudio.CommandBars;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using EnvDTE80;
using System.Windows.Forms;
using IOLEServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using ISystemServiceProvider = System.IServiceProvider;

namespace cyberoot.SimpleActions
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(UIContextGuids.SolutionExists)]
    [Guid(GuidList.guidSimpleActionsPkgString)]
    public sealed class SimpleActionsPackage : Package
    {
        private DTE2 _applicationObject;

        private SettingsStore settingsStore;

        private IList<ActionBatch> actionBatches;

        private ISystemServiceProvider serviceProvider;

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public SimpleActionsPackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }


        private void InitSimpleActionsMenu(OleMenuCommandService mcs)
        {
            if (mcs == null)
            {
                return;
            }
            for (int i = 0; i < actionBatches.Count; i++)
            {
                var cmdID = new CommandID(GuidList.guidSimpleActionsCmdSet, (int)PkgCmdIDList.cmdidSimpleActionsStartCommand + i);
                var mc = new OleMenuCommand(MenuItemExecCallback, cmdID);
                mc.BeforeQueryStatus += OnCommandQueryStatus;
                mcs.AddCommand(mc);
            }
            
        }

        private void InitActionBatches()
        {
            actionBatches = settingsStore.ReadActionBatches().ToList();

            if (actionBatches.Count == 0)
            {
                Debug.WriteLine("No batches found, trying to initialize defaults");
                settingsStore.WriteActionBatches(Config.Init());
                actionBatches = settingsStore.ReadActionBatches().ToList();
                if (actionBatches == null || actionBatches.Count == 0)
                {
                    Debug.WriteLine("Could not read batches, unfortunately");
                    actionBatches = Config.Init();
                }
            }
        }


        private void InitSettingsStore()
        {
            serviceProvider = new ServiceProvider((IOLEServiceProvider)_applicationObject, true);
            settingsStore = new SettingsStore("SimpleActions", serviceProvider);
        }

        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Debug.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            _applicationObject = (DTE2)GetService(typeof(DTE));

            var mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

            InitSettingsStore();
            InitActionBatches();
            InitSimpleActionsMenu(mcs);

        }
        #endregion

        #region Handlers

        private void OnCommandQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand menuCommand = sender as OleMenuCommand;
            if (null != menuCommand)
            {
                int commandId = menuCommand.CommandID.ID - (int)PkgCmdIDList.cmdidSimpleActionsStartCommand;
                if (commandId >= 0 && commandId < actionBatches.Count)
                {
                    menuCommand.Text = actionBatches[commandId].Title;
                }
            }
        }

        private bool IsValidDynamicItem(int commandId)
        {
            return commandId >= (int)PkgCmdIDList.cmdidSimpleActionsStartCommand && ((commandId - (int)PkgCmdIDList.cmdidSimpleActionsStartCommand) < actionBatches.Count);
        }

        #endregion

        private void MenuItemExecCallback(object sender, EventArgs e)
        {
            var command = sender as OleMenuCommand;

            if (command == null)
            {
                return;
            }

            int cmdIdx = command.CommandID.ID - (int)PkgCmdIDList.cmdidSimpleActionsStartCommand;
            if (cmdIdx >= 0 && cmdIdx < actionBatches.Count)
            {
                var actionBatch = actionBatches[cmdIdx];

                if (actionBatch == null)
                {
                    return;
                }
                RunBatch(actionBatch);
            }
        }

        public void RunBatch(ActionBatch batch)
        {
            foreach (var action in batch.Actions)
            {
                action.Execute(_applicationObject);
            }
        }

    }
}
