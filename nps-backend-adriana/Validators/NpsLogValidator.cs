using FluentValidation;
using nps_backend_adriana.Models.Dto;

namespace nps_backend_adriana.Validators
{
    public class NpsLogValidator : AbstractValidator<NpsLogDto>
    {
        public NpsLogValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("O usuário deve ser informado.");

            RuleFor(x => x.Score)
                .NotEqual(0)
                .WithMessage("Obrigatório informar uma nota");

            RuleFor(x => x.CategoryNumber)
                .NotEmpty().WithMessage("A categoria deve ser informada.")
                .When(x => x.Score < 7)
                .WithMessage("Categoria é obrigatória quando a nota for menor que 7.");

            RuleFor(a => a.Description)
                .MaximumLength(150)
                .WithMessage("Tamanho máximo da Descrição é de 150 caracteres");
                        
        }

    }
}
