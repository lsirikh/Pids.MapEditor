using Caliburn.Micro;
using Ironwall.Enums;
using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.MapEditor.UI.DataProviders;
using Ironwall.MapEditor.UI.Models;
using Ironwall.MapEditor.UI.Models.Messages.Process;
using Ironwall.MapEditor.UI.ModelValidators;
using Ironwall.MapEditor.UI.ViewModels.ComboBoxSource;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.ViewModels.ContentControls
{
    public abstract class SymbolContentControlViewModel
        : BaseContentControl<IEntityModel>
    {
        #region - Ctors -
        public SymbolContentControlViewModel(int id
            , IEventAggregator eventAggregator = null
            , EntityCollectionProvider<SymbolContentControlViewModel> provider = null)
        {
            _model = new SymbolModel();
            _validator = new SymbolModelValidator();

            Id = id;
            _eventAggregator = eventAggregator;
            _provider = provider;
            Update();
        }
        public SymbolContentControlViewModel(
            int id, string nameArea, int typeDevice, string nameDevice
            , int idController, int idSensor, int typeShape
            , double x1, double y1, double x2, double y2
            , double width, double height, double angle
            , int map, bool used, bool visibility
            , IEventAggregator eventAggregator = null
            , EntityCollectionProvider<SymbolContentControlViewModel> provider = null)
        {
            _model = new SymbolModel();
            _validator = new SymbolModelValidator();
            _eventAggregator = eventAggregator;

            Id = id;
            NameArea = nameArea;
            TypeDevice = typeDevice;
            NameDevice = nameDevice;
            IdController = idController;
            IdSensor = idSensor;
            TypeShape = typeShape;
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
            Width = width;
            Height = height;
            Angle = angle;
            Map = map;
            Used = used;
            Visibility = visibility;
            _provider = provider;
            Update();
        }
        public SymbolContentControlViewModel(
            IEntityModel model
            , IEventAggregator eventAggregator
            , EntityCollectionProvider<SymbolContentControlViewModel> provider = null)
        {
            _model = model;
            _validator = new SymbolModelValidator();
            _eventAggregator = eventAggregator;
            _provider = provider;
            Update();
        }
        #endregion

        #region - Impementation of Interface -
        #endregion

        #region - Binding Methods -
        #endregion

        #region - IHanldes -
        #endregion
        #region - Processes -
        #endregion

        #region - Overrides -
        /// <summary>
        /// A function that requires compulsory use
        /// </summary>
        public override void Update()
        {
            NotifyOfPropertyChange(() => Id);
            NotifyOfPropertyChange(() => NameArea);
            NotifyOfPropertyChange(() => TypeDevice);
            NotifyOfPropertyChange(() => NameDevice);
            NotifyOfPropertyChange(() => IdController);
            NotifyOfPropertyChange(() => IdSensor);
            NotifyOfPropertyChange(() => TypeShape);
            NotifyOfPropertyChange(() => X1);
            NotifyOfPropertyChange(() => Y1);
            NotifyOfPropertyChange(() => X2);
            NotifyOfPropertyChange(() => Y2);
            NotifyOfPropertyChange(() => Width);
            NotifyOfPropertyChange(() => Height);
            NotifyOfPropertyChange(() => Angle);
            NotifyOfPropertyChange(() => Map);
            NotifyOfPropertyChange(() => Used);
            NotifyOfPropertyChange(() => Visibility);
            
            if(broadCastring)
                _eventAggregator?.PublishOnUIThreadAsync(new SymbolContentUpdateMessageModel(this));
        }
        /// <summary>
        /// Clear는 Model과 ViewModel을 초기화 하는 메소드
        /// </summary>
        public override void Clear()
        {
            #region Deprecated Code
            /*Id = 0;
            NameArea = null;
            TypeDevice = 0;
            NameDevice = null;
            IdController = 0;
            IdSensor = 0;
            TypeShape = 0;
            X1 = 0.0;
            Y1 = 0.0;
            X2 = 0.0;
            Y2 = 0.0;
            Width = 0.0;
            Height = 0.0;
            Angle = 0.0;
            Map = 0;
            Used = false;
            Visibility = false;*/
            #endregion
            Model = new SymbolModel();
            Update();
        }
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

        #region - Properties -
        public int Id
        {
            get { return _model.Id; }
            set
            {
                _model.Id = value;
                NotifyOfPropertyChange(() => Id);
                if (broadCastring)
                    _eventAggregator?.PublishOnUIThreadAsync(new SymbolContentUpdateMessageModel(this, "Id"));
                else
                    _eventAggregator?.PublishOnUIThreadAsync(new AddPanelUpdateMessageModel());
            }
        }

        /// <summary>
        /// NameArea - 구역 이름
        /// GroupContentControlViewModel은
        /// 중복된 이름 갖으면 안된다.
        /// 단, Controller는 구역 이름에 제약을 받지 않는다.
        /// </summary>
        public string NameArea
        {
            get { return _model.NameArea; }
            set
            {
                ///중복 체크
                if (this.GetType() == typeof(GroupContentControlViewModel))
                {
                    //Debug.WriteLine("GroupContentControlViewModel");
                    if (_provider?.Where(t => t.NameArea == value)?.Count() > 0)
                    {
                        NotifyOfPropertyChange(() => NameArea);
                        return;
                    }
                }
                _model.NameArea = value;
                NotifyOfPropertyChange(() => NameArea);
                IsValidationError = CheckValidationRule("NameArea");

                if (broadCastring)
                    _eventAggregator?.PublishOnUIThreadAsync(new SymbolContentUpdateMessageModel(this, "NameArea"));
                else
                    _eventAggregator?.PublishOnUIThreadAsync(new AddPanelUpdateMessageModel());
            }
        }

        /// <summary>
        /// TypeDevice - 장비 타입
        /// ControllerContetnControlViewModel, SensorContentControlViewModel은
        /// 반드시 장비타입을 갖어야 한다.
        /// GroupContentControlViewModel은
        /// TypeDeivce를 갖을 수 없다.
        /// </summary>
        public int TypeDevice
        {
            get { return _model.TypeDevice; }
            set
            {
                _model.TypeDevice = value;
                NotifyOfPropertyChange(() => TypeDevice);
                IsValidationError = CheckValidationRule("TypeDevice");
                if (broadCastring)
                    _eventAggregator?.PublishOnUIThreadAsync(new SymbolContentUpdateMessageModel(this, "TypeDevice"));
                else
                    _eventAggregator?.PublishOnUIThreadAsync(new AddPanelUpdateMessageModel());
            }
        }

        public string NameDevice
        {
            get { return _model.NameDevice; }
            set
            {

                _model.NameDevice = value;
                NotifyOfPropertyChange(() => NameDevice);
                IsValidationError = CheckValidationRule("NameDevice");
                if (broadCastring)
                    _eventAggregator?.PublishOnUIThreadAsync(new SymbolContentUpdateMessageModel(this, "NameDevice"));
                else
                    _eventAggregator?.PublishOnUIThreadAsync(new AddPanelUpdateMessageModel());
            }
        }
        /// <summary>
        /// IdController - 제어기 번호
        /// 제어기 번호는 고유의 번호로 중복을 허용하지 않고,
        /// 해당 제어기 번호에 따라 하위 센서 번호가 속하게 된다.
        /// </summary>
        public int IdController
        {
            get { return _model.IdController; }
            set
            {
                ///중복 체크
                if (this.GetType() == typeof(ControllerContentControlViewModel))
                {
                    //Debug.WriteLine("ControllerContentControlViewModel");
                    if (_provider?.Where(t => t.IdController == value)?.Count() > 0)
                    {
                        NotifyOfPropertyChange(() => IdController);
                        return;
                    }
                }
                
                _model.IdController = value;
                NotifyOfPropertyChange(() => IdController);
                if (broadCastring)
                    _eventAggregator?.PublishOnUIThreadAsync(new SymbolContentUpdateMessageModel(this, "IdController"));
                else
                    _eventAggregator?.PublishOnUIThreadAsync(new AddPanelUpdateMessageModel());
            }
        }

        public int IdSensor
        {
            get { return _model.IdSensor; }
            set
            {
                ///중복 체크
                if (this.GetType() == typeof(SensorContentControlViewModel))
                {
                    //Debug.WriteLine("SensorContentControlViewModel");
                    if (_provider?.Where(t => t.IdController == IdController && t.IdSensor == value)?.Count() > 0)
                    {
                        NotifyOfPropertyChange(() => IdSensor);
                        return;
                    }
                }
                _model.IdSensor = value;
                NotifyOfPropertyChange(() => IdSensor);
                if (broadCastring)
                    _eventAggregator?.PublishOnUIThreadAsync(new SymbolContentUpdateMessageModel(this, "IdSensor"));
                else
                    _eventAggregator?.PublishOnUIThreadAsync(new AddPanelUpdateMessageModel());
            }
        }

        public int TypeShape
        {
            get { return _model.TypeShape; }
            set
            {
                _model.TypeShape = value;
                NotifyOfPropertyChange(() => TypeShape);
                if (broadCastring)
                    _eventAggregator?.PublishOnUIThreadAsync(new SymbolContentUpdateMessageModel(this, "TypeShape"));
                else
                    _eventAggregator?.PublishOnUIThreadAsync(new AddPanelUpdateMessageModel());
            }
        }

        public double X1
        {
            get { return _model.X1; }
            set
            {
                _model.X1 = value;
                NotifyOfPropertyChange(() => X1);
                if (broadCastring)
                    _eventAggregator?.PublishOnUIThreadAsync(new SymbolContentUpdateMessageModel(this, "X1"));
                else
                    _eventAggregator?.PublishOnUIThreadAsync(new AddPanelUpdateMessageModel());
            }
        }

        public double Y1
        {
            get { return _model.Y1; }
            set
            {
                _model.Y1 = value;
                NotifyOfPropertyChange(() => Y1);
                if (broadCastring)
                    _eventAggregator?.PublishOnUIThreadAsync(new SymbolContentUpdateMessageModel(this, "Y1"));
                else
                    _eventAggregator?.PublishOnUIThreadAsync(new AddPanelUpdateMessageModel());
            }
        }

        public double X2
        {
            get { return _model.X2; }
            set
            {
                _model.X2 = value;
                NotifyOfPropertyChange(() => X2);
                if (broadCastring)
                    _eventAggregator?.PublishOnUIThreadAsync(new SymbolContentUpdateMessageModel(this, "X2"));
                else
                    _eventAggregator?.PublishOnUIThreadAsync(new AddPanelUpdateMessageModel());
            }
        }

        public double Y2
        {
            get { return _model.Y2; }
            set
            {
                _model.Y2 = value;
                NotifyOfPropertyChange(() => Y2);
                if (broadCastring)
                    _eventAggregator?.PublishOnUIThreadAsync(new SymbolContentUpdateMessageModel(this, "Y2"));
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
                if (broadCastring)
                    _eventAggregator?.PublishOnUIThreadAsync(new SymbolContentUpdateMessageModel(this, "Width"));
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
                if (broadCastring)
                    _eventAggregator?.PublishOnUIThreadAsync(new SymbolContentUpdateMessageModel(this, "Height"));
                else
                    _eventAggregator?.PublishOnUIThreadAsync(new AddPanelUpdateMessageModel());
            }
        }

        public double Angle
        {
            get { return _model.Angle; }
            set
            {
                _model.Angle = value;
                NotifyOfPropertyChange(() => Angle);
                if (broadCastring)
                    _eventAggregator?.PublishOnUIThreadAsync(new SymbolContentUpdateMessageModel(this, "Angle"));
                else
                    _eventAggregator?.PublishOnUIThreadAsync(new AddPanelUpdateMessageModel());
            }
        }

        public int Map
        {
            get { return _model.Map; }
            set
            {
                _model.Map = value;
                NotifyOfPropertyChange(() => Map);
                if (broadCastring)
                    _eventAggregator?.PublishOnUIThreadAsync(new SymbolContentUpdateMessageModel(this, "Map"));
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
                    _eventAggregator?.PublishOnUIThreadAsync(new SymbolContentUpdateMessageModel(this, "Used"));
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
                if (broadCastring)
                    _eventAggregator?.PublishOnUIThreadAsync(new SymbolContentUpdateMessageModel(this, "Visibility"));
                else
                    _eventAggregator?.PublishOnUIThreadAsync(new AddPanelUpdateMessageModel());
            }
        }
        #endregion

        #region - Attributes -
        protected EntityCollectionProvider<SymbolContentControlViewModel> _provider;
        #endregion
    }
}
