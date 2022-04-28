using FluentValidation;
using Ironwall.MapEditor.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.ModelValidators
{
    public class MapModelValidator : AbstractValidator<IMapModel>
    {
        public MapModelValidator()
        {
            RuleFor(MapModel => MapModel.MapName)
                .NotNull().WithMessage("{PropertyName}(을)를 빈칸으로 설정할 수 없습니다.")
                .NotEmpty().WithMessage("{PropertyName}(을)를 빈칸으로 설정할 수 없습니다.")
                .Length(2, 50).WithMessage("{PropertyName}의 길이가 알맞지 않습니다.");

            RuleFor(MapModel => MapModel.MapNumber)
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName}(을)를 0이하의 값은 입력할 수 없습니다.")
                .NotNull().WithMessage("{PropertyName}(을)를 빈칸으로 설정할 수 없습니다.")
                .NotEmpty().WithMessage("{PropertyName}(을)를 빈칸으로 설정할 수 없습니다.");

            RuleFor(MapModel => MapModel.Url)
                .NotNull().WithMessage("{PropertyName}(을)를 빈칸으로 설정할 수 없습니다.")
                .NotEmpty().WithMessage("{PropertyName}(을)를 빈칸으로 설정할 수 없습니다.");

            RuleFor(MapModel => MapModel.Width)
                .GreaterThanOrEqualTo(0.0).WithMessage("{PropertyName}(을)를 0.0이하의 값은 입력할 수 없습니다.")
                .NotNull().WithMessage("{PropertyName}(을)를 빈칸으로 설정할 수 없습니다.")
                .NotEmpty().WithMessage("{PropertyName}(을)를 빈칸으로 설정할 수 없습니다.");

            RuleFor(MapModel => MapModel.Height)
                .GreaterThanOrEqualTo(0.0).WithMessage("{PropertyName}(을)를 0.0이하의 값은 입력할 수 없습니다.")
                .NotNull().WithMessage("{PropertyName}(을)를 빈칸으로 설정할 수 없습니다.")
                .NotEmpty().WithMessage("{PropertyName}(을)를 빈칸으로 설정할 수 없습니다.");

        }
    }
}
