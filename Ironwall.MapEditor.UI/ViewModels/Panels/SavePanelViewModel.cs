using Caliburn.Micro;
using Ironwall.Enums;
using Ironwall.Framework.Helpers;
using Ironwall.Framework.Models;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.MapEditor.UI.DataProviders;
using Ironwall.MapEditor.UI.Models;
using Ironwall.MapEditor.UI.Models.Messages.Panels;
using Ironwall.MapEditor.UI.Models.Messages.PopupDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.ViewModels.Panels
{
    internal sealed class SavePanelViewModel
        : BaseViewModel
        , IPanelViewModel
    {
        
        #region - Ctors -
        public SavePanelViewModel(
            IEventAggregator eventAggregator
            , MapProvider mapProvider
            , ControllerProvider controllerProvider
            , SensorProvider sensorProvider
            , GroupProvider groupProvider
            , CameraProvider cameraProvider
            )
        {
            #region - Settings -
            Id = 16;
            Content = "";
            Category = CategoryEnum.PANEL_SHELL_VM_ITEM;
            #endregion - Settings -

            _eventAggregator = eventAggregator;

            _mapProvider = mapProvider;
            _controllerProvider = controllerProvider;
            _sensorProvider = sensorProvider;
            _groupProvider = groupProvider;
            _cameraProvider = cameraProvider;
        }
        #endregion
        #region - Implementation of Interface -
        public void ModelUpdate()
        {
        }

        /// <summary>
        /// NotifyOfPropertyChange(nameof(CanClickOk))이 있어야 갱신이 가능하다.
        /// </summary>
        public bool CanClickOkAsync => true;

        public async void ClickOkAsync()
        {

            ///모델정보 가져오기
            var mapModels = _mapProvider.Select(model => model.Model);
            var controllerModels = _controllerProvider.Select(model => model.Model);
            var sensorModels = _sensorProvider.Select(model => model.Model);
            var groupModels = _groupProvider.Select(model => model.Model);
            var cameraModels = _cameraProvider.Select(model => model.Model);


            ///로딩화면 시현
            await _eventAggregator.PublishOnCurrentThreadAsync(new OpenProgressPopupMessageModel(), _cancellationTokenSource.Token);

            ///파일 저장 Task 실행
            var mapTask = FileManager.SaveCSVFile<IMapModel>(mapModels.ToList(), "Csvs", "Map", _cancellationTokenSource.Token);
            var controllerTask = FileManager.SaveCSVFile<IEntityModel>(controllerModels.ToList(), "Csvs", "Controller", _cancellationTokenSource.Token);
            var sensorTask = FileManager.SaveCSVFile<IEntityModel>(sensorModels.ToList(), "Csvs", "Sensor", _cancellationTokenSource.Token);
            var groupTask = FileManager.SaveCSVFile<IEntityModel>(groupModels.ToList(), "Csvs", "Group", _cancellationTokenSource.Token);
            var cameraTask = FileManager.SaveCSVFile<IEntityModel>(cameraModels.ToList(), "Csvs", "Camera", _cancellationTokenSource.Token);

            await mapTask;
            await controllerTask;
            await sensorTask;
            await groupTask;
            await cameraTask;

            await Task.Delay(500)
                .ContinueWith((_, t) => _eventAggregator.PublishOnUIThreadAsync(new ClosePopupDialogMessageModel()), _cancellationTokenSource.Token)
                .ContinueWith((_, t) => _eventAggregator.PublishOnUIThreadAsync(new ClosePanelMessageModel()), _cancellationTokenSource.Token);
        }

        public async void ClickCancelAsync()
        {
            ///AddMapPanel 종료
            await _eventAggregator.PublishOnCurrentThreadAsync(new ClosePanelMessageModel(), _cancellationTokenSource.Token);
        }
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -

        public bool CanClickToSaveMapData => true;

        public async void ClickToSaveMapData()
        {
            ///모델 정보만 가져오기
            var models = _mapProvider.Select(model => model.Model);

            ///프로그래스 이미지 시현
            await _eventAggregator.PublishOnUIThreadAsync(new OpenProgressPopupMessageModel(), _cancellationTokenSource.Token);

            ///파일 저장 Task 실행
            var task = FileManager.SaveCSVFile<IMapModel>(models.ToList(), "Csvs", "Map", _cancellationTokenSource.Token);
            if(await task)
            {
                await Task.Delay(1000).ContinueWith((_, t) => _eventAggregator.PublishOnUIThreadAsync(new ClosePopupDialogMessageModel()), _cancellationTokenSource.Token);
            }
        }

        public bool CanClickToSaveControllerData => true;

        public async void ClickToSaveControllerData()
        {
            ///모델 정보만 가져오기
            var models = _controllerProvider.Select(model => model.Model);

            ///프로그래스 이미지 시현
            await _eventAggregator.PublishOnUIThreadAsync(new OpenProgressPopupMessageModel(), _cancellationTokenSource.Token);

            ///파일 저장 Task 실행
            var task = FileManager.SaveCSVFile<IEntityModel>(models.ToList(), "Csvs", "Controller", _cancellationTokenSource.Token);
            if (await task)
            {
                await Task.Delay(1000).ContinueWith((_, t) => _eventAggregator.PublishOnUIThreadAsync(new ClosePopupDialogMessageModel()), _cancellationTokenSource.Token);
            }
        }

        public bool CanClickToSaveSensorData => true;

        public async void ClickToSaveSensorData()
        {
            ///모델 정보만 가져오기
            var models = _sensorProvider.Select(model => model.Model);

            ///프로그래스 이미지 시현
            await _eventAggregator.PublishOnUIThreadAsync(new OpenProgressPopupMessageModel(), _cancellationTokenSource.Token);

            ///파일 저장 Task 실행
            var task = FileManager.SaveCSVFile<IEntityModel>(models.ToList(), "Csvs", "Sensor", _cancellationTokenSource.Token);
            if (await task)
            {
                await Task.Delay(1000).ContinueWith((_, t) => _eventAggregator.PublishOnUIThreadAsync(new ClosePopupDialogMessageModel()), _cancellationTokenSource.Token);
            }
        }

        public bool CanClickToSaveGroupData => true;

        public async void ClickToSaveGroupData()
        {
            ///모델 정보만 가져오기
            var models = _groupProvider.Select(model => model.Model);

            ///프로그래스 이미지 시현
            await _eventAggregator.PublishOnUIThreadAsync(new OpenProgressPopupMessageModel(), _cancellationTokenSource.Token);

            ///파일 저장 Task 실행
            var task = FileManager.SaveCSVFile<IEntityModel>(models.ToList(), "Csvs", "Group", _cancellationTokenSource.Token);
            if (await task)
            {
                await Task.Delay(1000).ContinueWith((_, t) => _eventAggregator.PublishOnUIThreadAsync(new ClosePopupDialogMessageModel()), _cancellationTokenSource.Token);
            }
        }

        public bool CanClickToSaveCameraData => true;

        public async void ClickToSaveCameraData()
        {
            ///모델 정보만 가져오기
            var models = _cameraProvider.Select(model => model.Model);

            ///프로그래스 이미지 시현
            await _eventAggregator.PublishOnUIThreadAsync(new OpenProgressPopupMessageModel(), _cancellationTokenSource.Token);

            ///파일 저장 Task 실행
            var task = FileManager.SaveCSVFile<IEntityModel>(models.ToList(), "Csvs", "Camera", _cancellationTokenSource.Token);
            if (await task)
            {
                await Task.Delay(1000).ContinueWith((_, t) => _eventAggregator.PublishOnUIThreadAsync(new ClosePopupDialogMessageModel()), _cancellationTokenSource.Token);
            }
        }
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private MapProvider _mapProvider;
        private ControllerProvider _controllerProvider;
        private SensorProvider _sensorProvider;
        private GroupProvider _groupProvider;
        private CameraProvider _cameraProvider;
        #endregion
    }
}
