using A2v10.Module.Infrastructure;

namespace MainApp
{
    public class Class1 : IAppContainer
    {
        public Guid Id => throw new NotImplementedException();

        public string? Authors => throw new NotImplementedException();

        public string? Company => throw new NotImplementedException();

        public string? Description => throw new NotImplementedException();

        public string? Copyright => throw new NotImplementedException();

        public bool IsLicensed => throw new NotImplementedException();

        public IEnumerable<string> EnumerateFiles(string prefix, string pattern)
        {
            throw new NotImplementedException();
        }

        public bool FileExists(string path)
        {
            throw new NotImplementedException();
        }

        public string? GetText(string path)
        {
            throw new NotImplementedException();
        }
    }
}
