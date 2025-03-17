using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace SemanticKernel.Plugins;
public class ArchivePlugin
{
    [KernelFunction("archive_data")]
    [Description("Saves data to a file on your computer")]
    public async Task WriteData(Kernel kernel, string fileName, string data)
    {
        await File.WriteAllTextAsync(
            $@"C:\Users\ashwin.p\Desktop\Repos\SemanticKernel_test\SemanticKernel_test\Archive Files\{fileName}.txt", data);
    }
}