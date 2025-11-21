
namespace MainApp.Catalog;

// Цей клас ми пишемо вручну!
public partial class Agent
{
    protected override void Init()
    {
        BeforeSave = OnBeforeSave;
        AfterSave = OnAfterSave;
    }

    private Task<Boolean> OnBeforeSave()
    {
        Code = Code?.Trim();    
        Name = Name?.Trim();
        Code += "_BEFORE_SAVE";
        return Task.FromResult(true);
    }

    private Task OnAfterSave()
    {
        // логіка після збереження
        return Task.CompletedTask;
    }

    public override string ToString()
    {
        return $"Id: {Id}, Code: {Code}, Name: {Name}";
    }
}
