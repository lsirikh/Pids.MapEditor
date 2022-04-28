using Caliburn.Micro;
using FluentValidation.Results;
using Ironwall.Enums;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.Framework.Helpers;
using Ironwall.MapEditor.UI.Models;
using Ironwall.MapEditor.UI.Models.Messages.Process;
using Ironwall.MapEditor.UI.ModelValidators;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Ironwall.MapEditor.UI.DataProviders;

namespace Ironwall.MapEditor.UI.ViewModels.ContentControls
{
    public class MapContentControlViewModel
        : BaseContentControl<IMapModel>
    {
        private MapProvider _provider;
        #region - Ctors -
        public MapContentControlViewModel(
            int id
            , IEventAggregator eventAggregator = null
            , MapProvider provider = null)
        {
            DisplayName = "Untitled MapContentControlViewModel";

            _validator = new MapModelValidator();
            _eventAggregator = eventAggregator;
            _provider = provider;

            _model = new MapModel();
            Id = id;
            broadCastring = false;
            Update();
        }
        public MapContentControlViewModel(
            int id, string mapName, int mapNumber
            , string fileName, string fileType, string url
            , double width, double height, bool used, bool visibility
            , IEventAggregator eventAggregator = null
            , MapProvider provider = null)
        {
            DisplayName = mapName;

            _validator = new MapModelValidator();
            _eventAggregator = eventAggregator;
            _provider = provider;

            _model = new MapModel();
            Id = id;
            MapName = mapName;
            MapNumber = mapNumber;
            FileName = fileName;
            FileType = fileType;
            Url = url;
            Width = width;
            Height = height;
            Used = used;
            Visibility = visibility;
            broadCastring = true;
            Update();
        }
        public MapContentControlViewModel(
            IMapModel model
            , IEventAggregator eventAggregator = null
            , MapProvider provider = null)
        {
            _model = model;
            _validator = new MapModelValidator();
            _eventAggregator = eventAggregator;
            _provider = provider;

            broadCastring = true;
            Update();
        }
        #endregion

        #region - Implementation of Interface -
        /// <summary>
        /// A function that requires compulsory use
        /// </summary>
        public override void Update()
        {
            NotifyOfPropertyChange(() => Id);
            NotifyOfPropertyChange(() => MapName);
            NotifyOfPropertyChange(() => MapNumber);
            NotifyOfPropertyChange(() => FileName);
            NotifyOfPropertyChange(() => FileType);
            NotifyOfPropertyChange(() => Url);
            NotifyOfPropertyChange(() => Width);
            NotifyOfPropertyChange(() => Height);
            NotifyOfPropertyChange(() => Used);
            NotifyOfPropertyChange(() => Visibility);
            NotifyOfPropertyChange(() => Model);
            _eventAggregator?.PublishOnUIThreadAsync(new MapContentUpdateMessageModel(this));
        }

        public override void Clear()
        {
            Model = new MapModel();
            Update();
        }
        #endregion

        #region - Overrides -
        /// <summary>
        /// An overrided function that requires not compulsory use
        /// </summary>
        /// <returns></returns>
        protected override Task InitialProcess()
        {
            base.InitialProcess();
            return Task.CompletedTask;
        }
        #endregion

        #region - Binding Methods -
        #endregion

        #region - IHanldes -
        #endregion

        #region - Processes -
        public async void ClickOpenFile()
        {
            if (!SetFileLoader())
                return;

            // 파일 복사
            Url = await FileManager.CopyUriFile("Maps", tempUrl);
        }

        /// <summary>
        /// SetFileLoader 파일 불러오기 로직
        /// 이미지 파일로 필터 적용
        /// </summary>
        /// <returns></returns>
        private bool SetFileLoader()
        {
            #region - 파일 불러오기 -
            var dialog = new OpenFileDialog();
            dialog.Filter = "Images (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF|" +
                            "All files (*.*)|*.*";
            dialog.Title = "맵 이미지 파일 선택하기";
            dialog.CheckFileExists = true; // 파일 존재 여부 확인
            dialog.CheckPathExists = true; // 폴더 존재 여부 확인
            dialog.InitialDirectory = @"C:\";

            dialog.RestoreDirectory = true; // 폴더 위치 저장하기

            if (dialog.ShowDialog() == true)
            {
                var relativeUrl = dialog.FileName;
                FileInfo fi = new FileInfo(relativeUrl);
                if (fi.Exists)
                {
                    //File경로와 File명을 모두 가지고 온다.
                    tempUrl = Path.GetFullPath(relativeUrl);
                    //Url = await FileManager.CopyUriFile("Maps", relativeUrl);
                    //파일 이름을 등록한다.
                    FileName = Path.GetFileName(relativeUrl);
                    //확장자를 가지고 온다.
                    FileType = Path.GetExtension(relativeUrl).Trim('.');

                    try
                    {
                        var pic = new BitmapImage(new Uri(tempUrl));
                        Width = pic.PixelWidth;
                        Height = pic.PixelHeight;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Raise Exception in SetFileLoader : {ex.Message}");
                        FileName = null;
                        FileType = null;
                        return false;
                    }
                    return true;
                }
            }
            return false;
            #endregion
        }
        #endregion

        #region - Properties -
        public int Id
        {
            get { return _model.Id; }
            set
            {
                _model.Id = value;
                NotifyOfPropertyChange(() => Id);
                if (broadCastring)
                    _eventAggregator?.PublishOnUIThreadAsync(new MapContentUpdateMessageModel(this));
                else
                    _eventAggregator?.PublishOnUIThreadAsync(new AddPanelUpdateMessageModel());
            }
        }

        public string MapName
        {
            get { return _model.MapName; }
            set
            {
                _model.MapName = value;
                NotifyOfPropertyChange(() => MapName);
                IsValidationError = CheckValidationRule("MapName");
                if (broadCastring)
                    _eventAggregator?.PublishOnUIThreadAsync(new MapContentUpdateMessageModel(this));
                else
                    _eventAggregator?.PublishOnUIThreadAsync(new AddPanelUpdateMessageModel());
            }
        }

        public int MapNumber
        {
            get { return _model.MapNumber; }
            set
            {
                ///중복 체크
                if (this.GetType() == typeof(MapContentControlViewModel))
                {
                    //Debug.WriteLine("ControllerContentControlViewModel");
                    if (_provider?.Where(t => t.MapNumber == value)?.Count() > 0)
                    {
                        NotifyOfPropertyChange(() => MapNumber);
                        return;
                    }
                }
                _model.MapNumber = value;
                NotifyOfPropertyChange(() => MapNumber);
                IsValidationError = CheckValidationRule("MapNumber");
                
                if (broadCastring)
                    _eventAggregator?.PublishOnUIThreadAsync(new MapContentUpdateMessageModel(this));
                else
                    _eventAggregator?.PublishOnUIThreadAsync(new AddPanelUpdateMessageModel());
            }
        }

        public string FileName
        {
            get { return _model.FileName; }
            set
            {
                _model.FileName = value;
                NotifyOfPropertyChange(() => FileName);
                //CheckValidationRule("FileName");
                if (broadCastring)
                    _eventAggregator?.PublishOnUIThreadAsync(new MapContentUpdateMessageModel(this));
                else
                    _eventAggregator?.PublishOnUIThreadAsync(new AddPanelUpdateMessageModel());
            }
        }

        public string FileType
        {
            get { return _model.FileType; }
            set
            {
                _model.FileType = value;
                NotifyOfPropertyChange(() => FileType);
                //CheckValidationRule("FileType");
                if (broadCastring)
                    _eventAggregator?.PublishOnUIThreadAsync(new MapContentUpdateMessageModel(this));
                else
                    _eventAggregator?.PublishOnUIThreadAsync(new AddPanelUpdateMessageModel());
            }
        }

        public string Url
        {
            get { return _model.Url; }
            set
            {
                _model.Url = value;
                NotifyOfPropertyChange(() => Url);
                IsValidationError = CheckValidationRule("Url");
                //FileType 및 FileName Input Logic
                if (broadCastring)
                    _eventAggregator?.PublishOnUIThreadAsync(new MapContentUpdateMessageModel(this));
                else
                    _eventAggregator?.PublishOnUIThreadAsync(new AddPanelUpdateMessageModel());
            }
        }

        public double Width
        {
            get { return _model.Width; }
            set
            {
                _model.Width = value;
                NotifyOfPropertyChange(() => Width);
                IsValidationError = CheckValidationRule("Width");
                if (broadCastring)
                    _eventAggregator?.PublishOnUIThreadAsync(new MapContentUpdateMessageModel(this));
                else
                    _eventAggregator?.PublishOnUIThreadAsync(new AddPanelUpdateMessageModel());

            }
        }

        public double Height
        {
            get { return _model.Height; }
            set
            {
                _model.Height = value;
                NotifyOfPropertyChange(() => Height);
                IsValidationError = CheckValidationRule("Height");
                if (broadCastring)
                    _eventAggregator?.PublishOnUIThreadAsync(new MapContentUpdateMessageModel(this));
                else
                    _eventAggregator?.PublishOnUIThreadAsync(new AddPanelUpdateMessageModel());
            }
        }

        public bool Used
        {
            get { return _model.Used; }
            set
            {
                _model.Used = value;
                NotifyOfPropertyChange(() => Used);
                if (broadCastring)
                    _eventAggregator?.PublishOnUIThreadAsync(new MapContentUpdateMessageModel(this));
                else
                    _eventAggregator?.PublishOnUIThreadAsync(new AddPanelUpdateMessageModel());
            }
        }

        public bool Visibility
        {
            get { return _model.Visibility; }
            set
            {
                _model.Visibility = value;
                NotifyOfPropertyChange(() => Visibility);
                if(broadCastring)
                    _eventAggregator?.PublishOnUIThreadAsync(new MapContentUpdateMessageModel(this));
                else
                    _eventAggregator?.PublishOnUIThreadAsync(new AddPanelUpdateMessageModel());
            }
        }


        #endregion

        #region - Attributes -
        private string tempUrl;
        #endregion
    }
}
