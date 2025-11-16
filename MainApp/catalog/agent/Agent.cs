
namespace MainApp.catalog.agent;

// Цей клас ми пишемо вручну!
public partial class Agent
{
    public Agent()
    {
        BeforeSave = OnBeforeSave;
        AfterSave = OnAfterSave;
    }
    private Task<Boolean> OnBeforeSave()
    {
        Code = Code?.Trim();    
        Name = Name?.Trim();
        return Task.FromResult(true);
    }

    private Task OnAfterSave()
    {
        // логіка після збереження
        return Task.CompletedTask;
    }
}
