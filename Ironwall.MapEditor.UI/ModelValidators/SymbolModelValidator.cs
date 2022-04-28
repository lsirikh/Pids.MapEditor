using FluentValidation;
using Ironwall.Framework.Models;
using Ironwall.MapEditor.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.MapEditor.UI.ModelValidators
{
    public class SymbolModelValidator : AbstractValidator<IEntityModel>
    {
        public SymbolModelValidator()
        {
            RuleFor(SymbolModel => SymbolModel.NameArea)
                .NotNull().WithMessage("{PropertyName}(을)를 빈칸으로 둘 수 없습니다.")
                .NotEmpty().WithMessage("{PropertyName}(을)를 빈칸으로 둘 수 없습니다.")
                .Length(1, 20).WithMessage("{PropertyName}의 길이가 알맞지 않습니다.");
            
            RuleFor(SymbolModel => SymbolModel.NameDevice)
                .NotNull().WithMessage("{PropertyName}(을)를 빈칸으로 둘 수 없습니다.")
                .NotEmpty().WithMessage("{PropertyName}(을)를 빈칸으로 둘 수 없습니다.")
                .Length(1, 10).WithMessage("{PropertyName}의 길이가 알맞지 않습니다.");

            /*RuleFor(SymbolModel => SymbolModel.TypeDevice)
                .GreaterThan(0).WithMessage("{PropertyName}(을)를 선택하세요.");*/
        }
    }
}
