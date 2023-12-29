using AutoMapper;
using FluentValidation;
using NetSchool.Common.ValidationRules;
using NetSchool.Context.Entities;

namespace NetSchool.Services.Books.Books.Models;

public class UpdateBookModel
{
    public string Title { get; set; }
    public string Description { get; set; }
}

public class UpdateModelProfile : Profile
{
    public UpdateModelProfile()
    {
        CreateMap<UpdateBookModel, Book>();
    }
}

public class UpdateModelValidator : AbstractValidator<UpdateBookModel>
{
    public UpdateModelValidator()
    {
        RuleFor(x => x.Title).BookTitle();

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Maximum length is 1000");
    }
}