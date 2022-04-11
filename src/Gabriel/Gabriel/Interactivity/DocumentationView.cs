using Disqord;
using Disqord.Extensions.Interactivity.Menus;
using Gabriel.Extensions;
using Gabriel.Services;

namespace Gabriel.Menus;

public class DocumentationView : ViewBase
{
    private readonly ButtonViewComponent _summaryButton;
    private readonly ButtonViewComponent _propertiesButton;
    private readonly ButtonViewComponent _methodsButton;
    private readonly ButtonViewComponent _extensionMethodsButton;
    private readonly ButtonViewComponent _previousPageButton;
    private readonly ButtonViewComponent _nextPageButton;

    private List<List<string>> _sections;
    private int _currentSection;
    private int _currentSectionPage;

    public DocumentationView(TypeDocumentation typeDocumentation)
        : base(new LocalMessage()
        .AddEmbed(new LocalEmbed()
            .WithTitle(typeDocumentation.Name)
            .WithDescription(typeDocumentation.Summary)))
    {
        _sections = new List<List<string>>
        {
            new() { typeDocumentation.Summary },
            typeDocumentation.Properties.GetCodePages(5),
            typeDocumentation.Methods.GetCodePages(5),
            typeDocumentation.ExtensionMethods.GetCodePages(5)
        };

        _summaryButton = new ButtonViewComponent(SummaryAsync)
        {
            Label = "Summary",
            IsDisabled = true
        };

        _propertiesButton = new ButtonViewComponent(PropertiesAsync)
        {
            Label = "Properties"
        };

        _methodsButton = new ButtonViewComponent(MethodsAsync)
        {
            Label = "Methods"
        };

        _extensionMethodsButton = new ButtonViewComponent(ExtensionMethodsAsync)
        {
            Label = "Extension Methods"
        };

        _previousPageButton = new ButtonViewComponent(PreviousPageAsync)
        {
            Label = "<─",
            Style = LocalButtonComponentStyle.Secondary,
            IsDisabled = true,
            Row = 1
        };

        _nextPageButton = new ButtonViewComponent(NextPageAsync)
        {
            Label = "─>",
            Style = LocalButtonComponentStyle.Secondary,
            IsDisabled = true,
            Row = 1
        };

        var deleteButton = new ButtonViewComponent(DeleteAsync)
        {
            Label = "🗑️",
            Style = LocalButtonComponentStyle.Danger,
            Row = 1
        };

        AddComponent(_summaryButton);
        if (typeDocumentation.Properties.Count > 0) AddComponent(_propertiesButton);
        if (typeDocumentation.Methods.Count > 0) AddComponent(_methodsButton);
        if (typeDocumentation.ExtensionMethods.Count > 0) AddComponent(_extensionMethodsButton);
        AddComponent(_previousPageButton);
        AddComponent(_nextPageButton);
        AddComponent(deleteButton);
    }

    public ValueTask SummaryAsync(ButtonEventArgs e)
    {
        _currentSection = 0;

        Update(true);
        _summaryButton.IsDisabled = true;
        return default;
    }

    public ValueTask PropertiesAsync(ButtonEventArgs e)
    {
        _currentSection = 1;

        Update(true);
        _propertiesButton.IsDisabled = true;
        return default;
    }

    public ValueTask MethodsAsync(ButtonEventArgs e)
    {
        _currentSection = 2;

        Update(true);
        _methodsButton.IsDisabled = true;
        return default;
    }

    public ValueTask ExtensionMethodsAsync(ButtonEventArgs e)
    {
        _currentSection = 3;

        Update(true);
        _extensionMethodsButton.IsDisabled = true;
        return default;
    }

    private ValueTask PreviousPageAsync(ButtonEventArgs e)
    {
        _currentSectionPage--;

        Update(false);
        return default;
    }

    private ValueTask NextPageAsync(ButtonEventArgs e)
    {
        _currentSectionPage++;

        Update(false);
        return default;
    }

    public ValueTask DeleteAsync(ButtonEventArgs e)
    {
        return DisposeAsync();
    }

    private void Update(bool changedSection)
    {
        if (changedSection)
        {
            _currentSectionPage = 0;
            _summaryButton.IsDisabled = false;
            _propertiesButton.IsDisabled = false;
            _methodsButton.IsDisabled = false;
            _extensionMethodsButton.IsDisabled = false;
        }

        var sectionName = _currentSection switch
        {
            0 => "Summary",
            1 => "Properties",
            2 => "Methods",
            3 => "Extension Methods",
            _ => null
        };

        var descriptionHeader = _sections[_currentSection].Count == 1
            ? $"**{sectionName}**\n\n"
            : $"**{sectionName}**\nPage {_currentSectionPage + 1} of {_sections[_currentSection].Count}\n\n";

        _previousPageButton.IsDisabled = _currentSectionPage == 0;
        _nextPageButton.IsDisabled = _sections[_currentSection].Count == 1 || _currentSectionPage == _sections[_currentSection].Count - 1;

        TemplateMessage.Embeds[0].Description = descriptionHeader + _sections[_currentSection][_currentSectionPage];
        ReportChanges();
    }
}
