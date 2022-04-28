using Autofac;
using Caliburn.Micro;
using Ironwall.Framework;
using Ironwall.MapEditor.UI.DataProviders;
using Ironwall.MapEditor.UI.Models;
using Ironwall.MapEditor.UI.Services;
using Ironwall.MapEditor.UI.ViewModels;
using Ironwall.MapEditor.UI.ViewModels.Canvases;
using Ironwall.MapEditor.UI.ViewModels.Conductors;
using Ironwall.MapEditor.UI.ViewModels.DataGridItems;
using Ironwall.MapEditor.UI.ViewModels.Dialogs;
using Ironwall.MapEditor.UI.ViewModels.Panels;
using Ironwall.MapEditor.UI.ViewModels.PopupDialogs;
using Ironwall.MapEditor.UI.ViewModels.RegisteredItems;
using Ironwall.MapEditor.UI.ViewModels.Sections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Ironwall.MapEditor.UI
{
    /// <summary>
    /// The bootstrapper.
    /// </summary>
    internal sealed class UIBootstrapper : Bootstrapper<ExShellViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UIBootstrapper"/> class.
        /// </summary>
        public UIBootstrapper()
        {
            Initialize();
        }

        #region - Overrides -
        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);
            await Start();
            //await Task.Delay(500).ContinueWith((t, _) => DisplayRootViewFor<ExShellViewModel>(), null);
            await DisplayRootViewFor<ExShellViewModel>();

            #region - Visual Tree Check -
            #endregion
        }

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            try
            {
                base.ConfigureContainer(builder);
                builder.RegisterType<TopMenuSectionViewModel>().SingleInstance();
                builder.RegisterType<RegisteredStateSectionViewModel>().SingleInstance();
                builder.RegisterType<CanvasSectionViewModel>().SingleInstance();
                builder.RegisterType<PropertySectionViewModel>().SingleInstance();

                #region - Conductors -
                builder.RegisterType<ConductorControlViewModel>().SingleInstance();
                builder.RegisterType<PanelShellViewModel>().SingleInstance();
                builder.RegisterType<DialogShellViewModel>().SingleInstance();
                builder.RegisterType<PopupDialogShellViewModel>().SingleInstance();
                #endregion

                #region - Panels -
                builder.RegisterType<ShowListPanelViewModel>().SingleInstance();        //10
                builder.RegisterType<AddMapPanelViewModel>().SingleInstance();          //11
                builder.RegisterType<AddControllerPanelViewModel>().SingleInstance();   //12
                builder.RegisterType<AddSensorPanelViewModel>().SingleInstance();       //13
                builder.RegisterType<AddGroupPanelViewModel>().SingleInstance();        //14
                builder.RegisterType<AddCameraPanelViewModel>().SingleInstance();       //15
                builder.RegisterType<SavePanelViewModel>().SingleInstance();            //16
                builder.RegisterType<LoadPanelViewModel>().SingleInstance();            //17
                #endregion

                #region - Dialogs -

                #endregion

                #region - PopupDialogs -
                builder.RegisterType<ProgressPopupDialogViewModel>().SingleInstance();
                #endregion 

                var setupModel = new SetupModel();
                builder.RegisterInstance(setupModel).AsSelf().SingleInstance();

                #region - Services -
                //builder.RegisterType<InsertTreeItemService>().AsSelf().SingleInstance();
                //builder.RegisterType<InsertTreeItemService>().SingleInstance();
                #endregion

                builder.RegisterType<MapProvider>().SingleInstance();
                builder.RegisterType<SensorProvider>().SingleInstance();
                builder.RegisterType<ControllerProvider>().SingleInstance();
                builder.RegisterType<GroupProvider>().SingleInstance();
                builder.RegisterType<CameraProvider>().SingleInstance();

                builder.RegisterType<CanvasMapEntityProvider>().SingleInstance();

                builder.RegisterType<SymbolControllerProvider>().SingleInstance();
                //builder.RegisterType<MapComboBox>().SingleInstance();
                //builder.RegisterType<ControllerComboBox>().SingleInstance();
                //builder.RegisterType<GroupComboBox>().SingleInstance();

                builder.RegisterType<MapTreeViewModel>().SingleInstance();
                builder.RegisterType<DeviceTreeViewModel>().SingleInstance();
                builder.RegisterType<GroupTreeViewModel>().SingleInstance();
                builder.RegisterType<CameraTreeViewModel>().SingleInstance();


                //builder.RegisterType<CanvasControllerViewModel>().SingleInstance();
            }
            catch (Exception ex)
            {
                
            }
        }


        #endregion

        #region - Procedures - 
        #endregion

        #region - Properties - 
        #endregion

        #region - Attributes - 
        #endregion

    }
}
