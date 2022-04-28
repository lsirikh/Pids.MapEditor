using Caliburn.Micro;
using FluentValidation;
using FluentValidation.Results;
using Ironwall.MapEditor.UI.ViewModels.RegisteredItems;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.ViewModels.ContentControls
{
    public abstract class BaseContentControl<T>
        : Screen, IBaseContentControl<T>
    { 
        #region - Ctors -
        #endregion
        #region - Implementation of Interface -
        public void Insert(T model)
        {
            _model = model;
            Update();
        }

        public abstract void Update();
        public abstract void Clear();

        #endregion
        #region - Overrides -
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            base.OnActivateAsync(cancellationToken);
            Debug.WriteLine($"######### {DisplayName}({this.GetType().Name.ToString()}) OnActivate!! #########");
            
            ///커스텀 초기화 프로세스 실행
            return InitialProcess();
        }
        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            base.OnDeactivateAsync(close, cancellationToken);
            Debug.WriteLine($"######### {DisplayName}({this.GetType().Name.ToString()}) OnDeactivate!! #########");
            
            ///커스텀 종료 프로세스 실행
            return ClosingProcess();
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        /// <summary>
        /// 시작 시, 실행 작업
        /// </summary>
        /// <returns></returns>
        protected virtual Task InitialProcess()
        {
            ///_evnetAggregator 수신
            _eventAggregator?.SubscribeOnPublishedThread(this);
            ///CancellationTokenSource 생성
            _cancellationTokenSource = new CancellationTokenSource();
            broadCastring = true;
            return Task.CompletedTask;
        }
        /// <summary>
        /// 종료 시, 실행 작업
        /// </summary>
        /// <returns></returns>
        protected virtual Task ClosingProcess()
        {
            ///_eventAggregator를 수신 취소
            _eventAggregator?.Unsubscribe(this);
            ///Nullify _eventAggregator
            _eventAggregator = null;
            ///Clear instance
            Clear();
            ///CancellationTokenSource Cancel
            _cancellationTokenSource.Cancel();

            return Task.CompletedTask;
        }
        /// <summary>
        /// CheckValidationRule은 설정된 입력규칙에 어긋나는 부분이 있으면,
        /// 리턴 값으로 참을 반환하고, 어긋나는 부분이 없으면, 리턴 값으로
        /// 거짓을 반환한다.
        /// </summary>
        /// <param name="name">PropertyName, string으로 입력한다.</param>
        /// <returns></returns>
        protected bool CheckValidationRule(string name)
        {
            results = _validator?.Validate(_model);

            if (!results.IsValid)
            {
                foreach (var failure in results.Errors)
                {
                    if (failure.PropertyName == name)
                    {
                        Notice = failure.ErrorMessage;
                        //Console.WriteLine(failure.ErrorMessage);
                        return true;
                    }
                }
            }
            Notice = "";
            return false;
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public T Model
        {
            get => _model;
            set
            {
                _model = value;
                NotifyOfPropertyChange(() => Model);
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        public string Notice
        {
            get { return _notice; }
            set
            {
                _notice = value;
                NotifyOfPropertyChange(() => Notice);
            }
        }

        public bool IsValidationError
        {
            get { return _isValidationError; }
            set
            {
                _isValidationError = value;
                NotifyOfPropertyChange(() => IsValidationError);
            }
        }

        #endregion
        #region - Attributes -
        public ValidationResult results;
        public bool broadCastring = false;
        protected AbstractValidator<T> _validator;
        protected IEventAggregator _eventAggregator;
        protected T _model;
        protected CancellationTokenSource _cancellationTokenSource;
        private bool _isValidationError;
        private string _notice;
        private bool _isSelected;
        #endregion
    }
}
